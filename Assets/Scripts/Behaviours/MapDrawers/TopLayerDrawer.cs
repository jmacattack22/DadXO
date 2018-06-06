﻿using System.Collections;
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

		transform.localScale = new Vector3(0.1f, 0.1f);
    }

	private void loadContent()
    {
        content.Add(RegionCreator.TileType.Town, Resources.Load<Transform>("Prefabs/Town"));
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
			tile.localScale = new Vector3(3.5f, 3.5f);

			tile.parent = transform;
		}
	}
}