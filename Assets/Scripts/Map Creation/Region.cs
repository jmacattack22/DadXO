using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class Region
{
    public enum TileType
    {
        Water, Shallows, Beach, Coastal, Land, Mountain, Rise, Peak, Town
    }

    public static TileType[,] CreateRegion(int width, int height)
    {
        TileType[,] map = new TileType[width, height];

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                map[x, y] = TileType.Water;

        Perlin perlinNoise = new Perlin(width, height, 8, 0);

        double[,] perlinMap = perlinNoise.GetPerlinNoiseArray();

        double max = 0;
        double min = 10100000;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (perlinMap[x, y] > max)
                    max = perlinMap[x, y];

                if (perlinMap[x, y] < min)
                    min = perlinMap[x, y];
            }
        }

        double newMax = 0;
        double newMin = 12;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                perlinMap[x, y] = (perlinMap[x, y] - min) / (max - min);

                if (perlinMap[x, y] > newMax)
                    newMax = perlinMap[x, y];

                if (perlinMap[x, y] < newMin)
                    newMin = perlinMap[x, y];
            }
        }

        List<Vector2> gradientPoints = new List<Vector2>();

        //Choose gradient points to build island peaks
        gradientPoints.Add(new Vector2(90, 90));
        gradientPoints.Add(new Vector2(125, 125));
        //gradientPoints.Add(new Vector2(225, 175));
        //gradientPoints.Add(new Vector2(175, 125));
        //gradientPoints.Add(new Vector2(175, 225));

        double[,] gradientNoise = new double[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 closestPoint = new Vector2();
                double minDistance = 100000000;

                //Search for closest peak
                foreach (Vector2 p in gradientPoints)
                {
                    double distance = Mathf.Sqrt(Mathf.Pow(p.x - x, 2) + Mathf.Pow(p.y - y, 2));

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestPoint = p;
                    }
                }

                gradientNoise[x, y] = minDistance / (width * 1.0f);
                perlinMap[x, y] = perlinMap[x, y] - gradientNoise[x, y];

                #region Map Charting

                double currentHeight = perlinMap[x, y] * 1000;

                map[x, y] = getTileFromHeight(currentHeight);

                #endregion

                if ((x >= 0 && x < 7) || (x >= width - 8 && x < width - 1))
                    if ((int)map[x, y] > 1)
                        map[x, y] = TileType.Shallows;

                if ((y >= 0 && y < 7) || (y >= height - 8 && y < height))
                    if ((int)map[x, y] > 1)
                        map[x, y] = TileType.Shallows;
            }
        }

        if (calculateLandMass(width, height, ref map) < 1000){
            raiseLand(width, height, ref perlinMap, ref map);
        }

        return map;
    }

    public static int calculateLandMass(int width, int height, ref TileType[,] map)
    {
        int landMass = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!map[x, y].Equals(TileType.Water))
                    landMass++;
            }
        }

        return landMass;
    }

    private static TileType getTileFromHeight(double height){
        if (height >= 825)
        {
            return TileType.Peak;
        }
        else if (height >= 750 && height < 825)
        {
            return TileType.Rise;
        }
        else if (height >= 675 && height < 750)
        {
            return TileType.Mountain;
        }
        else if (height >= 600 && height < 675)
        {
            return TileType.Land;
        }
        else if (height >= 550 && height < 600)
        {
            return TileType.Coastal;
        }
        else if (height >= 520 && height < 550)
        {
            return TileType.Beach;
        }
        else if (height >= 480 && height < 520)
        {
            return TileType.Shallows;
        }

        return TileType.Water;
    }

    private static void raiseLand(int width, int height, ref double[,] perlinMap, ref TileType[,] map){
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                double currentHeight = perlinMap[x, y] * 1000;

                map[x, y] = getTileFromHeight(currentHeight + 100);
            }
        }
    }
}
