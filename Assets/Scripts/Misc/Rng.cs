using UnityEngine;
using System.Collections;

public static class Rng
{
	public static int generateRandomInt(int min, int max)
    {
        return new System.Random((int)System.DateTime.Now.Ticks).Next(min, max);
    }
}
