using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentResult {

	private Record record;

    private bool quarterlyWin;

	//TODO Stat Changes calculated from matches

	public TournamentResult(){
		record = new Record (120.0f);
        quarterlyWin = false;
	}

    public void wonQuarterly(){
        quarterlyWin = true;
    }

	//Getters
	public Record Record {
		get { return record; }
	}

    public bool QuarterlyWin {
        get { return quarterlyWin; }
    }
}
