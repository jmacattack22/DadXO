using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TournamentTest : MonoBehaviour {

	private DataPool worldData;

	void Start () {
        worldData = new DataPool();
        WorldBuilderProtocol.createWorld(ref worldData, 220, 220);

        Dictionary<TournamentProtocol.Level, int> dist = new Dictionary<TournamentProtocol.Level, int>();

        foreach (ManagerProtocol mp in worldData.ManagerProtocols){
            if (!dist.ContainsKey(mp.Rank))
                dist.Add(mp.Rank, 0);

            dist[mp.Rank] += 1;
        }

        foreach (TournamentProtocol.Level rank in dist.Keys){
            Debug.Log(rank.ToString() + " - " + dist[rank]);
        }

        //foreach (ManagerProtocol mp in worldData.ManagerProtocols){
        //    worldData.Boxers[mp.BoxerIndex].logBoxerStats(mp.Rank);
        //}

        weeklyTest();
	}

    private void weeklyTest()
    {
        List<int> thisWeeksTournaments = new List<int>();

        int index = 0;
        foreach (Town town in worldData.Towns)
        {
            if (town.Tournament.TournamentDate.sameWeek(worldData.Calendar.GetCalendarDate()))
            {
                thisWeeksTournaments.Add(index);
            }
            index++;
        }

        if (worldData.Capitols[0].Quarterlies[TournamentProtocol.Level.E].TournamentDate.sameWeek(worldData.Calendar.GetCalendarDate())){
            foreach (Capitol c in worldData.Capitols){
                foreach (TournamentProtocol.Level level in c.Quarterlies.Keys){
                    
                }
            }
        }

        foreach (int tIndex in thisWeeksTournaments){
            List<int> recruits = recruit(worldData.Towns[tIndex].Tournament.TournamentLevel, worldData.Towns[tIndex].Location);

            for (int i = 0; i < recruits.Count; i++){
                if (worldData.Towns[tIndex].Tournament.spaceLeft()){
                    worldData.Towns[tIndex].Tournament.addContestant(recruits[i]);
                    worldData.ManagerProtocols[recruits[i]].attendTournament();
                } else{
                    worldData.ManagerProtocols[recruits[i]].bumpTournamentPriority();
                }
            }
        }

        foreach (ManagerProtocol mp in worldData.ManagerProtocols){
            mp.executeWeek(ref worldData);
        }

        foreach (int tIndex in thisWeeksTournaments)
        {
            if (worldData.Towns[tIndex].Tournament.Attendees > 2){
                Debug.Log(worldData.Towns[tIndex].Location.ToString() + " - " + worldData.Towns[tIndex].Tournament.getDetails());
                worldData.Towns[tIndex].Tournament.scheduleTournament();
                worldData.Towns[tIndex].Tournament.SimWholeTournament(ref worldData);
            } else {
                worldData.Towns[tIndex].Tournament.cancelTournament(ref worldData);
            }

            worldData.Towns[tIndex].Tournament.refreshTournament(false);
        }
    }

    private List<int> recruit(TournamentProtocol.Level level, Vector2Int p1){
        List<int> potentialRecruits = new List<int>();

        int index = 0;
        foreach (ManagerProtocol mp in worldData.ManagerProtocols)
        {
            Vector2Int p2 = worldData.Towns[worldData.Managers[mp.ManagerIndex].TownIndex].Location;

            float distance = Mathf.Sqrt(Mathf.Pow((p2.x - p1.x), 2) + Mathf.Pow((p2.y - p1.y), 2));

            if (distance < getDistanceFromLevel(level) && mp.Rank.Equals(level) && !mp.isBusy())
                potentialRecruits.Add(index);

            index++;
        }

        //potentialRecruits.Shuffle();
        potentialRecruits.OrderByDescending(t => worldData.ManagerProtocols[t].TournamentPriority);
        return potentialRecruits;
    }

    public void logBoxerResults(){
        foreach (ManagerProtocol mp in worldData.ManagerProtocols){
            worldData.Boxers[mp.BoxerIndex].logBoxerStats(mp.Rank);
        }
    }

    public void logManagerResults(){
        foreach (ManagerProtocol mp in worldData.ManagerProtocols)
        {
            mp.logManagerStats(ref worldData);
        }
    }

    private float getDistanceFromLevel(TournamentProtocol.Level level){
        if (level.Equals(TournamentProtocol.Level.S))
            return 1000.0f;
        else if (level.Equals(TournamentProtocol.Level.A))
            return 700.0f;
        else if (level.Equals(TournamentProtocol.Level.B))
            return 400.0f;
        else if (level.Equals(TournamentProtocol.Level.C))
            return 300.0f;
        else if (level.Equals(TournamentProtocol.Level.D))
            return 300.0f;
        else if (level.Equals(TournamentProtocol.Level.E))
            return 350.0f;

        return 1200.0f;
    }

    public void progressWeek(){
        worldData.Calendar.progessWeek();
        Debug.Log(worldData.Calendar.getDate(Calendar.DateType.fullLong));
        weeklyTest();
    }

    private void pollTest(){
        int index = 0;
        int tIndex = 0;
        bool foundIndex = false;

        Dictionary<TournamentProtocol.Level, int> tournamentCounts = new Dictionary<TournamentProtocol.Level, int>();

        foreach (Town t in worldData.Towns)
        {
            if (!tournamentCounts.ContainsKey(t.Tournament.TournamentLevel))
            {
                tournamentCounts.Add(t.Tournament.TournamentLevel, 0);
            }

            tournamentCounts[t.Tournament.TournamentLevel] += 1;

            if (!foundIndex)
            {
                if (t.Tournament.TournamentLevel == TournamentProtocol.Level.C)
                {
                    tIndex = index;
                    foundIndex = true;
                }
            }

            index++;
        }

        foreach (TournamentProtocol.Level level in tournamentCounts.Keys)
        {
            Debug.Log(level.ToString() + " - " + tournamentCounts[level]);
        }

        Debug.Log(worldData.Towns[tIndex].Location.ToString() + " - " + worldData.Towns[tIndex].Tournament.getDetails());
        Vector2Int p1 = worldData.Towns[tIndex].Location;

        List<ManagerProtocol> potentialContestants = new List<ManagerProtocol>();

        foreach (ManagerProtocol mp in worldData.ManagerProtocols)
        {
            Vector2Int p2 = worldData.Towns[worldData.Managers[mp.ManagerIndex].TownIndex].Location;

            float distance = Mathf.Sqrt(Mathf.Pow((p2.x - p1.x), 2) + Mathf.Pow((p2.y - p1.y), 2));

            if (distance < 150 && mp.Rank.Equals(TournamentProtocol.Level.C))
                potentialContestants.Add(mp);
        }

        Debug.Log(potentialContestants.Count);

        foreach (ManagerProtocol mp in potentialContestants)
        {
            Debug.Log(worldData.Managers[mp.ManagerIndex].FirstName);
        }

        //      TournamentProtocol tp = new TournamentProtocol (
        //          new CalendarDate (1, 1, 1), 1000.0f, worldData.ManagerProtocols.Count, TournamentProtocol.Level.E);

        //      for (int i = 0; i < worldData.ManagerProtocols.Count; i++) {
        //          tp.addContestant (i);
        //      }

        ////        tp.addContestant (0);
        ////        tp.addContestant (1);
        ////        tp.addContestant (2);
        ////        tp.addContestant (3);
        ////        tp.addContestant (4);
        ////        tp.addContestant (5);
        ////        tp.addContestant (6);
        ////        tp.addContestant (7);

        //tp.scheduleTournament ();

        //for (int i = 0; i < tp.Size - 1; i++) {
        //  tp.simNextRound (ref worldData);
        //}

        //tp.logResults ();
    }
}
