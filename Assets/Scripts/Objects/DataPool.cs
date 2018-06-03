using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System;

public class DataPool 
{
	private List<Boxer> boxers;
	private List<Manager> managers;

    private List<Capitol> capitols;
	private List<Town> towns;

	private Dictionary<string, string> exerciseDescriptions;
	private Dictionary<string, List<List<int>>> exerciseProgress;

    private List<string> firstNames;
    private List<string> lastNames;
    private List<string> townNames;
    private List<string> tournamentNames;
    private List<string> backTournamentNames;

	private Calendar calendar;

    private Dijkstras dijkstras;
    private List<Region> regions;

    private Dictionary<TournamentProtocol.Level, int> distribution;

	private string temporarySaveFile;
	private string temporarySaveDirectory;

	public DataPool(){
		boxers = new List<Boxer> ();
		managers = new List<Manager> ();

        capitols = new List<Capitol>();
		towns = new List<Town> ();

		exerciseDescriptions = new Dictionary<string, string> ();
		exerciseProgress = new Dictionary<string, List<List<int>>> ();

        firstNames = new List<string>();
        lastNames = new List<string>();
        townNames = new List<string>();
        tournamentNames = new List<string>();
        backTournamentNames = new List<string>();

		calendar = new Calendar ();

        dijkstras = new Dijkstras();
        regions = new List<Region>();

        loadNameData();
	}

	public void addExerciseDescription(string exercise, string description){
		exerciseDescriptions.Add (exercise, description);
	}

	public void addExerciseProgress(string exercise, List<int> progress){
		if (!exerciseProgress.Keys.Contains (exercise))
			exerciseProgress.Add (exercise, new List<List<int>> ());

		exerciseProgress [exercise].Add (progress);
	}

    public void calculateRegionDistances()
	{
		for (int i = 0; i < regions.Count; i++)
		{
			for (int j = 0; j < regions.Count; j++)
			{
				if (i != j)
				{
					regions[i].addDistance(j, dijkstras.shortestPath(regions[i].Position, regions[j].Position).Count);
				}
				else regions[i].addDistance(i, 0);
			}
		}
	}

	public string EncryptDecrypt(string textToEncrypt)
    {
        StringBuilder inSb = new StringBuilder(textToEncrypt);
        StringBuilder outSb = new StringBuilder(textToEncrypt.Length);
        char c;
        for (int i = 0; i < textToEncrypt.Length; i++)
        {
            c = inSb[i];
			c = (char)(c ^ 129);
            outSb.Append(c);
        }
        return outSb.ToString();
    }

    public string generateFirstName(){
		return firstNames[generateRandomInt(0, firstNames.Count - 1)];
    }

    public string generateLastName(){
		return lastNames[generateRandomInt(0, lastNames.Count - 1)];
    }

	private static int generateRandomInt(int min, int max)
    {
        return new System.Random((int)System.DateTime.Now.Ticks).Next(min, max);
    }
    
    public string generateTownName(){
		int index = generateRandomInt(0, townNames.Count - 1);
        string townName = townNames[index];
        townNames.RemoveAt(index);
        return townName;
    }

    public string generateTournamentName(){
		if (tournamentNames.Count < 5)
        {
            tournamentNames = new List<string>(backTournamentNames);
        }

		int index = generateRandomInt(0, tournamentNames.Count - 1); 
              
        string name = tournamentNames[index];
        tournamentNames.RemoveAt(index);      

        return name;
    }

	public string getExerciseDescription(string exercise){
		return exerciseDescriptions [exercise];
	}

	public List<int> getExerciseProgress(string exercise, int level){
		return exerciseProgress [exercise] [level - 1];
	}

	public List<int> getExerciseProgressAcculumative(string exercise, int level){
		List<int> progress = new List<int> (new int[] { 0, 0, 0, 0, 0 });

		for (int i = 0; i < level; i++) {
			List<int> currentLevel = exerciseProgress [exercise] [i];
			progress [0] += currentLevel [0];
			progress [1] += currentLevel [1];
			progress [2] += currentLevel [2];
			progress [3] += currentLevel [3];
			progress [4] += currentLevel [4];
		}

		return progress;
	}

    public Tournament getTournamentFromCapitolIndex(int capitolIndex, TournamentProtocol.Level level)
	{
		return capitols[capitolIndex].Quarterlies[level];
	}

	public Tournament getTournamentFromTownIndex(int townIndex)
	{
		return towns[townIndex].Tournament;
	}

	private bool isAdjacent(Vector2Int pos1, Vector2Int pos2)
    {
        Vector2Int above = new Vector2Int(pos1.x, pos1.y + 1);
        Vector2Int below = new Vector2Int(pos1.x, pos1.y - 1);
        Vector2Int left = new Vector2Int(pos1.x - 1, pos1.y);
        Vector2Int right = new Vector2Int(pos1.x + 1, pos1.y);

        if (pos2.Equals(above) || pos2.Equals(below) || pos2.Equals(left) || pos2.Equals(right))
            return true;

        return false;
    }


    public void loadWorld(string saveDirectory, string saveFile)
    {
        loadNameData();

		temporarySaveDirectory = saveDirectory;
		temporarySaveFile = saveFile;

		Thread thread = new Thread(new ThreadStart(loadWorldThread));
		thread.Start();
    }

    public void loadWorldThread()
	{
		using (StreamReader reader = new StreamReader("Assets/Resources/Saves/" + temporarySaveDirectory + "/" + temporarySaveFile + ".save"))
        {
            string saveContents = EncryptDecrypt(reader.ReadToEnd());

            JSONObject json = new JSONObject(saveContents);
            readJson(json);
        }

        foreach (Region r in regions)
        {
            using (StreamReader reader = new StreamReader("Assets/Resources/Saves/" + temporarySaveDirectory + "/Maps/Region_" + r.CapitolIndex + ".tiles"))
            {
                string mapContents = EncryptDecrypt(reader.ReadToEnd());

                JSONObject json = new JSONObject(mapContents);
                r.addWorldMap(readMapJson(json));
            }
        }
	}

    public void loadNameData()
    {
        string firstNameContents = "";
        string lastNameContents = "";
        string townNameContents = "";

        using (StreamReader reader = new StreamReader("Assets/Resources/SourceMaterial/FirstNames.txt"))
        {
            firstNameContents = reader.ReadToEnd();
        }

        using (StreamReader reader = new StreamReader("Assets/Resources/SourceMaterial/LastNames.txt"))
        {
            lastNameContents = reader.ReadToEnd();
        }

        using (StreamReader reader = new StreamReader("Assets/Resources/SourceMaterial/Places.txt"))
        {
            townNameContents = reader.ReadToEnd();
        }

        using (StreamReader reader = new StreamReader("Assets/Resources/SourceMaterial/TournamentNames.txt"))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                tournamentNames.Add(line.Trim());
                backTournamentNames.Add(line.Trim());
            }
        }

        firstNames = firstNameContents.Split(';').ToList();
        lastNames = lastNameContents.Split(';').ToList();
        townNames = townNameContents.Split(';').ToList();
    }

	private RegionCreator.TileType[,] readMapJson(JSONObject json)
    {
        List<List<RegionCreator.TileType>> mapList = new List<List<RegionCreator.TileType>>();

        foreach (JSONObject r in json.list)
        {
            List<RegionCreator.TileType> row = new List<RegionCreator.TileType>();
            foreach (JSONObject c in r.GetField("rowvalues").list)
            {
                row.Add((RegionCreator.TileType)Enum.Parse(typeof(RegionCreator.TileType), c.str));
            }

            mapList.Add(row);
        }

        int width = mapList.Count;
        int height = mapList[0].Count;

        RegionCreator.TileType[,] map = new RegionCreator.TileType[width, height];

        for (int x = 0; x < mapList.Count; x++)
        {
            for (int y = 0; y < mapList[x].Count; y++)
            {
                map[x, y] = mapList[x][y];
            }
        }

        return map;
    }

    private void readJson(JSONObject json)
    {
        boxers = new List<Boxer>();
        foreach (JSONObject r in json.GetField("boxers").list)
        {
            boxers.Add(new Boxer(r));
        }

        managers = new List<Manager>();
        foreach (JSONObject r in json.GetField("managers").list)
        {
            managers.Add(new Manager(r));
        }

        capitols = new List<Capitol>();
        foreach (JSONObject r in json.GetField("capitols").list)
        {
            capitols.Add(new Capitol(r));
        }

        towns = new List<Town>();
        foreach (JSONObject r in json.GetField("towns").list)
        {
            towns.Add(new Town(r));
        }

        calendar = new Calendar(json.GetField("calendar"));

        regions = new List<Region>();
        foreach (JSONObject r in json.GetField("regions").list)
        {
            regions.Add(new Region(r));
        }

        dijkstras = new Dijkstras(json.GetField("dijkstras"));

        updateBoxerDistribution();
    }

    public void saveWorld(string saveDirectory, string saveFile)
	{
		temporarySaveFile = saveFile;
		temporarySaveDirectory = saveDirectory;

		Thread saveThread = new Thread(new ThreadStart(saveWorldThread));
		saveThread.Start();
	}

    private void saveWorldThread()
	{
		JSONObject world = jsonify();
        
        if (!Directory.Exists("Assets/Resources/Saves/" + temporarySaveDirectory + "/"))
        {
			Directory.CreateDirectory("Assets/Resources/Saves/" + temporarySaveDirectory + "/");

        }

        using (StreamWriter writer = new StreamWriter("Assets/Resources/Saves/" + temporarySaveDirectory + "/" + temporarySaveFile + ".save"))
        {
            writer.Write(EncryptDecrypt(world.Print(true)));
        }

		if (!Directory.Exists("Assets/Resources/Saves/" + temporarySaveDirectory + "/Maps/"))
        {
            Directory.CreateDirectory("Assets/Resources/Saves/" + temporarySaveDirectory + "/Maps/");

			Thread saveMapThread = new Thread(new ThreadStart(saveWorldMapThread));
			saveMapThread.Start();
        }
	}

    public void saveWorldMapThread()
	{
		if (!Directory.Exists("Assets/Resources/Saves/" + temporarySaveDirectory + "/Maps/"))
        {
			Directory.CreateDirectory("Assets/Resources/Saves/" + temporarySaveDirectory + "/Maps");

        }

		foreach (Region r in regions)
        {
            using (StreamWriter writer = new StreamWriter("Assets/Resources/Saves/" + temporarySaveDirectory + "/Maps/Region_" + r.CapitolIndex + ".tiles"))
            {
                JSONObject map = new JSONObject(JSONObject.Type.ARRAY);
                for (int x = 0; x < r.Map.GetLength(0); x++)
                {
                    JSONObject row = new JSONObject(JSONObject.Type.OBJECT);
                    row.AddField("x", x);

                    JSONObject column = new JSONObject(JSONObject.Type.ARRAY);
                    for (int y = 0; y < r.Map.GetLength(0); y++)
                    {
                        column.Add(r.Map[x, y].ToString());
                    }
                    row.AddField("rowvalues", column);

                    map.Add(row);
                }

                writer.Write(EncryptDecrypt(map.Print(true)));
            }
        }
	}

    public void setSaveDirectories(string saveDirectory, string saveFile)
	{
		temporarySaveDirectory = saveDirectory;
		temporarySaveFile = saveFile;
	}

    public void updateBoxerDistribution(){
        distribution = new Dictionary<TournamentProtocol.Level, int>();

        foreach (Manager m in managers)
		{
			if (!distribution.ContainsKey(m.Rank))
				distribution.Add(m.Rank, 0);
            
			distribution[m.Rank] += 1;
		}

        if (distribution[TournamentProtocol.Level.E] > 400)
		{
			balanceRank(TournamentProtocol.Level.E);
		}

        if (distribution[TournamentProtocol.Level.D] > 400)
		{
			balanceRank(TournamentProtocol.Level.D);
		}

		if (distribution[TournamentProtocol.Level.C] > 400)
        {
            balanceRank(TournamentProtocol.Level.C);
        }
    }

	private void balanceRank(TournamentProtocol.Level rank)
	{
		foreach (Region r in regions)
		{
			List<int> managerIndexes = r.getRegionsManagerIndexes().FindAll(index => managers[index].Rank.Equals(rank)).ToList();

			managerIndexes = managerIndexes.OrderByDescending(index => EvaluationProtocol.evaluateBoxer(boxers[managers[index].BoxerIndex])).ToList();

            if (managerIndexes.Count > 12)
			    managers[managerIndexes[0]].graduateRank();
		}
	}

	public void updateDijkstras(){
        dijkstras.clearVertices();
        foreach (Region r in regions){
            Dictionary<Vector2Int, int> edges = new Dictionary<Vector2Int, int>();
            foreach (Region s in regions){
                if (isAdjacent(r.Position, s.Position) && !edges.ContainsKey(s.Position)){
                    edges.Add(s.Position, 1);
                }
            }

            dijkstras.addVertex(r.Position, edges);
        }
    }

    public JSONObject jsonify()
	{
		JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

		JSONObject bxrs = new JSONObject(JSONObject.Type.ARRAY);
        foreach (Boxer b in boxers)
		{
			bxrs.Add(b.jsonify());
		}
		json.AddField("boxers", bxrs);

		JSONObject mgrs = new JSONObject(JSONObject.Type.ARRAY);
        foreach (Manager m in managers)
		{
			mgrs.Add(m.jsonify());
		}
		json.AddField("managers", mgrs);

		JSONObject cptl = new JSONObject(JSONObject.Type.ARRAY);
        foreach (Capitol c in capitols)
		{
			cptl.Add(c.jsonify());
		}
		json.AddField("capitols", cptl);

		JSONObject twns = new JSONObject(JSONObject.Type.ARRAY);
        foreach (Town t in towns)
		{
			twns.Add(t.jsonify());
		}
		json.AddField("towns", twns);

		json.AddField("calendar", calendar.jsonify());
		json.AddField("dijkstras", dijkstras.jsonify());

		JSONObject rgns = new JSONObject(JSONObject.Type.ARRAY);
        foreach (Region r in regions)
		{
			rgns.Add(r.jsonify());
		}
		json.AddField("regions", rgns);

		return json;
	}
   

    //Getters
	public List<Boxer> Boxers {
		get { return boxers; }
	}

	public List<Manager> Managers {
		get { return managers; }
	}

	public Calendar Calendar {
		get { return calendar; }
	}

    public List<Capitol> Capitols {
        get { return capitols; }
    }

    public List<Town> Towns {
        get { return towns; }
    }

    public List<Region> Regions {
        get { return regions; }
    }

    public List<string> FirstNames {
        get { return firstNames; }
    }

    public List<string> LastNames {
        get { return lastNames; }
    }

    public List<string> TownNames {
        get { return townNames; }
    }

    public List<string> TournamentNames {
        get { return tournamentNames; }
    }

    public Dictionary<TournamentProtocol.Level, int> Distribution {
        get { return distribution; }
    }

    public Dijkstras Dijkstras {
        get { return dijkstras; }
    }
}
