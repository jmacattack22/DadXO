using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingResult {

	public enum Outcome {
		Excellent, Failure, Success
	}

	private int accuracy;
	private int endurance;
	private int health;
	private int speed;
	private int strength;

	private Outcome outcome;

	private int fatigue;

	public TrainingResult(Outcome outcome, int acc, int end, int hlt, int spd, int str, int ftg){
		this.outcome = outcome;

		accuracy = acc;
		endurance = end;
		health = hlt;
		speed = spd;
		strength = str;

		fatigue = ftg;
	}

	public void logTrainingResult(){
		Debug.Log (outcome.ToString () + " - Acc " + accuracy + ", End " + endurance + ", Hlt " + health + ", Spd " + speed + ", Str " + strength);
	}

	//Getters
	public int Accuracy {
		get { return accuracy; }
	}

	public int Endurance { 
		get { return endurance; }
	}

	public int Health {
		get { return health; }
	}

	public int Speed {
		get { return speed; }
	}

	public int Strength {
		get { return strength; }
	}

	public Outcome Result {
		get { return outcome; }
	}

	public int Fatigue {
		get { return fatigue; }
	}
}
