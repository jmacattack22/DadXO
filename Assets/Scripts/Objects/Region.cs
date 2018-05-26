using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

	public Region(JSONObject json)
	{
		position = JSONTemplates.ToVector2Int(json.GetField("position"));
		regionName = json.GetField("name").str;
		landMass = (int)json.GetField("landmass").i;
		capitolIndex = (int)json.GetField("capitolindex").i;
		level = (TournamentProtocol.Level)Enum.Parse(typeof(TournamentProtocol.Level), json.GetField("level").str);

		distances = new Dictionary<int, int>();
		foreach (JSONObject record in json.GetField("distances").list)
		{
			distances.Add((int)record.GetField("key").i, (int)record.GetField("value").i);
		}

		townIndexes = new List<int>();
        foreach (JSONObject record in json.GetField("townindexes").list)
		{
			townIndexes.Add((int)record.i);
		}

		managerProtocolIndexes = new List<int>();
        foreach (JSONObject record in json.GetField("managerindexes").list)
		{
			managerProtocolIndexes.Add((int)record.i);
		}
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

    public JSONObject jsonify()
	{
		JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

		json.AddField("position", JSONTemplates.FromVector2Int(position));
		json.AddField("name", regionName);
		json.AddField("landmass", landMass);
		json.AddField("capitolindex", capitolIndex);
		json.AddField("level", level.ToString());

		JSONObject distance = new JSONObject(JSONObject.Type.ARRAY);
        foreach (int key in distances.Keys)
		{
			JSONObject record = new JSONObject(JSONObject.Type.OBJECT);

			record.AddField("key", key);
			record.AddField("value", distances[key]);

			distance.Add(record);
		}
		json.AddField("distances", distance);

		JSONObject town = new JSONObject(JSONObject.Type.ARRAY);
        foreach (int index in townIndexes)
		{
			town.Add(index);
		}
		json.AddField("townindexes", town);

		JSONObject manager = new JSONObject(JSONObject.Type.ARRAY);
        foreach (int index in managerProtocolIndexes)
		{
			manager.Add(index);
		}
		json.AddField("managerindexes", manager);

		return json;
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
