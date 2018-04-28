using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentResult {

	private Record record;

	//TODO Stat Changes calculated from matches

	public TournamentResult(){
		record = new Record (120.0f);
	}

	//Getters
	public Record Record {
		get { return record; }
	}
}
