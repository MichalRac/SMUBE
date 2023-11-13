using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;
using SMUBE.Pathfinding;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SMUBE.DataStructures.BattleScene
{
    public class InitialGridData
    {
        public int width;
        public int height;

        public List<(SMUBEVector2<int> pos, UnitIdentifier id)> InitialUnitSetup = new List<(SMUBEVector2<int> pos, UnitIdentifier id)>();
        public List<SMUBEVector2<int>> InitialObstacleCells = new List<SMUBEVector2<int>>();
        public List<SMUBEVector2<int>> InitialDefensiveCells = new List<SMUBEVector2<int>>();
        public List<SMUBEVector2<int>> InitialUnstableCells = new List<SMUBEVector2<int>>();
    }

    public class GridBattleScene : BattleSceneBase
    {
        public int Width { get; }
        public int Height { get; }

        private BattleScenePosition[,] _grid;
        public BattleScenePosition[,] Grid => _grid;


        public GridBattleScene(InitialGridData initialGridData)
        {
            Width = initialGridData.width;
            Height = initialGridData.height;

            _grid = new BattleScenePosition[Width, Height];
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Grid[i, j] = new BattleScenePosition(i, j);
                }
            }

            foreach (var unit in initialGridData.InitialUnitSetup)
            {
                TryAddUnit(unit.pos, unit.id);
            }

            foreach (var obstaclePos in initialGridData.InitialObstacleCells)
            {
                TryAddObstacle(obstaclePos);
            }

            foreach (var defensiveCellPos in initialGridData.InitialDefensiveCells)
            {
                TryAddDefensive(defensiveCellPos);
            }

            foreach (var unstableCellPs in initialGridData.InitialUnstableCells)
            {
                TryAddUnstable(unstableCellPs);
            }
        }

        public bool TryAddUnit(SMUBEVector2<int> pos, UnitIdentifier unitIdentifier)
        {
            if(!IsValid(pos))
            {
                return false;
            }

            var gridEntry = _grid[pos.x, pos.y];

            if(!gridEntry.IsEmpty())
            {
                return false;
            }

            gridEntry.ApplyUnit(unitIdentifier);
            return true;
        }

        public bool TryAddObstacle(SMUBEVector2<int> pos)
        {
            if (!IsValid(pos))
            {
                return false;
            }

            var gridEntry = _grid[pos.x, pos.y];

            if (!gridEntry.IsEmpty())
            {
                return false;
            }

            gridEntry.ApplyObstacle();
            return true;
        }

        public bool TryAddDefensive(SMUBEVector2<int> pos)
        {
            if (!IsValid(pos))
            {
                return false;
            }

            _grid[pos.x, pos.y].ApplyDefensive();
            return true;
        }

        public bool TryAddUnstable(SMUBEVector2<int> pos)
        {
            if (!IsValid(pos))
            {
                return false;
            }

            _grid[pos.x, pos.y].ApplyUnstable();
            return true;
        }

        public bool IsValid(SMUBEVector2<int> pos)
        {
            return pos.x >= 0 
                && pos.y >= 0 
                && pos.x < Width 
                && pos.y < Height
                && Grid[pos.x, pos.y] != null;
        }

        public bool IsEmpty(SMUBEVector2<int> pos)
        {
            return _grid[pos.x, pos.y].IsEmpty();
        }

        public bool TryFindUnitPosition(UnitIdentifier unitIdentifier, out BattleScenePosition battleScenePosition)
        {
            battleScenePosition = null;

            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    if (_grid[i,j].UnitIdentifier == unitIdentifier)
                    {
                        battleScenePosition = _grid[i, j];
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
