namespace SMUBE.Commands.Args
{
    public enum ArgsEnemyTargetingPreference
    {
        None = 0,
        
        Closest = 1,
        LeastHpPoints = 2,
        //MostHpPoints = 3,
        LeastHpPercentage = 4,
        //MostHpPercentage = 5,
        MostDmgDealt = 6,
        EnemyWithMostAlliesInRange = 7,
        MinimizeReachableEnemiesAfterTurn = 8,
        MaximiseReachableEnemiesAfterTurn = 9,
        MinimisePositionBuffAfterTurn = 10,
        MaximisePositionBuffAfterTurn = 11,
    }
    
    public enum ArgsMovementTargetingPreference
    {
        None = 0,
        
        GetOutOfReach = 1,
        GetCloserCarefully = 2,
        GetCloserAggressively = 3,
        OptimizeFortifiedPosition = 4,
    }
    
    public enum ArgsPositionTargetingPreference
    {
        None = 0,
        
        OnLeastHpPercentageAlly = 1,
        OnMostHpPercentageAlly = 2,
        NextToClosestEnemy = 3,
        OnAllyWithMostEnemiesInReach = 4,
        InBetweenEnemies = 5,
        InBetweenTeams = 6,
    }
    
    public class ArgsPreferences
    {
        public readonly ArgsEnemyTargetingPreference TargetingPreference;
        public readonly ArgsMovementTargetingPreference MovementTargetingPreference;
        public readonly ArgsPositionTargetingPreference PositionTargetingPreference;

        public static ArgsPreferences Default()
        {
            return new ArgsPreferences();
        }

        private ArgsPreferences()
        {
            TargetingPreference = ArgsEnemyTargetingPreference.None;
            MovementTargetingPreference = ArgsMovementTargetingPreference.None;
            PositionTargetingPreference = ArgsPositionTargetingPreference.None;
        }
        
        public ArgsPreferences(ArgsEnemyTargetingPreference targetingPreference)
        {
            TargetingPreference = targetingPreference;
        }

        public ArgsPreferences(ArgsMovementTargetingPreference movementTargetingPreference)
        {
            MovementTargetingPreference = movementTargetingPreference;
        }

        public ArgsPreferences(ArgsPositionTargetingPreference positionTargetingPreference)
        {
            PositionTargetingPreference = positionTargetingPreference;
        }

        public ArgsPreferences(
            ArgsEnemyTargetingPreference targetingPreference = ArgsEnemyTargetingPreference.None, 
            ArgsMovementTargetingPreference movementTargetingPreference = ArgsMovementTargetingPreference.None, 
            ArgsPositionTargetingPreference positionTargetingPreference = ArgsPositionTargetingPreference.None)
        {
            TargetingPreference = targetingPreference;
            MovementTargetingPreference = movementTargetingPreference;
            PositionTargetingPreference = positionTargetingPreference;
        }
    }
}