using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapDrawer : MonoBehaviour {

    private Dictionary<RegionCreator.TileType, Transform> content;

	void Awake()
	{
        content = new Dictionary<RegionCreator.TileType, Transform>();
        loadContent();
	}

	void Start () {
        
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

    public void drawRegions(ref DataPool worldData){
        if (transform.childCount > 0)
            cleanTileMap();

        populateTileMapWithRegions(ref worldData);
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
					addRegionTileToWorldMap(worldData, regionIndex, new Vector3(x,y));
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
}
