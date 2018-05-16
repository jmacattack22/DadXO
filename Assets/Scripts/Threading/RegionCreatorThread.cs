using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;

public class RegionCreatorThread : MonoBehaviour
{
	public int regionWidth;
	public int regionHeight;
	public int regionsToCreate;

	private bool regionLocked;
	private List<Region> regions;

	private bool locationsGenerated;
	private bool locationLock;
	private Thread locationThread;
	private List<Vector2Int> regionLocations;

	private List<Thread> threads;
	private int batchSize;

	private bool processRegions;
	private bool regionsCreated;

	private float time;

    void Awake()
    {
		time = Time.time;
		regions = new List<Region>();
		regionLocked = false;

		locationsGenerated = false;
		locationLock = false;
		regionLocations = new List<Vector2Int>();

		processRegions = false;
		regionsCreated = false;

		batchSize = 4;
		threads = new List<Thread>();

		locationThread = new Thread(new ThreadStart(generateRegionLocations));
		locationThread.Start();               

		for (int i = 0; i < batchSize; i++)
		{
			threads.Add(new Thread(new ThreadStart(createRegion)));
		}
    }

	void Update()
	{
		if (processRegions && locationsGenerated)
		{           
			int activeCount = 0;
			for (int i = 0; i < threads.Count; i++)
			{
				if (threads[i].IsAlive)
					activeCount++;
				else
				{
					threads[i].Abort();
				}
			}

			if (regions.Count < regionsToCreate)
			{
				////print( calculatePercentageDone() + "% - " + (Time.time - time));
				for (int i = 0; i < batchSize - activeCount; i++)
				{
					threads.Add(new Thread(new ThreadStart(createRegion)));
					threads[threads.Count - 1].Start();
				}
			}
			else
			{
				regionsCreated = true;
				processRegions = false;
			}
		}
    }


	public float calculatePercentageDone(){
		float done = regions.Count;
		float target = regionsToCreate;

		return (done / target);
	}

    public void createRegions()
	{
		regions.Clear();

		foreach(Thread t in threads)
		{
			t.Start();
		}
		processRegions = true;
	}

	public void createRegion(object state)
    {
        while (locationLock)
        { }

        locationLock = true;
        Region region = new Region("", regionLocations[0]);
        regionLocations.RemoveAt(0);
        locationLock = false;

        region.addWorldMap(RegionCreator.CreateRegion(regionWidth, regionHeight));

        while (regionLocked)
        {
        }

        regionLocked = true;
        regions.Add(region);
        regionLocked = false;
    }

    public void createRegion()
	{
		while (locationLock)
		{}

		locationLock = true;
		Region region = new Region("", regionLocations[0]);
		regionLocations.RemoveAt(0);
		locationLock = false;

		region.addWorldMap(RegionCreator.CreateRegion(regionWidth, regionHeight));

        while (regionLocked)
		{
		}

		regionLocked = true;
		regions.Add(region);
		regionLocked = false;
	}

    public void generateRegionLocations()
	{
		List<Vector2Int> newlyCreatedRegionLocations = new List<Vector2Int>();
		regionLocations.Add(new Vector2Int(0, 0));
		newlyCreatedRegionLocations.Add(new Vector2Int(0, 0));

		while (regionLocations.Count < regionsToCreate)
        {
			List<Vector2Int> temporaryLocations = new List<Vector2Int>();

            foreach (Vector2Int pos in newlyCreatedRegionLocations)
			{
				List<Vector2Int> newLocationsToAdd = getAdjacents(pos);

				if (newLocationsToAdd.Count > 0)
				{
					foreach (Vector2Int newPos in newLocationsToAdd)
					{
						temporaryLocations.Add(newPos);
						regionLocations.Add(newPos);
					}
				}
				else
				{
					temporaryLocations.Add(pos);
				}
			}

			newlyCreatedRegionLocations = temporaryLocations;
        }

		locationsGenerated = true;
	}

	private List<Vector2Int> getAdjacents(Vector2Int newRegion)
    {
        List<Vector2Int> adjacents = new List<Vector2Int>();

        Vector2Int above = new Vector2Int(newRegion.x, newRegion.y + 1);
        Vector2Int below = new Vector2Int(newRegion.x, newRegion.y - 1);
        Vector2Int left = new Vector2Int(newRegion.x - 1, newRegion.y);
        Vector2Int right = new Vector2Int(newRegion.x + 1, newRegion.y);

        bool aboveExists = false;
        bool belowExists = false;
        bool leftExists = false;
        bool rightExists = false;

		foreach (Vector2Int loc in regionLocations)
		{
			if (loc.Equals(above))
				aboveExists = true;

			if (loc.Equals(below))
				belowExists = true;

			if (loc.Equals(left))
				leftExists = true;

			if (loc.Equals(right))
				rightExists = true;
		}
        

        if (!aboveExists && shouldAddRegion(regionLocations.Count))
        {
            adjacents.Add(above);
        }

		if (!belowExists && shouldAddRegion(regionLocations.Count))
        {
            adjacents.Add(below);
        }

		if (!leftExists && shouldAddRegion(regionLocations.Count))
        {
            adjacents.Add(left);
        }

		if (!rightExists && shouldAddRegion(regionLocations.Count))
        {
            adjacents.Add(right);
        }

        return adjacents;
    }

    public int getRegionCount()
	{
		return regions.Count;
	}

	public List<Region> getRegions()
	{
		return regions;
	}

	private bool shouldAddRegion(int numRegions)
    {
        int baseChance = 95;
        baseChance -= (numRegions * 2);
		int chance = new System.Random().Next(100);

        return chance < baseChance;
    }

	public bool RegionsCreated 
	{
		get { return regionsCreated; }
	}
}
