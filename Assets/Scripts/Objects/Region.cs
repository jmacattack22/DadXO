using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region {

    private Vector2Int position;
    private string regionName;

    private int capitolIndex;
    private List<int> townIndexes;

    RegionCreator.TileType[,] worldMap;

    public Region(string name, Vector2Int pos){
        position = pos;
        regionName = name;

        capitolIndex = -1;
        townIndexes = new List<int>();
    }

    public void addCapitol(int capitol){
        capitolIndex = capitol;
    }

    public void addTown(int townIndex){
        townIndexes.Add(townIndex);
    }

    public void addWorldMap(RegionCreator.TileType[,] worldMap){
        this.worldMap = worldMap;
    }

    public List<int> getRegionsTownIndexes(){
        return townIndexes;
    }

    //Getters
    public string Name {
        get { return regionName; }
    }

    public Vector2Int Position {
        get { return position; }
    }

    public int CapitolIndex {
        get { return capitolIndex; }
    }

    public RegionCreator.TileType[,] Map {
        get { return worldMap; }
    }
}
