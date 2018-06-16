using UnityEngine;
using System.Collections.Generic;

public class RegionDrawer : MapDrawer
{
	private Transform region;
    
	void Awake()
    {
        loadContent();
    }

	public void drawRegions(ref DataPool worldData)
    {
        if (transform.childCount > 0)
            cleanTileMap();

        populateTileMapWithRegions(ref worldData);
    }

	private void loadContent()
	{
		region = Resources.Load<Transform>("Prefabs/Tiles/Region");
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
                    tile = Instantiate(region, new Vector3(x / 2.0f, y / 2.0f), Quaternion.identity) as Transform;
                    tile.gameObject.GetComponent<TileInfo>().setId(regionIndex);
                    tile.gameObject.GetComponent<TileInfo>().setIsRegion(true);
                    tile.localScale = new Vector3(0.5f, 0.5f);
                    tile.GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = worldData.Regions[regionIndex].Level.ToString();
                }

                if (tile != null)
                    tile.parent = transform;
            }
        }

        transform.localScale = new Vector3(1.0f, 1.0f);
    }
}
