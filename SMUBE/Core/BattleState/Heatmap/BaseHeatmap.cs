using System;
using System.Collections.Generic;
using System.Linq;
using SMUBE.DataStructures.Utils;

namespace SMUBE.BattleState.Heatmap
{
    public class BaseHeatmap
    {
        protected virtual int PrefillValue => 0;
        
        public List<List<int>> Heatmap { get; protected set; }
        
        protected int _width;
        protected int _height;
        
        public BaseHeatmap(int width, int height)
        {
            _height = height;
            _width = width;
        }
        
        protected BaseHeatmap(BattleStateModel battleStateModel)
        {
            _width = battleStateModel.BattleSceneState.Width;
            _height = battleStateModel.BattleSceneState.Height;
        }
        
        public virtual void Prefill()
        {
            Heatmap = new List<List<int>>();
            for (var x = 0; x < _width; x++)
            {
                var column = new List<int>();
                for (var y = 0; y < _height; y++)
                {
                    column.Add(PrefillValue);
                }
                Heatmap.Add(column);
            }
        }

        protected void Set(int x, int y, int value)
        {
            Heatmap[x][y] = value;
        }

        public (int x, int y) GetMaxScoreCoordinates()
        {
            var max = int.MinValue;
            var equalScores = new List<(int, int)>();
            
            for (var x = 0; x < Heatmap.Count; x++)
            {
                var column = Heatmap[x];
                for (var y = 0; y < column.Count; y++)
                {
                    var cell = column[y];
                    if (cell == PrefillValue)
                    {
                        continue;
                    }
                    if (cell > max)
                    {
                        max = cell;
                        equalScores = new List<(int, int)>();
                        equalScores.Add((x,y));
                    }
                    else if (cell == max)
                    {
                        equalScores.Add((x,y));
                    }
                }
            }

            equalScores.Shuffle();
            return equalScores.First();
        }
        
        public (int x, int y) GetMinScoreCoordinates()
        {
            var min = int.MaxValue;
            var equalScores = new List<(int, int)>();
            
            for (var x = 0; x < Heatmap.Count; x++)
            {
                var column = Heatmap[x];
                for (var y = 0; y < column.Count; y++)
                {
                    var cell = column[y];
                    if (cell == PrefillValue)
                    {
                        continue;
                    }
                    if (cell < min)
                    {
                        min = cell;
                        equalScores = new List<(int, int)>();
                        equalScores.Add((x,y));
                    }

                    if (cell == min)
                    {
                        equalScores.Add((x,y));
                    }
                }
            }

            equalScores.Shuffle();
            return equalScores.First();
        }

        public virtual void ProcessHeatmap(BattleStateModel battleStateModel)
        {
            Prefill();
        }

        public virtual bool IsValidScore(int value)
        {
            return true;
        }
        
        public virtual ConsoleColor GetDebugConsoleColor(int value)
        {
            return ConsoleColor.White;
        }

        public virtual string GetDebugMessage(int value)
        {
            return $"value:{value}";
        }
    }
}