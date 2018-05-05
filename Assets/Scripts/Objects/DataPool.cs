using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class DataPool {

	private List<Boxer> boxers;
	private List<Manager> managers;
	private List<ManagerProtocol> managerProtocols;

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

	public DataPool(){
		boxers = new List<Boxer> ();
		managers = new List<Manager> ();
		managerProtocols = new List<ManagerProtocol> ();

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

    public string generateFirstName(){
        return firstNames[Random.Range(0, firstNames.Count)];
    }

    public string generateLastName(){
        return lastNames[Random.Range(0, lastNames.Count)];
    }

    public string generateTownName(){
        int index = Random.Range(0, townNames.Count);
        string townName = townNames[index];
        townNames.RemoveAt(index);
        return townName;
    }

    public string generateTournamentName(){
        int index = Random.Range(0, tournamentNames.Count);
        string name = tournamentNames[index];
        tournamentNames.RemoveAt(index);

        if (tournamentNames.Count < 2){
            tournamentNames = backTournamentNames;
        }

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

    public void loadNameData(){
        string firstNameContents = "";
        string lastNameContents = "";
        string townNameContents = "";

        using (StreamReader reader = new StreamReader("Assets/Resources/SourceMaterial/FirstNames.txt")){
            firstNameContents = reader.ReadToEnd();
        } 

        using (StreamReader reader = new StreamReader("Assets/Resources/SourceMaterial/LastNames.txt")){
            lastNameContents = reader.ReadToEnd();
        } 

        using (StreamReader reader = new StreamReader("Assets/Resources/SourceMaterial/Places.txt")){
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

    public void updateBoxerDistribution(){
        distribution = new Dictionary<TournamentProtocol.Level, int>();

        foreach (ManagerProtocol mp in ManagerProtocols)
        {
            if (!distribution.ContainsKey(mp.Rank))
                distribution.Add(mp.Rank, 0);

            distribution[mp.Rank] += 1;
        }
    }

    public void updateDijkstras(){
        dijkstras.clearVertices();
        foreach (Region r in regions){
            Dictionary<Vector2Int, int> edges = new Dictionary<Vector2Int, int>();
            foreach (Region s in regions){
                if (isAdjacent(r.Position, s.Position)){
                    edges.Add(s.Position, 1);
                }
            }

            dijkstras.addVertex(r.Position, edges);
        }
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

    //Getters
	public List<Boxer> Boxers {
		get { return boxers; }
	}

	public List<Manager> Managers {
		get { return managers; }
	}

	public List<ManagerProtocol> ManagerProtocols {
		get { return managerProtocols; }
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
