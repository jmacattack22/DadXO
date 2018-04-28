using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Homebase {

	private Dictionary<ManagerProtocol.FacilityShortcut, Facility> homeBaseFacilities;

	private int level;

	public Homebase(ref DataPool worldData){
		level = 1;

		homeBaseFacilities = new Dictionary<ManagerProtocol.FacilityShortcut, Facility> ();

		initializeBasicFacilities (ref worldData);
	}

	public void initializeBasicFacilities(ref DataPool worldData){
		initializeHomebaseFacility (ref worldData, ManagerProtocol.FacilityShortcut.DoubleEndBag);
		initializeHomebaseFacility (ref worldData, ManagerProtocol.FacilityShortcut.PunchGlove);
		initializeHomebaseFacility (ref worldData, ManagerProtocol.FacilityShortcut.Laps);
		initializeHomebaseFacility (ref worldData, ManagerProtocol.FacilityShortcut.Sprints);
		initializeHomebaseFacility (ref worldData, ManagerProtocol.FacilityShortcut.PunchingBag);
		//TODO Kitchen
		//TODO ETC
		//TODO Ship Facilities
	}

	private void initializeHomebaseFacility(ref DataPool worldData, ManagerProtocol.FacilityShortcut facility){
		Exercise training = new Exercise ("", "");

		if (facility.Equals (ManagerProtocol.FacilityShortcut.DoubleEndBag)) 
			training = new Exercise ("Double End Bag", worldData.getExerciseDescription ("Double End Bag"));
		else if (facility.Equals(ManagerProtocol.FacilityShortcut.PunchGlove))
			training = new Exercise ("Punch Glove", worldData.getExerciseDescription ("Punch Glove"));
		else if (facility.Equals(ManagerProtocol.FacilityShortcut.Laps))
			training = new Exercise ("Laps", worldData.getExerciseDescription ("Laps"));
		else if (facility.Equals(ManagerProtocol.FacilityShortcut.Sprints))
			training = new Exercise ("Sprints", worldData.getExerciseDescription ("Sprints"));
		else if (facility.Equals(ManagerProtocol.FacilityShortcut.PunchingBag))
			training = new Exercise ("Punching Bag", worldData.getExerciseDescription ("Punching Bag"));

		List<int> trainingFactors = worldData.getExerciseProgressAcculumative (training.Name, 1);

		training.setFactors (
			trainingFactors [0], trainingFactors [1], trainingFactors [2],
			trainingFactors [3], trainingFactors [4], 20);

		Facility newFacility = new Facility();
		newFacility.createExerciseFacility (1, training);
		homeBaseFacilities.Add (facility, newFacility);
	}

	public void train(ref DataPool worldData, int boxerIndex, ManagerProtocol.FacilityShortcut training){
		homeBaseFacilities [training].utilizeFacility (ref worldData, boxerIndex);
	}

	public void upgradeFacilities(ref DataPool worldData, float elo, BoxerClass.Type preference){
		int pointsToGive = EvaluationProtocol.getFacilityPointsFromElo (elo);

		List<ManagerProtocol.FacilityShortcut> upgrades = new List<ManagerProtocol.FacilityShortcut> ( new ManagerProtocol.FacilityShortcut[] {
			ManagerProtocol.FacilityShortcut.DoubleEndBag, ManagerProtocol.FacilityShortcut.PunchGlove, ManagerProtocol.FacilityShortcut.Laps, 
			ManagerProtocol.FacilityShortcut.Sprints, ManagerProtocol.FacilityShortcut.PunchingBag
		});

		List<EvaluationProtocol.Stats> bestStats = BoxerClass.getBuild (preference);

		foreach (EvaluationProtocol.Stats stat in bestStats) {
			if (stat.Equals (EvaluationProtocol.Stats.AccuracyGrowth))
				upgrades.Add(ManagerProtocol.FacilityShortcut.DoubleEndBag);
			else if (stat.Equals (EvaluationProtocol.Stats.EnduranceGrowth))
				upgrades.Add(ManagerProtocol.FacilityShortcut.PunchGlove);
			else if (stat.Equals (EvaluationProtocol.Stats.HealthGrowth))
				upgrades.Add(ManagerProtocol.FacilityShortcut.Laps);
			else if (stat.Equals (EvaluationProtocol.Stats.SpeedGrowth))
				upgrades.Add(ManagerProtocol.FacilityShortcut.Sprints);
			else if (stat.Equals (EvaluationProtocol.Stats.StrengthGrowth))
				upgrades.Add(ManagerProtocol.FacilityShortcut.PunchingBag);
		}

		for (int i = 0; i < pointsToGive; i++) {
			if (upgrades.Count > 0) {
				ManagerProtocol.FacilityShortcut upgrade = upgrades [Random.Range (0, upgrades.Count)];

				homeBaseFacilities [upgrade].upgradeFacility (ref worldData);

				if (homeBaseFacilities [upgrade].Level == 5) {
					upgrades.Remove (upgrade);

					if (upgrades.IndexOf(upgrade) >= 0)
						upgrades.Remove (upgrade);
				}
			}
		}
	}
}
