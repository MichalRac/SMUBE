using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.DataStructures.BattleScene;

namespace SMUBE.Pathfinding
{
    public class PathfindingHandler
    {
        private PathfindingAlgorithm Pathfinding { get; } = new AStarPathfindingAlgorithm();
        
        private List<PathfindingAlgorithm.PathfindingPathCache> _activeUnitReachablePositions = new List<PathfindingAlgorithm.PathfindingPathCache>();
        public IReadOnlyList<PathfindingAlgorithm.PathfindingPathCache> ActiveUnitReachablePositions => _activeUnitReachablePositions;

        public void OnNewTurn(BattleStateModel battleState)
        {
            EvaluateActiveUnitReachablePositions(battleState);
        }

        private void EvaluateActiveUnitReachablePositions(BattleStateModel battleState)
        {
            _activeUnitReachablePositions.Clear();

            var unit = battleState.ActiveUnit;

            var reachablePositions
                = Pathfinding.GetAllReachablePaths(battleState.BattleSceneState, unit.UnitData.BattleScenePosition, unit.UnitData.UnitStats.Speed);

            _activeUnitReachablePositions = reachablePositions;
        }

        public List<BattleScenePosition> GetSurroundingPositions(BattleStateModel battleState, BattleScenePosition target)
        {
            return Pathfinding.GetSurroundingPositions(battleState.BattleSceneState, target, true);
        }

        public List<BattleScenePosition> GetReachableSurroundingPositions(BattleStateModel battleState, BattleScenePosition target)
        {
            var result = new List<BattleScenePosition>();
            
            var surroundingPositions = GetSurroundingPositions(battleState, target);
            foreach (var surroundingPosition in surroundingPositions)
            {
                if (_activeUnitReachablePositions.Any(reachable => reachable.Position.Coordinates.Equals(surroundingPosition.Coordinates)))
                {
                    result.Add(surroundingPosition);
                }
            }

            return result;
        }
    }
}