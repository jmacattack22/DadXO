using UnityEngine;
using System.Collections.Generic;

public class MapDrawer: MonoBehaviour
{
	private float offset = 0.0f;
    private Vector3 scaler = new Vector3(0.001f, 0.001f, 0.0f);

    private float scaleFloor = 0.05f;
    private float scaleCeiling = 1.3f;

	private bool canMapPan = false;
    private bool isMapPanning = false;
    private float mapPanMovement = 0.04f;
    private float mapPanSpeed = 4.0f;
    private Vector3 mapPanTarget;

	void Awake()
    {
		
    }

	void Update()
    {
        if (isMapPanning && canMapPan)
        {
            if (MoveTowardsPoint(mapPanTarget, Time.deltaTime))
            {
                isMapPanning = false;
                mapPanTarget = transform.position;
            }
        }
    }

	public void cleanTileMap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        transform.localScale = new Vector3(1.0f, 1.0f);
    }

	private bool MoveTowardsPoint(Vector3 goal, float elapsed)
    {
        if (transform.position.Equals(goal)) return true;

        Vector3 direction = Vector3.Normalize(goal - transform.position);

        transform.Translate(direction * mapPanSpeed * elapsed);

        if (Mathf.Abs(Vector3.Dot(direction, Vector3.Normalize(goal - transform.position)) + 1) < 0.25f)
        {
            transform.Translate(goal - transform.position);
        }

        return transform.position.Equals(goal);
    }

    public void panMap(MapPositionCaster.CursorPosition cursorPosition)
    {
        if (cursorPosition.Equals(MapPositionCaster.CursorPosition.LeftBorder) && !isMapPanning)
        {
            Vector3 direction = new Vector3(1, 0, 0);
            setNewTarget(direction);
        }
        else if (cursorPosition.Equals(MapPositionCaster.CursorPosition.RightBorder) && !isMapPanning)
        {
            Vector3 direction = new Vector3(-1, 0, 0);
            setNewTarget(direction);
        }
        else if (cursorPosition.Equals(MapPositionCaster.CursorPosition.TopBorder) && !isMapPanning)
        {
            Vector3 direction = new Vector3(0, -1, 0);
            setNewTarget(direction);
        }
        else if (cursorPosition.Equals(MapPositionCaster.CursorPosition.BottomBorder) && !isMapPanning)
        {
            Vector3 direction = new Vector3(0, 1, 0);
            setNewTarget(direction);
        }
    }

    public void scaleAndTranslate(float scaler, float translate)
	{
		transform.localScale = new Vector3(scaler, scaler);
        //transform.Translate(new Vector3(translate, 0.0f, 0.0f));
	}

	public void setActive(bool value)
    {
        gameObject.SetActive(value);
    }
       
    public void setMapPanMovement(float value)
    {
        mapPanMovement = value;
    }

	private void setNewTarget(Vector3 direction)
	{
		mapPanTarget = transform.position + (direction * mapPanMovement);
		isMapPanning = true;
	}

    public void setOffset(float offset)
    {
        this.offset = offset;
    }

    public void setScaleCeiling(float ceiling)
	{
		scaleCeiling = ceiling;
	}

    public void setScaleFloor(float floor)
	{
		scaleFloor = floor;
	}

	public void toggleMapPanAbility()
    {
        canMapPan = !canMapPan;
    }

    public void zoomIn(Vector3 scaler)
    {
        transform.localScale -= scaler;
    }

    public void zoomOut(Vector3 scaler)
    {
        transform.localScale += scaler;
    }

    //Getters
    public float Offset
	{
		get { return offset; }
	}

	public Vector3 Scaler
	{
		get { return scaler; }
	}
}
