using UnityEngine;
using System.Collections;

public class TileCreationTest : MonoBehaviour
{
    private DataPool worldData;

	void Start()
	{
        worldData = new DataPool();
        WorldBuilderProtocol.createWorld(ref worldData, 220, 220);
	}
}
