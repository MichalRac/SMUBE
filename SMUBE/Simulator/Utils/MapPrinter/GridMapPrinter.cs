using System;
using System.Collections.Generic;
using System.Linq;
using Alba.CsConsoleFormat;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.Commands.Args;
using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Utils;

namespace SMUBE_Utils.Simulator.Utils.MapPrinter
{
    public class GridMapPathDisplayData
    {
        public SMUBEVector2<int> StartNode { get; }
        public SMUBEVector2<int> TargetNode { get; }
        public List<SMUBEVector2<int>> PathNodes { get; }

        public GridMapPathDisplayData(SMUBEVector2<int> start, SMUBEVector2<int> target, List<SMUBEVector2<int>> path)
        {
            StartNode = start;
            TargetNode = target;
            PathNodes = path ?? new List<SMUBEVector2<int>>();
        }
    }

    public class GridMapGenericDisplayData
    {
        public SMUBEVector2<int> Coordinates { get; set; }
        public string Label { get; set; }
        public ConsoleColor Color { get; set; } = ConsoleColor.Gray;
        public bool SetAsOccupied { get; set; } = false;
    }

    public class GridMapPrinter
    {
        private readonly ConsoleColor team0Color = ConsoleColor.DarkGreen;
        private readonly ConsoleColor team1Color = ConsoleColor.Red;

        private readonly GridBattleScene _battleScene;
        private List<GridMapGenericDisplayData> GenericDisplayData { get; }
        private GridMapPathDisplayData PathData { get; }

        private GridMapPrinter(GridBattleScene battleScene, List<GridMapGenericDisplayData> genericDisplayData, GridMapPathDisplayData pathData)
        {
            _battleScene = battleScene;
            GenericDisplayData = genericDisplayData ?? new List<GridMapGenericDisplayData>();
            PathData = pathData;
        }

        public static GridMapPrinter DefaultGridPrinter(GridBattleScene battleScene, List<GridMapGenericDisplayData> genericDisplayData = null)
        {
            return new GridMapPrinter(battleScene, genericDisplayData,null);
        }

        public static GridMapPrinter GridPrinterWithPath(GridBattleScene battleScene, GridMapPathDisplayData pathData, List<GridMapGenericDisplayData> genericDisplayData = null)
        {
            return new GridMapPrinter(battleScene, genericDisplayData, pathData);
        }

        public static GridMapPrinter GridPrinterForCommandArgs(BattleStateModel battleStateModel, ICommand command, CommandArgs commandArgs)
        {
            List<GridMapGenericDisplayData> genericDisplayData = new List<GridMapGenericDisplayData>();
            if (commandArgs.ActiveUnit != null)
            {
                genericDisplayData.Add(new GridMapGenericDisplayData()
                {
                    Color = ConsoleColor.Yellow,
                    Coordinates = commandArgs.ActiveUnit.BattleScenePosition.Coordinates,
                    Label = "Active"
                });
            }

            if (commandArgs.TargetUnits != null)
            {
                foreach (var target in commandArgs.TargetUnits)
                {
                    genericDisplayData.Add(new GridMapGenericDisplayData()
                    {
                        Color = ConsoleColor.DarkYellow,
                        Coordinates = target.BattleScenePosition.Coordinates,
                        Label = "Target"
                    });
                }
            }
            if (commandArgs.TargetPositions != null)
            {
                foreach (var target in commandArgs.TargetPositions)
                {
                    var targetPos = battleStateModel.BattleSceneState.Grid[target.x, target.y];
                    
                    genericDisplayData.Add(new GridMapGenericDisplayData()
                    {
                        Color = ConsoleColor.DarkYellow,
                        Coordinates = targetPos.Coordinates,
                        Label = "Target"
                    });
                }
            }
            
            GridMapPathDisplayData pathData = null;
            // todo if many position deltas per command will be supported, this should be updated
            if (commandArgs.PositionDelta != null)
            {
                var targetPosition = commandArgs.PositionDelta.Target;

                if (commandArgs.PositionDelta.IsPathless)
                {
                    pathData = new GridMapPathDisplayData(commandArgs.PositionDelta.Start, commandArgs.PositionDelta.Target, null);
                }
                else
                {
                    foreach (var reachablePosition in battleStateModel.BattleSceneState.PathfindingHandler.ActiveUnitReachablePositions)
                    {
                        if (!reachablePosition.TargetPosition.Coordinates.Equals(targetPosition)) continue;
                        var fullPathCoordinates = reachablePosition.ShortestKnownPath
                            .Select(p => p.Coordinates).ToList();
                        pathData = new GridMapPathDisplayData(commandArgs.PositionDelta.Start, commandArgs.PositionDelta.Target, fullPathCoordinates);
                    }
                }
            }
            
            return new GridMapPrinter(battleStateModel.BattleSceneState, genericDisplayData, pathData);
        }

        public void PrintMap()
        {
            Console.SetWindowSize(150, 50);
            
            var height = _battleScene.Height;
            var width = _battleScene.Width;
            var currentGrid = _battleScene.Grid;
            
            var grid = new Grid()
            {
                Stroke = LineThickness.Single,
                Columns = { GridLength.Auto }
            };
            for (int i = 0; i < width; i++)
            {            
                grid.Columns.Add(GridLength.Star(1));
            }
            
            for (int y = height - 1; y >= 0; y--)
            {
                grid.Children.Add(new Cell(y){Align = Align.Center, Color = ConsoleColor.Yellow});
                for (int x = 0; x < width; x++)
                {
                    var currentNode = currentGrid[x, y];
                    var color = currentNode.UnitIdentifier == null
                        ? ConsoleColor.Gray
                        : currentNode.UnitIdentifier.TeamId == 0
                            ? team0Color
                            : team1Color;

                    var gridCell = new Cell(){ Align = Align.Center};
                    bool isOccupied = false;
                    
                    if (currentNode.UnitIdentifier != null)
                    {
                        isOccupied = true;
                        
                        var unitCell = new Cell(currentNode.UnitIdentifier)
                        {
                            Color = color,
                            Align = Align.Center,
                        };
                        gridCell.Children.Add(unitCell);
                    }
                    
                    switch (currentNode.ContentType)
                    {
                        case BattleScenePositionContentType.None:
                        {
                            break;

                        }
                        case BattleScenePositionContentType.Obstacle:
                        {
                            isOccupied = true;
                            var contentCell = new Cell("[Wall]")
                            {
                                Color = ConsoleColor.White,
                                Align = Align.Center,
                            };
                            gridCell.Children.Add(contentCell);
                            break;
                        }
                        case BattleScenePositionContentType.ObstacleTimed:
                        {
                            var contentCell = new Cell($"[Wall:T{currentNode.RemainingDuration}]")
                            {
                                Color = ConsoleColor.White,
                                Align = Align.Center,
                            };
                            gridCell.Children.Add(contentCell);
                            break;
                        }
                        case BattleScenePositionContentType.DefensiveTimed:
                        {
                            var contentCell = new Cell($"<Fortified:T{currentNode.RemainingDuration}>")
                            {
                                Color = ConsoleColor.DarkCyan,
                                Align = Align.Center,
                            };
                            gridCell.Children.Add(contentCell);
                            break;
                        }
                        case BattleScenePositionContentType.Defensive:
                        {
                            var contentCell = new Cell("<Fortified>")
                            {
                                Color = ConsoleColor.DarkCyan,
                                Align = Align.Center,
                            };
                            gridCell.Children.Add(contentCell);
                            break;

                        }
                        case BattleScenePositionContentType.Unstable:
                        {
                            var contentCell = new Cell("_Muddy_")
                            {
                                Color = ConsoleColor.DarkBlue,
                                Align = Align.Center,
                            };
                            gridCell.Children.Add(contentCell);
                            break;
                        }
                    }

                    foreach (var genericDisplayData in GenericDisplayData)
                    {
                        if (genericDisplayData.Coordinates.Equals(currentNode.Coordinates))
                        {
                            var customMsgCell = new Cell(genericDisplayData.Label)
                            {
                                Color = genericDisplayData.Color,
                                Align = Align.Center,
                            };
                            gridCell.Children.Add(customMsgCell);
                        }
                    }

                    if (PathData != null)
                    {
                        if (PathData.StartNode.Equals(currentNode.Coordinates))
                        {
                            var activeCell = new Cell("Path Start")
                            {
                                Color = ConsoleColor.Cyan,
                                Align = Align.Center,
                            };
                            gridCell.Children.Add(activeCell);
                        }
                        
                        if (PathData.TargetNode.Equals(currentNode.Coordinates))
                        {
                            var activeCell = new Cell("Path End")
                            {
                                Color = ConsoleColor.Cyan,
                                Align = Align.Center,
                            };
                            gridCell.Children.Add(activeCell);
                        }

                        if (PathData.PathNodes.Contains(currentNode.Coordinates))
                        {
                            var activeCell = new Cell("Path")
                            {
                                Color = ConsoleColor.Blue,
                                Align = Align.Center,
                            };
                            gridCell.Children.Add(activeCell);
                        }
                    }
                    
                    if(!isOccupied)
                    {
                        var locationCell = new Cell("Empty")
                        {
                            Align = Align.Center,
                            Color = ConsoleColor.DarkGray
                        };
                        gridCell.Children.Add(locationCell);
                    }
                    
                    grid.Children.Add(gridCell);
                }
            }
            
            grid.Children.Add(new Cell("y/x"){Color = ConsoleColor.Yellow});
            
            for (int x = 0; x < width; x++)
            {
                grid.Children.Add(new Cell(x) {Align = Align.Center, Color = ConsoleColor.Yellow});
            }

            var doc = new Document(grid);
            ConsoleRenderer.RenderDocument(doc);
            return;

            for (int y = width - 1; y >= 0; y--)
            {
                for (int x = 0; x < height; x++)
                {
                    var currentNode = currentGrid[x, x];
                    string contentString = string.Empty;
                    switch (currentNode.ContentType)
                    {
                        case BattleScenePositionContentType.None:
                            contentString = "  [ ]";
                            break;
                        case BattleScenePositionContentType.Obstacle:
                            contentString = "  [X]";
                            break;
                        case BattleScenePositionContentType.Defensive:
                            contentString = " [!D!]";
                            break;
                        case BattleScenePositionContentType.Unstable:
                            contentString = " [!U!]";
                            break;
                    }

                    if (PathData != null && PathData.PathNodes.Contains(currentNode.Coordinates))
                    {
                        
                    }

                    if (currentNode.UnitIdentifier != null)
                    {
                        var id = $" [{currentNode.UnitIdentifier.TeamId}:{currentNode.UnitIdentifier.PersonalId}]";
                        contentString = id;
                        
                        Console.ForegroundColor = currentNode.UnitIdentifier.TeamId == 0
                            ? team0Color
                            : team1Color;
                    }

                    Console.Write($"{contentString}\t");
                    Console.ForegroundColor = ConsoleColor.Gray;
                }

                Console.Write("\n\n\n");
            }
            
        }
        
        /*
        public void PrintMapWithPath(GridBattleScene battleScene, bool printSimplified, List<SMUBEVector2<int>> path = null)
        {
            Console.SetWindowSize(150, 50);

            var predefinedPath = PathfindingConfigurations.PredefinedPaths[chosenPathId];
            var start = battleScene.Grid[predefinedPath.start.x, predefinedPath.start.y];
            var target = battleScene.Grid[predefinedPath.target.x, predefinedPath.target.y];

            Console.WriteLine($"Path id {chosenPathId}, {start} -> {target}:");
            Console.WriteLine($"Grid id {chosenGridId}:");
            for (int i = GridBattleScene.Height - 1; i >= 0; i--)
            {
                for (int j = 0; j < GridBattleScene.Width; j++)
                {
                    bool isPartOfPath = path != null 
                                        && path.Contains(new SMUBEVector2<int>(j, i));

                    if (printSimplified)
                    {
                        string contentString = string.Empty;
                        switch (battleScene.Grid[j, i].ContentType)
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
                        if (start.Coordinates == battleScene.Grid[j, i].Coordinates)
                        {
                            contentString = " <SSS>";
                        }
                        if (target.Coordinates == battleScene.Grid[j, i].Coordinates)
                        {
                            contentString = " <TTT>";
                        }

                        Console.Write($"{contentString}\t");
                    }
                    else
                    {
                        string contentString = string.Empty;
                        switch (battleScene.Grid[j, i].ContentType)
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

            if (printSimplified)
            {
                Console.Write("[ ] - Empty node, [X] - Obstacle node, [D] - Defensive node, [U] - Unstable node\n[-P-] - Path node, [-DP-] - Path node on defensive node, [-UP-] - Path on unstable node\n\n");
            }
            else
            {
                Console.Write("N - Empty node, Ob - Obstacle node, Def - Defensive node, Uns - Unstable node\nNP - Path node, DefP - Path node on defensive node, UnsP - Path on unstable node\n\n");
            }

        }
    */
    }
}