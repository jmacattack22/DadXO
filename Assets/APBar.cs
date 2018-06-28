using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class APBar : MonoBehaviour {
    private int currentAP;
    public GameObject player;
    private Image image;

    void Start () {
        currentAP = 100;
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (player != null)
        {
            currentAP = player.GetComponent<Stats>().ap;
            image.fillAmount = (float)currentAP / 100f;
        }
    }
}
