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
	public MapPositionCaster cursor;

    private Dictionary<RegionCreator.TileType, Transform> content;
	private Dictionary<RegionType, Transform> regionContent;
    
	private bool viewingRegions = true;
	private int regionToView = -1;
	private Dictionary<int, Vector2Int> loadedIndexes = new Dictionary<int, Vector2Int>();

	private float offset = 0.0f;
	private Vector3 scaler = new Vector3(0.001f, 0.001f, 0.0f);

	private float scaleFloor = 0.05f;
	private float scaleCeiling = 1.3f;

	private bool jobSent = false;
	private bool focused = false;
   
	void Awake()
	{
        content = new Dictionary<RegionCreator.TileType, Transform>();
		regionContent = new Dictionary<RegionType, Transform>();

        loadContent();
	}

	void Start () {
        
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Space))
		{
			focused = !focused;
		}     

        if (focused)
		{
			handleInput();
		}

		sendJobToInfoLayer();
	}
    
	public void addPlainRegionTileToWorldMap(Vector3 pos)
    {
        Transform tile = Instantiate(content[RegionCreator.TileType.Land], pos / 2.0f, Quaternion.identity) as Transform;
		tile.localScale = new Vector3(0.5f, 0.5f);
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

	private void cleanTileMap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void drawRegion(ref DataPool worldData, int regionIndex)
    {
        if (transform.childCount > 0)
            cleanTileMap();

		scaleFloor = 0.03f;
		scaleCeiling = 0.5f;

        populateTileMapWithRegion(ref worldData, regionIndex);
    }

    public void drawRegions(ref DataPool worldData)
    {
        if (transform.childCount > 0)
            cleanTileMap();

		scaleFloor = 0.35f;
        scaleCeiling = 1.3f;

        viewingRegions = true;
        loadedIndexes.Clear();
        for (int i = 0; i < worldData.Regions.Count; i++)
        {
            loadedIndexes.Add(i, worldData.Regions[i].Position);
        }

        populateTileMapWithRegions(ref worldData);
    }

	private void handleInput()
	{   
		if (Input.GetKey(KeyCode.Q) && transform.localScale.x < scaleCeiling)
		{
			transform.localScale += scaler;
		}

		if (Input.GetKey(KeyCode.E) && transform.localScale.x > scaleFloor)
		{
			transform.localScale -= scaler;
		}

        if (Input.GetKeyDown(KeyCode.T))
        {
            viewingRegions = false;
            regionToView = 0;
            offset = -125.0f;
			cursor.setMovement(0.125f);
        }
	}

	private void loadContent()
    {
        content.Add(RegionCreator.TileType.Water, Resources.Load<Transform>("Prefabs/Water"));
        content.Add(RegionCreator.TileType.Shallows, Resources.Load<Transform>("Prefabs/Shallows"));
        content.Add(RegionCreator.TileType.Beach, Resources.Load<Transform>("Prefabs/Beach"));
        content.Add(RegionCreator.TileType.Coastal, Resources.Load<Transform>("Prefabs/Coastal"));
        content.Add(RegionCreator.TileType.Land, Resources.Load<Transform>("Prefabs/Land"));
        content.Add(RegionCreator.TileType.Mountain, Resources.Load<Transform>("Prefabs/Mountain"));
        content.Add(RegionCreator.TileType.Rise, Resources.Load<Transform>("Prefabs/Rise"));
        content.Add(RegionCreator.TileType.Peak, Resources.Load<Transform>("Prefabs/Peak"));

        regionContent.Add(RegionType.Water, Resources.Load<Transform>("Prefabs/Water"));
        regionContent.Add(RegionType.Islands, Resources.Load<Transform>("Prefabs/Region"));
    }

	private void populateTileMapWithRegion(ref DataPool worldData, int regionIndex)
    {
		RegionCreator.TileType[,] map = worldData.Regions[regionIndex].Map;

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                Transform tile = null;

                if (!map[x, y].Equals(RegionCreator.TileType.Town))
                {
                    float xPos = x + offset;
                    float yPos = y + offset;


                    if (!map[x, y].Equals(RegionCreator.TileType.Water))
                    {
                        tile = Instantiate(content[map[x, y]], new Vector3(xPos, yPos), Quaternion.identity) as Transform;
                        tile.gameObject.GetComponent<TileInfo>().setPosition(new Vector2Int(x, y));
						tile.gameObject.GetComponent<TileInfo>().setIsRegion(false);

						int townIndex = isNearbyTown(worldData.Regions[regionIndex].TownSurroundingTiles, new Vector2Int(x, y));

						tile.gameObject.GetComponent<TileInfo>().setId(townIndex);
                    }
                }

                if (tile != null)
                    tile.parent = transform;
            }
        }

        transform.localScale = new Vector3(0.1f, 0.1f);
    }

	private int isNearbyTown(Dictionary<int, List<Vector2Int>> surroundingTiles, Vector2Int position)
	{
		int townIndex = -1;

		foreach (int index in surroundingTiles.Keys)
		{
			if (surroundingTiles[index].Contains(position))
			{
				townIndex = index;
			}
		}

		return townIndex;
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

				float xPos = x + offset;
				float yPos = y + offset;

				if (regionIndex != -1)
				{
					tile = Instantiate(regionContent[RegionType.Islands], new Vector3(xPos / 2.0f, yPos / 2.0f), Quaternion.identity) as Transform;
					tile.gameObject.GetComponent<TileInfo>().setId(regionIndex);
					tile.gameObject.GetComponent<TileInfo>().setIsRegion(true);
					tile.localScale = new Vector3(0.5f, 0.5f);
					tile.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = worldData.Regions[regionIndex].Level.ToString();
                }

                if (tile != null)
                    tile.parent = transform;
            }
        }
    }

	private void sendJobToInfoLayer()
	{
		if (focused && !jobSent)
        {
            if (viewingRegions)
            {
                //infoLayer.sendJob(new InfoLayerJob(InfoLayerJob.InfoJob.Region, getRegionIndexFromCursorPosition()));
                jobSent = !jobSent;
            }
        }
	}

    public void setOffset(float offset)
	{
		this.offset = offset;
	}

    public void setRegionToView(int region)
	{
		regionToView = region;
	}
       
    //Getters
    public bool ViewingRegions
	{
		get { return viewingRegions; }
	}

    public int RegionToView
	{
		get { return regionToView; }
	}

    public bool Focused 
	{
		get { return focused; }
	}
}
