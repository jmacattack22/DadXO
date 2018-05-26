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

    public TournamentResult(JSONObject json)
	{
		quarterlyWin = json.GetField("quarterlywin").b;
		record = new Record(json.GetField("record"));
	}

    public void wonQuarterly(){
        quarterlyWin = true;
    }

    public JSONObject jsonify()
	{
		JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

		json.AddField("record", record.jsonify());
		json.AddField("quaerterlywin", quarterlyWin);

		return json;
	}

	//Getters
	public Record Record {
		get { return record; }
	}

    public bool QuarterlyWin {
        get { return quarterlyWin; }
    }
}
