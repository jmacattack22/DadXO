using UnityEngine;
using System.Collections;
using System.Threading;

public class WorldHandlerBehaviour : MonoBehaviour
{
    public WorldMapDrawer mapDrawer;

	private bool creatingNewWorld = false;
	public WorldBuilderBehaviour worldBuilder;
	private Thread worldBuilderThread;

    private DataPool worldData;

	void Start()
	{
		worldBuilder.createNewWorld();
		creatingNewWorld = true;
	}

	void Update()
	{
        if (creatingNewWorld && worldBuilder.State.Equals(WorldBuilderBehaviour.BuilderState.Complete))
		{
			worldData = worldBuilder.getWorldData();
			mapDrawer.drawRegions(ref worldData);
			creatingNewWorld = false;
		}
	}

    public void advanceWeek(){
        TournamentHandlerProtocol.simTournamentsAndTraining(ref worldData);
        worldData.updateBoxerDistribution();

        worldData.Calendar.progessWeek();
        Debug.Log(worldData.Calendar.getDate(Calendar.DateType.fullLong));
    }

    //Getters
    public DataPool Data {
        get { return worldData; }
    }
}
