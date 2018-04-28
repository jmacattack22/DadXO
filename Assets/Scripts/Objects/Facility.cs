using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Facility {

	public enum FacilityType {
		Exercise, Kitchen, Chambers
	}

	private int level;
	private FacilityType type;

	private Exercise exercise;

	public Facility(){
		exercise = null;
	}

	public void createExerciseFacility(int facilityLevel, Exercise exercise){
		this.exercise = exercise;
		level = facilityLevel;
		type = FacilityType.Exercise;
	}

	public void createKitchenFacility(){
		type = FacilityType.Kitchen;
	}

	public void createChamberFacility(){
		type = FacilityType.Chambers;
	}

	public string getFacilityDescription(){
		if (type.Equals (FacilityType.Exercise))
			return exercise.Description;

		return "";
	}

	public string getFacilityName() {
		if (type.Equals(FacilityType.Exercise))
			return exercise.Name + " lvl " + level;

		return "";
	}

	private void train(ref DataPool worldData, int boxerIndex){
		TrainingResult results = exercise.train (ref worldData, boxerIndex);
		worldData.Boxers [boxerIndex].applyTrainingResults (results);
		//results.logTrainingResult ();
	}

	public void upgradeFacility(ref DataPool worldData){
		if (type.Equals (FacilityType.Exercise))
			upgradeExercise (ref worldData);
	}

	private void upgradeExercise(ref DataPool worldData){
		level++;

		exercise.upgradeExercise (worldData.getExerciseProgress (exercise.Name, level));
	}

	public void utilizeFacility(ref DataPool worldData, int boxerIndex){
		if (type.Equals(FacilityType.Exercise))
			train(ref worldData, boxerIndex);
	}

	//Getters
	public int Level {
		get { return level; }
	}
}
