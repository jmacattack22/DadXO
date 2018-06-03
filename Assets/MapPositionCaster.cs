using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPositionCaster : MonoBehaviour {

	public InfoLayerBehaviour infoLayer;

	private TileInfo currentTile;

	private bool targetSet = false;
    
	private float cursorMovement = 0.5f;
	private float cursorSpeed = 4.0f;
	private Vector3 cursorTarget;

	private bool jobSent = false;

	void Update () 
	{
		currentTile = mapCast();

		handleInput();
	}

    private void handleInput()
	{
        if (Input.GetKey(KeyCode.D) && !targetSet && transform.position.x <= 7.5f)
		{
			Vector3 direction = new Vector3(1, 0, 0);
			setNewTarget(direction);
		}

		if (Input.GetKey(KeyCode.A) && !targetSet && transform.position.x >= -7.5f)
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
              jobSent = false;
              sendJobToInfoLayer();
          }
        }
	}

	private TileInfo mapCast()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, fwd, out hit))
        {
            return hit.transform.gameObject.GetComponent<TileInfo>();
        }

		return null;
    }

	private bool MoveTowardsPoint(Vector3 goal, float elapsed)
    {
		if (transform.position.Equals(goal)) return true;

		Vector3 direction = Vector3.Normalize(goal - transform.position);
        
		transform.Translate(direction * cursorSpeed * elapsed);

        if (Mathf.Abs(Vector3.Dot(direction, Vector3.Normalize(goal - transform.position)) + 1) < 0.2f)
        {
            transform.Translate(goal - transform.position);
        }

        return transform.position.Equals(goal);
    }

	private void sendJobToInfoLayer()
    {
		if (currentTile != null && !jobSent)
		{
			if (currentTile.IsRegion)
			{
				infoLayer.sendJob(new InfoLayerJob(InfoLayerJob.InfoJob.Region, currentTile.ID));
			}
			else
			{
				print(currentTile.Position + " - " + currentTile.ID);
				infoLayer.sendJob(new InfoLayerJob(InfoLayerJob.InfoJob.Town, currentTile.ID));
			}
		}
		else if (currentTile == null)
		{
			infoLayer.sendJob(new InfoLayerJob(InfoLayerJob.InfoJob.Clear, 0));
		}

		jobSent = true;
    }
   
	private void setNewTarget(Vector3 direction)
	{
		cursorTarget = transform.position + (direction * cursorMovement);
		targetSet = true;
	}

	public void setMovement(float movement)
	{
		transform.position = new Vector3(0.0f, 0.0f, -5.0f);
		cursorMovement = movement;
	}

    //Getters
    public TileInfo CurrentTile 
	{
		get { return currentTile; }
	}
}
