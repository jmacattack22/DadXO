using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Record
{
	private int wins;
	private int losses;
	private int ties;

	private float elo;

	private float k;
	
	public Record (float k)
	{
		wins = 0;
		losses = 0;
		ties = 0;

		this.k = k;
	}

    public Record (JSONObject json)
	{
		wins = (int)json.GetField("wins").i;
		losses = (int)json.GetField("losses").i;
		ties = (int)json.GetField("ties").i;

		elo = json.GetField("elo").f;
		k = json.GetField("k").f;
	}

	public void addWin(float oppElo){
		wins++;

		calculateElo (oppElo, 1.0f);
	}

    public void addWinBulk(int numWins){
        wins += numWins;
    }

	public void addLoss(float oppElo){
		losses++;

		calculateElo (oppElo, 0.0f);
	}

    public void addLossBulk(int numLosses){
        losses += numLosses;
    }

	public void addTie(float oppElo){
		ties++;

		calculateElo (oppElo, 0.5f);
	}

    public void addTieBulk(int numTies){
        ties += numTies;
    }

	public void calculateElo(float oppElo, float outcome){
		float r1 = Mathf.Pow (10, (elo / 400));
		float r2 = Mathf.Pow (10, (oppElo / 400));

		float e1 = (r1 / (r1 + r2));

		elo = elo + (k * (outcome - e1)) + staticChange (outcome);
		elo = elo < 25.0f ? 25.0f : elo;
	}

	public float getWinPercentage(){
		return (float)wins / (wins + losses + ties);
	}

	public void setELO(float elo){
		this.elo = elo;
	}

	public float staticChange(float outcome){
		if (outcome == 1.0f)
			return k / 2.0f;
		else if (outcome == 0.0f)
			return -(k / 10.0f);

		return 0.0f; 
	}

    public JSONObject jsonify()
	{
		JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

		json.AddField("wins", wins);
		json.AddField("losses", losses);
		json.AddField("ties", ties);
		json.AddField("elo", elo);
		json.AddField("k", k);

		return json;
	}

	//Getters
	public float ELO {
		get { return elo; }
	}

	public int Wins {
		get { return wins; }
	}

	public int Losses {
		get { return losses; }
	}

	public int Ties {
		get { return ties; }
	}
}
	