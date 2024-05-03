using System.Collections.Generic;
using SMUBE.DataStructures.Utils;
using SMUBE.Pathfinding;

namespace SMUBE.DataStructures.BattleScene
{
    public class BattleSceneBase
    {
        public static List<SMUBEVector2<int>> DefaultTeam0Positions { get; } = new List<SMUBEVector2<int>>()
        {
            new SMUBEVector2<int>(0, 0),
            new SMUBEVector2<int>(1, 0),
            new SMUBEVector2<int>(2, 0),
        };
        public static List<SMUBEVector2<int>> DefaultTeam1Positions { get; } = new List<SMUBEVector2<int>>()
        {
            new SMUBEVector2<int>(0, 2),
            new SMUBEVector2<int>(1, 2),
            new SMUBEVector2<int>(2, 2),
        };
        
        public PathfindingHandler PathfindingHandler { get; } = new PathfindingHandler();

    }
}