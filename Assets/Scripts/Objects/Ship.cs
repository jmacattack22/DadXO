using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public void train(ref DataPool worldData, int boxerIndex, ManagerProtocol.FacilityShortcut training){
		shipFacilities [training].utilizeFacility (ref worldData, boxerIndex);
	}

	//Getters
	public ShipClass Clazz {
		get { return shipClass; }
	}

	public int Speed {
		get { return speed; }
	}
}
