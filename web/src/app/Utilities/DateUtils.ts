import { Injectable } from '@angular/core';

/**
 * Utility class for dates.
 */
 @Injectable({
    providedIn: 'root'
  })
export class DateUtils 
{
    /**
     * Initializes a new instance of the DateUtils class.
     */
    constructor() 
    {

    }

    /**
   * Calculates the amount of time between now and the specified target date as a 'days, hrs, min' string in the 
   * following format:
   *  - <= 0 hours: 'x min'
   *  - > 0 hours && < 1 day: 'x hrs, y min'
   *  - 1 day: '1 day, x hrs'
   *  - > 1 day: 'x days, y hrs'
   * @param targetDate The target date.
   * @returns The amount of time between now and the specified date as a string.
   */
   getExpiryTime(targetDate: Date)
   {
       let now = new Date().getTime();
       let end = targetDate.getTime();
       let millisecondsLeft = end - now;
 
       // 1- Convert to seconds:
       let seconds = millisecondsLeft / 1000;
 
       // 2 - Days:
       let days = parseInt((seconds / 86400).toString()) // 86,400 seconds in 1 day
       seconds = seconds % 86400;
 
       // 2- Extract hours:
       let hours = parseInt( (seconds / 3600).toString() ); // 3,600 seconds in 1 hour
       seconds = seconds % 3600; // seconds remaining after extracting hours
       // 3- Extract minutes:
       let minutes = parseInt( (seconds / 60).toString() ); // 60 seconds in 1 minute
       // 4- Keep only seconds not extracted to minutes:
       seconds = seconds % 60;
 
       if (hours == 0) { return minutes + " min"; }
       if (days == 0) { return hours + " hrs, " + minutes + " min"; }
       if (days == 1) { return days + " day, " + hours + " hrs"; }
       return days + " days, " + hours + " hrs";
   }
}