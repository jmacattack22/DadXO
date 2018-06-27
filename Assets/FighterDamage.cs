using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterDamage : MonoBehaviour, IDamageable {

	// Use this for initialization
	void Start () {
		
	}

    public void Damage(int damageTaken)
    {
        GetComponent<Stats>().currentHealth -= damageTaken;
        GetComponent<BaseController>().iframe = 1f;
    }
}
