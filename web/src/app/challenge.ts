export interface Challenge {
    challengeType: string;
    description: string;
    index: number;
    startDateUtc: string;
    endDateUtc: string;
    completed: boolean;
    timestamp: Date;
    stage?: number;
}