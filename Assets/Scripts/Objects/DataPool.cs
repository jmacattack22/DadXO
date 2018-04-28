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

	private Calendar calendar;

    Region.TileType[,] worldMap;

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

		calendar = new Calendar ();

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

        using (StreamReader reader = new StreamReader("Assets/Resources/FirstNames.txt")){
            firstNameContents = reader.ReadToEnd();
        } 

        using (StreamReader reader = new StreamReader("Assets/Resources/LastNames.txt")){
            lastNameContents = reader.ReadToEnd();
        } 

        using (StreamReader reader = new StreamReader("Assets/Resources/Places.txt")){
            townNameContents = reader.ReadToEnd();
        }

        firstNames = firstNameContents.Split(';').ToList();
        lastNames = lastNameContents.Split(';').ToList();
        townNames = townNameContents.Split(';').ToList();
    }

    public void setWorldMap(Region.TileType[,] worldMap){
        this.worldMap = worldMap;
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

    public List<string> FirstNames {
        get { return firstNames; }
    }

    public List<string> LastNames {
        get { return lastNames; }
    }

    public List<string> TownNames {
        get { return townNames; }
    }

    public Region.TileType[,] WorldMap {
        get { return worldMap; }
    }
}
