using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : Person {

	private BoxerClass.Type preference;

	private Record record;

	public Manager(
		string fName, string lName, int townId, float wt, BoxerClass.Type pref
	) : base(new Vector2Int(1,1), fName, lName, townId, wt){
		preference = pref;

		record = new Record (40.0f);
	}

    public string getDetails(){
        return FirstName + " " + LastName + ", W " + record.Wins + " L " + record.Losses + ", Preference " + preference.ToString();
    }

	//Getters
	public BoxerClass.Type Preference {
		get { return preference; }
	}

	public Record Record {
		get { return record; }
	}
}
