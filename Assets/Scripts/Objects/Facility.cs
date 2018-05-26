using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    public Facility(JSONObject json)
	{
		level = (int)json.GetField("level").i;
		type = (FacilityType)Enum.Parse(typeof(FacilityType), json.GetField("type").str);
		exercise = new Exercise(json.GetField("exercise"));
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

    public JSONObject jsonify()
	{
		JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

		json.AddField("level", level);
		json.AddField("type", type.ToString());
		json.AddField("exercise", exercise.jsonify());

		return json;
	}

	//Getters
	public int Level {
		get { return level; }
	}
}
