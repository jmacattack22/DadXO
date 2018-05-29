using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WorldMapDrawer : MonoBehaviour {

	public enum RegionType 
	{
	    Water, Islands	
	}

	public InfoLayerBehaviour infoLayer;

    private Dictionary<RegionCreator.TileType, Transform> content;
	private Dictionary<RegionType, Transform> regionContent;
    
	private bool viewingRegions = true;
	private Dictionary<int, Vector2Int> loadedIndexes = new Dictionary<int, Vector2Int>();

	public GameObject cursor;
	private Vector3 cursorPosition = new Vector3(0, 0, 0);
	private float cursorSpeed = 6.0f;
	private Vector3 cursorTarget = new Vector3(0, 0, 0);
	private bool targetSet = false;

	private bool jobSent = false;
	private bool focused = false;
   
	void Awake()
	{
        content = new Dictionary<RegionCreator.TileType, Transform>();
		regionContent = new Dictionary<RegionType, Transform>();

		cursor.SetActive(false);

        loadContent();
	}

	void Start () {
        
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Space))
		{
			focused = !focused;

            if (focused)
			{
				cursor.SetActive(true);
			}else
			{
				cursor.SetActive(false);
			}
		}     

        if (focused)
		{
			handleInput();
		}

		sendJobToInfoLayer();
	}
    
	private void handleInput()
	{
        if (Input.GetKey(KeyCode.D) && !targetSet)
		{
			cursorTarget = cursorPosition + new Vector3(1, 0, 0);
			targetSet = true;
		}

		if (Input.GetKey(KeyCode.A) && !targetSet)
        {
            cursorTarget = cursorPosition + new Vector3(-1, 0, 0);
            targetSet = true;
        }
        
		if (Input.GetKey(KeyCode.W) && !targetSet)
        {
            cursorTarget = cursorPosition + new Vector3(0, 1, 0);
            targetSet = true;
        }

		if (Input.GetKey(KeyCode.S) && !targetSet)
        {
            cursorTarget = cursorPosition + new Vector3(0, -1, 0);
            targetSet = true;
        }

		if (targetSet)
		{
			if (MoveTowardsPoint(cursorTarget, Time.deltaTime))
			{
				targetSet = false;
				cursorTarget = cursorPosition;
				jobSent = false;
				sendJobToInfoLayer();
			}
		}
	}

	private void sendJobToInfoLayer()
	{
		if (focused && !jobSent)
        {
            if (viewingRegions)
            {
                infoLayer.sendJob(new InfoLayerJob(InfoLayerJob.InfoJob.Region, getRegionIndexFromCursorPosition()));
                jobSent = !jobSent;
            }
        }
	}

	public void addPlainRegionTileToWorldMap(Vector3 pos)
    {
        Transform tile = Instantiate(content[RegionCreator.TileType.Land], pos, Quaternion.identity) as Transform;
        tile.parent = transform;
    }

    public void addRegionTileToWorldMap(DataPool worldData, int regionIndex, Vector3 pos)
    {
        Transform tile = null;

        if (worldData.Regions[regionIndex].Level.Equals(TournamentProtocol.Level.S))
        {
            tile = Instantiate(content[RegionCreator.TileType.Peak], pos, Quaternion.identity) as Transform;
        }
        else if (worldData.Regions[regionIndex].Level.Equals(TournamentProtocol.Level.A))
        {
            tile = Instantiate(content[RegionCreator.TileType.Rise], pos, Quaternion.identity) as Transform;
        }
        else if (worldData.Regions[regionIndex].Level.Equals(TournamentProtocol.Level.B))
        {
            tile = Instantiate(content[RegionCreator.TileType.Mountain], pos, Quaternion.identity) as Transform;
        }
        else if (worldData.Regions[regionIndex].Level.Equals(TournamentProtocol.Level.C))
        {
            tile = Instantiate(content[RegionCreator.TileType.Land], pos, Quaternion.identity) as Transform;
        }
        else if (worldData.Regions[regionIndex].Level.Equals(TournamentProtocol.Level.D))
        {
            tile = Instantiate(content[RegionCreator.TileType.Coastal], pos, Quaternion.identity) as Transform;
        }
        else if (worldData.Regions[regionIndex].Level.Equals(TournamentProtocol.Level.E))
        {
            tile = Instantiate(content[RegionCreator.TileType.Shallows], pos, Quaternion.identity) as Transform;
        }

        if (tile != null)
            tile.parent = transform;
    }

	private void loadContent(){
        content.Add(RegionCreator.TileType.Water, Resources.Load<Transform>("Prefabs/Water"));
        content.Add(RegionCreator.TileType.Shallows, Resources.Load<Transform>("Prefabs/Shallows"));
        content.Add(RegionCreator.TileType.Beach, Resources.Load<Transform>("Prefabs/Beach"));
        content.Add(RegionCreator.TileType.Coastal, Resources.Load<Transform>("Prefabs/Coastal"));
        content.Add(RegionCreator.TileType.Land, Resources.Load<Transform>("Prefabs/Land"));
        content.Add(RegionCreator.TileType.Mountain, Resources.Load<Transform>("Prefabs/Mountain"));
        content.Add(RegionCreator.TileType.Rise, Resources.Load<Transform>("Prefabs/Rise"));
        content.Add(RegionCreator.TileType.Peak, Resources.Load<Transform>("Prefabs/Peak"));

		regionContent.Add(RegionType.Water, Resources.Load<Transform>("Prefabs/Water"));
		regionContent.Add(RegionType.Islands, Resources.Load<Transform>("Prefabs/IslandRegion"));
    }

    private void cleanTileMap(){
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void drawRegion(RegionCreator.TileType[,] map){
        if (transform.childCount > 0)
            cleanTileMap();

        populateTileMapWithRegion(map);
    }

    public void drawRegions(ref DataPool worldData){
        if (transform.childCount > 0)
            cleanTileMap();

		cursor = Instantiate(cursor, cursorPosition, Quaternion.identity) as GameObject;
		cursor.transform.parent = transform;

		viewingRegions = true;
		loadedIndexes.Clear();
		for (int i = 0; i < worldData.Regions.Count; i++)
		{
			loadedIndexes.Add(i, worldData.Regions[i].Position);
		}

        populateTileMapWithRegions(ref worldData);
    }

	private int getRegionIndexFromCursorPosition()
	{
		float min = 100000000.0f;
		int key = 0;

        foreach (int k in loadedIndexes.Keys)
		{
			Vector2 v1 = new Vector2(loadedIndexes[k].x, loadedIndexes[k].y);
			Vector2 v2 = new Vector2(cursor.transform.position.x, cursor.transform.position.y);
            
			float distance = Mathf.Sqrt(Mathf.Pow((v1.x - v2.x), 2) + Mathf.Pow((v1.y - v2.y), 2));

			if (distance < min)
			{
				key = k;
				min = distance;
			}
		}

		return min < 0.6f ? key : 1;
	}

    private void populateTileMapWithRegion(RegionCreator.TileType[,] map)
    {
        for (int x = 0; x < map.GetLength(0); x++){
            for (int y = 0; y < map.GetLength(0); y++){
                Transform tile = null;

                if (!map[x,y].Equals(RegionCreator.TileType.Town)){
                    tile = Instantiate(content[map[x, y]], new Vector3(x, y), Quaternion.identity) as Transform;
                }

                if (tile != null)
                    tile.parent = transform;
            }
        }
    }

    private void populateTileMapWithRegions(ref DataPool worldData)
    {
        for (int x = -8; x < 9; x++)
        {
            for (int y = -8; y < 9; y++)
            {
                Transform tile = null;
                Vector2Int currentPos = new Vector2Int(x, y);
                int regionIndex = -1;

                for (int i = 0; i < worldData.Regions.Count; i++)
                {
                    if (worldData.Regions[i].Position.Equals(currentPos))
                    {
                        regionIndex = i;
                    }
                }

                if (regionIndex != -1)
                {
					tile = Instantiate(regionContent[RegionType.Islands], new Vector3(x, y), Quaternion.identity) as Transform;
					//addRegionTileToWorldMap(worldData, regionIndex, new Vector3(x,y));
                }
                else
                {
                    tile = Instantiate(content[RegionCreator.TileType.Water], new Vector3(x, y), Quaternion.identity) as Transform;
                }

                if (tile != null)
                    tile.parent = transform;
            }
        }
    }

	public void setCursorPosition(Vector3Int position)
	{
		cursorPosition = position;
	}

	private bool MoveTowardsPoint(Vector3 goal, float elapsed)
    {
		if (cursorPosition.Equals(goal)) return true;

		Vector3 direction = Vector3.Normalize(goal - cursorPosition);

		cursorPosition = cursorPosition + (direction * cursorSpeed * elapsed);

		cursor.transform.Translate(direction * cursorSpeed * elapsed);

        if (Mathf.Abs(Vector3.Dot(direction, Vector3.Normalize(goal - cursorPosition)) + 1) < 0.1f)
		{
			cursor.transform.Translate(goal - cursorPosition);
			cursorPosition = cursor.transform.position;
		}

		return cursorPosition.Equals(goal);
    }

    //Getters
	public Vector3 CursorPosition
	{
		get { return cursorPosition; }	
	}

    public bool Focused 
	{
		get { return focused; }
	}
}
