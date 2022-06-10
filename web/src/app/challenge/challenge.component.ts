import { AfterViewInit, Component, OnInit, ViewChildren, QueryList, ElementRef } from '@angular/core';
import { Challenge } from '../challenge';
import { ChallengeService } from '../challenge.service';


@Component({
  selector: 'app-challenge',
  templateUrl: './challenge.component.html',
  styleUrls: ['./challenge.component.scss']
})
export class ChallengesComponent implements OnInit, AfterViewInit {
  @ViewChildren("accordion", {read: ElementRef}) accordions: QueryList<ElementRef>;

  dailyExpiryMsg: string = "Resets in ? hrs, ? min";
  dailyChallenges: Challenge[] = [];
  allDailysCompleted: boolean = false;
  dailyRefreshMessage: string = "";

  weeklyExpiryMsg: string = "Resets in ? hrs, ? min";
  weeklyChallenges: Challenge[] = [];
  allWeeklysCompleted: boolean = false;
  weeklyRefreshMessage: string = "";

  seasonalExpiryMsg: string = "Resets in ? hrs, ? min";
  seasonalChallenges: Challenge[] = [];
  allSeasonalsCompleted: boolean = false;
  seasonalRefreshMessage: string = "";

  constructor(private challengeService: ChallengeService) 
  { 
    this.accordions = new QueryList<ElementRef>();
  }

  getCurrentChallenges(): void {
    this.challengeService.getCurrentChallenges()
      .subscribe(response => 
        {
          this.dailyChallenges = response.dailyChallenges;
          this.dailyRefreshMessage = this.getRefreshedMessage(new Date(response.dailyChallengeRefreshTimestamp));

          this.weeklyChallenges = response.weeklyChallenges;
          this.weeklyRefreshMessage = this.getRefreshedMessage(new Date(response.weeklyChallengeRefreshTimestamp));

          this.seasonalChallenges = response.seasonalChallenges;
          this.seasonalRefreshMessage = this.getRefreshedMessage(new Date(response.seasonalChallengeRefreshTimestamp));

          if(this.dailyChallenges.every(c => c.completed === true)) { this.allDailysCompleted = true; }
          if(this.weeklyChallenges.every(c => c.completed === true)) { this.allWeeklysCompleted = true; }
          if(this.seasonalChallenges.every(c => c.completed === true)) { this.allSeasonalsCompleted = true; }

          this.dailyExpiryMsg = this.dailyChallenges != null && this.dailyChallenges.length > 0 ? "Resets in " + this.getExpiryTime(this.dailyChallenges[0].endDateUtc) : "No challenges found";
          this.weeklyExpiryMsg = this.weeklyChallenges != null && this.weeklyChallenges.length > 0 ? "Resets in " + this.getExpiryTime(this.weeklyChallenges[0].endDateUtc) : "No challenges found";
          this.seasonalExpiryMsg = this.seasonalChallenges != null && this.seasonalChallenges.length > 0 ? "Resets in " + this.getExpiryTime(this.seasonalChallenges[0].endDateUtc) : "No challenges found";
        });
  }

  expandAccordions() {
    this.accordions.forEach(accordion => 
      {
        let element = accordion.nativeElement;
        let panel = element.nextElementSibling;
        panel.style.maxHeight = panel.scrollHeight + "px";
      });
  }

  toggleChallengeCompletion(challenge: Challenge): void {
    this.challengeService.toggleChallengeCompletion(challenge.challengeType + "," + challenge.index).subscribe(response => {});
  }

  getExpiryTime(challengeEndDate: string) {
      let now = new Date().getTime();
      let end = new Date(challengeEndDate).getTime();
      let millisecondsLeft = end - now;

      return this.msToHrsMin(millisecondsLeft);
  }

  msToHrsMin( ms: number ) {
    // 1- Convert to seconds:
    let seconds = ms / 1000;

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

  zeroPad(num : number, size: number) : string {
    let str = num.toString();
    while (str.length < size) str = "0" + str;
    return str;
  }

  getExpiryMessage(challenges: Challenge[]) : string 
  {
    return challenges != null && challenges.length > 0 ? "Resets in " + this.getExpiryTime(challenges[0].endDateUtc) : "No challenges found";
  }

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
    
    return "Refreshed on " + (refreshDate.getMonth() + 1) + '/' + refreshDate.getDate() + '/' + refreshDate.getFullYear() + " " + hours + ":" + this.zeroPad(refreshDate.getMinutes(), 2) + " " + amPm;
  }

  ngOnInit(): void {
    this.getCurrentChallenges();
    
    setInterval(() => 
    {
      this.dailyExpiryMsg = this.getExpiryMessage(this.dailyChallenges);
      this.weeklyExpiryMsg = this.getExpiryMessage(this.weeklyChallenges);
      this.seasonalExpiryMsg = this.getExpiryMessage(this.seasonalChallenges);
    }, 10000);
  }

  ngAfterViewInit(): void {
    
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
}