using System.Collections.Generic;
using SMUBE.DataStructures.Utils;
using SMUBE.Pathfinding;

namespace SMUBE.DataStructures.BattleScene
{
    public class BattleSceneBase
    {
        public static List<SMUBEVector2<int>> DefaultTeam0Positions { get; } = new List<SMUBEVector2<int>>()
        {
            new SMUBEVector2<int>(2, 1),
            new SMUBEVector2<int>(4, 1),
            new SMUBEVector2<int>(6, 1),
        };
        public static List<SMUBEVector2<int>> DefaultTeam1Positions { get; } = new List<SMUBEVector2<int>>()
        {
            new SMUBEVector2<int>(2, 7),
            new SMUBEVector2<int>(4, 7),
            new SMUBEVector2<int>(6, 7),
        };
        
        internal PathfindingHandler PathfindingHandler { get; } = new PathfindingHandler();

    }
}