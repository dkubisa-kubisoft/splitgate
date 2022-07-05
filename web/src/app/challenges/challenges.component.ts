import { AfterViewInit, Component, OnInit, ViewChildren, QueryList, ElementRef } from '@angular/core';
import { Challenge } from '../challenge';
import { ChallengeService } from '../challenge.service';
import { DateUtils } from '../Utilities/DateUtils';
import { NumberUtils } from '../Utilities/NumberUtils';

/**
 * Defines the behavior and data bindings for the Challenges component.
 * 
 * @implements OnInit
 * @implements AfterViewInit
 */
@Component({
  selector: 'app-challenges',
  templateUrl: './challenges.component.html',
  styleUrls: ['./challenges.component.scss']
})
export class ChallengesComponent implements OnInit, AfterViewInit 
{
  
  /**
   * Reference to all accordions on the page
   */
  @ViewChildren("accordion", {read: ElementRef}) accordions: QueryList<ElementRef>;

  dailyChallenges: Challenge[] = []; //The collection of all current daily challenges
  dailyExpiryMsg: string = "Resets in ? hrs, ? min"; //The "resets in" message for the daily challenges
  dailyLastUpdatedMessage: string = ""; //The "Last updated" message for the daily challenges
  allDailysCompleted: boolean = false; //Flag indicating whether all daily challenges have been completed

  weeklyChallenges: Challenge[] = []; //The collection of all current weekly challenges
  weeklyExpiryMsg: string = "Resets in ? hrs, ? min"; //The "resets in" message for the weekly challenges
  weeklyLastUpdatedMessage: string = ""; //The "Last updated" message for the weekly challenges
  allWeeklysCompleted: boolean = false; //Flag indicating whether all weekly challenges have been completed

  seasonalChallenges: Challenge[] = []; //The collection of all current seasonal challenges
  seasonalExpiryMsg: string = "Resets in ? hrs, ? min"; //The "resets in" message for the seasonal challenges
  seasonalLastUpdatedMessage: string = ""; //The "Last updated" message for the seasonal challenges
  allSeasonalsCompleted: boolean = false; //Flag indicating whether all seasonal challenges have been completed

  /**
   * Initializes a new instance of the ChallengesComponent class.
   * @param challengeService The challenge service.
   * @param numberUtils The number utilities helper.
   * @param dateUtils The date utilities helper.
   */
  constructor(private challengeService: ChallengeService, private numberUtils: NumberUtils, private dateUtils: DateUtils) 
  { 
    this.accordions = new QueryList<ElementRef>();
    this.dailyChallenges = [];
  }

  /**
   * Handles the OnInit Angular event.
   */
  ngOnInit(): void 
  {
    this.getCurrentChallenges();
    
    setInterval(() => 
    {
      this.dailyExpiryMsg = this.getExpiryMessage(this.dailyChallenges);
      this.weeklyExpiryMsg = this.getExpiryMessage(this.weeklyChallenges);
      this.seasonalExpiryMsg = this.getExpiryMessage(this.seasonalChallenges);
    }, 10000);
  }

  /**
   * Handles the AfterViewInit Angular event.
   */
  ngAfterViewInit(): void 
  {  
    this.accordions.forEach(accordion => 
      {
        let element = accordion.nativeElement;
        let panel = element.nextElementSibling;

        element.addEventListener("click", 
          function() 
          { 
            element.classList.toggle("active");
            panel.classList.toggle("closed");
          });
      });
  }

  /**
   * Retrieves the current challenges from the challenge service and binds all challenge properties
   */
  getCurrentChallenges(): void
  {
    this.challengeService.getCurrentChallenges()
      .subscribe(response => 
        {
          this.dailyChallenges = response.dailyChallenges;
          this.dailyLastUpdatedMessage = this.getRefreshedMessage(new Date(response.dailyChallengeRefreshTimestamp));

          this.weeklyChallenges = response.weeklyChallenges;
          this.weeklyLastUpdatedMessage = this.getRefreshedMessage(new Date(response.weeklyChallengeRefreshTimestamp));

          this.seasonalChallenges = response.seasonalChallenges;
          this.seasonalLastUpdatedMessage = this.getRefreshedMessage(new Date(response.seasonalChallengeRefreshTimestamp));

          this.updateAllGroupCompletions();

          this.dailyExpiryMsg = this.dailyChallenges != null && this.dailyChallenges.length > 0 ? "Resets in " + this.dateUtils.getExpiryTime(new Date(this.dailyChallenges[0].endDateUtc)) : "No challenges found";
          this.weeklyExpiryMsg = this.weeklyChallenges != null && this.weeklyChallenges.length > 0 ? "Resets in " + this.dateUtils.getExpiryTime(new Date(this.weeklyChallenges[0].endDateUtc)) : "No challenges found";
          this.seasonalExpiryMsg = this.seasonalChallenges != null && this.seasonalChallenges.length > 0 ? "Resets in " + this.dateUtils.getExpiryTime(new Date(this.seasonalChallenges[0].endDateUtc)) : "No challenges found";
        });
  }

  /**
   * Toggles the completion state of a specified challenge.
   * @param challenge The challenge to toggle.
   */
   toggleChallengeCompletion(challenge: Challenge): void
   {
     this.challengeService.toggleChallengeCompletion(challenge.challengeType + "," + challenge.index).subscribe(response => {});
     challenge.completed = !challenge.completed;
     this.updateAllGroupCompletions();
   }
  
  /**
   * Checks all daily, weekly and seasonal challenges to see if they have all been marked
   * as completed, and re-binds the group completion indicators to reflect the current
   * group completion status.
   */
  updateAllGroupCompletions() 
  {
    this.allDailysCompleted = this.dailyChallenges.every(c => c.completed === true);
    this.allWeeklysCompleted = this.weeklyChallenges.every(c => c.completed === true);
    this.allSeasonalsCompleted = this.seasonalChallenges.every(c => c.completed === true);
  }

  /**
   * Generates the challenge expiration message for the specified collection of challenges.
   * @param challenges The collection of challenges to check.
   * @returns The expiration date of the specified challenges.
   */
  getExpiryMessage(challenges: Challenge[]) : string 
  {
    return challenges != null && challenges.length > 0 ? "Resets in " + this.dateUtils.getExpiryTime(new Date(challenges[0].endDateUtc)) : "No challenges found";
  }

  /**
   * Generates the "last updated" message for the specified refresh date.
   * @param refreshDate The refresh date.
   * @returns The message.
   */
  getRefreshedMessage(refreshDate: Date) : string 
  {
    let amPm = "AM"
    let hours = refreshDate.getHours();

    if (hours >= 12) 
    {
      amPm = "PM";
    }

    if (hours > 12) 
    {
      hours -= 12;
    }

    return "Last updated " + (refreshDate.getMonth() + 1) + '/' + refreshDate.getDate() + '/' + refreshDate.getFullYear() + " " + hours + ":" + this.numberUtils.zeroPad(refreshDate.getMinutes(), 2) + " " + amPm;
  }
}