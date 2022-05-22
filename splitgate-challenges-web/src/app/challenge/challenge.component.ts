import { Component, OnInit } from '@angular/core';
import { Challenge } from '../challenge';
import { ChallengeService } from '../challenge.service';

@Component({
  selector: 'app-challenge',
  templateUrl: './challenge.component.html',
  styleUrls: ['./challenge.component.scss']
})
export class ChallengesComponent implements OnInit {
  challenges: Challenge[] = [];

  constructor(private challengeService: ChallengeService) { }

  getCurrentChallenges(): void {
    this.challengeService.getCurrentChallenges()
      .subscribe(response => this.challenges = response.challenges);
  }

  ngOnInit(): void {
    this.getCurrentChallenges();
  }
}