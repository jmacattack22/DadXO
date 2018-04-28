using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalendarDate{

	private int week;
	private int month;
	private int year;

	public CalendarDate(int w, int m, int y){
		week = w;
		month = m;
		year = y;
	}

	public void addWeeks(int numWeeks){
		for (int i = 0; i < numWeeks; i++) {
			week++;

			if (week > 4){
				week = 1;
				month++;

				if (month > 12) {
					month = 1;
					year++;
				}
			}
		}
	}

    public void advanceQuarter(){
        addWeeks(12);
    }

    public void advanceYear(){
        year++;
    }

	public void logDate(){
		Debug.Log (week + "," + month + "," + year);
	}

    public bool sameWeek(CalendarDate date){
        if (date.Week == week && date.Month == month && date.Year == year)
            return true;

        return false;
    }

	public override string ToString()
	{
        return week + "/" + month + "/" + year;
	}

	//Getters
	public int Week {
		get { return week; }
	}

	public int Month {
		get { return month; }
	}

	public int Year {
		get { return year; }
	}
}
