using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class TournamentScheduler {

	private const int BYE = -1;

	public static Dictionary<int, List<Vector2Int>> generateTournamentSchedule(int contestants){
		Dictionary<int, List<Vector2Int>> tournament = new Dictionary<int, List<Vector2Int>> ();

		int[,] tournamentSched = generateTournament (contestants);

		for (int round = 0; round < contestants - 1; round++) {
			tournament.Add (round, new List<Vector2Int> ());

			for (int boxer = 0; boxer < contestants; boxer++) {
				Vector2Int match = new Vector2Int (boxer, tournamentSched [boxer, round]);

				bool alreadyAdded = false;
				foreach (Vector2Int fight in tournament[round]) {
					if (fight.x == boxer || fight.y == boxer)
						alreadyAdded = true;
				}

				if (!alreadyAdded)
					tournament [round].Add (match);
			}
		}

//		for (int i = 0; i < contestants - 1; i++) {
//			Debug.Log ("Round " + (i + 1));
//			foreach (Vector2Int fight in tournament[i]) {
//				Debug.Log (fight.x + " vs. " + fight.y);
//			}
//		}

		return tournament;
	}

	public static int[,] generateTournament(int contestants){
		int[,] tournament;

		if (contestants % 2 == 0)
			tournament = generateRoundRobinEven(contestants);
		else
			tournament = generateRoundRobinOdd(contestants);

		return tournament;
	}

	private static int[,] generateRoundRobinEven(int contestants)
	{
		int[,] results = generateRoundRobinOdd(contestants - 1);

		int[,] results2 = new int[contestants, contestants - 1];
		for (int boxer = 0; boxer < contestants - 1; boxer++)
		{
			for (int round = 0; round < contestants - 1; round++)
			{
				if (results[boxer, round] == BYE)
				{
					results2[boxer, round] = contestants - 1;
					results2[contestants - 1, round] = boxer;
				}
				else
				{
					results2[boxer, round] = results[boxer, round];
				}
			}
		}

		return results2;
	}

	private static int [,] generateRoundRobinOdd(int num_teams)
	{
		int n2 = (int)((num_teams - 1) / 2);
		int[,] results = new int[num_teams, num_teams];

		int[] teams = new int[num_teams];
		for (int i = 0; i < num_teams; i++) teams[i] = i;

		for (int round = 0; round < num_teams; round++)
		{
			for (int i = 0; i < n2; i++)
			{
				int team1 = teams[n2 - i];
				int team2 = teams[n2 + i + 1];
				results[team1, round] = team2;
				results[team2, round] = team1;
			}

			results[teams[0], round] = BYE;

			rotateArray(teams);
		}

		return results;
	}

	private static void rotateArray(int[] teams)
	{
		int tmp = teams[teams.Length - 1];
		Array.Copy(teams, 0, teams, 1, teams.Length - 1);
		teams[0] = tmp;
	}
}
