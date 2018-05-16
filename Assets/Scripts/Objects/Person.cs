using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Person : WeightClass {

	private Vector2Int age;
	private string firstName;
	private string lastName;
	private int townIndex;
	private float weight;
	private int weeksRemaining;

	public Person(
		Vector2Int age, string fName, string lName, int townId, float wt) : base(wt) {
		this.age = age;
		this.firstName = fName;
		this.lastName = lName;
		this.townIndex = townId;
		this.weight = wt;

		weeksRemaining = generateRandomInt(280, 400);
	}

	public void ageWeek(){
		age.y++;

		if (age.y > 48) {
			age.x++;
			age.y = 1;
		}

		weeksRemaining--;
	}

    public void cutLife(int cut)
    {
        for (int week = 0; week < cut; week++){
            ageWeek();
        }
    }

	private static int generateRandomInt(int min, int max)
    {
        return new System.Random((int)System.DateTime.Now.Ticks).Next(min, max);
    }

	public string getName(){
		return firstName + " " + lastName;
	}

	//Getters
	public Vector2Int Age {
		get { return age; }
	}

	public string FirstName {
		get { return firstName; }
	}

	public string LastName {
		get { return lastName; }
	}

	public int TownIndex {
		get { return townIndex; }
	}

	public float Weight {
		get { return weight; }
	}

	public int WeeksRemaining {
		get { return weeksRemaining; }
	}
}
