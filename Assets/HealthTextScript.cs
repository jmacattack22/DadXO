using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class HealthTextScript : MonoBehaviour {

    public GameObject player;
    public TextMeshProUGUI textMesh;
    private int startingHealth;
    private int currentHealth;

    void Start () {
        startingHealth = player.GetComponent<Stats>().maxHealth;
        currentHealth = startingHealth;
        textMesh = GetComponent<TextMeshProUGUI>();
	}

	void Update () {
        if (player != null)
        {
            currentHealth = player.GetComponent<Stats>().currentHealth;
            textMesh.text = currentHealth.ToString() + " / " + startingHealth.ToString();
        }
	}
}
