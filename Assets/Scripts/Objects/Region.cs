using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region {

    private Vector2Int position;
    private string regionName;

    private int capitolIndex;
    private List<int> townIndexes;
    private List<int> managerProtocolIndexes;

    private TournamentProtocol.Level level;

    RegionCreator.TileType[,] worldMap;

    public Region(string name, Vector2Int pos){
        position = pos;
        regionName = name;

        capitolIndex = -1;
        managerProtocolIndexes = new List<int>();
        townIndexes = new List<int>();
    }

    public void addCapitol(int capitol){
        capitolIndex = capitol;
    }

    public void addManager(int managerIndex){
        managerProtocolIndexes.Add(managerIndex);
    }

    public void addTown(int townIndex){
        townIndexes.Add(townIndex);
    }

    public void addWorldMap(RegionCreator.TileType[,] worldMap){
        this.worldMap = worldMap;
    }

    private void determineRegionLevel(int regionDistance)
    {
        if (regionDistance < 2)
        {
            level = TournamentProtocol.Level.E;
        }
        else if (regionDistance < 3)
        {
            level = TournamentProtocol.Level.D;
        }
        else if (regionDistance < 4)
        {
            level = TournamentProtocol.Level.C;
        }
        else if (regionDistance < 5)
        {
            level = TournamentProtocol.Level.B;
        }
        else if (regionDistance < 6)
        {
            level = TournamentProtocol.Level.A;
        }
        else
        {
            level = TournamentProtocol.Level.S;
        }
    }

    public List<int> getRegionsManagerIndexes(){
        return managerProtocolIndexes;
    }

    public List<int> getRegionsTownIndexes(){
        return townIndexes;
    }

    public void setLevel(int regionDistance){
        determineRegionLevel(regionDistance);
    }

    //Getters
    public string Name {
        get { return regionName; }
    }

    public TournamentProtocol.Level Level {
        get { return level; }
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
