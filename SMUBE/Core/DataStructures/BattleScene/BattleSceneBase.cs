using System.Collections.Generic;
using SMUBE.DataStructures.Utils;
using SMUBE.Pathfinding;

namespace SMUBE.DataStructures.BattleScene
{
    public class BattleSceneBase
    {
        public static List<SMUBEVector2<int>> DefaultTeam0Positions { get; } = new List<SMUBEVector2<int>>()
        {
            new SMUBEVector2<int>(1, 0),
            new SMUBEVector2<int>(3, 0),
            new SMUBEVector2<int>(5, 0),
        };
        public static List<SMUBEVector2<int>> DefaultTeam1Positions { get; } = new List<SMUBEVector2<int>>()
        {
            new SMUBEVector2<int>(1, 11),
            new SMUBEVector2<int>(3, 11),
            new SMUBEVector2<int>(5, 11),
        };
        
        public PathfindingHandler PathfindingHandler { get; protected set; } = new PathfindingHandler();

    }
}