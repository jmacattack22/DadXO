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
}
