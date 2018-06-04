using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopLayerDrawer : MonoBehaviour {
	private Dictionary<RegionCreator.TileType, Transform> content;

	private float offset = 0.0f;
    private Vector3 scaler = new Vector3(0.001f, 0.001f, 0.0f);

    private float scaleFloor = 0.05f;
    private float scaleCeiling = 1.3f;

	void Awake()
    {
        content = new Dictionary<RegionCreator.TileType, Transform>();

        loadContent();
    }
    
	public void cleanTileMap()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

		transform.localScale = new Vector3(1.0f, 1.0f);
    }

	public void drawRegion(ref DataPool worldData, int regionIndex)
    {
        if (transform.childCount > 0)
            cleanTileMap();
        
        scaleFloor = 0.03f;
        scaleCeiling = 0.5f;
              
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

            float xPos = worldData.Towns[townIndex].Location.x + offset - 1.0f;
            float yPos = worldData.Towns[townIndex].Location.y + offset - 1.0f;

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

	public void zoomIn(Vector3 scaler)
	{
		transform.localScale -= scaler;
	}

    public void zoomOut(Vector3 scaler)
	{
		transform.localScale += scaler;
	}

	public void setActive(bool value)
    {
        gameObject.SetActive(value);
    }

	public void setOffset(float offset)
    {
        this.offset = offset;
    }
}
