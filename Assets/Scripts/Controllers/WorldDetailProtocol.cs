using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldDetailProtocol {

    public static List<int> getBoxersOfRank(ref DataPool worldData, TournamentProtocol.Level level){
        List<int> boxerIndexes = new List<int>();

        for (int i = 0; i < worldData.Managers.Count; i++){
            if (worldData.Managers[i].Rank.Equals(level)){
                boxerIndexes.Add(worldData.Managers[i].BoxerIndex);
            }
        }
        
        return boxerIndexes;
    }

    public static List<int> getBoxersOfClass(ref DataPool worldData, BoxerClass.Type type){
        List<int> boxerIndexes = new List<int>();

        for (int i = 0; i < worldData.Boxers.Count; i++){
            if (worldData.Boxers[i].BoxerClass.Equals(type)){
                boxerIndexes.Add(i);
            }
        }

        return boxerIndexes;
    }

    public static List<int> getManagersOfRank(ref DataPool worldData, TournamentProtocol.Level level){
        List<int> managerIndexes = new List<int>();
        
        for (int i = 0; i < worldData.Managers.Count; i++)
        {
            if (worldData.Managers[i].Rank.Equals(level))
            {
                managerIndexes.Add(i);
            }
        }

        return managerIndexes;
    }

	public static Dictionary<TournamentProtocol.Level, Dictionary<int, List<int>>> getRegionTournamentsForMonth(
	    ref DataPool worldData, int regionIndex, int monthModifier)
	{
		Dictionary<TournamentProtocol.Level, Dictionary<int, List<int>>> regionTournaments = 
			new Dictionary<TournamentProtocol.Level, Dictionary<int, List<int>>>();
		
        foreach (TournamentProtocol.Level level in TournamentProtocol.getLevels())
		{
			regionTournaments.Add(level, new Dictionary<int, List<int>>());

			for (int i = 1; i <= 4; i++)
			{
				regionTournaments[level].Add(i, new List<int>());
			}
		}      

		int month = worldData.Calendar.Month + monthModifier;
		
		foreach (int index in worldData.Regions[regionIndex].getRegionsTownIndexes())
        {
            Tournament tournament = worldData.Towns[index].Tournament;
			if (tournament.TournamentDate.Month.Equals(month))
			{
				regionTournaments[tournament.Level][tournament.TournamentDate.Week].Add(index);
			}
        }

		return regionTournaments;
	}
}
