using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class WorldBuilderProtocol {

	public static void createWorld(ref DataPool worldData, int width, int height){
        initExercises (ref worldData);
        createRegions(220, 220, ref worldData);
	}

    private static void ageAndDevelop(ref DataPool worldData, int index, TournamentProtocol.Level level){
        float agePercentage = getAgeFromLevel(level);

        List<EvaluationProtocol.Stats> bestTraits = new List<EvaluationProtocol.Stats>();
        List<EvaluationProtocol.Stats> secondaryTraits = new List<EvaluationProtocol.Stats>();

        List<EvaluationProtocol.Stats> build = BoxerClass.getBuild(worldData.Boxers[index].BoxerClass);

        if (build.Contains(EvaluationProtocol.Stats.AccuracyGrowth))
            bestTraits.Add(EvaluationProtocol.Stats.Accuracy);
        else
            secondaryTraits.Add(EvaluationProtocol.Stats.Accuracy);

        if (build.Contains(EvaluationProtocol.Stats.EnduranceGrowth))
            bestTraits.Add(EvaluationProtocol.Stats.Endurance);
        else
            secondaryTraits.Add(EvaluationProtocol.Stats.Endurance);

        if (build.Contains(EvaluationProtocol.Stats.HealthGrowth))
            bestTraits.Add(EvaluationProtocol.Stats.Health);
        else
            secondaryTraits.Add(EvaluationProtocol.Stats.Health);

        if (build.Contains(EvaluationProtocol.Stats.SpeedGrowth))
            bestTraits.Add(EvaluationProtocol.Stats.Speed);
        else
            secondaryTraits.Add(EvaluationProtocol.Stats.Speed);

        if (build.Contains(EvaluationProtocol.Stats.StrengthGrowth))
            bestTraits.Add(EvaluationProtocol.Stats.Strength);
        else
            secondaryTraits.Add(EvaluationProtocol.Stats.Strength);

        foreach (EvaluationProtocol.Stats stat in bestTraits){
            float value = getPrimaryStatFromLevel(level) * (agePercentage / 100.0f);
            value = value < 55.0f ? 55.0f : value;
            worldData.Boxers[index].modifyStat(stat, Mathf.RoundToInt(value));
        }

        foreach (EvaluationProtocol.Stats stat in secondaryTraits)
        {
            float value = getSecondaryStatFromLevel(level) * (agePercentage / 100.0f);
            value = value < 40.0f ? 40.0f : value;
            worldData.Boxers[index].modifyStat(stat, Mathf.RoundToInt(value));
        }

        worldData.Boxers[index].cutLife(Mathf.RoundToInt(worldData.Boxers[index].WeeksRemaining * (agePercentage / 100.0f)));
    }

	public static Boxer createBoxerBasedOnFame(string firstName, string lastName, int townIndex, float elo, WeightClass.WClass wClass){
		int pointsToGive = EvaluationProtocol.getBoxerPointsFromFame (elo);

		int badStatRates = Mathf.RoundToInt ((pointsToGive / 3) / 2);
		badStatRates = badStatRates < 1 ? 1 : badStatRates;

		List<BoxerClass.Type> possibleClasses = BoxerClass.getClassesBasedOnWeight (wClass);

		List<EvaluationProtocol.Stats> stats = new List<EvaluationProtocol.Stats> (new EvaluationProtocol.Stats[] {
			EvaluationProtocol.Stats.AccuracyGrowth, EvaluationProtocol.Stats.EnduranceGrowth, EvaluationProtocol.Stats.HealthGrowth,
			EvaluationProtocol.Stats.SpeedGrowth, EvaluationProtocol.Stats.StrengthGrowth
		});

		Dictionary<EvaluationProtocol.Stats, int> growthRates = new Dictionary<EvaluationProtocol.Stats, int> ();
		Dictionary<EvaluationProtocol.Stats, int> statValues = new Dictionary<EvaluationProtocol.Stats, int> ();

		foreach (EvaluationProtocol.Stats stat in stats) {
			growthRates.Add (stat, badStatRates);
		}

		BoxerClass.Type bClass = possibleClasses [Random.Range (0, possibleClasses.Count)];

		List<EvaluationProtocol.Stats> bestStats = BoxerClass.getBuild (bClass);

		for (int i = 0; i < 3; i++) {
			int baseStat = Mathf.RoundToInt (pointsToGive / (3 - i));

			if (pointsToGive > 1) {
				baseStat = Random.Range (baseStat - 1, baseStat + 2);
				baseStat = baseStat < 1 ? 1 : baseStat;
				baseStat = baseStat > 10 ? 10 : baseStat;
				baseStat = (pointsToGive - baseStat) < 2 ? baseStat - 1 : baseStat;
				baseStat = baseStat < 1 ? 1 : baseStat;
			} else
				baseStat = pointsToGive;


			EvaluationProtocol.Stats stat = bestStats [Random.Range (0, bestStats.Count)];

			growthRates [stat] = baseStat;
			bestStats.RemoveAt (bestStats.IndexOf (stat));
			pointsToGive -= baseStat;
		}

		int acc = EvaluationProtocol.getStatValueFromGrowthRate (growthRates [EvaluationProtocol.Stats.AccuracyGrowth]);
		int end = EvaluationProtocol.getStatValueFromGrowthRate (growthRates [EvaluationProtocol.Stats.EnduranceGrowth]);
		int hlt = EvaluationProtocol.getStatValueFromGrowthRate (growthRates [EvaluationProtocol.Stats.HealthGrowth]);
		int spd = EvaluationProtocol.getStatValueFromGrowthRate (growthRates [EvaluationProtocol.Stats.SpeedGrowth]);
		int str = EvaluationProtocol.getStatValueFromGrowthRate (growthRates [EvaluationProtocol.Stats.StrengthGrowth]);

		statValues.Add(EvaluationProtocol.Stats.Accuracy, Random.Range(acc - 4, acc + 4));
		statValues.Add(EvaluationProtocol.Stats.Endurance, Random.Range(end - 4, end + 4));
		statValues.Add(EvaluationProtocol.Stats.Health, Random.Range(hlt - 4, hlt + 4));
		statValues.Add(EvaluationProtocol.Stats.Speed, Random.Range(spd - 4, spd + 4));
		statValues.Add(EvaluationProtocol.Stats.Strength, Random.Range(str - 4, str + 4));

		return new Boxer (
			new Vector2Int(0,1), firstName, lastName, townIndex, generateWeightFromClass(wClass), bClass,
			statValues [EvaluationProtocol.Stats.Accuracy], growthRates [EvaluationProtocol.Stats.AccuracyGrowth],
			statValues [EvaluationProtocol.Stats.Endurance], growthRates [EvaluationProtocol.Stats.EnduranceGrowth],
			statValues [EvaluationProtocol.Stats.Health], growthRates [EvaluationProtocol.Stats.HealthGrowth],
			statValues [EvaluationProtocol.Stats.Speed], growthRates [EvaluationProtocol.Stats.SpeedGrowth],
			statValues [EvaluationProtocol.Stats.Strength], growthRates [EvaluationProtocol.Stats.StrengthGrowth]);

	}

    private static float getEloFromRegion(TournamentProtocol.Level rank){
        if (rank.Equals(TournamentProtocol.Level.E))
        {
            return Random.Range(1.0f, 375.0f);
        } 
        else if (rank.Equals(TournamentProtocol.Level.D))
        {
            return Random.Range(350.0f, 750.0f);
        } 
        else if (rank.Equals(TournamentProtocol.Level.C))
        {
            return Random.Range(750.0f, 1125.0f);
        } 
        else if (rank.Equals(TournamentProtocol.Level.B))
        {
            return Random.Range(1125.0f, 1500.0f);
        } 
        else if (rank.Equals(TournamentProtocol.Level.A))
        {
            return Random.Range(1500.0f, 1875.0f);
        }
        else if (rank.Equals(TournamentProtocol.Level.S))
        {
            return Random.Range(1875.0f, 2250.0f);
        }

        return Random.Range(2250.0f, 2450.0f);
    }

    private static void createManagerBasedOnTown(ref DataPool worldData, int townIndex){
        List<BoxerClass.Type> typeList = BoxerClass.getTypeList();

        Manager manager = new Manager(
            worldData.generateFirstName(), worldData.generateLastName(), townIndex, Random.Range(145.0f, 225.0f), typeList[Random.Range(0, typeList.Count)]);
        manager.Record.setELO(getEloFromRegion(worldData.Towns[townIndex].RegionLevel));
        worldData.Managers.Add(manager);

        ManagerProtocol mp = new ManagerProtocol(ref worldData, worldData.Managers.Count - 1);

        List<Boxer> boxers = WorldBuilderProtocol.generateBoxerRecruits(ref worldData, manager.TownIndex, manager.Record.ELO);

        int bIndex = 0;
        float max = 0.0f;

        for (int i = 0; i < boxers.Count; i++)
        {
            float boxerEval = EvaluationProtocol.evaluateBoxer(boxers[i], worldData.Managers[mp.ManagerIndex].Preference);

            if (boxerEval > max)
            {
                max = boxerEval;
                bIndex = i;
            }
        }

        TournamentProtocol.Level boxerLevel = (TournamentProtocol.Level)Random.Range(0, ((int)worldData.Towns[townIndex].RegionLevel) + 1);

        worldData.Boxers.Add(boxers[bIndex]);
        mp.recruitBoxer(worldData.Boxers.Count - 1);
        mp.updateELO(ref worldData);
        mp.upgradeFacilities(ref worldData);
        mp.setRank(boxerLevel);
        ageAndDevelop(ref worldData, worldData.Boxers.Count - 1, boxerLevel);

        worldData.ManagerProtocols.Add(mp);
    }

    public static void createRegions(int width, int height, ref DataPool worldData){
        Region.TileType[,] worldMap = new Region.TileType[width * 5, height * 5];
        List<int> landMass = new List<int>();

        for (int i = 0; i < 5; i++){
            for (int j = 0; j < 5; j++){
                Region.TileType[,] map = Region.CreateRegion(width, height);
                landMass.Add(Region.calculateLandMass(width, height, ref map));

                for (int x = 0; x < width; x++){
                    for (int y = 0; y < height; y++){
                        worldMap[x + (width * i), y + (height * j)] = map[x, y];
                    }
                }
            }
        }

        worldData.setWorldMap(worldMap);
        createTowns(ref worldData);

        Dictionary<TournamentProtocol.Level, int> townByRegion = new Dictionary<TournamentProtocol.Level, int>();

        foreach (Town t in worldData.Towns){
            if (!townByRegion.ContainsKey(t.RegionLevel))
                townByRegion.Add(t.RegionLevel, 0);

            townByRegion[t.RegionLevel] += 1;
        }

        using (StreamWriter writer =
            new StreamWriter("important.txt"))
        {
            writer.WriteLine(worldData.Towns.Count);
            foreach (int mass in landMass){
                writer.Write(mass + ",");
            }
            writer.Write("\n");
            foreach (TournamentProtocol.Level r in townByRegion.Keys){
                writer.WriteLine(r.ToString() + " - " + townByRegion[r].ToString());
            }

            for (int x = 0; x < worldMap.GetLength(0); x++){
                for (int y = 0; y < worldMap.GetLength(0); y++){
                    writer.Write((int)worldMap[x, y]);
                }
                writer.Write("\n");
            }
        }

        //foreach (Town t in worldData.Towns){
        //    if (t.Tournament.TournamentLevel.Equals(TournamentProtocol.Level.E)){
        //        Debug.Log(t.Location.ToString() + " - " + t.Tournament.getDetails());
        //    }
        //}
    }

    public static TournamentProtocol createQuarterlyTournament(TournamentProtocol.Level level, CalendarDate date){
        return new TournamentProtocol(date, getPrizeMoney(level) + 1000.0f, 16, level);
    }

    public static TournamentProtocol createTournamentBasedOnRegion(TournamentProtocol.Level regionLevel, CalendarDate date){
        List<int> tournamentPercentages = getPercentagesForRegion(regionLevel);

        int rng = Random.Range(0, 100);
        TournamentProtocol.Level tournamentLevel = TournamentProtocol.Level.E;

        if (rng >= tournamentPercentages[0])
        {
            tournamentLevel = TournamentProtocol.Level.S;
        } 
        else if (rng >= tournamentPercentages[1])
        {
            tournamentLevel = TournamentProtocol.Level.A;
        } 
        else if (rng >= tournamentPercentages[2])
        {
            tournamentLevel = TournamentProtocol.Level.B;
        }
        else if (rng >= tournamentPercentages[3])
        {
            tournamentLevel = TournamentProtocol.Level.C;
        }
        else if (rng >= tournamentPercentages[4]) 
        {
            tournamentLevel = TournamentProtocol.Level.D;
        }
        else
        {
            tournamentLevel = TournamentProtocol.Level.E;
        }

        return new TournamentProtocol(date, getPrizeMoney(tournamentLevel), getRandomTournamentSize((int)tournamentLevel), tournamentLevel);
    }

    public static void createTowns(
        ref DataPool worldData)
    {
        bool leftTopRegion = false;
        bool leftBottomRegion = false;
        bool rightTopRegion = false;
        bool rightBottomRegion = false;

        for (int x = 10; x < worldData.WorldMap.GetLength(0) - 11; x++){
            for (int y = 10; y < worldData.WorldMap.GetLength(0) - 11; y++){
                if (worldData.WorldMap[x,y].Equals(Region.TileType.Beach)){
                    if (townAccepatble(new Vector2Int(x,y), ref worldData)){
                        if (leftTopRegion && x >= 220 && x <= 440 && y >= 220 && y <= 440)
                        {
                            worldData.Capitols.Add(new Capitol(worldData.generateTownName(), new Vector2Int(x, y)));
                            worldData.WorldMap[x, y] = Region.TileType.Town;
                        }
                        else if (leftBottomRegion && x >= 220 && x <= 440 && y >= 660 && y <= 880)
                        {
                            worldData.Capitols.Add(new Capitol(worldData.generateTownName(), new Vector2Int(x, y)));
                            worldData.WorldMap[x, y] = Region.TileType.Town;
                        }
                        else if (rightTopRegion && x >= 660 && x <= 880 && y >= 220 && y <= 440)
                        {
                            worldData.Capitols.Add(new Capitol(worldData.generateTownName(), new Vector2Int(x, y)));
                            worldData.WorldMap[x, y] = Region.TileType.Town;
                        }
                        else if (rightBottomRegion && x >= 660 && x <= 880 && y >= 660 && y <= 880)
                        {
                            worldData.Capitols.Add(new Capitol(worldData.generateTownName(), new Vector2Int(x, y)));
                            worldData.WorldMap[x, y] = Region.TileType.Town;
                        }
                        else 
                        {
                            worldData.Towns.Add(new Town(worldData.generateTownName(), new Vector2Int(x, y)));
                            worldData.WorldMap[x, y] = Region.TileType.Town;
                            createManagerBasedOnTown(ref worldData, worldData.Towns.Count - 1);
                            worldData.Towns[worldData.Towns.Count - 1].setTournament(
                                createTournamentBasedOnRegion(worldData.Towns[worldData.Towns.Count - 1].RegionLevel, generateDateFromOffset(x + y)));
                        }
                    }
                }
            }
        }
    }

	public static List<WeightClass.WClass> generateBoxersToCreate(float elo){
		List<WeightClass.WClass> weigths = new List<WeightClass.WClass> ();

		for (int i = 0; i < 3; i++) {
			weigths.Add (WeightClass.WClass.FlyWeight);
		}

		if (elo > 300) {
			for (int i = 0; i < 3; i++) {
				weigths.Add (WeightClass.WClass.LightWeight);
			}
		}

		if (elo > 500) {
			for (int i = 0; i < 3; i++) {
				weigths.Add (WeightClass.WClass.WelterWeight);
			}
		}

		if (elo > 900) {
			for (int i = 0; i < 3; i++) {
				weigths.Add (WeightClass.WClass.MiddleWeight);
			}
		}

		if (elo > 1400) {
			for (int i = 0; i < 3; i++) {
				weigths.Add (WeightClass.WClass.CruiserWeight);
			}
		}

		if (elo > 1700) {
			for (int i = 0; i < 3; i++) {
				weigths.Add (WeightClass.WClass.HeavyWeight);
			}
		}

		return weigths;
	}

	public static List<Boxer> generateBoxerRecruits(ref DataPool worldData, int townIndex, float elo){
		List<WeightClass.WClass> boxersToCreate = generateBoxersToCreate (elo);

		List<Boxer> boxers = new List<Boxer> ();

		foreach (WeightClass.WClass wClass in boxersToCreate) {
			boxers.Add (createBoxerBasedOnFame (worldData.generateFirstName(), worldData.generateLastName(), townIndex, elo, wClass));
		}

		return boxers;
	}

    private static CalendarDate generateDateFromOffset(int offset){
        CalendarDate date = new CalendarDate(1, 1, 1602);

        int rng = Random.Range(0, 100);
        int weeks = Random.Range(offset - 9, offset + 9);

        if (rng > 50)
            date.addWeeks(weeks);
        else 
            date.addWeeks(weeks + 24);

        return new CalendarDate(date.Week, date.Month, 1602);
    }

	public static float generateWeightFromClass(WeightClass.WClass wClass){
		if (wClass.Equals (WeightClass.WClass.FlyWeight))
			return (float)Random.Range (100, 118);
		else if (wClass.Equals (WeightClass.WClass.LightWeight))
			return (float)Random.Range (120, 138);
		else if (wClass.Equals (WeightClass.WClass.WelterWeight))
			return (float)Random.Range (140, 158);
		else if (wClass.Equals (WeightClass.WClass.MiddleWeight))
			return (float)Random.Range (160, 173);
		else if (wClass.Equals (WeightClass.WClass.CruiserWeight))
			return (float)Random.Range (175, 198);
		else if (wClass.Equals (WeightClass.WClass.HeavyWeight))
			return (float)Random.Range (200, 225);

		return (float)Random.Range (100, 118);
	}

    private static float getAgeFromLevel(TournamentProtocol.Level level){
        if (level.Equals(TournamentProtocol.Level.S))
            return Random.Range(50.0f, 88.0f);
        else if (level.Equals(TournamentProtocol.Level.A))
            return Random.Range(50.0f, 88.0f);
        else if (level.Equals(TournamentProtocol.Level.B))
            return Random.Range(40.0f, 85.0f);
        else if (level.Equals(TournamentProtocol.Level.C))
            return Random.Range(35.0f, 85.0f);
        else if (level.Equals(TournamentProtocol.Level.D))
            return Random.Range(30.0f, 85.0f);
        else if (level.Equals(TournamentProtocol.Level.E))
            return Random.Range(10.0f, 85.0f);

        return 85.0f;
    }

    private static List<int> getPercentagesForRegion(TournamentProtocol.Level regionLevel){
        if (regionLevel.Equals(TournamentProtocol.Level.E))
        {
            return new List<int>(new int[]{ 98, 95, 90, 80, 60 });
        } 
        else if (regionLevel.Equals(TournamentProtocol.Level.D))
        {
            return new List<int>(new int[] { 96, 90, 80, 65, 20 });
        }
        else if (regionLevel.Equals(TournamentProtocol.Level.C))
        {
            return new List<int>(new int[] { 96, 90, 75, 30, 10 });
        }
        else if (regionLevel.Equals(TournamentProtocol.Level.B))
        {
            return new List<int>(new int[] { 95, 80, 35, 15, 5 });
        }
        else if (regionLevel.Equals(TournamentProtocol.Level.A))
        {
            return new List<int>(new int[] { 85, 40, 20, 10, 4 });
        }
        else if (regionLevel.Equals(TournamentProtocol.Level.S))
        {
            return new List<int>(new int[] { 70, 45, 25, 10, 3 });
        }

        return new List<int>(new int[] { 98, 95, 90, 80, 60 });
    }

    private static float getPrimaryStatFromLevel(TournamentProtocol.Level level){
        if (level.Equals(TournamentProtocol.Level.S))
            return Random.Range(700.0f, 850.0f);
        else if (level.Equals(TournamentProtocol.Level.A))
            return Random.Range(600.0f, 750.0f);
        else if (level.Equals(TournamentProtocol.Level.B))
            return Random.Range(500.0f, 700.0f);
        else if (level.Equals(TournamentProtocol.Level.C))
            return Random.Range(400.0f, 575.0f);
        else if (level.Equals(TournamentProtocol.Level.D))
            return Random.Range(300.0f, 400.0f);
        else if (level.Equals(TournamentProtocol.Level.E))
            return Random.Range(200.0f, 300.0f);

        return 850.0f;
    }

    private static float getSecondaryStatFromLevel(TournamentProtocol.Level level){
        if (level.Equals(TournamentProtocol.Level.S))
            return Random.Range(300.0f, 650.0f);
        else if (level.Equals(TournamentProtocol.Level.A))
            return Random.Range(250.0f, 600.0f);
        else if (level.Equals(TournamentProtocol.Level.B))
            return Random.Range(200.0f, 550.0f);
        else if (level.Equals(TournamentProtocol.Level.C))
            return Random.Range(175.0f, 450.0f);
        else if (level.Equals(TournamentProtocol.Level.D))
            return Random.Range(125.0f, 300.0f);
        else if (level.Equals(TournamentProtocol.Level.E))
            return Random.Range(100.0f, 200.0f);

        return 650.0f;
    }

    private static float getPrizeMoney(TournamentProtocol.Level level){
        if (level.Equals(TournamentProtocol.Level.E))
        {
            return Random.Range(800.0f, 1200.0f);
        } 
        else if (level.Equals(TournamentProtocol.Level.D))
        {
            return Random.Range(1400.0f, 2000.0f);
        } 
        else if (level.Equals(TournamentProtocol.Level.C))
        {
            return Random.Range(2200.0f, 3000.0f);
        }
        else if (level.Equals(TournamentProtocol.Level.B))
        {
            return Random.Range(3300.0f, 4500.0f);
        }
        else if (level.Equals(TournamentProtocol.Level.A))
        {
            return Random.Range(4800.0f, 6500.0f);
        }
        else if (level.Equals(TournamentProtocol.Level.S))
        {
            return Random.Range(7000.0f, 11000.0f);
        }

        return Random.Range(11500.0f, 14000.0f);
    }

    private static int getRandomTournamentSize(int level){
        int rng = Random.Range(0, 100);

        if (rng > 66 + (20 - (level * 4)))
            return 8;
        else if (rng > 33 + (10 - (level * 2)))
            return 6;

        return 4;
    }

	private static void initExercises(ref DataPool worldData){
		string strengthExercise = "Punching Bag";
		string speedExercise = "Sprints";
		string enduranceExercise = "Punch Glove";
		string accuracyExercise = "Double End Bag";
		string healthExercise = "Laps";

		worldData.addExerciseDescription (strengthExercise, "Strength Training");
		worldData.addExerciseDescription (speedExercise, "Speed Training");
		worldData.addExerciseDescription (enduranceExercise, "Endurance Training");
		worldData.addExerciseDescription (accuracyExercise, "Accuracy Training");
		worldData.addExerciseDescription (healthExercise, "Health Training");

		worldData.addExerciseProgress(strengthExercise, new List<int>(new int[]{ 0, 0, 0, 0, 1}));
		worldData.addExerciseProgress(strengthExercise, new List<int>(new int[]{ 0, 0, 0, 0, 1}));
		worldData.addExerciseProgress(strengthExercise, new List<int>(new int[]{ 0, 0, 0, 0, 2}));
		worldData.addExerciseProgress(strengthExercise, new List<int>(new int[]{ 0, 0, 0, 0, 2}));
		worldData.addExerciseProgress(strengthExercise, new List<int>(new int[]{ 0, 0, 0, 0, 3}));
		worldData.addExerciseProgress(strengthExercise, new List<int>(new int[]{ 0, 0, 0, 0, 3}));

		worldData.addExerciseProgress(speedExercise, new List<int>(new int[]{ 0, 0, 0, 1, 0}));
		worldData.addExerciseProgress(speedExercise, new List<int>(new int[]{ 0, 0, 0, 1, 0}));
		worldData.addExerciseProgress(speedExercise, new List<int>(new int[]{ 0, 0, 0, 2, 0}));
		worldData.addExerciseProgress(speedExercise, new List<int>(new int[]{ 0, 0, 0, 2, 0}));
		worldData.addExerciseProgress(speedExercise, new List<int>(new int[]{ 0, 0, 0, 3, 0}));
		worldData.addExerciseProgress(speedExercise, new List<int>(new int[]{ 0, 0, 0, 3, 0}));

		worldData.addExerciseProgress(enduranceExercise, new List<int>(new int[]{ 0, 1, 0, 0, 0}));
		worldData.addExerciseProgress(enduranceExercise, new List<int>(new int[]{ 0, 1, 0, 0, 0}));
		worldData.addExerciseProgress(enduranceExercise, new List<int>(new int[]{ 0, 2, 0, 0, 0}));
		worldData.addExerciseProgress(enduranceExercise, new List<int>(new int[]{ 0, 2, 0, 0, 0}));
		worldData.addExerciseProgress(enduranceExercise, new List<int>(new int[]{ 0, 3, 0, 0, 0}));
		worldData.addExerciseProgress(enduranceExercise, new List<int>(new int[]{ 0, 3, 0, 0, 0}));

		worldData.addExerciseProgress(accuracyExercise, new List<int>(new int[]{ 1, 0, 0, 0, 0}));
		worldData.addExerciseProgress(accuracyExercise, new List<int>(new int[]{ 1, 0, 0, 0, 0}));
		worldData.addExerciseProgress(accuracyExercise, new List<int>(new int[]{ 2, 0, 0, 0, 0}));
		worldData.addExerciseProgress(accuracyExercise, new List<int>(new int[]{ 2, 0, 0, 0, 0}));
		worldData.addExerciseProgress(accuracyExercise, new List<int>(new int[]{ 3, 0, 0, 0, 0}));
		worldData.addExerciseProgress(accuracyExercise, new List<int>(new int[]{ 3, 0, 0, 0, 0}));

		worldData.addExerciseProgress(healthExercise, new List<int>(new int[]{ 0, 0, 1, 0, 0}));
		worldData.addExerciseProgress(healthExercise, new List<int>(new int[]{ 0, 0, 1, 0, 0}));
		worldData.addExerciseProgress(healthExercise, new List<int>(new int[]{ 0, 0, 2, 0, 0}));
		worldData.addExerciseProgress(healthExercise, new List<int>(new int[]{ 0, 0, 2, 0, 0}));
		worldData.addExerciseProgress(healthExercise, new List<int>(new int[]{ 0, 0, 3, 0, 0}));
		worldData.addExerciseProgress(healthExercise, new List<int>(new int[]{ 0, 0, 3, 0, 0}));
	}

    private static bool townAccepatble(Vector2Int p, ref DataPool worldData)
    {
        foreach (Town t in worldData.Towns){
            Vector2Int p1 = p;
            Vector2Int p2 = t.Location;

            float distance = Mathf.Sqrt(Mathf.Pow((float)(p2.x - p1.x), 2.0f) + Mathf.Pow((float)(p2.y - p1.y), 2.0f));

            if (distance < 15)
                return false;
        }

        int blueHit = 0;
        if (worldData.WorldMap[p.x - 1, p.y].Equals(Region.TileType.Shallows))
            blueHit++;
        if (worldData.WorldMap[p.x + 1, p.y].Equals(Region.TileType.Shallows))
            blueHit++;
        if (worldData.WorldMap[p.x, p.y - 1].Equals(Region.TileType.Shallows))
            blueHit++;
        if (worldData.WorldMap[p.x, p.y + 1].Equals(Region.TileType.Shallows))
            blueHit++;

        if (blueHit > 0 && blueHit < 3)
            return true;

        return false;
    }
}
