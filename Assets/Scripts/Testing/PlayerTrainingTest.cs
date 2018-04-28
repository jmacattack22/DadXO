using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrainingTest : MonoBehaviour {

	private DataPool worldData;

	private Facility accurracyTrainingFacility;
	private Facility enduranceTrainingFacility;
	private Facility healthTrainingFacility;
	private Facility speedTrainingFacility;
	private Facility strengthTrainingFacility;

	void Start () {
		worldData = new DataPool ();

		WorldBuilderProtocol.createWorld (ref worldData, 220, 220);

        List<Boxer> boxers = WorldBuilderProtocol.generateBoxerRecruits (ref worldData, 0, 500);

		foreach (Boxer b in boxers) {
			b.logBoxerStats ();
		}

		Debug.Log ("-----------");
		Boxer boxer = boxers [Random.Range (0, boxers.Count)];
		Debug.Log ("Introducing ");

		worldData.Boxers.Add (boxer);

		Exercise accuracyTraining = new Exercise ("Double End Bag", worldData.getExerciseDescription ("Double End Bag"));
		Exercise enduranceTraining = new Exercise ("Punch Glove", worldData.getExerciseDescription ("Punch Glove"));
		Exercise speedTraining = new Exercise ("Sprints", worldData.getExerciseDescription ("Sprints"));
		Exercise strengthTraining = new Exercise ("Punching Bag", worldData.getExerciseDescription("Punching Bag"));
		Exercise healthTraining = new Exercise ("Laps", worldData.getExerciseDescription ("Laps"));

		List<int> accuracyTrainingFactors = worldData.getExerciseProgressAcculumative (accuracyTraining.Name, 2);
		List<int> enduranceTrainingFactors = worldData.getExerciseProgressAcculumative (enduranceTraining.Name, 2);
		List<int> speedTrainingFactors = worldData.getExerciseProgressAcculumative (speedTraining.Name, 2);
		List<int> strengthTrainingFactors = worldData.getExerciseProgressAcculumative (strengthTraining.Name, 2);
		List<int> healthTrainingFactors = worldData.getExerciseProgressAcculumative (healthTraining.Name, 3);

		accuracyTraining.setFactors (
			accuracyTrainingFactors [0], accuracyTrainingFactors [1], accuracyTrainingFactors [2],
			accuracyTrainingFactors [3], accuracyTrainingFactors [4], 20);
		enduranceTraining.setFactors (
			enduranceTrainingFactors [0], enduranceTrainingFactors [1], enduranceTrainingFactors [2],
			enduranceTrainingFactors [3], enduranceTrainingFactors [4], 20);
		speedTraining.setFactors (
			speedTrainingFactors [0], speedTrainingFactors [1], speedTrainingFactors [2],
			speedTrainingFactors [3], speedTrainingFactors [4], 20);
		strengthTraining.setFactors (
			strengthTrainingFactors [0], strengthTrainingFactors [1], strengthTrainingFactors [2], 
			strengthTrainingFactors [3], strengthTrainingFactors [4], 20);
		healthTraining.setFactors(
			healthTrainingFactors [0], healthTrainingFactors [1], healthTrainingFactors [2], 
			healthTrainingFactors [3], healthTrainingFactors [4], 20);

		accurracyTrainingFacility = new Facility ();
		accurracyTrainingFacility.createExerciseFacility (2, accuracyTraining);

		enduranceTrainingFacility = new Facility ();
		enduranceTrainingFacility.createExerciseFacility (2, enduranceTraining);

		healthTrainingFacility = new Facility ();
		healthTrainingFacility.createExerciseFacility (3, healthTraining);

		speedTrainingFacility = new Facility ();
		speedTrainingFacility.createExerciseFacility (2, speedTraining);

		strengthTrainingFacility = new Facility ();
		strengthTrainingFacility.createExerciseFacility (2, strengthTraining);

		//trainStrength ();
		trainEverything();
	}

	void trainStrength(){
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 12; j++) {
				worldData.Boxers [0].logBoxerStats ();
				strengthTrainingFacility.utilizeFacility (ref worldData, 0);

				worldData.Boxers [0].ageWeek ();
			}

			for (int j = 0; j < 30; j++) {
				worldData.Boxers [0].ageWeek ();
			}
		}

		worldData.Boxers [0].logBoxerStats ();
	}

	void trainEverything(){
		for (int i = 0; i < 4; i++) {
			for (int j = 0; j < 48; j++) {
				worldData.Boxers [0].logBoxerStats ();

				if (!worldData.Boxers [0].isBoxerFatigued ()) {
					if (j % 5 == 0)
						accurracyTrainingFacility.utilizeFacility (ref worldData, 0);
					else if (j % 5 == 1)
						enduranceTrainingFacility.utilizeFacility (ref worldData, 0);
					else if (j % 5 == 2)
						speedTrainingFacility.utilizeFacility (ref worldData, 0);
					else if (j % 5 == 3)
						strengthTrainingFacility.utilizeFacility (ref worldData, 0);
					else if (j % 5 == 4)
						healthTrainingFacility.utilizeFacility (ref worldData, 0);
				} else {
					worldData.Boxers [0].rest ();
				}

				worldData.Boxers [0].ageWeek ();
			}
		}

		worldData.Boxers [0].logBoxerStats ();
	}

}
