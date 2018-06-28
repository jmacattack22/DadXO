using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class APTextScript : MonoBehaviour {

    public GameObject player;
    public TextMeshProUGUI textMesh;
    private int currentAP;

    void Start () {
        currentAP = 100;
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        if (player != null)
        {
            currentAP = player.GetComponent<Stats>().ap;
            textMesh.text = currentAP.ToString() + " / 100";
        }
    }
}
