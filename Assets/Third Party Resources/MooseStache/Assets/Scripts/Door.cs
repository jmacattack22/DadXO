// This script is an example of how to make level exits/end points just attach it to a gameobject with a collider2d component with isTrigger On

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour {

	[SerializeField]
	private bool Pickable = true;

	void OnTriggerEnter2D (Collider2D other) {
		if (other.CompareTag ("Player") && Pickable) {
			var playercomponent = other.GetComponent<Player> ();
			if (playercomponent != null) {
				OnPlayerTrigger (playercomponent);
			}
		}
	}

	void OnTriggerStay2D (Collider2D other) {
		if (other.CompareTag ("Player") && Pickable) {
			var playercomponent = other.GetComponent<Player> ();
			if (playercomponent != null) {
				OnPlayerTrigger (playercomponent);
			}
		}
	}

	void OnPlayerTrigger (Player player) {
		Pickable = false;
		GameManager.instance.NextLevel ();
	}
}
