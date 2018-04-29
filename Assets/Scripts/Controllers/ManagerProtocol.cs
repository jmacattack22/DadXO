using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerProtocol
{

    public enum FacilityShortcut
    {
        DoubleEndBag, PunchGlove, Laps, Sprints, PunchingBag
    }

    private int boxerIndex;
    private int managerIndex;

    private bool atSea;
    private bool atTournament;

    private FinanceProtocol finance;

    private Homebase homebase;
    private Ship ship;

    private List<FacilityShortcut> trainingRegime;

    private float currentBoxerELO;
    private float currentManagerELO;
    private List<float> archivedBoxerELO;
    private List<float> archivedManagerELO;

    //TODO Match Class
    public List<int> previousOpponents;

    private int tournamentPriority;

    private TournamentProtocol.Level currentRanking;

    public ManagerProtocol(ref DataPool worldData, int mIndex)
    {
        managerIndex = mIndex;

        finance = new FinanceProtocol(5000);

        homebase = new Homebase(ref worldData);
        ship = new Ship(ref worldData);
        trainingRegime = new List<FacilityShortcut>();

        setupTrainingRegime(ref worldData);

        atSea = false;
        atTournament = false;

        archivedBoxerELO = new List<float>();
        archivedManagerELO = new List<float>();

        previousOpponents = new List<int>();

        tournamentPriority = 0;
    }

    public void attendTournament()
    {
        atTournament = true;

        tournamentPriority = tournamentPriority - 2 < 0 ? 0 : tournamentPriority - 2;
    }

    public void backOutOfTournament(){
        tournamentPriority += 2;
        atTournament = false;
    }

    public void bumpTournamentPriority()
    {
        tournamentPriority += 1;
    }

    public void completeTournament(ref DataPool worldData, TournamentResult result)
    {
        atTournament = false;

        worldData.Managers[managerIndex].Record.addWinBulk(result.Record.Wins);
        worldData.Managers[managerIndex].Record.addLossBulk(result.Record.Losses);
        worldData.Managers[managerIndex].Record.addTieBulk(result.Record.Ties);

        worldData.Boxers[BoxerIndex].Record.addWinBulk(result.Record.Wins);
        worldData.Boxers[BoxerIndex].Record.addLossBulk(result.Record.Losses);
        worldData.Boxers[BoxerIndex].Record.addTieBulk(result.Record.Ties);

        if (result.QuarterlyWin){
            currentRanking = (TournamentProtocol.Level)(((int)currentRanking) + 1);
        }
    }

    public bool isBusy()
    {
        return atTournament;
    }

    private FacilityShortcut chooseTraining()
    {
        return trainingRegime[Random.Range(0, trainingRegime.Count)];
    }

    public void executeWeek(ref DataPool worldData)
    {
        if (!atTournament){
            if (worldData.Boxers[boxerIndex].isBoxerFatigued())
            {
                worldData.Boxers[boxerIndex].rest();
            }
            else
            {
                train(ref worldData);
            }
        }

        worldData.Boxers[boxerIndex].ageWeek();

        if (worldData.Boxers[boxerIndex].WeeksRemaining == 0)
        {
            disposeAndRenewBoxer(ref worldData);
            currentRanking = TournamentProtocol.Level.E;
        }
    }

    public bool fightRecently(int oppIndex)
    {
        return previousOpponents.Contains(oppIndex);
    }

    private void disposeAndRenewBoxer(ref DataPool worldData)
    {
        List<Boxer> boxers = WorldBuilderProtocol.generateBoxerRecruits(ref worldData, worldData.Managers[managerIndex].TownIndex, currentManagerELO);

        int bIndex = 0;
        float max = 0.0f;

        for (int i = 0; i < boxers.Count; i++)
        {
            float boxerEval = EvaluationProtocol.evaluateBoxer(boxers[i], worldData.Managers[managerIndex].Preference);

            if (boxerEval > max)
            {
                max = boxerEval;
                bIndex = i;
            }
        }

        worldData.Boxers.Add(boxers[bIndex]);
        recruitBoxer(worldData.Boxers.Count - 1);
        updateELO(ref worldData);
    }

    public float getBoxerELOHistory()
    {
        return archivedBoxerELO[0];
    }

    public float getManagerELOHistory()
    {
        return archivedManagerELO[0];
    }

    public string getManagerStats(ref DataPool worldData){
        return currentManagerELO + " - " + currentRanking + " - " + tournamentPriority + " - " + worldData.Managers[managerIndex].getDetails();
    }

    public void logManagerStats(ref DataPool worldData){
        Debug.Log(currentManagerELO + " - " + currentRanking + " - " + tournamentPriority + " - " + worldData.Managers[managerIndex].getDetails());
    }

    public void setRank(TournamentProtocol.Level rank)
    {
        currentRanking = rank;
    }

    private void train(ref DataPool worldData)
    {
        FacilityShortcut training = chooseTraining();

        if (atSea)
            ship.train(ref worldData, boxerIndex, training);
        else
            homebase.train(ref worldData, boxerIndex, training);
    }

    public void recruitBoxer(int bIndex)
    {
        boxerIndex = bIndex;
    }

    private void setupTrainingRegime(ref DataPool worldData)
    {
        BoxerClass.Type preference = worldData.Managers[managerIndex].Preference;

        trainingRegime.Add(FacilityShortcut.DoubleEndBag);
        trainingRegime.Add(FacilityShortcut.Laps);
        trainingRegime.Add(FacilityShortcut.PunchGlove);
        trainingRegime.Add(FacilityShortcut.PunchingBag);
        trainingRegime.Add(FacilityShortcut.Sprints);

        List<EvaluationProtocol.Stats> bestStats = BoxerClass.getBuild(preference);

        foreach (EvaluationProtocol.Stats stat in bestStats)
        {
            FacilityShortcut shortcut = FacilityShortcut.DoubleEndBag;

            if (stat.Equals(EvaluationProtocol.Stats.AccuracyGrowth))
                shortcut = FacilityShortcut.DoubleEndBag;
            else if (stat.Equals(EvaluationProtocol.Stats.EnduranceGrowth))
                shortcut = FacilityShortcut.PunchGlove;
            else if (stat.Equals(EvaluationProtocol.Stats.HealthGrowth))
                shortcut = FacilityShortcut.Laps;
            else if (stat.Equals(EvaluationProtocol.Stats.SpeedGrowth))
                shortcut = FacilityShortcut.Sprints;
            else if (stat.Equals(EvaluationProtocol.Stats.StrengthGrowth))
                shortcut = FacilityShortcut.PunchingBag;

            for (int i = 0; i < 2; i++)
            {
                trainingRegime.Add(shortcut);
            }
        }
    }

    public void updateELO(ref DataPool worldData)
    {
        if (currentBoxerELO > 0)
            archivedBoxerELO.Add(currentBoxerELO);

        if (currentManagerELO > 0)
            archivedManagerELO.Add(currentManagerELO);

        currentManagerELO = worldData.Managers[managerIndex].Record.ELO;
        currentBoxerELO = worldData.Boxers[boxerIndex].Record.ELO;
    }

    public void upgradeFacilities(ref DataPool worldData)
    {
        homebase.upgradeFacilities(ref worldData, currentManagerELO, worldData.Managers[managerIndex].Preference);
    }

    //Getters
    public int BoxerIndex
    {
        get { return boxerIndex; }
    }

    public int ManagerIndex
    {
        get { return managerIndex; }
    }

    public FinanceProtocol Finance
    {
        get { return finance; }
    }

    public float CurrentBoxerELO
    {
        get { return currentBoxerELO; }
    }

    public float CurrentManagerELO
    {
        get { return currentManagerELO; }
    }

    public TournamentProtocol.Level Rank
    {
        get { return currentRanking; }
    }

    public int TournamentPriority {
        get { return tournamentPriority; }
    }
}
