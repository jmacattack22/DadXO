using UnityEngine;
using System.Threading;

public class WorldBuilderMenuHandler : MonoBehaviour
{
	private bool creatingNewWorld = false;
	private WorldBuilderBehaviour worldBuilder;

    void Awake()
    {
		worldBuilder = GetComponent<WorldBuilderBehaviour>();
    }
    
    void Update()
    {
		newWorldCreationCheck();
    }

    public void createWorld()
	{
		worldBuilder.createNewWorld();
        creatingNewWorld = true;
	}

    public DataPool extractWorldData()
	{
		return worldBuilder.getWorldData();
	}

	private void newWorldCreationCheck()
    {
        if (creatingNewWorld && worldBuilder.State.Equals(WorldBuilderBehaviour.BuilderState.Complete))
        {
            worldBuilder.getWorldData().setSaveDirectories("AutoSave1", "AutoSave1");
			Thread saveMapThread = new Thread(new ThreadStart(worldBuilder.getWorldData().saveWorldMapThread));
            saveMapThread.Start();
        }
    }

    //Getters
    public bool creationInProgress
	{
		get { return creatingNewWorld && !worldBuilder.State.Equals(WorldBuilderBehaviour.BuilderState.Complete); }
	}
}
