using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.Tilemaps;

public class WorldMapDrawer : MapDrawer {

	public TopLayerDrawer topLayer;

    private Dictionary<RegionCreator.TileType, Transform> content;
	private Dictionary<RegionCreator.TileType, Tile> tiles;

   
	void Awake()
	{
        content = new Dictionary<RegionCreator.TileType, Transform>();
		tiles = new Dictionary<RegionCreator.TileType, Tile>();

        loadContent();

		toggleMapPanAbility();
	}
    
	public void addPlainRegionTileToWorldMap(Vector3 pos)
    {
        Transform tile = Instantiate(content[RegionCreator.TileType.Land], pos, Quaternion.identity) as Transform;
		tile.transform.SetParent(transform, false);
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
		Dictionary<int, Vector3> townPositions = populateTileMapWithRegion(ref worldData, regionIndex);
		topLayer.drawRegion(townPositions);
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

		tiles.Add(RegionCreator.TileType.Water, Resources.Load<Tile>("Prefabs/Tiles/EmptyTile"));
		tiles.Add(RegionCreator.TileType.Shallows, Resources.Load<Tile>("Prefabs/Tiles/ShallowsTile"));
		tiles.Add(RegionCreator.TileType.Beach, Resources.Load<Tile>("Prefabs/Tiles/BeachTile"));
		tiles.Add(RegionCreator.TileType.Coastal, Resources.Load<Tile>("Prefabs/Tiles/CoastalTile"));
		tiles.Add(RegionCreator.TileType.Land, Resources.Load<Tile>("Prefabs/Tiles/LandTile"));
		tiles.Add(RegionCreator.TileType.Mountain, Resources.Load<Tile>("Prefabs/Tiles/MountainTile"));
		tiles.Add(RegionCreator.TileType.Peak, Resources.Load<Tile>("Prefabs/Tiles/PeakTile"));
		tiles.Add(RegionCreator.TileType.Rise, Resources.Load<Tile>("Prefabs/Tiles/RiseTile"));
    }

	private Dictionary<int, Vector3> populateTileMapWithRegion(ref DataPool worldData, int regionIndex)
    {      
		RegionCreator.TileType[,] map = worldData.Regions[regionIndex].Map;
		Dictionary<int, Vector3> townPositions = new Dictionary<int, Vector3>();

        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(0); y++)
            {
                if (!map[x, y].Equals(RegionCreator.TileType.Town))
                {
					GetComponent<Tilemap>().SetTile(new Vector3Int(x, y, 0), tiles[map[x, y]]);
                }
				else
				{
					int id = getTownIdFromPosition(
						ref worldData, worldData.Regions[regionIndex].getRegionsTownIndexes(), x, y);

                    if (id != -1)
					{
						townPositions.Add(id, transform.parent.GetComponent<GridLayout>().CellToLocal(new Vector3Int(x,y,0)));
					}
				}
            }
        }

		return townPositions;
    }

	private int getTownIdFromPosition(ref DataPool worldData, List<int> townIndexes, int x, int y)
	{
		foreach (int index in townIndexes)
		{
			if (worldData.Towns[index].Location.Equals(new Vector2Int(x,y)))
			{
				return index;
			}
		}

		return -1;
	}

	private Vector3 getParent()
	{
		return transform.parent.parent.transform.position;
	}
}
