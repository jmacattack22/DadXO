using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region {
    
    private Vector2Int position;
    private string regionName;

	private int landMass;

	private Dictionary<int, int> distances;

    private int capitolIndex;
    private List<int> townIndexes;
    private List<int> managerProtocolIndexes;

    private TournamentProtocol.Level level;

    RegionCreator.TileType[,] worldMap;

    public Region(string name, Vector2Int pos){
        position = pos;
        regionName = name;
		landMass = 0;

		distances = new Dictionary<int, int>();

        capitolIndex = -1;
        managerProtocolIndexes = new List<int>();
        townIndexes = new List<int>();
    }

    public void addCapitol(int capitol){
        capitolIndex = capitol;
    }

    public void addDistance(int regionIndex, int distance)
	{
		if (!distances.ContainsKey(regionIndex))
		{
			distances.Add(regionIndex, distance);
		}
	}

    public void addManager(int managerIndex){
        managerProtocolIndexes.Add(managerIndex);
    }

    public void addTown(int townIndex){
        townIndexes.Add(townIndex);
    }

    public void addWorldMap(RegionCreator.TileType[,] worldMap){
        this.worldMap = worldMap;

		landMass = getLandmass();
    }

	public int getDistanceToRegion(int regionIndex)
	{
		return distances[regionIndex];
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

    private int getLandmass()
	{
		int mass = 0;
		for (int x = 0; x < worldMap.GetLength(0); x++)
		{
			for (int y = 0; y < worldMap.GetLength(0); y++)
			{
				if (!worldMap[x,y].Equals(RegionCreator.TileType.Water) && !worldMap[x, y].Equals(RegionCreator.TileType.Shallows))
				{
					mass++;
				}
			}
		}

		return mass;
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

    public int LandMass
	{
		get { return landMass; }
	}
}
