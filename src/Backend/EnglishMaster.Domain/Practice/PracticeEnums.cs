namespace EnglishMaster.Domain.Practice;

public enum PracticeItemStatus
{
    New = 0,
    Due = 1,
    Learning = 2,
    Reviewing = 3,
    Mastered = 4,
    Suspended = 5
}

public enum PracticeSessionStatus
{
    Started = 0,
    Completed = 1,
    Abandoned = 2
}

public enum PracticeResult
{
    Again = 0,
    Hard = 1,
    Good = 2,
    Easy = 3
}

