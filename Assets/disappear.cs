using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disappear : MonoBehaviour {
    private Material mat;
    void Awake()
    {
        mat = gameObject.GetComponent<Renderer>().material;
    }
	void Update () {
        var col = mat.color;
        col.a -= Time.deltaTime;
        mat.color = col;
    }
}
