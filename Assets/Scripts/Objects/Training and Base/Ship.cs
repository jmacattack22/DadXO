using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Ship {

	public enum ShipClass
	{
		ship1, ship2, ship3
	}

	private ShipClass shipClass;

	private int speed;

	private Dictionary<ManagerProtocol.FacilityShortcut, Facility> shipFacilities;

	public Ship(ref DataPool worldData){
		shipFacilities = new Dictionary<ManagerProtocol.FacilityShortcut, Facility> ();

		shipClass = ShipClass.ship1;
	}

    public Ship(JSONObject json)
	{
		shipClass = (ShipClass)Enum.Parse(typeof(ShipClass), json.GetField("class").str);
		speed = (int)json.GetField("speed").i;

		shipFacilities = new Dictionary<ManagerProtocol.FacilityShortcut, Facility>();
		foreach (JSONObject record in json.GetField("facilities").list)
		{
			ManagerProtocol.FacilityShortcut facility =
				(ManagerProtocol.FacilityShortcut)Enum.Parse(typeof(ManagerProtocol.FacilityShortcut), record.GetField("key").str);
			shipFacilities.Add(facility, new Facility(record.GetField("value")));
		}
	}

	public void train(ref DataPool worldData, int boxerIndex, ManagerProtocol.FacilityShortcut training){
		shipFacilities [training].utilizeFacility (ref worldData, boxerIndex);
	}

    public JSONObject jsonify()
	{
		JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

        json.AddField("class", shipClass.ToString());
		json.AddField("speed", speed);

        JSONObject facilities = new JSONObject(JSONObject.Type.ARRAY);
        foreach (ManagerProtocol.FacilityShortcut key in shipFacilities.Keys)
        {
            JSONObject record = new JSONObject(JSONObject.Type.OBJECT);
            record.AddField("key", key.ToString());
            record.AddField("value", shipFacilities[key].jsonify());
            facilities.Add(record);
        }
        json.AddField("facilities", facilities);

        return json;
	}

	//Getters
	public ShipClass Clazz {
		get { return shipClass; }
	}

	public int Speed {
		get { return speed; }
	}
}
