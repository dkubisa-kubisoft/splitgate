import { Component, OnInit } from '@angular/core';
import { Challenge } from '../challenge';
import { ChallengeService } from '../challenge.service';

@Component({
  selector: 'app-challenge',
  templateUrl: './challenge.component.html',
  styleUrls: ['./challenge.component.scss']
})
export class ChallengesComponent implements OnInit {
  dailyChallenges: Challenge[] = [];
  weeklyChallenges: Challenge[] = [];
  seasonChallenges: Challenge[] = [];

  constructor(private challengeService: ChallengeService) { }

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
        }
        );
  }

  toggleChallengeCompletion(challenge: Challenge): void {
    this.challengeService.toggleChallengeCompletion(challenge.challengeType + "," + challenge.index).subscribe(response => {});
  }

  ngOnInit(): void {
    this.getCurrentChallenges();
  }
}