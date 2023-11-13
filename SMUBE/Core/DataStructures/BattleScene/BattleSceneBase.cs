using SMUBE.Pathfinding;

namespace SMUBE.DataStructures.BattleScene
{
    public class BattleSceneBase
    {
        internal virtual PathfindingAlgorithm Pathfinding { get; } = new AStarPathfindingAlgorithm();


    }
}