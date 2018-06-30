using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour {
    public string boxerName;
    public int accuracy;
    public int endurance;
    public int maxHealth;
    public int currentHealth;
    public int speed;
    public int strength;
    public float moveForce;
    public float maxSpeed;

    public float jumpForce = 100f;
    public float attackForce = 120f;
    [HideInInspector]
    public int ap = 100;
    [HideInInspector]
    public float apCheck;


    void Start () {
        setBoxer(new Boxer(new Vector2Int(0, 0), "Dusty", "Bushwhacker", 0, 0, BoxerClass.Type.BushWacker, 30, 0, 30, 0, 30, 0, 30, 0, 30, 0));
        currentHealth = maxHealth;
    }
	
    public void setBoxer(Boxer boxer)
    {
        this.moveForce = boxer.Speed * 2;
        this.maxSpeed = 0.5f + ((float)(boxer.Speed / 999) * 0.7f);
        this.accuracy = boxer.Accuracy;
        this.maxHealth = boxer.Health;
        this.endurance = boxer.Endurance;
        this.speed = boxer.Speed;
        this.strength = boxer.Strength;
    }

}
