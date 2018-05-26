using System;
using System.Collections.Generic;
using UnityEngine;

public class Dijkstras
{
    private Dictionary<Vector2Int, Dictionary<Vector2Int, int>> vertices = new Dictionary<Vector2Int, Dictionary<Vector2Int, int>>();

    public Dijkstras(){
        
    }
   
	public Dijkstras(JSONObject json)
	{
		foreach (JSONObject r in json.list)
		{
			Vector2Int position = JSONTemplates.ToVector2Int(r.GetField("position"));

			Dictionary<Vector2Int, int> tempDict = new Dictionary<Vector2Int, int>();
            foreach (JSONObject v in r.GetField("adjacents").list)
			{
				Vector2Int position2 = JSONTemplates.ToVector2Int(v.GetField("position"));
				int distance = (int)v.GetField("distance").i;

				tempDict.Add(position2, distance);
			}
			vertices.Add(position, tempDict);
		}
	}

    public void addVertex(Vector2Int position, Dictionary<Vector2Int, int> edges){
        vertices[position] = edges;
    }

    public void clearVertices(){
        vertices.Clear();
    }

    public List<Vector2Int> shortestPath(Vector2Int start, Vector2Int finish){
        var previous = new Dictionary<Vector2Int, Vector2Int>();
        var distances = new Dictionary<Vector2Int, int>();
        var nodes = new List<Vector2Int>();

        List<Vector2Int> path = null;

        foreach (var vertex in vertices){
            if (vertex.Key == start)
            {
                distances[vertex.Key] = 0;
            }
            else 
            {
                distances[vertex.Key] = int.MaxValue;
            }

            nodes.Add(vertex.Key);
        }

        while (nodes.Count != 0){
            nodes.Sort((x, y) => distances[x] - distances[y]);

            var smallest = nodes[0];
            nodes.Remove(smallest);

            if (smallest == finish){
                path = new List<Vector2Int>();
                while (previous.ContainsKey(smallest))
                {
                    path.Add(smallest);
                    smallest = previous[smallest];
                }

                break; 
            }

            if (distances[smallest] == int.MaxValue)
            {
                break;
            }

            foreach (var neighbor in vertices[smallest])
            {
                var alt = distances[smallest] + neighbor.Value;
                if (alt < distances[neighbor.Key])
                {
                    distances[neighbor.Key] = alt;
                    previous[neighbor.Key] = smallest;
                }
            }
        }

        return path;
    }

    public JSONObject jsonify()
	{
		JSONObject json = new JSONObject(JSONObject.Type.ARRAY);

		foreach (Vector2Int v in vertices.Keys)
		{
			JSONObject tempJson = new JSONObject(JSONObject.Type.OBJECT);
			tempJson.AddField("position", JSONTemplates.FromVector2Int(v));

			JSONObject adjacents = new JSONObject(JSONObject.Type.ARRAY);
            foreach (Vector2Int v2 in vertices[v].Keys)
			{
				JSONObject adjacent = new JSONObject(JSONObject.Type.OBJECT);
				adjacent.AddField("position", JSONTemplates.FromVector2Int(v2));
				adjacent.AddField("distance", vertices[v][v2]);
				adjacents.Add(adjacent);
			}

			tempJson.AddField("adjacents", adjacents);
			json.Add(tempJson);
		}

		return json;
	}
}
