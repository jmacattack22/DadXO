using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour {

    public enum State
	{
		None, FadeOut, FadeIn
	}

	private State state;

	private float alpha = 255.0f;
    
	void Start () {
		state = State.None;
	}

	void Update () {
		if (state.Equals(State.FadeOut))
		{
			Color tmp = transform.GetComponent<SpriteRenderer>().color;
			if (tmp.a >= 1.0f)
				alpha -= Time.deltaTime;
			else
				state = State.None;

			tmp.a = alpha;
            transform.GetComponent<SpriteRenderer>().color = tmp;
		}
		else if (state.Equals(State.FadeIn))
		{
			Color tmp = transform.GetComponent<SpriteRenderer>().color;
			if (tmp.a <= 254.0f)
				alpha += Time.deltaTime;
			else
				state = State.None;

			tmp.a = alpha;
			transform.GetComponent<SpriteRenderer>().color = tmp;
		}
	}

    public void fadeOut()
	{
		state = State.FadeOut;
	}

    public void fadeIn()
	{
		state = State.FadeIn;
	}
}
