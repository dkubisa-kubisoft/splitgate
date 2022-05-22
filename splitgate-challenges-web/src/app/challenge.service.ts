import { Injectable } from '@angular/core';
import { Challenge } from './challenge';
import { CHALLENGES } from './mock-challenges';
import { Observable, of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { ChallengeResponse } from './ChallengeResponse';

@Injectable({
  providedIn: 'root'
})
export class ChallengeService {
  private challengesUrl = 'https://splitgate-challenge-api.azurewebsites.net/api/GetCurrentChallenges';

  getCurrentChallenges(): Observable<ChallengeResponse> {
    return this.http.get<ChallengeResponse>(this.challengesUrl);
  }

  constructor(
    private http: HttpClient) { }
}
