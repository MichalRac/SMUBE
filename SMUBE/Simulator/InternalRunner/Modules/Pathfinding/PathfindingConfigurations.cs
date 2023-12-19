using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Utils;
using System.Collections.Generic;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.Pathfinding
{
    internal static class PathfindingConfigurations
    {
        private readonly static int WIDTH = 10;
        private readonly static int HEIGHT = 10;

        public static List<(SMUBEVector2<int> start, SMUBEVector2<int> target)> PredefinedPaths = new List<(SMUBEVector2<int>, SMUBEVector2<int>)>()
        {
            (new SMUBEVector2<int>(1, 1), new SMUBEVector2<int>(8, 8)),
            (new SMUBEVector2<int>(1, 1), new SMUBEVector2<int>(1, 8)),
            (new SMUBEVector2<int>(1, 1), new SMUBEVector2<int>(8, 1)),
            (new SMUBEVector2<int>(6, 6), new SMUBEVector2<int>(1, 1)),

            (new SMUBEVector2<int>(8, 1), new SMUBEVector2<int>(1, 8)),
            (new SMUBEVector2<int>(3, 6), new SMUBEVector2<int>(6, 3)),
            (new SMUBEVector2<int>(3, 6), new SMUBEVector2<int>(8, 8)),
            (new SMUBEVector2<int>(6, 3), new SMUBEVector2<int>(1, 1)),
        };

        public static List<InitialGridData> PredefinedGrids = new List<InitialGridData>()
        {
            InitialGrid0,
            InitialGrid1,
            InitialGrid2,
            InitialGrid3,
            InitialGrid4,
            InitialGrid5,
            InitialGrid6,
            InitialGrid7,
        };

        private static List<SMUBEVector2<int>> MakeHorizontalLine(int y, int startX, int endX)
        {
            if (startX > endX)
            {
                var buf = endX;
                endX = startX;
                startX = buf;
            }
            var result = new List<SMUBEVector2<int>>();
            for (int x = startX; x <= endX; x++)
            {
                result.Add(new SMUBEVector2<int>(x, y));
            }
            return result;
        }

        private static List<SMUBEVector2<int>> MakeVerticalLine(int x, int startY, int endY)
        {
            if (startY > endY)
            {
                var buf = endY;
                endY = startY;
                startY = buf;
            }
            var result = new List<SMUBEVector2<int>>();
            for (int y = startY; y <= endY; y++)
            {
                result.Add(new SMUBEVector2<int>(x, y));
            }
            return result;
        }

        private static InitialGridData initialGrid0;
        public static InitialGridData InitialGrid0
        {
            get
            {
                if (initialGrid0 == null)
                {
                    initialGrid0 = new InitialGridData()
                    {
                        width = WIDTH,
                        height = HEIGHT,
                    };
                }
                return initialGrid0;
            }
        }

        private static InitialGridData initialGrid1;
        public static InitialGridData InitialGrid1
        {
            get
            {
                if (initialGrid1 == null)
                {
                    var ObstaclePos = new List<SMUBEVector2<int>>();
                    ObstaclePos.AddRange(MakeHorizontalLine(5, 1, 9));
                    ObstaclePos.AddRange(MakeHorizontalLine(7, 0, 8));

                    initialGrid1 = new InitialGridData()
                    {
                        width = WIDTH,
                        height = HEIGHT,
                        InitialObstacleCells = ObstaclePos,
                    };

                }
                return initialGrid1;
            }
        }

        private static InitialGridData initialGrid2;
        public static InitialGridData InitialGrid2
        {
            get
            {
                if (initialGrid2 == null)
                {
                    var ObstaclePos = new List<SMUBEVector2<int>>();
                    ObstaclePos.AddRange(MakeHorizontalLine(2, 1, 9));
                    ObstaclePos.AddRange(MakeHorizontalLine(7, 0, 8));

                    ObstaclePos.Add(new SMUBEVector2<int>(2, 3));
                    ObstaclePos.Add(new SMUBEVector2<int>(2, 4));
                    ObstaclePos.Add(new SMUBEVector2<int>(3, 4));
                    ObstaclePos.Add(new SMUBEVector2<int>(3, 5));

                    ObstaclePos.Add(new SMUBEVector2<int>(7, 5));
                    ObstaclePos.Add(new SMUBEVector2<int>(7, 6));
                    ObstaclePos.Add(new SMUBEVector2<int>(6, 5));
                    ObstaclePos.Add(new SMUBEVector2<int>(6, 4));

                    initialGrid2 = new InitialGridData()
                    {
                        width = WIDTH,
                        height = HEIGHT,
                        InitialObstacleCells = ObstaclePos,
                    };

                }
                return initialGrid2;
            }
        }

        private static InitialGridData initialGrid3;
        public static InitialGridData InitialGrid3
        {
            get
            {
                if (initialGrid3 == null)
                {
                    var ObstaclePos = new List<SMUBEVector2<int>>();
                    ObstaclePos.AddRange(MakeHorizontalLine(4, 0, 4));
                    ObstaclePos.AddRange(MakeHorizontalLine(5, 5, 9));


                    initialGrid3 = new InitialGridData()
                    {
                        width = WIDTH,
                        height = HEIGHT,
                        InitialObstacleCells = ObstaclePos,
                    };

                }
                return initialGrid3;
            }
        }

        private static InitialGridData initialGrid4;
        public static InitialGridData InitialGrid4
        {
            get
            {
                if (initialGrid4 == null)
                {
                    var ObstaclePos = new List<SMUBEVector2<int>>();
                    ObstaclePos.AddRange(MakeVerticalLine(0, 9, 7));
                    ObstaclePos.AddRange(MakeHorizontalLine(7, 0, 2));
                    ObstaclePos.AddRange(MakeHorizontalLine(8, 2, 7));
                    ObstaclePos.AddRange(MakeHorizontalLine(7, 7, 8));
                    ObstaclePos.AddRange(MakeHorizontalLine(3, 0, 6));
                    ObstaclePos.AddRange(MakeHorizontalLine(5, 2, 7));
                    ObstaclePos.AddRange(MakeVerticalLine(8, 2, 7));
                    ObstaclePos.Add(new SMUBEVector2<int>(7, 2));
                    ObstaclePos.AddRange(MakeHorizontalLine(1, 2, 7));


                    initialGrid4 = new InitialGridData()
                    {
                        width = WIDTH,
                        height = HEIGHT,
                        InitialObstacleCells = ObstaclePos,
                    };

                }
                return initialGrid4;
            }
        }

        private static InitialGridData initialGrid5;
        public static InitialGridData InitialGrid5
        {
            get
            {
                if (initialGrid5 == null)
                {
                    var ObstaclePos = new List<SMUBEVector2<int>>();
                    ObstaclePos.AddRange(MakeHorizontalLine(4, 0, 9));
                    ObstaclePos.AddRange(MakeVerticalLine(4, 5, 9));
                    ObstaclePos.AddRange(MakeHorizontalLine(7, 5, 9));

                    var UnstablePos = new List<SMUBEVector2<int>>();
                    UnstablePos.AddRange(MakeVerticalLine(4, 0, 2));

                    var DefensivePos = new List<SMUBEVector2<int>>();
                    DefensivePos.AddRange(MakeHorizontalLine(3, 3, 5));

                    initialGrid5 = new InitialGridData()
                    {
                        width = WIDTH,
                        height = HEIGHT,
                        InitialObstacleCells = ObstaclePos,
                        InitialUnstableCells = UnstablePos,
                        InitialDefensiveCells = DefensivePos,
                    };

                }
                return initialGrid5;
            }
        }

        private static InitialGridData initialGrid6;
        public static InitialGridData InitialGrid6
        {
            get
            {
                if (initialGrid6 == null)
                {
                    var ObstaclePos = new List<SMUBEVector2<int>>();
                    ObstaclePos.AddRange(MakeHorizontalLine(2, 2, 7));
                    ObstaclePos.AddRange(MakeHorizontalLine(7, 2, 7));
                    ObstaclePos.AddRange(MakeVerticalLine(2, 3, 6));
                    ObstaclePos.AddRange(MakeVerticalLine(7, 3, 6));

                    var DefensivePos = new List<SMUBEVector2<int>>();
                    DefensivePos.AddRange(MakeHorizontalLine(3, 3, 6));
                    DefensivePos.AddRange(MakeHorizontalLine(4, 3, 6));
                    DefensivePos.AddRange(MakeHorizontalLine(5, 3, 6));
                    DefensivePos.AddRange(MakeHorizontalLine(6, 3, 6));


                    initialGrid6 = new InitialGridData()
                    {
                        width = WIDTH,
                        height = HEIGHT,
                        InitialDefensiveCells = DefensivePos,
                        InitialObstacleCells = ObstaclePos,
                    };

                }
                return initialGrid6;
            }
        }

        private static InitialGridData initialGrid7;
        public static InitialGridData InitialGrid7
        {
            get
            {
                if (initialGrid7 == null)
                {
                    var ObstaclePos = new List<SMUBEVector2<int>>();
                    ObstaclePos.AddRange(MakeHorizontalLine(1, 6, 7));
                    ObstaclePos.AddRange(MakeHorizontalLine(2, 0, 4));
                    ObstaclePos.AddRange(MakeHorizontalLine(3, 4, 5));
                    ObstaclePos.AddRange(MakeHorizontalLine(4, 0, 1));
                    ObstaclePos.AddRange(MakeHorizontalLine(4, 3, 4));
                    ObstaclePos.AddRange(MakeHorizontalLine(7, 1, 8));
                    ObstaclePos.AddRange(MakeVerticalLine(4, 2, 5));
                    ObstaclePos.AddRange(MakeVerticalLine(7, 1, 8));
                    ObstaclePos.Add(new SMUBEVector2<int>(1, 0));
                    ObstaclePos.Add(new SMUBEVector2<int>(3, 1));
                    ObstaclePos.Add(new SMUBEVector2<int>(6, 1));
                    ObstaclePos.Add(new SMUBEVector2<int>(9, 1));
                    ObstaclePos.Add(new SMUBEVector2<int>(5, 3));
                    ObstaclePos.Add(new SMUBEVector2<int>(8, 3));
                    ObstaclePos.Add(new SMUBEVector2<int>(1, 5));
                    ObstaclePos.Add(new SMUBEVector2<int>(6, 5));
                    ObstaclePos.Add(new SMUBEVector2<int>(9, 5));
                    ObstaclePos.Add(new SMUBEVector2<int>(3, 8));
                    ObstaclePos.Add(new SMUBEVector2<int>(1, 9));
                    ObstaclePos.Add(new SMUBEVector2<int>(5, 9));

                    var DefensivePos = new List<SMUBEVector2<int>>();
                    DefensivePos.AddRange(MakeHorizontalLine(0, 6, 9));
                    DefensivePos.AddRange(MakeHorizontalLine(8, 1, 2));
                    DefensivePos.AddRange(MakeHorizontalLine(9, 2, 4));
                    DefensivePos.AddRange(MakeHorizontalLine(8, 4, 6));
                    DefensivePos.AddRange(MakeHorizontalLine(9, 6, 9));
                    DefensivePos.Add(new SMUBEVector2<int>(8, 8));
                    DefensivePos.AddRange(MakeVerticalLine(8, 1, 2));
                    DefensivePos.AddRange(MakeVerticalLine(9, 2, 4));
                    DefensivePos.AddRange(MakeVerticalLine(8, 4, 6));
                    DefensivePos.AddRange(MakeVerticalLine(9, 6, 8));

                    var UnstablePos = new List<SMUBEVector2<int>>();
                    UnstablePos.AddRange(MakeHorizontalLine(1, 4, 5));
                    UnstablePos.AddRange(MakeHorizontalLine(2, 5, 6));
                    UnstablePos.AddRange(MakeHorizontalLine(3, 0, 3));
                    UnstablePos.Add(new SMUBEVector2<int>(6, 3));
                    UnstablePos.AddRange(MakeHorizontalLine(4, 5, 6));
                    UnstablePos.Add(new SMUBEVector2<int>(2, 4));
                    UnstablePos.AddRange(MakeHorizontalLine(5, 2, 3));
                    UnstablePos.Add(new SMUBEVector2<int>(5, 5));
                    UnstablePos.AddRange(MakeHorizontalLine(6, 1, 6));


                    initialGrid7 = new InitialGridData()
                    {
                        width = WIDTH,
                        height = HEIGHT,
                        InitialDefensiveCells = DefensivePos,
                        InitialObstacleCells = ObstaclePos,
                        InitialUnstableCells = UnstablePos,
                    };

                }
                return initialGrid7;
            }
        }

    }
}
