using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinanceProtocol {

	private int balance;

	public FinanceProtocol(int initiallBalance){
		balance = initiallBalance;
	}

    public FinanceProtocol(JSONObject json)
	{
		balance = (int)json.GetField("balance").i;
	}

	public void addFunds(int funds){
		balance += funds;
	}

	public void deductFunds(int funds){
		balance -= funds;
	}

    public JSONObject jsonify()
	{
		JSONObject json = new JSONObject(JSONObject.Type.OBJECT);

		json.AddField("balance", balance);

		return json;
	}

	//Getters
	public int Balance {
		get { return balance; }
	}
}
