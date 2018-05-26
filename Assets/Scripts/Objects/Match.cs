using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Match {

	private CalendarDate date;

	private int opponentIndex;

	public Match(CalendarDate date, int oppIndex){
		this.date = date;

		this.opponentIndex = oppIndex;
	}

	//Getters
	public CalendarDate Date {
		get { return date; }
	}

	public int Opponent {
		get { return opponentIndex; }
	}
}
