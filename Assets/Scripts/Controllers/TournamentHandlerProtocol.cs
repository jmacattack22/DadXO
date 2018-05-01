using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class TournamentHandlerProtocol {

    public static void simTournamentsAndTraining(ref DataPool worldData)
    {
        if (isQualifyingWeek(ref worldData))
        {
            recruitForQualifiers(ref worldData);
        }

        List<int> townTournaments = recruitForTournaments(ref worldData);

        simTraining(ref worldData);

        if (isQualifyingWeek(ref worldData))
        {
            simQualifiers(ref worldData);
        }

        simTournaments(ref worldData, townTournaments);
    }

    private static float getDistanceFromLevel(TournamentProtocol.Level level)
    {
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

    public static bool isQualifyingWeek(ref DataPool worldData){
        return ((worldData.Calendar.getWeekOfYear() - 1) % 8 == 0);
    }

    private static List<int> recruit(DataPool worldData, TournamentProtocol.Level level, Vector2Int p1, bool highestRated)
    {
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

    private static void recruitForQualifiers(ref DataPool worldData){
        foreach (Capitol c in worldData.Capitols)
        {
            foreach (TournamentProtocol.Level level in c.Quarterlies.Keys)
            {
                List<int> recruits = recruit(worldData, level, c.Location, true);

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

    private static List<int> recruitForTournaments(ref DataPool worldData){
        List<int> townTournaments = new List<int>();

        int townIndex = 0;
        foreach (Town town in worldData.Towns)
        {
            if (town.Tournament.TournamentDate.sameWeek(worldData.Calendar.GetCalendarDate()))
            {
                townTournaments.Add(townIndex);

                List<int> recruits = recruit(worldData, worldData.Towns[townIndex].Tournament.TournamentLevel, worldData.Towns[townIndex].Location, false);

                for (int i = 0; i < recruits.Count; i++)
                {
                    if (worldData.Towns[townIndex].Tournament.spaceLeft())
                    {
                        worldData.Towns[townIndex].Tournament.addContestant(recruits[i]);
                        worldData.ManagerProtocols[recruits[i]].attendTournament();
                    }
                    else
                    {
                        worldData.ManagerProtocols[recruits[i]].bumpTournamentPriority();
                    }
                }
            }
            townIndex++;
        }

        return townTournaments;
    }

    private static void simQualifiers(ref DataPool worldData){
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
                }
                else
                {
                    c.Quarterlies[level].cancelTournament(ref worldData);
                }

                c.Quarterlies[level].refreshTournament(true);
            }
        }
    }

    private static void simTournaments(ref DataPool worldData, List<int> townTournaments){
        foreach (int townIndex in townTournaments)
        {
            if (worldData.Towns[townIndex].Tournament.Attendees > 2)
            {
                //Debug.Log(worldData.Towns[tIndex].Location.ToString() + " - " + worldData.Towns[tIndex].Tournament.getDetails());
                worldData.Towns[townIndex].Tournament.scheduleTournament();
                worldData.Towns[townIndex].Tournament.SimWholeTournament(ref worldData);
            }
            else
            {
                worldData.Towns[townIndex].Tournament.cancelTournament(ref worldData);
            }

            worldData.Towns[townIndex].Tournament.refreshTournament(false);
        }
    }

    private static void simTraining(ref DataPool worldData){
        foreach (ManagerProtocol mp in worldData.ManagerProtocols)
        {
            mp.executeWeek(ref worldData);
        }
    }
}
