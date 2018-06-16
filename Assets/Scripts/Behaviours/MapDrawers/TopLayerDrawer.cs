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

	public void drawRegion(ref DataPool worldData, int regionIndex)
    {
        if (transform.childCount > 0)
            cleanTileMap();

		setScaleFloor(0.03f);
		setScaleCeiling(0.5f);
              
        populateTileMapWithRegion(ref worldData, regionIndex);

		scaleAndTranslate(0.033f, -9.8f);
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

			float xPos = worldData.Towns[townIndex].Location.x + Offset - 1.0f;
			float yPos = worldData.Towns[townIndex].Location.y + Offset - 1.0f;

			tile = Instantiate(content[RegionCreator.TileType.Town],
							   new Vector3(xPos, yPos, 0),
							   Quaternion.identity) as Transform;

			tile.gameObject.GetComponent<TileInfo>().setPosition(worldData.Towns[townIndex].Location);
			tile.gameObject.GetComponent<TileInfo>().setIsRegion(false);
			tile.gameObject.GetComponent<TileInfo>().setId(townIndex);
			tile.localScale = new Vector3(6.5f, 6.5f);

			tile.parent = transform;
		}
	}
}
