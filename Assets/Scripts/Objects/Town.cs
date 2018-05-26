using System;
using UnityEngine;

public class Town
{
	private string name;
	private Vector2Int location;
    private TournamentProtocol.Level regionLevel;
	private Tournament tournament;

	public Town (string name, Vector2Int loc, int regionDistance)
	{
		this.name = name;
		location = loc;

        determineRegion(regionDistance);
	}

    public Town (string name, Vector2Int loc, TournamentProtocol.Level level)
	{
		this.name = name;
		this.location = loc;
		this.regionLevel = level;
	}

    public Town(JSONObject json)
	{
		name = json.GetField("name").str;
		location = JSONTemplates.ToVector2Int(json.GetField("location"));
		regionLevel = (TournamentProtocol.Level)Enum.Parse(typeof(TournamentProtocol.Level), json.GetField("level").str);

		tournament = new Tournament(json.GetField("tournament"));
	}

    public void changeName(string newName)
	{
		this.name = newName;
	}

    private void determineRegion(int regionDistance)
    {
        if (regionDistance < 2)
        {
            regionLevel = TournamentProtocol.Level.E;
        }
        else if (regionDistance < 3)
        {
            regionLevel = TournamentProtocol.Level.D;
        }
        else if (regionDistance < 4)
        {
            regionLevel = TournamentProtocol.Level.C;
        }
        else if (regionDistance < 5)
        {
            regionLevel = TournamentProtocol.Level.B;
        }
        else if (regionDistance < 6)
        {
            regionLevel = TournamentProtocol.Level.A;
        } else 
        {
            regionLevel = TournamentProtocol.Level.S;
        }
    }

    public void setTournament(Tournament tournament)
	{
		this.tournament = tournament;
	}

    public JSONObject jsonify()
	{
		JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

		json.AddField("name", name);
		json.AddField("location", JSONTemplates.FromVector2Int(location));
		json.AddField("level", regionLevel.ToString());
        if (tournament != null)
		    json.AddField("tournament", tournament.jsonify());

		return json;
	}

	//Getters
	public string Name {
		get { return name; }
	}

	public Vector2Int Location {
		get { return location; }
	}

    public TournamentProtocol.Level RegionLevel {
        get { return regionLevel; }
    }

    public Tournament Tournament {
        get { return tournament; }
    }
}

