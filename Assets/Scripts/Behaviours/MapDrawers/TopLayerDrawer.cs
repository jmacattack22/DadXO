using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopLayerDrawer : MapDrawer {
	private Dictionary<RegionCreator.TileType, Transform> content;

	void Awake()
    {
        content = new Dictionary<RegionCreator.TileType, Transform>();

        loadContent();

		toggleMapPanAbility();
    }

	public void drawRegion(Dictionary<int, Vector3> townPositions)
    {
        if (transform.childCount > 0)
            cleanTileMap();
      
        foreach (int key in townPositions.Keys)
		{
			Transform tile = null;

			tile = Instantiate(content[RegionCreator.TileType.Town], townPositions[key], Quaternion.identity) as Transform;
			tile.gameObject.GetComponent<TileInfo>().setIsRegion(false);
			tile.gameObject.GetComponent<TileInfo>().setId(key);
			tile.localScale = new Vector3(7.0f, 7.0f);

			tile.SetParent(transform, false);
		}
    }

    public void drawRegion(ref DataPool worldData, int regionIndex)
	{
		if (transform.childCount > 0)
            cleanTileMap();

		populateTileMapWithRegion(ref worldData, regionIndex);
	}

	private void loadContent()
    {
        content.Add(RegionCreator.TileType.Town, Resources.Load<Transform>("Prefabs/Tiles/Town"));
    }

	private void populateTileMapWithRegion(ref DataPool worldData, int regionIndex)
	{
		foreach (int townIndex in worldData.Regions[regionIndex].getRegionsTownIndexes())
		{
			Transform tile = null;

			float xPos = worldData.Towns[townIndex].Location.x + XOffset - 1.0f;
			float yPos = worldData.Towns[townIndex].Location.y + YOffset - 1.0f;

			tile = Instantiate(content[RegionCreator.TileType.Town],
			                   new Vector3(xPos, yPos, 0),
							   Quaternion.identity) as Transform;

			tile.gameObject.GetComponent<TileInfo>().setPosition(worldData.Towns[townIndex].Location);
			tile.gameObject.GetComponent<TileInfo>().setIsRegion(false);
			tile.gameObject.GetComponent<TileInfo>().setId(townIndex);

			tile.transform.SetParent(transform);
		}
	}
}
