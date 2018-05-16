using UnityEngine;
using System.Collections;
using System;
using System.Threading;

public class WorldBuilderBehaviour : MonoBehaviour
{
    public enum BuilderState
	{
		None, Exercises, Regions, PopulatingTowns, Qualifiers, Complete
	}

	private BuilderState state;
    
    public RegionCreatorThread regionCreatorThread;

	private bool startQualifierThread;
	private Thread qualifierThread;

	private Thread townThread;

    private DataPool worldData;

    void Start()
    {
		state = BuilderState.None;
		startQualifierThread = false;
    }

    public void createNewWorld()
	{
		state = BuilderState.Exercises;
        worldData = new DataPool();

        WorldBuilderProtocol.initExercises(ref worldData);

		state = BuilderState.Regions;
		regionCreatorThread.createRegions();             
	}

    private void addRegionsToWorld()
	{
		foreach (Region r in regionCreatorThread.getRegions())
        {
            worldData.Regions.Add(r);
        }

        worldData.updateDijkstras();
	}
    
    private float calculateTownCompletePercentage()
	{
		float current = worldData.Towns.Count;
		float target = 660.0f;

		return (current / target);
	}

    private void defineQualifiers()
	{
		WorldBuilderProtocol.defineQualifiers(ref worldData);

        worldData.updateBoxerDistribution();
		state = BuilderState.Complete;
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
			baseComplete += 40.0f * regionCreatorThread.calculatePercentageDone();
		}
		else if (state.Equals(BuilderState.PopulatingTowns))
		{
			baseComplete = 50.0f;
			baseComplete += 40 * calculateTownCompletePercentage();
		}
		else if (state.Equals(BuilderState.Qualifiers))
		{
			baseComplete = 90.0f;
		}      

		return baseComplete;
	}

	private void populateWorldWithTowns()
    {
		for (int i = 0; i < worldData.Regions.Count; i++)
        {
			if (worldData.Regions[i].getLandmass() >= 1100)
                WorldBuilderProtocol.createCapitol(ref worldData, i);
			
			WorldBuilderProtocol.createTowns(ref worldData, i);
        }

		startQualifierThread = true;
    }

	void Update()
	{
		if (regionCreatorThread.RegionsCreated && state.Equals(BuilderState.Regions))
		{
			print(regionCreatorThread.getRegionCount() + " Regions created");

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
			print(state + " " + getPercentage() + "% Complete.");
		}
	}

    //Getters
    public BuilderState State 
	{
		get { return state; }
	}
}
