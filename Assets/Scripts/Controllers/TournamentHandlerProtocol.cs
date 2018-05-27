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

    private static int getDistanceFromLevel(TournamentProtocol.Level level){
        if (level.Equals(TournamentProtocol.Level.S))
            return 10;
        else if (level.Equals(TournamentProtocol.Level.A))
            return 4;
        else if (level.Equals(TournamentProtocol.Level.B))
            return 3;
        else if (level.Equals(TournamentProtocol.Level.C))
            return 3;
        else if (level.Equals(TournamentProtocol.Level.D))
            return 2;
        else if (level.Equals(TournamentProtocol.Level.E))
            return 2;

        return 10;
    }

    public static bool isQualifyingWeek(ref DataPool worldData){
        return ((worldData.Calendar.getWeekOfYear() - 1) % 8 == 0);
    }

    private static List<int> recruit(DataPool worldData, TournamentProtocol.Level level, List<int> regionsWithinJurisdiction, bool highestRated)
    {
        List<int> potentialRecruits = new List<int>();

        foreach (int rIndex in regionsWithinJurisdiction){
            foreach (int mIndex in worldData.Regions[rIndex].getRegionsManagerIndexes()){
                if (worldData.Managers[mIndex].Rank.Equals(level) && !worldData.Managers[mIndex].isBusy()){
                    potentialRecruits.Add(mIndex);   
                }
            }
        }

        if (highestRated)
            potentialRecruits = potentialRecruits.OrderByDescending(t => EvaluationProtocol.evaluateBoxer(worldData.Boxers[t])).ToList();
        else
            potentialRecruits = potentialRecruits.OrderByDescending(t => worldData.Managers[t].Priority).ToList();

        return potentialRecruits;
    }

    private static void recruitForQualifiers(ref DataPool worldData){
        foreach (Region region in worldData.Regions){
            foreach (TournamentProtocol.Level level in worldData.Capitols[region.CapitolIndex].Quarterlies.Keys){
                List<int> regionsWithinJurisdiction = new List<int>();

                for (int i = 0; i < worldData.Regions.Count; i++){
					if (region.CapitolIndex == worldData.Regions[i].CapitolIndex)
                    {
                        regionsWithinJurisdiction.Add(i);
                    }
                    else
                    {
                        int distance = region.getDistanceToRegion(i);

                        if (distance < getDistanceFromLevel(level))
                        {
                            regionsWithinJurisdiction.Add(i);
                        }   
                    }  
                }

                List<int> recruits = recruit(worldData, level, regionsWithinJurisdiction, true);

                for (int i = 0; i < recruits.Count; i++)
                {
                    if (worldData.Capitols[region.CapitolIndex].Quarterlies[level].spaceLeft())
                    {
                        worldData.Capitols[region.CapitolIndex].Quarterlies[level].addContestant(recruits[i]);
                        worldData.Managers[recruits[i]].attendTournament();
                    }
                }
            }
        }
    }

    private static List<int> recruitForTournaments(ref DataPool worldData){
        List<int> townTournaments = new List<int>();

        foreach (Region region in worldData.Regions){
            foreach (int tIndex in region.getRegionsTownIndexes()){
                if (worldData.Towns[tIndex].Tournament.TournamentDate.sameWeek(worldData.Calendar.GetCalendarDate())){
                    townTournaments.Add(tIndex);

                    List<int> regionsWithinJurisdiction = new List<int>();

                    for (int i = 0; i < worldData.Regions.Count; i++)
                    {
						if (region.CapitolIndex == worldData.Regions[i].CapitolIndex)
                        {
                            regionsWithinJurisdiction.Add(i);
                        }
                        else
                        {
                            int distance = region.getDistanceToRegion(i);

                            if (distance < getDistanceFromLevel(worldData.Towns[tIndex].Tournament.Level))
                            {
                                regionsWithinJurisdiction.Add(i);
                            }
                        }
                    }

                    List<int> recruits = recruit(worldData, worldData.Towns[tIndex].Tournament.Level, regionsWithinJurisdiction, false);

                    for (int i = 0; i < recruits.Count; i++)
                    {
                        if (worldData.Towns[tIndex].Tournament.spaceLeft())
                        {
                            worldData.Towns[tIndex].Tournament.addContestant(recruits[i]);
                            worldData.Managers[recruits[i]].attendTournament();
                        }
                        else
                        {
                            worldData.Managers[recruits[i]].bumpTournamentPriority();
                        }
                    }
                }
            }
        }

        return townTournaments;
    }

    private static void simQualifiers(ref DataPool worldData){
		for (int i = 0; i < worldData.Capitols.Count; i++)
		{
			
			foreach (TournamentProtocol.Level level in worldData.Capitols[i].Quarterlies.Keys)
			{
				if (worldData.Capitols[i].Quarterlies[level].Attendees > 2)
				{
					worldData.Capitols[i].Quarterlies[level].scheduleTournament();
					TournamentProtocol.simWholeQualifier(ref worldData, i, level);
				}
                else
				{
					TournamentProtocol.cancelTournament(ref worldData, i);
				}

				worldData.Capitols[i].Quarterlies[level].refreshTournament(true);
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
				TournamentProtocol.SimWholeTournament(ref worldData, townIndex);
            }
            else
            {
				TournamentProtocol.cancelTournament(ref worldData, townIndex);
            }

            worldData.Towns[townIndex].Tournament.refreshTournament(false);
        }
    }

    private static void simTraining(ref DataPool worldData){
		for (int index = 0; index < worldData.Managers.Count; index++)
		{
			ManagerProtocol.executeWeek(ref worldData, index);
		}
    }
}
