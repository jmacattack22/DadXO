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
                            worldData.Managers[recruits[i]].attendTournament();
                        }
                    }
                }
            }
        }

        foreach (int tIndex in thisWeeksTournaments){
            List<int> recruits = recruit(worldData.Towns[tIndex].Tournament.Level, worldData.Towns[tIndex].Location, false);

            for (int i = 0; i < recruits.Count; i++){
                if (worldData.Towns[tIndex].Tournament.spaceLeft()){
                    worldData.Towns[tIndex].Tournament.addContestant(recruits[i]);
                    worldData.Managers[recruits[i]].attendTournament();
                } else{
                    worldData.Managers[recruits[i]].bumpTournamentPriority();
                }
            }
        }

		for (int i = 0; i < worldData.Managers.Count; i++)
		{
			ManagerProtocol.executeWeek(ref worldData, i);
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
                        //c.Quarterlies[level].SimWholeTournament(ref worldData);
                        //c.Quarterlies[level].logResults(ref worldData);
                    } else {
                        //c.Quarterlies[level].cancelTournament(ref worldData);
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
                //worldData.Towns[tIndex].Tournament.SimWholeTournament(ref worldData);
            } else {
                //worldData.Towns[tIndex].Tournament.cancelTournament(ref worldData);
            }

            worldData.Towns[tIndex].Tournament.refreshTournament(false);
        }
    }

    private List<int> recruit(TournamentProtocol.Level level, Vector2Int p1, bool highestRated){
        List<int> potentialRecruits = new List<int>();

        int index = 0;
        foreach (Manager m in worldData.Managers)
        {
            Vector2Int p2 = worldData.Towns[worldData.Managers[index].TownIndex].Location;

            float distance = Mathf.Sqrt(Mathf.Pow((p2.x - p1.x), 2) + Mathf.Pow((p2.y - p1.y), 2));

            if (level.Equals(TournamentProtocol.Level.S))
            {
                if (m.Rank.Equals(level) && !m.isBusy())
                    potentialRecruits.Add(index);
            }
            else
            {
                if (distance < getDistanceFromLevel(level) && m.Rank.Equals(level) && !m.isBusy())
                    potentialRecruits.Add(index);
            }

            index++;
        }

        if (highestRated)
            potentialRecruits = potentialRecruits.OrderByDescending(t => EvaluationProtocol.evaluateBoxer(worldData.Boxers[t])).ToList();
        else
            potentialRecruits = potentialRecruits.OrderByDescending(t => worldData.Managers[t].Priority).ToList();
        
        return potentialRecruits;
    }

    public void logBoxerResults(){
        foreach (Manager m in worldData.Managers){
            worldData.Boxers[m.BoxerIndex].logBoxerStats(m.Rank);
        }
    }

    public void logManagerResults(){
        foreach (Manager m in worldData.Managers)
        {
            m.logManagerStats();
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
            if (!tournamentCounts.ContainsKey(t.Tournament.Level))
            {
                tournamentCounts.Add(t.Tournament.Level, 0);
            }

            tournamentCounts[t.Tournament.Level] += 1;
            
            if (!foundIndex)
            {
                if (t.Tournament.Level == TournamentProtocol.Level.C)
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

        List<Manager> potentialContestants = new List<Manager>();

		index = 0;
        foreach (Manager m in worldData.Managers)
        {
            Vector2Int p2 = worldData.Towns[worldData.Managers[index].TownIndex].Location;

            float distance = Mathf.Sqrt(Mathf.Pow((p2.x - p1.x), 2) + Mathf.Pow((p2.y - p1.y), 2));

            if (distance < 150 && m.Rank.Equals(TournamentProtocol.Level.C))
                potentialContestants.Add(m);

			index++;
        }

        Debug.Log(potentialContestants.Count);

		index = 0;
        foreach (Manager m in potentialContestants)
        {
            Debug.Log(worldData.Managers[index].FirstName);
			index++;
        }
    }
}
