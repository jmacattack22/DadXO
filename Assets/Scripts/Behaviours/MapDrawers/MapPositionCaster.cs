using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPositionCaster : MonoBehaviour {

    public enum CursorPosition
	{
		Central, LeftBorder, RightBorder, TopBorder, BottomBorder
	}

	private TileInfo currentTile;

	private bool targetSet = false;
    
	private float cursorMovement = 0.5f;
	private float cursorSpeed = 10.0f;
	private Vector3 cursorTarget;

	void Update () 
	{
		currentTile = mapCast();

		handleInput();
	}

    private void handleInput()
	{
        if (Input.GetKey(KeyCode.D) && !targetSet && transform.position.x <= 3.5f)
		{
			Vector3 direction = new Vector3(1, 0, 0);
			setNewTarget(direction);
		}

		if (Input.GetKey(KeyCode.A) && !targetSet && transform.position.x >= -3.5f)
        {
            Vector3 direction = new Vector3(-1, 0, 0);
			setNewTarget(direction);
        }
        
        if (Input.GetKey(KeyCode.W) && !targetSet && transform.position.y <= 3.5f)
        {
            Vector3 direction = new Vector3(0, 1, 0);
			setNewTarget(direction);
        }

        if (Input.GetKey(KeyCode.S) && !targetSet && transform.position.y >= -3.5f)
        {
            Vector3 direction = new Vector3(0, -1, 0);
			setNewTarget(direction);
        }

		if (targetSet)
        {
          if (MoveTowardsPoint(cursorTarget, Time.deltaTime))
          {
              targetSet = false;
              cursorTarget = transform.position;
          }
        }
	}

	public CursorPosition getCursorPosition()
	{
		if (transform.position.x > 7.7f)
		{
			return CursorPosition.RightBorder;
		}

        if (transform.position.x < -7.7f)
		{
			return CursorPosition.LeftBorder;
		}

        if (transform.position.y > 3.7f)
		{
			return CursorPosition.TopBorder;
		}

        if (transform.position.y < -3.7f)
		{
			return CursorPosition.BottomBorder;
		}

		return CursorPosition.Central;
	}

    private TileInfo mapCast()
    {
		for (float x = -0.02f; x < 0.02f; x += 0.005f)
		{
			for (float y = -0.02f; y < 0.02f; y += 0.005f)
			{
				Vector3 fwd = transform.TransformDirection(Vector3.forward);

				fwd.x += x;
				fwd.y += y;

				RaycastHit hit;

                if (Physics.Raycast(transform.position, fwd, out hit))
                {
                    return hit.transform.gameObject.GetComponent<TileInfo>();
                }
			}
		}

		return null;
    }

	private bool MoveTowardsPoint(Vector3 goal, float elapsed)
    {
		if (transform.position.Equals(goal)) return true;

		Vector3 direction = Vector3.Normalize(goal - transform.position);
        
		transform.Translate(direction * cursorSpeed * elapsed);

        if (Mathf.Abs(Vector3.Dot(direction, Vector3.Normalize(goal - transform.position)) + 1) < 0.25f)
        {
            transform.Translate(goal - transform.position);
        }

        return transform.position.Equals(goal);
    }
   
	private void setNewTarget(Vector3 direction)
	{
		cursorTarget = transform.position + (direction * cursorMovement);
		targetSet = true;
	}

	public void setMovement(float movement)
	{
		transform.position = new Vector3(0.0f, 0.0f, -6.0f);
		cursorMovement = movement;
	}

	public void setTarget(Vector3 position)
	{
		cursorTarget = position;
        targetSet = true;
	}

    //Getters
    public TileInfo CurrentTile 
	{
		get { return currentTile; }
	}
}
