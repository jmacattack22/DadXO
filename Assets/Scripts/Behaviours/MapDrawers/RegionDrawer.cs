using UnityEngine;
using System.Collections.Generic;

public class RegionDrawer : MapDrawer
{
	private Transform region;
	private Transform emptyRegion;

	private Color normal;
	private Color highlighted;

	public GameObject xLegend;
	public GameObject yLegend;

	private int currentHighlightedIndex = -1;
    
	void Awake()
    {
        loadContent();
    }

	public void drawRegions(ref DataPool worldData)
    {
		populateTileMapWithRegions(ref worldData);
    }

	private void loadContent()
	{
		region = Resources.Load<Transform>("Prefabs/Tiles/Region");
		emptyRegion = Resources.Load<Transform>("Prefabs/Tiles/Empty");

		normal = region.GetComponent<SpriteRenderer>().color;
		highlighted = Resources.Load<Transform>("Prefabs/Tiles/HighlightedRegion").GetComponent<SpriteRenderer>().color;
    }

    private int determineIndex(int x, int y)
	{
		int index = 0;
		int row = 8 + x;
        
		index += row * 17;

		int column = 8 + y;

		index += column;

		return index;
	}

	public void highlightRegion(RowInfoInitializer tile)
	{
		int index = determineIndex((int)tile.Position.x, (int)tile.Position.y);

        if (index != currentHighlightedIndex)
		{
			if (currentHighlightedIndex >= 0)
			{
				transform.GetChild(currentHighlightedIndex).GetComponent<SpriteRenderer>().color = normal;
			}

			transform.GetChild(index).GetComponent<SpriteRenderer>().color = highlighted;
			currentHighlightedIndex = index;
		}
	}

	private void populateTileMapWithRegions(ref DataPool worldData)
    {
        for (int x = -8; x < 9; x++)
        {
            for (int y = -8; y < 9; y++)
			{
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
					transform.GetChild(determineIndex(x, y)).GetComponent<SpriteRenderer>().enabled = true;
				}
				else
				{
					transform.GetChild(determineIndex(x, y)).GetComponent<SpriteRenderer>().enabled = false;
				}

				transform.GetChild(determineIndex(x, y)).GetComponent<TileInfo>().setPosition(new Vector2Int(x, y));
                transform.GetChild(determineIndex(x, y)).GetComponent<TileInfo>().setId(regionIndex);
                transform.GetChild(determineIndex(x, y)).GetComponent<TileInfo>().setIsRegion(true);
            }
        }
    }

    public void toggleLegend(bool desiredState)
	{
		xLegend.SetActive(desiredState);
		yLegend.SetActive(desiredState);
	}
}
