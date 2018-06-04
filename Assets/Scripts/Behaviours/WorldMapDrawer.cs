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
	public TopLayerDrawer topLayer;

    private Dictionary<RegionCreator.TileType, Transform> content;
	private Dictionary<RegionType, Transform> regionContent;
      
	private float offset = 0.0f;
	private Vector3 scaler = new Vector3(0.001f, 0.001f, 0.0f);

	private float scaleFloor = 0.05f;
	private float scaleCeiling = 1.3f;
   
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

		transform.localScale = new Vector3(1.0f, 1.0f);
    }

    public void handleInput()
	{
		if (Input.GetKey(KeyCode.Q))
		{
			zoomIn();
		}

		if (Input.GetKey(KeyCode.E))
        {
            zoomOut();
        }
	}

    public void drawRegion(ref DataPool worldData, int regionIndex)
    {
        if (transform.childCount > 0)
            cleanTileMap();
		
		offset = -125.0f;
		topLayer.setOffset(-125.0f);
		topLayer.drawRegion(ref worldData, regionIndex);

		scaleFloor = 0.03f;
		scaleCeiling = 0.5f;
              
        populateTileMapWithRegion(ref worldData, regionIndex);

		transform.localScale = new Vector3(0.1f, 0.1f);
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
		content.Add(RegionCreator.TileType.Town, Resources.Load<Transform>("Prefabs/Town"));

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
                    }
                }

                if (tile != null)
                    tile.parent = transform;
            }
        }
    }

    public void setActive(bool value)
	{
		gameObject.SetActive(value);
		topLayer.setActive(value);
	}

    public void setOffset(float offset)
	{
		this.offset = offset;
	}

    public void zoomIn()
	{
		transform.localScale -= scaler;
        topLayer.zoomIn(scaler);
	}
     
    public void zoomOut()
	{
		transform.localScale += scaler;
        topLayer.zoomOut(scaler);
	}

    //Getters

}
