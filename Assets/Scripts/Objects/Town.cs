using System;
using UnityEngine;

public class Town
{
	private string name;
	private Vector2Int location;
    private TournamentProtocol.Level regionLevel;
    private TournamentProtocol tournament;

	public Town (string name, Vector2Int loc)
	{
		this.name = name;
		location = loc;

        determineRegion();
	}

    private void determineRegion()
    {
        int x = (location.x - (location.x % 220)) / 220;
        int y = (location.y - (location.y % 220)) / 220;

        if ((x == 0 && y == 0) || (x == 0 && y == 4) || (x == 4 && y == 0) || (x == 4 && y == 4))
        {
            regionLevel = TournamentProtocol.Level.S;
        }
        else if ((x == 0 && y == 1) || (x == 0 && y == 3) || (x == 1 && y == 0) || (x == 1 && y == 4) ||
                 (x == 3 && y == 0) || (x == 3 && y == 4) || (x == 4 && y == 1) || (x == 4 && y == 3))
        {
            regionLevel = TournamentProtocol.Level.A;
        }
        else if ((x == 0 && y == 2) | (x == 2 && y == 0) || (x == 2 && y == 4) || (x == 4 && y == 2)) {
            regionLevel = TournamentProtocol.Level.B;
        } 
        else if ((x == 1 && y == 1) || (x == 1 && y == 3) || (x == 3 && y == 1) || (x == 3 && y == 3))
        {
            regionLevel = TournamentProtocol.Level.C;
        } 
        else if ((x == 1 && y == 2) || (x == 2 && y == 1) || (x == 2 && y == 3) || (x == 2 && y == 3) || (x == 3 && y == 2))
        {
            regionLevel = TournamentProtocol.Level.D;
        }
        else if (x == 2 && y == 2)
        {
            regionLevel = TournamentProtocol.Level.E;
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

