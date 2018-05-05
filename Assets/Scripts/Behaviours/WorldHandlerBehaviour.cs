using UnityEngine;
using System.Collections;

public class WorldHandlerBehaviour : MonoBehaviour
{
    public WorldMapDrawer mapDrawer;

    private DataPool worldData;

	void Start()
	{
        worldData = new DataPool();
        //WorldBuilderProtocol.createRegions(175, 175, ref worldData);
        WorldBuilderProtocol.createWorld(ref worldData, 220, 220);
        mapDrawer.drawRegionsTemp(ref worldData);
        //mapDrawer.drawRegions(worldData.Regions);
        worldData.updateBoxerDistribution();
	}

	void Update()
	{
			
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
