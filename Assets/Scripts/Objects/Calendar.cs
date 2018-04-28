using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Calendar{

	public enum DateType
	{
		fullLong, fullShort, weekAndMonth, weekFull, monthFull, weekShort, monthShort, year
	}

	private int month;
	private int week;
	private int year;

	private List<string> monthsInYear;

	public Calendar(){
		month = 1;
		week = 1;
		year = 1602;

		monthsInYear = new List<string> (new string[]{
			"Aries", 
			"Taurus", 
			"Gemini", 
			"Cancer", 
			"Leo", 
			"Virgo", 
			"Libra",
			"Scorpio",
			"Sagittarius",
			"Capricorn",
			"Aquarius",
			"Pisces"
		});
	}

	public string convertWeekToString(int week){
		if (week == 1)
			return "1st";
		else if (week == 2)
			return "2nd";
		else if (week == 3)
			return "3rd";
		else if (week == 4)
			return "4th";

		return "";
	}
		
	public string getDate(DateType type){
		if (type.Equals (DateType.fullLong))
			return convertWeekToString (week) + " week of " + monthsInYear [month - 1] + ", " + year;
		else if (type.Equals (DateType.fullShort))
			return week + "/" + month + "/" + year;
		else if (type.Equals (DateType.weekAndMonth))
			return convertWeekToString (week) + " week of " + monthsInYear [month - 1];
		else if (type.Equals (DateType.weekFull))
			return convertWeekToString (week);
		else if (type.Equals (DateType.weekShort))
			return week.ToString ();
		else if (type.Equals (DateType.monthFull))
			return monthsInYear [month - 1];
		else if (type.Equals (DateType.monthShort))
			return month.ToString ();
		else if (type.Equals (DateType.year))
			return year.ToString ();

		return "";
	}

    public CalendarDate GetCalendarDate(){
        return new CalendarDate(week, month, year);
    }

	public int getWeekOfYear(){
		return (((month - 1) * 4) + week);
	}

	public void progessWeek(){
		week++;

		if (week > 4) {
			week = 1;
			month++;

			if (month > 12) {
				month = 1;
				year++;
			}
		}
	}

	public int weeksAway(CalendarDate date){
		CalendarDate now = new CalendarDate (week, month, year);

//		if (date.Year < year)
//			return -1;
//		else {
//			if (date.Month < month)
//				return -1;
//			else {
//				if (date.Week < week)
//					return -1;
//			}
//		}

		int weeksAway = 0;
		while (true) {
			if (now.Year == date.Year && now.Month == date.Month && now.Week == date.Week)
				return weeksAway;
			
			now.addWeeks (1);
			weeksAway++;
		}
	}

	//Getters

	public int Month {
		get { return month; }
	}

	public List<string> Months {
		get { return monthsInYear; }
	}

	public int Week {
		get { return week; }
	}

	public int Year {
		get { return year; }
	}
}
	