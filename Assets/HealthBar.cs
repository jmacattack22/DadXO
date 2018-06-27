using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    private int StartingHealth;
    private int CurrentHealth;
    public GameObject player;
    private Image image;

    // Use this for initialization
    void Start () {
        StartingHealth = player.GetComponent<Stats>().maxHealth;
        image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
        if(player!= null)
        {
            CurrentHealth = player.GetComponent<Stats>().currentHealth;
            image.fillAmount = (float)CurrentHealth / (float)StartingHealth;
        }
    }
}
