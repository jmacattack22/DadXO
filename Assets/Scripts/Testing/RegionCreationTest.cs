using UnityEngine;
using System.Collections;

public class RegionCreationTest : MonoBehaviour
{
    private DataPool worldData;

    void Start()
    {
        worldData = new DataPool();
        //WorldBuilderProtocol.createRegions(250, 250, ref worldData);

        print(worldData.Regions.Count);

        foreach (Region r in worldData.Regions){
            print(r.Position);
        }

    }

}
