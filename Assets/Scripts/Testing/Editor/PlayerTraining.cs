using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class PlayerTraining {

	private DataPool worldData;

	private Facility strengthTrainingFacility;

	[Test]
	public void PlayerTraining_StrenthTraining() {
		worldData = new DataPool ();

		Boxer boxer = new Boxer (new Vector2Int(0,1), "Glass", "Joe", 0, 115.0f, BoxerClass.Type.Bullseye, 30, 2, 40, 2, 110, 2, 25, 2, 35, 2);
		worldData.Boxers.Add (boxer);

		string strengthExercise = "Punching Bag";

		worldData.addExerciseDescription (strengthExercise, "Strength Training");

		worldData.addExerciseProgress(strengthExercise, new List<int>(new int[]{ 0, 0, 0, 0, 4}));
		worldData.addExerciseProgress(strengthExercise, new List<int>(new int[]{ 0, 0, 0, 0, 1}));
		worldData.addExerciseProgress(strengthExercise, new List<int>(new int[]{ 0, 0, 0, 0, 1}));
		worldData.addExerciseProgress(strengthExercise, new List<int>(new int[]{ 0, 0, 0, 0, 1}));
		worldData.addExerciseProgress(strengthExercise, new List<int>(new int[]{ 0, 0, 0, 0, 1}));
		worldData.addExerciseProgress(strengthExercise, new List<int>(new int[]{ 0, 0, 0, 0, 1}));

		Exercise strengthTraining = new Exercise ("Punching Bag", worldData.getExerciseDescription("Punching Bag"), true);
		List<int> strengthTrainingFactors = worldData.getExerciseProgressAcculumative (strengthTraining.Name, 2);
		strengthTraining.setFactors (
			strengthTrainingFactors [0], strengthTrainingFactors [1], strengthTrainingFactors [2], 
			strengthTrainingFactors [3], strengthTrainingFactors [4], 20);

		strengthTrainingFacility = new Facility ();
		strengthTrainingFacility.createExerciseFacility (2, strengthTraining);

		Assert.IsNotNull (strengthTrainingFacility);
		Assert.IsNotNull (worldData);
		for (int i = 0; i < 10; i++) {
			strengthTrainingFacility.utilizeFacility (ref worldData, 0);
		}
	}
}
