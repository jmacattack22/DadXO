using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinanceProtocol {

	private int balance;

	public FinanceProtocol(int initiallBalance){
		balance = initiallBalance;
	}

	public void addFunds(int funds){
		balance += funds;
	}

	public void deductFunds(int funds){
		balance -= funds;
	}

	//Getters
	public int Balance {
		get { return balance; }
	}
}
