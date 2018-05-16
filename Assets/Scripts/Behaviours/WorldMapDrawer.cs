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
        for (int x = -6; x < 7; x++)
        {
            for (int y = -6; y < 7; y++)
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
                   // print(worldData.Capitols[worldData.Regions[regionIndex].CapitolIndex].Quarterlies.Keys);
                    if (worldData.Capitols[worldData.Regions[regionIndex].CapitolIndex].Quarterlies.ContainsKey(TournamentProtocol.Level.A)){
                        tile = Instantiate(content[RegionCreator.TileType.Beach], new Vector3(x, y), Quaternion.identity) as Transform;
                    } else {
                        tile = Instantiate(content[RegionCreator.TileType.Land], new Vector3(x, y), Quaternion.identity) as Transform;
                    }
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
