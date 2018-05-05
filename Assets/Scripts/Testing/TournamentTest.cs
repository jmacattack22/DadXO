using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TournamentTest : MonoBehaviour {

	private DataPool worldData;

    List<int> thisWeeksTournaments;

	void Start () {
        worldData = new DataPool();
        WorldBuilderProtocol.createWorld(ref worldData, 220, 220);
        worldData.updateBoxerDistribution();

        thisWeeksTournaments = new List<int>();

        //foreach (ManagerProtocol mp in worldData.ManagerProtocols){
        //    worldData.Boxers[mp.BoxerIndex].logBoxerStats(mp.Rank);
        //}

        weeklyTest();
	}

    private void weeklyTest()
    {
        thisWeeksTournaments = new List<int>();

        int index = 0;
        foreach (Town town in worldData.Towns)
        {
            if (town.Tournament.TournamentDate.sameWeek(worldData.Calendar.GetCalendarDate()))
            {
                thisWeeksTournaments.Add(index);
            }
            index++;
        }

        if ((worldData.Calendar.getWeekOfYear() - 1) % 8 == 0){
            foreach (Capitol c in worldData.Capitols)
            {
                foreach (TournamentProtocol.Level level in c.Quarterlies.Keys)
                {
                    List<int> recruits = recruit(level, c.Location, true);

                    for (int i = 0; i < recruits.Count; i++)
                    {
                        if (c.Quarterlies[level].spaceLeft())
                        {
                            c.Quarterlies[level].addContestant(recruits[i]);
                            worldData.ManagerProtocols[recruits[i]].attendTournament();
                        }
                    }
                }
            }
        }

        foreach (int tIndex in thisWeeksTournaments){
            List<int> recruits = recruit(worldData.Towns[tIndex].Tournament.TournamentLevel, worldData.Towns[tIndex].Location, false);

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

        if ((worldData.Calendar.getWeekOfYear() - 1) % 8 == 0)
        {
            foreach (Capitol c in worldData.Capitols)
            {
                foreach (TournamentProtocol.Level level in c.Quarterlies.Keys)
                {
                    if (c.Quarterlies[level].Attendees > 2)
                    {
                        //Debug.Log(c.Location.ToString() + " - " + c.Quarterlies[level].getDetails());
                        c.Quarterlies[level].scheduleTournament();
                        c.Quarterlies[level].SimWholeTournament(ref worldData);
                        //c.Quarterlies[level].logResults(ref worldData);
                    } else {
                        c.Quarterlies[level].cancelTournament(ref worldData);
                    }

                    c.Quarterlies[level].refreshTournament(true);
                }
            }
            //worldData.updateBoxerDistribution();
            //distribution();
        }

        foreach (int tIndex in thisWeeksTournaments)
        {
            if (worldData.Towns[tIndex].Tournament.Attendees > 2){
                //Debug.Log(worldData.Towns[tIndex].Location.ToString() + " - " + worldData.Towns[tIndex].Tournament.getDetails());
                worldData.Towns[tIndex].Tournament.scheduleTournament();
                worldData.Towns[tIndex].Tournament.SimWholeTournament(ref worldData);
            } else {
                worldData.Towns[tIndex].Tournament.cancelTournament(ref worldData);
            }

            worldData.Towns[tIndex].Tournament.refreshTournament(false);
        }
    }

    private List<int> recruit(TournamentProtocol.Level level, Vector2Int p1, bool highestRated){
        List<int> potentialRecruits = new List<int>();

        int index = 0;
        foreach (ManagerProtocol mp in worldData.ManagerProtocols)
        {
            Vector2Int p2 = worldData.Towns[worldData.Managers[mp.ManagerIndex].TownIndex].Location;

            float distance = Mathf.Sqrt(Mathf.Pow((p2.x - p1.x), 2) + Mathf.Pow((p2.y - p1.y), 2));

            if (level.Equals(TournamentProtocol.Level.S))
            {
                if (mp.Rank.Equals(level) && !mp.isBusy())
                    potentialRecruits.Add(index);
            }
            else
            {
                if (distance < getDistanceFromLevel(level) && mp.Rank.Equals(level) && !mp.isBusy())
                    potentialRecruits.Add(index);
            }

            index++;
        }

        if (highestRated)
            potentialRecruits = potentialRecruits.OrderByDescending(t => EvaluationProtocol.evaluateBoxer(worldData.Boxers[t])).ToList();
        else
            potentialRecruits = potentialRecruits.OrderByDescending(t => worldData.ManagerProtocols[t].TournamentPriority).ToList();
        
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
            return 1200.0f;
        else if (level.Equals(TournamentProtocol.Level.A))
            return 900.0f;
        else if (level.Equals(TournamentProtocol.Level.B))
            return 800.0f;
        else if (level.Equals(TournamentProtocol.Level.C))
            return 700.0f;
        else if (level.Equals(TournamentProtocol.Level.D))
            return 700.0f;
        else if (level.Equals(TournamentProtocol.Level.E))
            return 700.0f;

        return 1200.0f;
    }

    public void progressWeek(){
        worldData.Calendar.progessWeek();
        Debug.Log(worldData.Calendar.getDate(Calendar.DateType.fullLong));
        weeklyTest();
        worldData.updateBoxerDistribution();
    }

    public void progressFourYears(){

        for (int i = 0; i < 4; i++){
            for (int w = 0; w < 48; w++){
                worldData.Calendar.progessWeek();
                Debug.Log(worldData.Calendar.getDate(Calendar.DateType.fullLong));
                weeklyTest();
                worldData.updateBoxerDistribution();
            }
        }
    }

    public void distribution(){
        foreach (TournamentProtocol.Level rank in worldData.Distribution.Keys)
        {
            Debug.Log(rank.ToString() + " - " + worldData.Distribution[rank]);
        }
    }

    public void logWeekTournaments(){
        foreach (int tIndex in thisWeeksTournaments)
        {
            Debug.Log(worldData.Towns[tIndex].Location.ToString() + " - " + worldData.Towns[tIndex].Tournament.getDetails());
        }
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
