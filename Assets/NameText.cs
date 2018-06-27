using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NameText : MonoBehaviour {
    public GameObject player;
    private string playerName;
    public TextMeshProUGUI textMesh;

    void Start () {
        playerName = player.GetComponent<Stats>().boxerName;
        textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = playerName;
    }
	
}
