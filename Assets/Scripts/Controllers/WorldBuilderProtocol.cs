using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public static class WorldBuilderProtocol {

	public static void createWorld(ref DataPool worldData, int width, int height){
        initExercises (ref worldData);
        createRegions(220, 220, ref worldData);
        setRegionLevels(ref worldData);

        Dictionary<TournamentProtocol.Level, int> regionCount = new Dictionary<TournamentProtocol.Level, int>();

        foreach (Region r in worldData.Regions){
            if (!regionCount.ContainsKey(r.Level))
                regionCount.Add(r.Level, 0);

            regionCount[r.Level]++;
        }

        foreach (TournamentProtocol.Level r in regionCount.Keys){
            Debug.Log(r + " - " + regionCount[r]);
        }
	}

    private static void setRegionLevels(ref DataPool worldData)
    {
        foreach (Region r in worldData.Regions){
            List<Vector2Int> path = worldData.Dijkstras.shortestPath(new Vector2Int(0,0), r.Position);

            r.setLevel(path.Count);
        }
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
    
    private static int generateRandomInt(int min, int max)
	{
		return new System.Random((int)DateTime.Now.Ticks).Next(min, max);
	}

    public static Boxer createBoxerBasedOnFame(string firstName, string lastName, int townIndex, float elo, WeightClass.WClass wClass)
    {
		
        int pointsToGive = EvaluationProtocol.getBoxerPointsFromFame(elo);

        int badStatRates = Mathf.RoundToInt((pointsToGive / 3) / 2);
        badStatRates = badStatRates < 1 ? 1 : badStatRates;

        List<BoxerClass.Type> possibleClasses = BoxerClass.getClassesBasedOnWeight(wClass);

        List<EvaluationProtocol.Stats> stats = new List<EvaluationProtocol.Stats>(new EvaluationProtocol.Stats[] {
            EvaluationProtocol.Stats.AccuracyGrowth, EvaluationProtocol.Stats.EnduranceGrowth, EvaluationProtocol.Stats.HealthGrowth,
            EvaluationProtocol.Stats.SpeedGrowth, EvaluationProtocol.Stats.StrengthGrowth
        });

        Dictionary<EvaluationProtocol.Stats, int> growthRates = new Dictionary<EvaluationProtocol.Stats, int>();
        Dictionary<EvaluationProtocol.Stats, int> statValues = new Dictionary<EvaluationProtocol.Stats, int>();

        foreach (EvaluationProtocol.Stats stat in stats)
        {
            growthRates.Add(stat, badStatRates);
        }

		BoxerClass.Type bClass = possibleClasses[generateRandomInt(0, possibleClasses.Count - 1)];

        List<EvaluationProtocol.Stats> bestStats = BoxerClass.getBuild(bClass);
        
        for (int i = 0; i < 3; i++)
        {
            int baseStat = Mathf.RoundToInt(pointsToGive / (3 - i));

            if (pointsToGive > 1)
            {
				baseStat = generateRandomInt(baseStat - 1, baseStat + 1);
                baseStat = baseStat < 1 ? 1 : baseStat;
                baseStat = baseStat > 10 ? 10 : baseStat;
                baseStat = (pointsToGive - baseStat) < 2 ? baseStat - 1 : baseStat;
                baseStat = baseStat < 1 ? 1 : baseStat;
            }
            else
                baseStat = pointsToGive;


            EvaluationProtocol.Stats stat = bestStats[generateRandomInt(0, bestStats.Count - 1)];

            growthRates[stat] = baseStat;
            bestStats.RemoveAt(bestStats.IndexOf(stat));
            pointsToGive -= baseStat;
        }

        int acc = EvaluationProtocol.getStatValueFromGrowthRate(growthRates[EvaluationProtocol.Stats.AccuracyGrowth]);
        int end = EvaluationProtocol.getStatValueFromGrowthRate(growthRates[EvaluationProtocol.Stats.EnduranceGrowth]);
        int hlt = EvaluationProtocol.getStatValueFromGrowthRate(growthRates[EvaluationProtocol.Stats.HealthGrowth]);
        int spd = EvaluationProtocol.getStatValueFromGrowthRate(growthRates[EvaluationProtocol.Stats.SpeedGrowth]);
        int str = EvaluationProtocol.getStatValueFromGrowthRate(growthRates[EvaluationProtocol.Stats.StrengthGrowth]);

        statValues.Add(EvaluationProtocol.Stats.Accuracy, generateRandomInt(acc - 4, acc + 4));
		statValues.Add(EvaluationProtocol.Stats.Endurance, generateRandomInt(end - 4, end + 4));
		statValues.Add(EvaluationProtocol.Stats.Health, generateRandomInt(hlt - 4, hlt + 4));
		statValues.Add(EvaluationProtocol.Stats.Speed, generateRandomInt(spd - 4, spd + 4));
		statValues.Add(EvaluationProtocol.Stats.Strength, generateRandomInt(str - 4, str + 4));

        return new Boxer(
            new Vector2Int(0, 1), firstName, lastName, townIndex, generateWeightFromClass(wClass), bClass,
            statValues[EvaluationProtocol.Stats.Accuracy], growthRates[EvaluationProtocol.Stats.AccuracyGrowth],
            statValues[EvaluationProtocol.Stats.Endurance], growthRates[EvaluationProtocol.Stats.EnduranceGrowth],
            statValues[EvaluationProtocol.Stats.Health], growthRates[EvaluationProtocol.Stats.HealthGrowth],
            statValues[EvaluationProtocol.Stats.Speed], growthRates[EvaluationProtocol.Stats.SpeedGrowth],
            statValues[EvaluationProtocol.Stats.Strength], growthRates[EvaluationProtocol.Stats.StrengthGrowth]);

    }

    public static void createCapitol(ref DataPool worldData, int regionIndex)
    {
        bool capitolCreated = false;
        while (!capitolCreated)
        {
            for (int x = 10; x < worldData.Regions[regionIndex].Map.GetLength(0) - 11; x++)
            {
                for (int y = 10; y < worldData.Regions[regionIndex].Map.GetLength(0) - 11; y++)
                {
                    if (worldData.Regions[regionIndex].Map[x, y].Equals(RegionCreator.TileType.Beach))
                    {
                        if (blueHit(new Vector2Int(x, y), worldData.Regions[regionIndex].Map) && generateRandomInt(0,100) < 60 && !capitolCreated)
                        {
							List<Vector2Int> path = worldData.Dijkstras.shortestPath(new Vector2Int(0, 0), worldData.Regions[regionIndex].Position);
                            int regionDistance = path.Count;
                            worldData.Capitols.Add(new Capitol(worldData.generateTownName(), new Vector2Int(x, y), regionDistance));
                            worldData.Regions[regionIndex].addCapitol(worldData.Capitols.Count - 1);
                            capitolCreated = true;
                        }
                    }
                }
            }
        }
    }

    private static void createManagerBasedOnTown(ref DataPool worldData, int townIndex, int regionIndex)
    {
        for (int j = 0; j < 1; j++)
        {
            List<BoxerClass.Type> typeList = BoxerClass.getTypeList();

            Manager manager = new Manager(
				worldData.generateFirstName(), worldData.generateLastName(), townIndex, generateRandomInt(145, 225), typeList[generateRandomInt(0, typeList.Count - 1)]);
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

			TournamentProtocol.Level boxerLevel = (TournamentProtocol.Level)generateRandomInt(0, (int)worldData.Towns[townIndex].RegionLevel);

            worldData.Boxers.Add(boxers[bIndex]);
            mp.recruitBoxer(worldData.Boxers.Count - 1);
            mp.updateELO(ref worldData);
            mp.upgradeFacilities(ref worldData);
            mp.setRank(boxerLevel);
            ageAndDevelop(ref worldData, worldData.Boxers.Count - 1, boxerLevel);

            worldData.ManagerProtocols.Add(mp);
            worldData.Regions[regionIndex].addManager(worldData.ManagerProtocols.Count - 1);
        }
    }

    public static void createRegion(ref DataPool worldData, int width, int height, Vector2Int position){
        Region origin = new Region("", position);
        RegionCreator.TileType[,] originMap = RegionCreator.CreateRegion(width, height);
        origin.addWorldMap(originMap);
        worldData.Regions.Add(origin);
    }

	public static void createRegion(ref DataPool worldData, Vector2Int position, RegionCreator.TileType[,] map){
		Region region = new Region("", position);
		region.addWorldMap(map);
		worldData.Regions.Add(region);
	}

    public static void createRegions(ref DataPool worldData, List<Region> regions)
	{
		foreach (Region r in regions)
		{
			worldData.Regions.Add(r);
		}

		Debug.Log(worldData.Regions.Count);
		worldData.updateDijkstras();

        for (int i = 0; i < worldData.Regions.Count; i++)
        {
            createCapitol(ref worldData, i);
            createTowns(ref worldData, i);
        }

        foreach (Region region in worldData.Regions)
        {
            Dictionary<TournamentProtocol.Level, bool> qualifierMap = generateQualifierMap(ref worldData, region.Position);

            worldData.Capitols[region.CapitolIndex].setupQualifier(qualifierMap);
        }

        setRegionLevels(ref worldData);
	}

	public static void createRegions(ref DataPool worldData, List<RegionCreator.TileType[,]> regions){
		int rIndex = generateRandomInt(0, regions.Count - 1);
		createRegion(ref worldData, new Vector2Int(0, 0), regions[rIndex]);
		regions.RemoveAt(rIndex);

		List<int> newlyCreatedRegionIndexes = new List<int>();
        newlyCreatedRegionIndexes.Add(0);

        int regionCount = 0;
		int regionsToAdd = regions.Count;

        List<int> temporaryNewIndexes = new List<int>();

		while (regionCount < regionsToAdd)
        {
            temporaryNewIndexes = new List<int>();
            foreach (int index in newlyCreatedRegionIndexes)
            {
                List<Vector2Int> newRegionsToAdd = getAdjacents(ref worldData, worldData.Regions[index].Position);

                if (newRegionsToAdd.Count > 0)
                {
                    foreach (Vector2Int pos in newRegionsToAdd)
                    {
						rIndex = generateRandomInt(0, regions.Count - 1);
						createRegion(ref worldData, pos, regions[rIndex]);
						regions.RemoveAt(rIndex);
                        temporaryNewIndexes.Add(worldData.Regions.Count - 1);
                        regionCount++;
                    }
                }
                else
                {
                    temporaryNewIndexes.Add(index);
                }
            }

            newlyCreatedRegionIndexes = temporaryNewIndexes;
        }

		worldData.updateDijkstras();

        for (int i = 0; i < worldData.Regions.Count; i++)
        {
            createCapitol(ref worldData, i);
            createTowns(ref worldData, i);
        }

        foreach (Region region in worldData.Regions)
        {
            Dictionary<TournamentProtocol.Level, bool> qualifierMap = generateQualifierMap(ref worldData, region.Position);

            worldData.Capitols[region.CapitolIndex].setupQualifier(qualifierMap);
        }

		setRegionLevels(ref worldData);
	}

    public static void createRegions(int width, int height, ref DataPool worldData){
        List<int> landMass = new List<int>();

        createRegion(ref worldData, width, height, new Vector2Int(0, 0));

        List<int> newlyCreatedRegionIndexes = new List<int>();
        newlyCreatedRegionIndexes.Add(0);

        int regionCount = 0;

        List<int> temporaryNewIndexes = new List<int>();

        while (regionCount < 30){
            temporaryNewIndexes = new List<int>();
            foreach (int index in newlyCreatedRegionIndexes){
                List<Vector2Int> newRegionsToAdd = getAdjacents(ref worldData, worldData.Regions[index].Position);

                if (newRegionsToAdd.Count > 0){
                    foreach (Vector2Int pos in newRegionsToAdd)
                    {
                        createRegion(ref worldData, width, height, pos);
                        temporaryNewIndexes.Add(worldData.Regions.Count - 1);
                        regionCount++;
                    }
                } else {
                    temporaryNewIndexes.Add(index);
                }
            }

            newlyCreatedRegionIndexes = temporaryNewIndexes;
        }

        worldData.updateDijkstras();

        for (int i = 0; i < worldData.Regions.Count; i++){
            createCapitol(ref worldData, i);
            createTowns(ref worldData, i);
        }

        foreach (Region region in worldData.Regions){
            Dictionary<TournamentProtocol.Level, bool> qualifierMap = generateQualifierMap(ref worldData, region.Position);

            worldData.Capitols[region.CapitolIndex].setupQualifier(qualifierMap);
        }

        //createTowns(ref worldData);
    }

    public static TournamentProtocol createQuarterlyTournament(TournamentProtocol.Level level, CalendarDate date){
        return new TournamentProtocol("Regional " + level.ToString() + " Qualifier", date, getPrizeMoney(level) + 1000.0f, 16, level, true);
    }

    public static TournamentProtocol createTournamentBasedOnRegion(ref DataPool worldData, TournamentProtocol.Level regionLevel, CalendarDate date){
        List<int> tournamentPercentages = getPercentagesForRegion(regionLevel);

		int rng = generateRandomInt(0, 100);
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

        return new TournamentProtocol(worldData.generateTournamentName() + " Cup", date, getPrizeMoney(tournamentLevel), getRandomTournamentSize((int)tournamentLevel), tournamentLevel, false);
    }

    public static void createTowns(ref DataPool worldData, int regionIndex){
        for (int x = 10; x < worldData.Regions[regionIndex].Map.GetLength(0) - 11; x++)
        {
            for (int y = 10; y < worldData.Regions[regionIndex].Map.GetLength(0) - 11; y++)
            {
                if (worldData.Regions[regionIndex].Map[x, y].Equals(RegionCreator.TileType.Beach))
                {
                    if (townAccepatble(new Vector2Int(x, y), regionIndex, ref worldData))
                    {
						List<Vector2Int> path = worldData.Dijkstras.shortestPath(new Vector2Int(0,0), worldData.Regions[regionIndex].Position);
						int regionDistance = path.Count;
                        worldData.Towns.Add(new Town(worldData.generateTownName(), new Vector2Int(x, y), regionDistance));
                        //worldData.WorldMap[x, y] = Region.TileType.Town;
                        createManagerBasedOnTown(ref worldData, worldData.Towns.Count - 1, regionIndex);
                        worldData.Towns[worldData.Towns.Count - 1].setTournament(
                            createTournamentBasedOnRegion(ref worldData, worldData.Towns[worldData.Towns.Count - 1].RegionLevel, generateDateFromOffset(x + y)));
                        worldData.Regions[regionIndex].addTown(worldData.Towns.Count - 1);
                    }
                }
            }
        }
    }

    public static void defineQualifiers(ref DataPool worldData)
	{
		foreach (Region region in worldData.Regions)
        {
            Dictionary<TournamentProtocol.Level, bool> qualifierMap = generateQualifierMap(ref worldData, region.Position);

            worldData.Capitols[region.CapitolIndex].setupQualifier(qualifierMap);
        }

        setRegionLevels(ref worldData);
	}

    public static bool doesRegionExistAt(ref DataPool worldData, Vector2Int position)
    {
        bool regionExists = false;
        foreach (Region region in worldData.Regions)
        {
            if (region.Position.Equals(position))
                regionExists = true;
        }

        return regionExists;
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

		int rng = generateRandomInt(0, 100);
		int weeks = generateRandomInt(offset - 9, offset + 9);

        if (rng > 50)
            date.addWeeks(weeks);
        else 
            date.addWeeks(weeks + 24);

        return new CalendarDate(date.Week, date.Month, 1602);
    }

    public static Dictionary<TournamentProtocol.Level, bool> generateQualifierMap(ref DataPool worldData, Vector2Int pos)
    {
        Dictionary<TournamentProtocol.Level, bool> qMap = new Dictionary<TournamentProtocol.Level, bool>();

        qMap.Add(TournamentProtocol.Level.E, false);
        qMap.Add(TournamentProtocol.Level.D, false);
        qMap.Add(TournamentProtocol.Level.C, false);
        qMap.Add(TournamentProtocol.Level.B, false);
        qMap.Add(TournamentProtocol.Level.A, false);
        qMap.Add(TournamentProtocol.Level.S, false);

        qMap[TournamentProtocol.Level.E] = addQualifier(ref worldData, pos, TournamentProtocol.Level.E);
        qMap[TournamentProtocol.Level.D] = addQualifier(ref worldData, pos, TournamentProtocol.Level.D);
        qMap[TournamentProtocol.Level.C] = addQualifier(ref worldData, pos, TournamentProtocol.Level.C);
        qMap[TournamentProtocol.Level.B] = addQualifier(ref worldData, pos, TournamentProtocol.Level.B);
        qMap[TournamentProtocol.Level.A] = addQualifier(ref worldData, pos, TournamentProtocol.Level.A);
        if (pos.Equals(new Vector2Int(0, 0)))
            qMap[TournamentProtocol.Level.S] = true;

        return qMap;
    }

    private static bool addQualifier(ref DataPool worldData, Vector2Int pos, TournamentProtocol.Level level)
    {
        List<int> qualifiers = new List<int>();

        for (int i = 0; i < worldData.Regions.Count; i++){
            if (worldData.Capitols[worldData.Regions[i].CapitolIndex].Quarterlies.ContainsKey(level))
                qualifiers.Add(i);
        }

        bool tooClose = false;
        foreach (int index in qualifiers){
            List<Vector2Int> path = worldData.Dijkstras.shortestPath(pos, worldData.Regions[index].Position);

            if (path.Count < getSpacing(level))
                tooClose = true;
        }

        if (qualifiers.Count == 0)
            return true;

        return !tooClose;
    }

    private static int getSpacing(TournamentProtocol.Level level)
    {
        switch (level){
            case TournamentProtocol.Level.E:
                return 2;
            case TournamentProtocol.Level.D:
                return 3;
            case TournamentProtocol.Level.C:
                return generateRandomInt(3,4);
            case TournamentProtocol.Level.B:
                return generateRandomInt(3,4);
            case TournamentProtocol.Level.A:
                return generateRandomInt(4,5);
        }

        return 7;
    }

    public static float generateWeightFromClass(WeightClass.WClass wClass){
		if (wClass.Equals(WeightClass.WClass.FlyWeight))
			return (float)generateRandomInt(100, 118);
		else if (wClass.Equals(WeightClass.WClass.LightWeight))
			return (float)generateRandomInt(120, 138);
		else if (wClass.Equals(WeightClass.WClass.WelterWeight))
			return (float)generateRandomInt(140, 158);
		else if (wClass.Equals(WeightClass.WClass.MiddleWeight))
			return (float)generateRandomInt(160, 173);
		else if (wClass.Equals(WeightClass.WClass.CruiserWeight))
			return (float)generateRandomInt(175, 198);
		else if (wClass.Equals(WeightClass.WClass.HeavyWeight))
			return (float)generateRandomInt(200, 225);

		return (float)generateRandomInt(100, 118);
	}

    public static List<Vector2Int> getAdjacents(ref DataPool worldData, Vector2Int newRegion)
    {
        List<Vector2Int> adjacents = new List<Vector2Int>();

        Vector2Int above = new Vector2Int(newRegion.x, newRegion.y + 1);
        Vector2Int below = new Vector2Int(newRegion.x, newRegion.y - 1);
        Vector2Int left = new Vector2Int(newRegion.x - 1, newRegion.y);
        Vector2Int right = new Vector2Int(newRegion.x + 1, newRegion.y);

        bool aboveExists = false;
        bool belowExists = false;
        bool leftExists = false;
        bool rightExists = false;

        foreach (Region region in worldData.Regions)
        {
            if (region.Position.Equals(above))
                aboveExists = true;

            if (region.Position.Equals(below))
                belowExists = true;

            if (region.Position.Equals(left))
                leftExists = true;

            if (region.Position.Equals(right))
                rightExists = true;
        }

        if (!aboveExists && shouldAddRegion(worldData.Regions.Count))
        {
            adjacents.Add(above);
        }

        if (!belowExists && shouldAddRegion(worldData.Regions.Count))
        {
            adjacents.Add(below);
        }

        if (!leftExists && shouldAddRegion(worldData.Regions.Count))
        {
            adjacents.Add(left);
        }

        if (!rightExists && shouldAddRegion(worldData.Regions.Count))
        {
            adjacents.Add(right);
        }

        return adjacents;
    }

    public static List<Vector2Int> getDirections(Vector2Int newRegion)
    {
        List<Vector2Int> directions = new List<Vector2Int>();

        Vector2Int above = new Vector2Int(newRegion.x, newRegion.y + 1);
        Vector2Int below = new Vector2Int(newRegion.x, newRegion.y - 1);
        Vector2Int left = new Vector2Int(newRegion.x - 1, newRegion.y);
        Vector2Int right = new Vector2Int(newRegion.x + 1, newRegion.y);

        directions.Add(above);
        directions.Add(below);
        directions.Add(left);
        directions.Add(right);

        return directions;
    }

    private static float getAgeFromLevel(TournamentProtocol.Level level){
		if (level.Equals(TournamentProtocol.Level.S))
			return (float)generateRandomInt(50, 88);
        else if (level.Equals(TournamentProtocol.Level.A))
			return (float)generateRandomInt(50, 88);
        else if (level.Equals(TournamentProtocol.Level.B))
			return (float)generateRandomInt(40, 85);
        else if (level.Equals(TournamentProtocol.Level.C))
			return (float)generateRandomInt(35, 85);
        else if (level.Equals(TournamentProtocol.Level.D))
			return (float)generateRandomInt(30, 85);
        else if (level.Equals(TournamentProtocol.Level.E))
			return (float)generateRandomInt(10, 85);
        
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

    private static float getEloFromRegion(TournamentProtocol.Level rank)
    {
        if (rank.Equals(TournamentProtocol.Level.E))
        {
			return (float)generateRandomInt(1, 375);
        }
        else if (rank.Equals(TournamentProtocol.Level.D))
        {
			return (float)generateRandomInt(350, 750);
        }
        else if (rank.Equals(TournamentProtocol.Level.C))
        {
			return (float)generateRandomInt(750, 1125);
        }
        else if (rank.Equals(TournamentProtocol.Level.B))
        {
			return (float)generateRandomInt(1125, 1500);
        }
        else if (rank.Equals(TournamentProtocol.Level.A))
        {
			return (float)generateRandomInt(1500, 1875);
        }
        else if (rank.Equals(TournamentProtocol.Level.S))
        {
			return (float)generateRandomInt(1875, 2250);
        }

		return (float)generateRandomInt(2250, 2450);
    }

    private static float getPrimaryStatFromLevel(TournamentProtocol.Level level){
        if (level.Equals(TournamentProtocol.Level.S))
			return (float)generateRandomInt(700, 850);
        else if (level.Equals(TournamentProtocol.Level.A))
			return (float)generateRandomInt(600, 750);
        else if (level.Equals(TournamentProtocol.Level.B))
			return (float)generateRandomInt(500, 700);
        else if (level.Equals(TournamentProtocol.Level.C))
			return (float)generateRandomInt(400, 575);
        else if (level.Equals(TournamentProtocol.Level.D))
			return (float)generateRandomInt(300, 400);
        else if (level.Equals(TournamentProtocol.Level.E))
			return (float)generateRandomInt(200, 300);

        return 850.0f;
    }

    private static float getSecondaryStatFromLevel(TournamentProtocol.Level level){
        if (level.Equals(TournamentProtocol.Level.S))
			return (float)generateRandomInt(300, 650);
        else if (level.Equals(TournamentProtocol.Level.A))
			return (float)generateRandomInt(250, 600);
        else if (level.Equals(TournamentProtocol.Level.B))
			return (float)generateRandomInt(200, 550);
        else if (level.Equals(TournamentProtocol.Level.C))
			return (float)generateRandomInt(175, 450);
        else if (level.Equals(TournamentProtocol.Level.D))
			return (float)generateRandomInt(125, 300);
        else if (level.Equals(TournamentProtocol.Level.E))
			return (float)generateRandomInt(100, 200);

        return 650.0f;
    }

    private static float getPrizeMoney(TournamentProtocol.Level level){
        if (level.Equals(TournamentProtocol.Level.E))
        {
			return (float)generateRandomInt(800, 1200);
        } 
        else if (level.Equals(TournamentProtocol.Level.D))
        {
			return (float)generateRandomInt(1400, 2000);
        } 
        else if (level.Equals(TournamentProtocol.Level.C))
        {
			return (float)generateRandomInt(2200, 3000);
        }
        else if (level.Equals(TournamentProtocol.Level.B))
        {
			return (float)generateRandomInt(3300, 4500);
        }
        else if (level.Equals(TournamentProtocol.Level.A))
        {
			return (float)generateRandomInt(4800, 6500);
        }
        else if (level.Equals(TournamentProtocol.Level.S))
        {
			return (float)generateRandomInt(7000, 11000);
        }

		return (float)generateRandomInt(11500, 14000);
    }

    private static int getRandomTournamentSize(int level){
		int rng = generateRandomInt(0, 100);

        if (rng > 66 + (20 - (level * 4)))
            return 8;
        else if (rng > 33 + (10 - (level * 2)))
            return 6;

        return 4;
    }

    public static void initExercises(ref DataPool worldData)
    {
        string strengthExercise = "Punching Bag";
        string speedExercise = "Sprints";
        string enduranceExercise = "Punch Glove";
        string accuracyExercise = "Double End Bag";
        string healthExercise = "Laps";

        worldData.addExerciseDescription(strengthExercise, "Strength Training");
        worldData.addExerciseDescription(speedExercise, "Speed Training");
        worldData.addExerciseDescription(enduranceExercise, "Endurance Training");
        worldData.addExerciseDescription(accuracyExercise, "Accuracy Training");
        worldData.addExerciseDescription(healthExercise, "Health Training");

        worldData.addExerciseProgress(strengthExercise, new List<int>(new int[] { 0, 0, 0, 0, 1 }));
        worldData.addExerciseProgress(strengthExercise, new List<int>(new int[] { 0, 0, 0, 0, 1 }));
        worldData.addExerciseProgress(strengthExercise, new List<int>(new int[] { 0, 0, 0, 0, 2 }));
        worldData.addExerciseProgress(strengthExercise, new List<int>(new int[] { 0, 0, 0, 0, 2 }));
        worldData.addExerciseProgress(strengthExercise, new List<int>(new int[] { 0, 0, 0, 0, 3 }));
        worldData.addExerciseProgress(strengthExercise, new List<int>(new int[] { 0, 0, 0, 0, 3 }));

        worldData.addExerciseProgress(speedExercise, new List<int>(new int[] { 0, 0, 0, 1, 0 }));
        worldData.addExerciseProgress(speedExercise, new List<int>(new int[] { 0, 0, 0, 1, 0 }));
        worldData.addExerciseProgress(speedExercise, new List<int>(new int[] { 0, 0, 0, 2, 0 }));
        worldData.addExerciseProgress(speedExercise, new List<int>(new int[] { 0, 0, 0, 2, 0 }));
        worldData.addExerciseProgress(speedExercise, new List<int>(new int[] { 0, 0, 0, 3, 0 }));
        worldData.addExerciseProgress(speedExercise, new List<int>(new int[] { 0, 0, 0, 3, 0 }));

        worldData.addExerciseProgress(enduranceExercise, new List<int>(new int[] { 0, 1, 0, 0, 0 }));
        worldData.addExerciseProgress(enduranceExercise, new List<int>(new int[] { 0, 1, 0, 0, 0 }));
        worldData.addExerciseProgress(enduranceExercise, new List<int>(new int[] { 0, 2, 0, 0, 0 }));
        worldData.addExerciseProgress(enduranceExercise, new List<int>(new int[] { 0, 2, 0, 0, 0 }));
        worldData.addExerciseProgress(enduranceExercise, new List<int>(new int[] { 0, 3, 0, 0, 0 }));
        worldData.addExerciseProgress(enduranceExercise, new List<int>(new int[] { 0, 3, 0, 0, 0 }));

        worldData.addExerciseProgress(accuracyExercise, new List<int>(new int[] { 1, 0, 0, 0, 0 }));
        worldData.addExerciseProgress(accuracyExercise, new List<int>(new int[] { 1, 0, 0, 0, 0 }));
        worldData.addExerciseProgress(accuracyExercise, new List<int>(new int[] { 2, 0, 0, 0, 0 }));
        worldData.addExerciseProgress(accuracyExercise, new List<int>(new int[] { 2, 0, 0, 0, 0 }));
        worldData.addExerciseProgress(accuracyExercise, new List<int>(new int[] { 3, 0, 0, 0, 0 }));
        worldData.addExerciseProgress(accuracyExercise, new List<int>(new int[] { 3, 0, 0, 0, 0 }));

        worldData.addExerciseProgress(healthExercise, new List<int>(new int[] { 0, 0, 1, 0, 0 }));
        worldData.addExerciseProgress(healthExercise, new List<int>(new int[] { 0, 0, 1, 0, 0 }));
        worldData.addExerciseProgress(healthExercise, new List<int>(new int[] { 0, 0, 2, 0, 0 }));
        worldData.addExerciseProgress(healthExercise, new List<int>(new int[] { 0, 0, 2, 0, 0 }));
        worldData.addExerciseProgress(healthExercise, new List<int>(new int[] { 0, 0, 3, 0, 0 }));
        worldData.addExerciseProgress(healthExercise, new List<int>(new int[] { 0, 0, 3, 0, 0 }));
    }

    public static bool shouldAddRegion(int numRegions)
    {
        int baseChance = 70;
        baseChance -= (numRegions * 2);

		return generateRandomInt(0, 100) < baseChance;
    }

    private static bool townAccepatble(Vector2Int p, int regionIndex, ref DataPool worldData)
    {
        Vector2Int cPos = worldData.Capitols[worldData.Regions[regionIndex].CapitolIndex].Location;
        if (Mathf.Sqrt(Mathf.Pow((float)(cPos.x - p.x), 2.0f) + Mathf.Pow((float)(cPos.y - p.y), 2.0f)) < 15)
            return false;

        foreach (int index in worldData.Regions[regionIndex].getRegionsTownIndexes()){
            Vector2Int p1 = p;
            Vector2Int p2 = worldData.Towns[index].Location;

            float distance = Mathf.Sqrt(Mathf.Pow((float)(p2.x - p1.x), 2.0f) + Mathf.Pow((float)(p2.y - p1.y), 2.0f));

            if (distance < 23)
                return false;
        }

        return blueHit(p, worldData.Regions[regionIndex].Map);
    }

    private static bool blueHit(Vector2Int p, RegionCreator.TileType[,] map)
    {
        int waterHit = 0;
        if (map[p.x - 1, p.y].Equals(RegionCreator.TileType.Shallows))
            waterHit++;
        if (map[p.x + 1, p.y].Equals(RegionCreator.TileType.Shallows))
            waterHit++;
        if (map[p.x, p.y - 1].Equals(RegionCreator.TileType.Shallows))
            waterHit++;
        if (map[p.x, p.y + 1].Equals(RegionCreator.TileType.Shallows))
            waterHit++;

        if (waterHit > 0 && waterHit < 3)
            return true;

        return false;
    }
}
