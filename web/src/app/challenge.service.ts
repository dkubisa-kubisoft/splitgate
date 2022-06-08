import { Injectable } from '@angular/core';
import { Challenge } from './challenge';
import { CHALLENGES } from './mock-challenges';
import { Observable, of } from 'rxjs';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { GetCurrentChallengesResponse } from './Responses/GetCurrentChallengesResponse';
import { BasicResponse } from './Responses/BasicResponse';
import { ToggleChallengeCompletionRequest } from './Requests/ToggleChallengeCompletionRequest';


@Injectable({
  providedIn: 'root'
})
export class ChallengeService {
  // private getCurrentChallengesUrl = 'https://splitgate-challenge-api.azurewebsites.net/api/GetCurrentChallenges';
  // private toggleChallengeCompletionUrl = 'https://splitgate-challenge-api.azurewebsites.net/api/ToggleChallengeCompletion?code=wExZuwtDZKs61WDzVjdGpnwngCYhQblATQzbPIvN8kWoAzFuC158EA==';

  private getCurrentChallengesUrl = 'http://localhost:7071/api/GetCurrentChallenges';
  private toggleChallengeCompletionUrl = 'http://localhost:7071/api/ToggleChallengeCompletion?code=wExZuwtDZKs61WDzVjdGpnwngCYhQblATQzbPIvN8kWoAzFuC158EA==';

  getCurrentChallenges(): Observable<GetCurrentChallengesResponse> {
    return this.http.get<GetCurrentChallengesResponse>(this.getCurrentChallengesUrl);
  }

  toggleChallengeCompletion(challengeId : string): Observable<BasicResponse> {
    return this.http.post<BasicResponse>(this.toggleChallengeCompletionUrl, new ToggleChallengeCompletionRequest(challengeId));
  }

  constructor(
    private http: HttpClient) { }
}
