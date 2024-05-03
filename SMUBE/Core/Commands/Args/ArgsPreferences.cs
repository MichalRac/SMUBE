namespace SMUBE.Commands.Args
{
    public enum ArgsEnemyTargetingPreference
    {
        None = 0,
        
        Closest,
        LeastHpPoints,
        LeastHpPercentage,
        MostDmgDealt,
        EnemyWithMostAlliesInRange,
        MinimizeReachableEnemiesAfterTurn,
        MaximiseReachableEnemiesAfterTurn,
        MinimisePositionBuffAfterTurn,
        MaximisePositionBuffAfterTurn,
    }
    
    public enum ArgsMovementTargetingPreference
    {
        None = 0,
        
        GetOutOfReach,
        GetCloserCarefully,
        GetCloserAggressively,
        GetKeepFortifiedPosition,
    }
    
    public enum ArgsPositionTargetingPreference
    {
        None = 0,
        
        OnLeastHpPercentageAlly,
        OnMostHpPercentageAlly,
        NextToEnemyInReach,
        OnAllyWithMostEnemiesInReach,
        InBetweenEnemies,
        InBetweenTeams,
        OnShortestPathInBetweenTeams,
    }
}