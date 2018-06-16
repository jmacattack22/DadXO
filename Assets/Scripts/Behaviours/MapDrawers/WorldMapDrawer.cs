using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class WorldMapDrawer : MapDrawer {

	public TopLayerDrawer topLayer;

    private Dictionary<RegionCreator.TileType, Transform> content;
   
	void Awake()
	{
        content = new Dictionary<RegionCreator.TileType, Transform>();

        loadContent();

		toggleMapPanAbility();
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
       
    public void handleInput()
	{
		if (Input.GetKey(KeyCode.Q))
		{
			zoomIn(Scaler);
			topLayer.zoomIn(Scaler);
		}

		if (Input.GetKey(KeyCode.E))
        {
            zoomOut(Scaler);
			topLayer.zoomOut(Scaler);
        }
	}

    public void drawRegion(ref DataPool worldData, int regionIndex)
    {
        if (transform.childCount > 0)
            cleanTileMap();

		setOffset(-125.0f);
		topLayer.setOffset(-125.0f);
		topLayer.drawRegion(ref worldData, regionIndex);

		setScaleFloor(0.02f);
		setScaleCeiling(0.5f);
              
        populateTileMapWithRegion(ref worldData, regionIndex);

		scaleAndTranslate(0.033f, -9.8f);
    }

	private void loadContent()
    {
        content.Add(RegionCreator.TileType.Water, Resources.Load<Transform>("Prefabs/Tiles/Water"));
		content.Add(RegionCreator.TileType.Shallows, Resources.Load<Transform>("Prefabs/Tiles/Shallows"));
		content.Add(RegionCreator.TileType.Beach, Resources.Load<Transform>("Prefabs/Tiles/Beach"));
		content.Add(RegionCreator.TileType.Coastal, Resources.Load<Transform>("Prefabs/Tiles/Coastal"));
		content.Add(RegionCreator.TileType.Land, Resources.Load<Transform>("Prefabs/Tiles/Land"));
		content.Add(RegionCreator.TileType.Mountain, Resources.Load<Transform>("Prefabs/Tiles/Mountain"));
		content.Add(RegionCreator.TileType.Rise, Resources.Load<Transform>("Prefabs/Tiles/Rise"));
		content.Add(RegionCreator.TileType.Peak, Resources.Load<Transform>("Prefabs/Tiles/Peak"));
		content.Add(RegionCreator.TileType.Town, Resources.Load<Transform>("Prefabs/Tiles/Town"));
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
                    float xPos = x + Offset;
                    float yPos = y + Offset;


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
}
