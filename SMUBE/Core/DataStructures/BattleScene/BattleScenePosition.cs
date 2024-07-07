using SMUBE.BattleState;
using SMUBE.Commands._Common;
using SMUBE.Commands.Effects;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;
using SMUBE.Pathfinding;
using SMUBE.Units;

namespace SMUBE.DataStructures.BattleScene
{
    public enum BattleScenePositionContentType
    {
        None,
        Obstacle,
        Defensive,
        Unstable,
        ObstacleTimed,
        DefensiveTimed,
    }

    public class BattleScenePosition
    {
        public SMUBEVector2<int> Coordinates { get; }
        public UnitIdentifier UnitIdentifier { get; private set; }
        public BattleScenePositionContentType ContentType { get; private set; }
        public int RemainingDuration { get; private set; } = -1;

        public BattleScenePosition(int x, int y)
        {
            Coordinates = new SMUBEVector2<int>(x, y);
        }

        public BattleScenePosition(SMUBEVector2<int> pos)
        {
            Coordinates = pos;
        }

        private BattleScenePosition(BattleScenePosition sourceBattleScenePosition)
        {
            UnitIdentifier = sourceBattleScenePosition.UnitIdentifier;
            Coordinates = new SMUBEVector2<int>(sourceBattleScenePosition.Coordinates.x, sourceBattleScenePosition.Coordinates.y);
            ContentType = sourceBattleScenePosition.ContentType;
            RemainingDuration = sourceBattleScenePosition.RemainingDuration;
        }

        public BattleScenePosition DeepCopy()
        {
            return new BattleScenePosition(this);
        }

        public void OnNewTurn(BattleStateModel battleStateModel)
        {
            ProcessTimedObstacle(battleStateModel);
        }

        public void ProcessAssignedUnit(Unit assignedUnit)
        {
            var defenseMultiplier = new DamageAppliedMultiplier(3/4f, UnitRoundStartTrigger.OnAnyUnitRoundStart, 1);
            
            if (ContentType == BattleScenePositionContentType.DefensiveTimed)
            {
                assignedUnit.UnitData.UnitStats.PersistentEffects.Add(defenseMultiplier);
            }
            
            if (ContentType == BattleScenePositionContentType.Defensive)
            {
                assignedUnit.UnitData.UnitStats.PersistentEffects.Add(defenseMultiplier);
            }
        }

        private void ProcessTimedObstacle(BattleStateModel battleStateModel)
        {
            if (RemainingDuration > 0)
            {
                RemainingDuration--;
            }
            
            if (RemainingDuration == 0 && (ContentType == BattleScenePositionContentType.ObstacleTimed 
                                           || ContentType == BattleScenePositionContentType.DefensiveTimed))
            {
                RemainingDuration = -1;
                ContentType = BattleScenePositionContentType.None;
                battleStateModel.BattleSceneState.PathfindingHandler.AggregatedDirtyPositionCache.Add((Coordinates, true));
            }
        }

        public bool IsOccupied()
        {
            return UnitIdentifier != null;
        }

        // but potentially occupied
        public bool IsWalkable()
        {
            return ContentType != BattleScenePositionContentType.Obstacle && ContentType != BattleScenePositionContentType.ObstacleTimed;
        }
        
        public bool IsSpecial()
        {
            return ContentType == BattleScenePositionContentType.Defensive || ContentType == BattleScenePositionContentType.Unstable;
        }

        public void Clear()
        {
            UnitIdentifier = null;
        }

        public void ApplyUnit(UnitIdentifier unitIdentifier)
        {
            Clear();
            UnitIdentifier = unitIdentifier;
        }

        public void ApplyObstacle()
        {
            Clear();
            ContentType = BattleScenePositionContentType.Obstacle;
        }

        public void ApplyDefensive()
        {
            ContentType = BattleScenePositionContentType.Defensive;
        }

        public void ApplyObstacleTimed(int duration)
        {
            Clear();
            RemainingDuration = duration;
            ContentType = BattleScenePositionContentType.ObstacleTimed;
        }
        
        public void ApplyDefensiveTimed(int duration)
        {
            RemainingDuration = duration;
            ContentType = BattleScenePositionContentType.DefensiveTimed;
        }
        
        public void ApplyUnstable()
        {
            Clear();
            ContentType = BattleScenePositionContentType.Unstable;
        }

        public override string ToString()
        {
            return $"({Coordinates.x},{Coordinates.y})";
        }
    }
}
