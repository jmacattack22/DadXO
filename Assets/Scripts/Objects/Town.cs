using System;
using UnityEngine;

public class Town
{
	private string name;
	private Vector2Int location;
    private TournamentProtocol.Level regionLevel;
    private TournamentProtocol tournament;

	public Town (string name, Vector2Int loc, int regionDistance)
	{
		this.name = name;
		location = loc;

        determineRegion(regionDistance);
	}

    private void determineRegion(int regionDistance)
    {
        if (regionDistance < 2)
        {
            regionLevel = TournamentProtocol.Level.E;
        }
        else if (regionDistance < 4)
        {
            regionLevel = TournamentProtocol.Level.D;
        }
        else if (regionDistance < 6)
        {
            regionLevel = TournamentProtocol.Level.C;
        }
        else if (regionDistance < 7)
        {
            regionLevel = TournamentProtocol.Level.B;
        }
        else if (regionDistance < 8)
        {
            regionLevel = TournamentProtocol.Level.A;
        } else 
        {
            regionLevel = TournamentProtocol.Level.S;
        }
    }

    public void setTournament(TournamentProtocol tournamentProtocol){
        tournament = tournamentProtocol;
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

    public TournamentProtocol Tournament {
        get { return tournament; }
    }
}

