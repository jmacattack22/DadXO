﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Diagnostics;

public class WorldBuilderBehaviour : MonoBehaviour
{
    public enum BuilderState
	{
		None, Exercises, Regions, PopulatingTowns, Qualifiers, Complete
	}

	private BuilderState state;

	private bool startQualifierThread;
	private Thread qualifierThread;

	public int regionTarget;
	public int regionWidth;
	public int regionHeight;

	private int debugRegionID = 0;
	private string debugState = "";

	private bool regionsCreated = false;
	private List<Vector2Int> regionLocations;
	private Thread regionThread;

	private Thread townThread;

    private DataPool worldData;

	private Stopwatch stopwatch;

    void Start()
    {
		stopwatch = new Stopwatch();
		state = BuilderState.None;
		startQualifierThread = false;
		regionLocations = new List<Vector2Int>();
    }

	private void addRegionsToWorld()
    {
        foreach (Vector2Int pos in regionLocations)
		{
			Region region = new Region("", pos);
			region.addWorldMap(RegionCreator.CreateRegion(regionWidth, regionHeight));

			worldData.Regions.Add(region);
		}

        worldData.updateDijkstras();
    }

    private float calculateQualifierSetupCompletePercentage()
	{
		float current = 0.0f;
		float target = regionTarget;

        foreach (Capitol c in worldData.Capitols)
		{
			if (c.Quarterlies.Keys.Count > 0)
				current += 1.0f;
		}

		return current / target;
	}

	private float calculateRegionCompletePercentage()
    {
        float current = worldData.Regions.Count;
        float target = regionTarget;

        return (current / target);
    }

    private float calculateTownCompletePercentage()
    {
        float current = worldData.Towns.Count;
        float target = regionTarget * 21.0f;

        return (current / target);
    }

	public void createNewWorld()
	{
		stopwatch.Start();
		state = BuilderState.Exercises;
		worldData = new DataPool();

		WorldBuilderProtocol.initExercises(ref worldData);

		state = BuilderState.Regions;
		regionThread = new Thread(new ThreadStart(generateRegions));
		regionThread.Start();
	}

	private void createRegion(string regionName, Vector2Int pos)
	{
		worldData.Regions.Add(new Region(regionName, pos));
		worldData.Regions[worldData.Regions.Count - 1].addWorldMap(RegionCreator.CreateRegion(regionWidth, regionHeight));
	}

	private void defineQualifiers()
    {
        WorldBuilderProtocol.defineQualifiers(ref worldData);

        worldData.updateBoxerDistribution();
        state = BuilderState.Complete;

        stopwatch.Stop();
        print(stopwatch.Elapsed);
    }

    private void generateRegions()
	{
		createRegion("", new Vector2Int(0, 0));

		List<int> regionIndexes = new List<int>();
		regionIndexes.Add(0);

		List<int> temporaryNewIndexes = new List<int>();
		       
        while (worldData.Regions.Count < regionTarget)
		{
			temporaryNewIndexes = new List<int>();

            foreach (int index in regionIndexes)
			{
				List<Vector2Int> newRegionsToAdd = getAdjacents(ref worldData, worldData.Regions[index].Position);

                if (newRegionsToAdd.Count > 0)
				{
					foreach (Vector2Int pos in newRegionsToAdd)
					{
						createRegion("", pos);
						temporaryNewIndexes.Add(worldData.Regions.Count - 1);
					}
				}
				else
				{
					temporaryNewIndexes.Add(index);
				}
			}

			regionIndexes = temporaryNewIndexes;
		}

		regionsCreated = true;
	}

	public List<Vector2Int> getAdjacents(ref DataPool worldData, Vector2Int newRegion)
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

        foreach (Region region in worldData.Regions)
        {
            if (region.Position.Equals(above))
                aboveExists = true;

            if (region.Position.Equals(below))
                belowExists = true;

            if (region.Position.Equals(left))
                leftExists = true;

            if (region.Position.Equals(right))
                rightExists = true;
        }

        if (!aboveExists && shouldAddRegion(worldData.Regions.Count))
        {
            adjacents.Add(above);
        }

        if (!belowExists && shouldAddRegion(worldData.Regions.Count))
        {
            adjacents.Add(below);
        }

        if (!leftExists && shouldAddRegion(worldData.Regions.Count))
        {
            adjacents.Add(left);
        }

        if (!rightExists && shouldAddRegion(worldData.Regions.Count))
        {
            adjacents.Add(right);
        }

        return adjacents;
     }
 
    public DataPool getWorldData()
	{
		return worldData;
	}

    private float getPercentage()
	{
		float baseComplete = 0.0f;

		if (state.Equals(BuilderState.Exercises))
		{
			baseComplete = 0.0f;
		}
		else if (state.Equals(BuilderState.Regions))
		{
			baseComplete = 10.0f;
			baseComplete += 40.0f * calculateRegionCompletePercentage();
		}
		else if (state.Equals(BuilderState.PopulatingTowns))
		{
			baseComplete = 50.0f;
			baseComplete += 40.0f * calculateTownCompletePercentage();
		}
		else if (state.Equals(BuilderState.Qualifiers))
		{
			baseComplete = 90.0f;
			baseComplete += 10.0f * calculateQualifierSetupCompletePercentage();
		}      

		return baseComplete;
	}

	private void populateWorldWithTowns()
    {
		for (int i = 0; i < worldData.Regions.Count; i++)
        {
			debugRegionID = i;

			//if (worldData.Regions[i].getLandmass() >= 1600)
			//WorldBuilderProtocol.createCapitol(ref worldData, i);
			debugState = "capitol";
			WorldBuilderProtocol.createCapitol(ref worldData, i);
			debugState = "town";
			WorldBuilderProtocol.createTowns(ref worldData, i);
        }

		startQualifierThread = true;
    }

	private bool shouldAddRegion(int numRegions)
    {
        int baseChance = 140;
        baseChance -= (numRegions * 2);
        int chance = new System.Random().Next(100);

        return chance < baseChance;
    }

	void Update()
	{
		if (regionsCreated && state.Equals(BuilderState.Regions))
		{
			print(regionLocations + " Regions created");

			addRegionsToWorld();

			state = BuilderState.PopulatingTowns;
			townThread = new Thread(new ThreadStart(populateWorldWithTowns));
			townThread.Start();
		} 
		else if (state.Equals(BuilderState.PopulatingTowns))
		{
            if (startQualifierThread)
			{
				qualifierThread = new Thread(new ThreadStart(defineQualifiers));
				qualifierThread.Start();

				state = BuilderState.Qualifiers;
			}
		}
		else if (state.Equals(BuilderState.Qualifiers))
		{
			
		}
			

		if (!state.Equals(BuilderState.None) && !state.Equals(BuilderState.Complete))
		{
			//if (worldData != null)
			//{
			//	if (worldData.Regions.Count > 0)
			//	{
			//		print(debugRegionID + " - " + worldData.Regions[debugRegionID].getLandmass());
			//	}
			//}

			print(debugRegionID + " - " + worldData.Regions[debugRegionID].LandMass + " - " + 
			      debugState + " - " + worldData.Regions[debugRegionID].getRegionsTownIndexes().Count + " - " +
			     worldData.Towns.Count);
			print(state + " " + getPercentage() + "% Complete.");
		}
	}

    //Getters
    public BuilderState State 
	{
		get { return state; }
	}
}
