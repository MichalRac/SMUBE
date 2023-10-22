using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Utils;
using SMUBE.Pathfinding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SMUBE_Utils.Simulator
{
    internal class PathfindingSimulator
    {
        private int chosenPathId = 0;
        private int chosenGridId = 0;
        private GridBattleScene GridBattleScene { get; set; }
        private bool SimplifiedGrid = true;
        private PathfindingAlgorithm pathfindingModel;

        public PathfindingSimulator()
        {
            GridBattleScene = new GridBattleScene(PathfindingConfigurations.InitialGrid0);
            pathfindingModel = new DijkstraPathfindingAlgorithm();
        }

        public void Run()
        {
            Console.Clear();
            DisplayGrid();

            Console.WriteLine("Options:");

            Console.WriteLine("Up arrow. Next Path Id");
            Console.WriteLine("Down arrow. Prev Path Id");
            Console.WriteLine("Right arrow. Next Grid Id");
            Console.WriteLine("Left arrow: Prev Grid Id");

            Console.WriteLine("1. Run predefined path setup");
            Console.WriteLine("2. Run manual path setup");
            Console.WriteLine("3. Run all predefined configurations");

            Console.WriteLine("P. Toggle pathfinding algorithm");
            Console.WriteLine("G. Toggle simplified grid");
            Console.WriteLine("0. Close");

            Console.WriteLine("\nChoice:");
            var key = Console.ReadKey(true);
            Console.Write("\n");

            switch (key.Key)
            {
                case ConsoleKey.G:
                    SimplifiedGrid = !SimplifiedGrid;
                    break;
                case ConsoleKey.P:
                    if(pathfindingModel is DijkstraPathfindingAlgorithm)
                    {
                        pathfindingModel = new AStarPathfindingAlgorithm();
                    }
                    else
                    {
                        pathfindingModel = new DijkstraPathfindingAlgorithm();
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    chosenGridId--;
                    if (chosenGridId < 0)
                    {
                        chosenGridId = PathfindingConfigurations.PredefinedGrids.Count - 1;
                    }
                    GridBattleScene = new GridBattleScene(PathfindingConfigurations.PredefinedGrids[chosenGridId]);
                    break;
                case ConsoleKey.RightArrow:
                    chosenGridId++;
                    if(chosenGridId >= PathfindingConfigurations.PredefinedGrids.Count)
                    {
                        chosenGridId = 0;
                    }
                    GridBattleScene = new GridBattleScene(PathfindingConfigurations.PredefinedGrids[chosenGridId]);
                    break;
                case ConsoleKey.DownArrow:
                    chosenPathId--;
                    if (chosenPathId < 0)
                    {
                        chosenPathId = PathfindingConfigurations.PredefinedPaths.Count - 1;
                    }
                    break;
                case ConsoleKey.UpArrow:
                    chosenPathId++;
                    if (chosenPathId >= PathfindingConfigurations.PredefinedPaths.Count)
                    {
                        chosenPathId = 0;
                    }
                    break;
                case ConsoleKey.D1:
                    {
                        var predefinedPath = PathfindingConfigurations.PredefinedPaths[chosenPathId];
                        var start = GridBattleScene.Grid[predefinedPath.start.x, predefinedPath.start.y];
                        var target = GridBattleScene.Grid[predefinedPath.target.x, predefinedPath.target.y];
                        FindAndPrintShortestPath(start, target);
                        break;
                    }
                case ConsoleKey.D2:
                    {
                        var (start, target) = GetPathTargets();
                        FindAndPrintShortestPath(start, target);
                        break;
                    }
                case ConsoleKey.D3:
                    {
                        RunAllConfigurations();
                        break;
                    }
                case ConsoleKey.D0:
                    return;
            }

            Run();
        }

        private void RunAllConfigurations()
        {
            var numberOfGrids = PathfindingConfigurations.PredefinedGrids.Count;
            var numberOfPaths = PathfindingConfigurations.PredefinedPaths.Count;

            int[,] dijkstraVisitedNodesResults = new int[numberOfGrids, numberOfPaths];
            int[,] aStarVisitedNodesResults = new int[numberOfGrids, numberOfPaths];

            long[,] dijkstraRuntimeResults = new long[numberOfGrids, numberOfPaths];
            long[,] aStarRuntimeResults = new long[numberOfGrids, numberOfPaths];

            long[,] dijkstraPathLenghts = new long[numberOfGrids, numberOfPaths];
            long[,] aStarPathLenghts = new long[numberOfGrids, numberOfPaths];

            var dijkstraPathfinding = new DijkstraPathfindingAlgorithm();
            var aStarPathfinding = new AStarPathfindingAlgorithm();

            //prewarm
            dijkstraPathfinding.TryFindPathFromTo(GridBattleScene, GridBattleScene.Grid[1, 1], GridBattleScene.Grid[7, 7], out var _, out var _);
            aStarPathfinding.TryFindPathFromTo(GridBattleScene, GridBattleScene.Grid[1, 1], GridBattleScene.Grid[7, 7], out var _, out var _);


            for (int gridId = 0; gridId < numberOfGrids; gridId++)
            {
                InitialGridData boardSetup = PathfindingConfigurations.PredefinedGrids[gridId];
                for (int pathId = 0; pathId < numberOfPaths; pathId++)
                {
                    (SMUBEVector2<int> start, SMUBEVector2<int> target) path = PathfindingConfigurations.PredefinedPaths[pathId];
                    GridBattleScene = new GridBattleScene(boardSetup);
                    var start = GridBattleScene.Grid[path.start.x, path.start.y];
                    var target = GridBattleScene.Grid[path.target.x, path.target.y];


                    var dijkstraStopwatch = new Stopwatch();

                    dijkstraStopwatch.Start();
                    dijkstraPathfinding.TryFindPathFromTo(GridBattleScene, start, target, out var dijkstraResultPath, out var dijkstraVisitedNodesCount);
                    dijkstraStopwatch.Stop();

                    dijkstraVisitedNodesResults[gridId, pathId] = dijkstraVisitedNodesCount;
                    dijkstraRuntimeResults[gridId, pathId] = dijkstraStopwatch.ElapsedTicks;
                    if (dijkstraResultPath != null)
                    {
                        dijkstraPathLenghts[gridId, pathId] = dijkstraResultPath.Count;
                    }
                    else
                    {
                        dijkstraPathLenghts[gridId, pathId] = -1;
                    }

                    GridBattleScene = new GridBattleScene(boardSetup);

                    var aStarStopwatch = new Stopwatch();

                    aStarStopwatch.Start();
                    aStarPathfinding.TryFindPathFromTo(GridBattleScene, start, target, out var aStarPath, out var aStarVisitedNodesCount);
                    aStarStopwatch.Stop();

                    aStarVisitedNodesResults[gridId, pathId] = aStarVisitedNodesCount;
                    aStarRuntimeResults[gridId, pathId] = aStarStopwatch.ElapsedTicks;
                    if (aStarPath != null)
                    {
                        aStarPathLenghts[gridId, pathId] = aStarPath.Count;
                    }
                    else
                    {
                        aStarPathLenghts[gridId, pathId] = -1;
                    }
                }
            }

            Console.WriteLine("Dijkstra results:");
            Console.Write($"\n\t");
            for (int pathId = 0; pathId < numberOfPaths; pathId++)
            {
                Console.Write($"path {pathId}\t");
            }
            Console.Write($"\n");

            for (int gridId = 0; gridId < numberOfGrids; gridId++)
            {
                Console.Write($"grid {gridId}\t");

                Console.Write("\n\t");
                for (int pathId = 0; pathId < numberOfPaths; pathId++)
                {
                    Console.Write($"n:{dijkstraVisitedNodesResults[gridId, pathId]}\t");
                }
                Console.Write("\n\t");
                for (int pathId = 0; pathId < numberOfPaths; pathId++)
                {
                    Console.Write($"t:{dijkstraRuntimeResults[gridId, pathId]}\t");
                }
                Console.Write("\n\t");
                for (int pathId = 0; pathId < numberOfPaths; pathId++)
                {
                    var pathLenght = dijkstraVisitedNodesResults[gridId, pathId];
                    var pathResult = pathLenght != -1 ? pathLenght.ToString() : "X";
                    Console.Write($"s:{pathResult}\t");
                }
                Console.Write("\n\n");
            }

            Console.Write("\n");
            Console.WriteLine("A* results:");
            Console.Write($"\n\t");
            for (int pathId = 0; pathId < numberOfPaths; pathId++)
            {
                Console.Write($"path {pathId}\t");
            }
            Console.Write($"\n");
            for (int gridId = 0; gridId < numberOfGrids; gridId++)
            {
                Console.Write($"grid {gridId}\t");

                Console.Write("\n\t");

                for (int pathId = 0; pathId < numberOfPaths; pathId++)
                {
                    Console.Write($"n:{aStarVisitedNodesResults[gridId, pathId]}\t");
                }
                Console.Write("\n\t");
                for (int pathId = 0; pathId < numberOfPaths; pathId++)
                {
                    Console.Write($"t:{aStarRuntimeResults[gridId, pathId]}\t");
                }
                Console.Write("\n\t");
                for (int pathId = 0; pathId < numberOfPaths; pathId++)
                {
                    var pathLenght = aStarPathLenghts[gridId, pathId];
                    var pathResult = pathLenght != -1 ? pathLenght.ToString() : "X";
                    Console.Write($"s:{pathResult}\t");
                }
                Console.Write("\n\n");
            }


            Console.Write("\n");
            Console.WriteLine("A* - Dijkstra (performance difference):");
            Console.Write($"\n\t");
            for (int pathId = 0; pathId < numberOfPaths; pathId++)
            {
                Console.Write($"path {pathId}\t");
            }
            Console.Write($"\n");
            for (int gridId = 0; gridId < numberOfGrids; gridId++)
            {
                Console.Write($"grid {gridId}\t");

                Console.Write("\n\t");

                for (int pathId = 0; pathId < numberOfPaths; pathId++)
                {
                    Console.Write($"n:{aStarVisitedNodesResults[gridId, pathId] - dijkstraVisitedNodesResults[gridId, pathId]}\t");
                }
                Console.Write("\n\t");
                for (int pathId = 0; pathId < numberOfPaths; pathId++)
                {
                    Console.Write($"t:{aStarRuntimeResults[gridId, pathId] - dijkstraVisitedNodesResults[gridId, pathId]}\t");
                }
                Console.Write("\n\t");
                for (int pathId = 0; pathId < numberOfPaths; pathId++)
                {
                    var dijkstraPathLenght = dijkstraPathLenghts[gridId, pathId];
                    var aStarPathLenght = aStarPathLenghts[gridId, pathId];
                    string pathResult = "X";
                    if (dijkstraPathLenght != -1 && aStarPathLenght != -1)
                    {
                        pathResult = (aStarPathLenght - dijkstraPathLenght).ToString();
                    }
                    Console.Write($"s:{pathResult}\t");
                }
                Console.Write("\n\n");
            }

            Console.WriteLine("[n] - number of visited nodes");
            Console.WriteLine("[t] - runtime duration");
            Console.WriteLine("[s] - number of steps in the resulting path (X if there was no path found)");

            Console.ReadKey();
        }

        private (BattleScenePosition, BattleScenePosition) GetPathTargets()
        {
            Console.WriteLine("Setup path:");
            var startPos = InputPosition("start");
            var targetPos = InputPosition("target");

            return (startPos, targetPos);
        }

        private void FindAndPrintShortestPath(BattleScenePosition start, BattleScenePosition target)
        {
            var success = pathfindingModel.TryFindPathFromTo(GridBattleScene, start, target, out var path, out var visitedNodesCount);
            Console.Clear();

            List<SMUBEVector2<int>> displayPath = null;
            if(path != null)
            {
                displayPath = path.Select(x => x.Coordinates).ToList();
                displayPath.Add(target.Coordinates);
            }
            DisplayGrid(displayPath);
            if (success)
            {
                Console.WriteLine("Path exists! It goes through following:");


                foreach(var node in path) 
                {
                    Console.Write(node.ToString() + " -> ");
                }
                Console.Write($"{target}\n");
            }
            else
            {
                Console.WriteLine("Path does not exists!");
            }
            Console.WriteLine($"Number of visited nodes: {visitedNodesCount}");

            Console.WriteLine("1. Toggle simplified grid");
            Console.WriteLine("2. Run another!");
            Console.WriteLine("\nChoice:");
            var key = Console.ReadKey(true);
            Console.Write("\n");

            while (key.Key != ConsoleKey.D2)
            {
                if (key.Key == ConsoleKey.D1)
                {
                    SimplifiedGrid = !SimplifiedGrid;
                }

                Console.Clear();
                DisplayGrid(displayPath);

                Console.WriteLine("1. Toggle simplified grid");
                Console.WriteLine("2. Run another!");
                Console.WriteLine("\nChoice:");
                key = Console.ReadKey(true);
                Console.Write("\n");
            }

        }

        private void DisplayGrid(List<SMUBEVector2<int>> path = null)
        {
            Console.SetWindowSize(150, 50);
            Console.WriteLine($"Pathfinding algorithm: {pathfindingModel.GetType().Name}");

            var predefinedPath = PathfindingConfigurations.PredefinedPaths[chosenPathId];
            var start = GridBattleScene.Grid[predefinedPath.start.x, predefinedPath.start.y];
            var target = GridBattleScene.Grid[predefinedPath.target.x, predefinedPath.target.y];

            Console.WriteLine($"Path id {chosenPathId}, {start} -> {target}:");
            Console.WriteLine($"Grid id {chosenGridId}:");
            for (int i = GridBattleScene.Height - 1; i >= 0; i--)
            {
                for (int j = 0; j < GridBattleScene.Width; j++)
                {
                    bool isPartOfPath = false;
                    if (path != null && path.Contains(new SMUBEVector2<int>(j, i)))
                    {
                        isPartOfPath = true;
                    }

                    if (SimplifiedGrid)
                    {
                        string contentString = string.Empty;
                        switch (GridBattleScene.Grid[j, i].ContentType)
                        {
                            case BattleScenePositionContentType.None:
                                contentString = isPartOfPath ? " [-P-]" : "  [ ]";
                                break;
                            case BattleScenePositionContentType.Obstacle:
                                contentString = isPartOfPath ? "  [XP]" : "  [X]";
                                break;
                            case BattleScenePositionContentType.Defensive:
                                contentString = isPartOfPath ? "[-DP-]" : " [!D!]";
                                break;
                            case BattleScenePositionContentType.Unstable:
                                contentString = isPartOfPath ? " [-UP-]" : " [!U!]";
                                break;
                        }
                        if(start.Coordinates == GridBattleScene.Grid[j, i].Coordinates)
                        {
                            contentString = " <SSS>";
                        }
                        if(target.Coordinates == GridBattleScene.Grid[j, i].Coordinates)
                        {
                            contentString = " <TTT>";
                        }

                        Console.Write($"{contentString}\t");
                    }
                    else
                    {
                        string contentString = string.Empty;
                        switch (GridBattleScene.Grid[j, i].ContentType)
                        {
                            case BattleScenePositionContentType.None:
                                contentString = isPartOfPath ? "NP" : "N";
                                break;
                            case BattleScenePositionContentType.Obstacle:
                                contentString = isPartOfPath ? "ObP" : "Ob";
                                break;
                            case BattleScenePositionContentType.Defensive:
                                contentString = isPartOfPath ? "DefP" : "Def";
                                break;
                            case BattleScenePositionContentType.Unstable:
                                contentString = isPartOfPath ? "UnsP" : "Uns";
                                break;
                        }
                        Console.Write($"{j},{i}-{contentString}\t");
                    }
                }
                Console.Write("\n\n\n");
            }

            if (SimplifiedGrid)
            {
                Console.Write("[ ] - Empty node, [X] - Obstacle node, [D] - Defensive node, [U] - Unstable node\n[-P-] - Path node, [-DP-] - Path node on defensive node, [-UP-] - Path on unstable node\n\n");
            }
            else
            {
                Console.Write("N - Empty node, Ob - Obstacle node, Def - Defensive node, Uns - Unstable node\nNP - Path node, DefP - Path node on defensive node, UnsP - Path on unstable node\n\n");
            }

        }

        private BattleScenePosition InputPosition(string posName)
        {
            Console.WriteLine($"{posName} start x: ");
            var x = InputCoord();
            Console.WriteLine($"{posName} start y: ");
            var y = InputCoord();

            var battleScenePosition = new BattleScenePosition(x, y);
            if(!GridBattleScene.IsValid(battleScenePosition.Coordinates))
            {
                Console.WriteLine("\nPosition out of bounds! Input it again!");
                return InputPosition(posName);
            }

            return battleScenePosition;
        }

        private int InputCoord()
        {
            var read = Console.ReadLine();
            if (!int.TryParse(read, out var number))
            {
                Console.WriteLine("\nNon int value provided! Try again!");
                return InputCoord();
            }
            return number;
        }
    }
}
