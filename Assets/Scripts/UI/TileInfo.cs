using UnityEngine;
using System.Collections;

public class TileInfo : MonoBehaviour
{
	private Vector2Int position;
	private int id = -1;
	private bool isRegion = false;

    public void setId(int id)
	{
		this.id = id;
	}

	public void setPosition(Vector2Int pos)
	{
		this.position = pos;
	}

    public void setIsRegion(bool isRegion)
	{
		this.isRegion = isRegion;
	}

    //Getters
    public int ID
	{
		get { return id; }
	}

	public Vector2Int Position
	{
		get { return position; }
	}

    public bool IsRegion
	{
		get { return isRegion; }
	}
}
