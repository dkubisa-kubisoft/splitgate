import { Challenge } from "../challenge"

export interface GetCurrentChallengesResponse {
    dailyChallenges: Challenge[];
    dailyChallengeRefreshTimestamp: Date;
    weeklyChallenges: Challenge[];
    weeklyChallengeRefreshTimestamp: Date;
    seasonalChallenges: Challenge[];
    seasonalChallengeRefreshTimestamp: Date;
}