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

  dailyExpiryTime: string = "? hrs, ? min";
  dailyChallenges: Challenge[] = [];

  weeklyExpiryTime: string = "? hrs, ? min";
  weeklyChallenges: Challenge[] = [];

  seasonExpiryTime: string = "? hrs, ? min";
  seasonChallenges: Challenge[] = [];

  constructor(private challengeService: ChallengeService) 
  { 
    this.accordions = new QueryList<ElementRef>();
  }

  getCurrentChallenges(): void {
    this.challengeService.getCurrentChallenges()
      .subscribe(response => 
        {
          for (let i = 0; i < response.challenges.length; i++) {
            let c = response.challenges[i];

            switch (c.challengeType) {
              case 'daily':
                this.dailyChallenges.push(c);
                break;
              case 'weekly':
                this.weeklyChallenges.push(c);
                break;
              case 'seasonal':
                this.seasonChallenges.push(c);
                break;
            }
          }

          this.dailyExpiryTime = this.getExpiryTime(this.dailyChallenges[0].endDateUtc);
          this.weeklyExpiryTime = this.getExpiryTime(this.weeklyChallenges[0].endDateUtc);
          this.seasonExpiryTime = this.getExpiryTime(this.seasonChallenges[0].endDateUtc);
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

    if (seconds >= 30) {
        minutes = minutes + 1;
    }

    if (hours == 0) { return minutes + " min"; }
    if (days == 0) { return hours + " hrs, " + minutes + " min"; }
    if (days == 1) { return days + " day, " + hours + " hrs"; }
    return days + " days, " + hours + " hrs";
}

  ngOnInit(): void {
    this.getCurrentChallenges();
    
    setInterval(() => {
      this.dailyExpiryTime = this.getExpiryTime(this.dailyChallenges[0].endDateUtc);
      this.weeklyExpiryTime = this.getExpiryTime(this.weeklyChallenges[0].endDateUtc);
      this.seasonExpiryTime = this.getExpiryTime(this.seasonChallenges[0].endDateUtc);
    }, 31000);
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

            // if (panel.style.maxHeight) {
            //   panel.style.maxHeight = null;
            // }
            // else {
            //   panel.style.maxHeight = panel.scrollHeight + "px";
            // }

          });
      });
  }
}