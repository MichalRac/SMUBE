using System;
using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands._Common;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;

namespace SMUBE.Commands.Args.ArgsPicker
{
    public sealed class OneToOneArgsPicker : ArgsPicker
    {
        private List<SMUBEVector2<int>> ValidTargets = new List<SMUBEVector2<int>>();
        private int _currentTargetIndex = 0;
        
        public OneToOneArgsPicker(ICommand command, BattleStateModel battleStateModel) : base(command, battleStateModel)
        {
            SetDefaultTarget();
        }

        public override void Submit()
        {
            var args = GetCommandArgs();
            IsResolved = true;
            ArgsConfirmed?.Invoke(args);
        }

        public override void Return()
        {
            ValidTargets.Clear();
            OperationCancelled?.Invoke();
        }

        protected override void SetDefaultTarget()
        {
            ValidTargets.Clear();

            foreach (var potentialTargetUnit in BattleStateModel.Units)
            {
                if (ContainsValidTarget(potentialTargetUnit.UnitData.BattleScenePosition.Coordinates))
                {
                    ValidTargets.Add(potentialTargetUnit.UnitData.BattleScenePosition.Coordinates);
                }
            }
        }

        public override bool IsAnyValid()
        {
            foreach (var potentialTargetUnit in BattleStateModel.Units)
            {
                if (ContainsValidTarget(potentialTargetUnit.UnitData.BattleScenePosition.Coordinates))
                {
                    return true;
                }
            }
            return false;
        }

        public override CommandArgs GetCommandArgs()
        {
            var activeUnit = BattleStateModel.ActiveUnit;

            var targetUnitData = new List<UnitData>();
            var currentTargetCoordinates = ValidTargets[_currentTargetIndex];
            if (BattleStateModel.BattleSceneState.Grid[currentTargetCoordinates.x, currentTargetCoordinates.y].UnitIdentifier != null)
            {
                var targetUnitId = BattleStateModel.BattleSceneState.Grid[currentTargetCoordinates.x, currentTargetCoordinates.y].UnitIdentifier;
                targetUnitData.Add(BattleStateModel.Units.Find(u => u.UnitData.UnitIdentifier.Equals(targetUnitId)).UnitData);
            }
            
            return new CommonArgs(activeUnit.UnitData, targetUnitData, BattleStateModel);
        }

        public override void Up()
        {
            HandlePickerInput(0, 1);
        }

        public override void Down()
        {
            HandlePickerInput(0, -1);
        }

        public override void Left()
        {
            HandlePickerInput(-1, 0);
        }

        public override void Right()
        {
            HandlePickerInput(1, 0);
        }
        
        private void HandlePickerInput(int xDelta, int yDelta)
        {
            if (xDelta == 1 || yDelta == 1)
            {
                _currentTargetIndex++;

                if (_currentTargetIndex >= ValidTargets.Count)
                {
                    _currentTargetIndex = 0;
                }
            }
            else
            {
                _currentTargetIndex--;

                if (_currentTargetIndex < 0)
                {
                    _currentTargetIndex = ValidTargets.Count -1;
                }
            }
            ArgsUpdated?.Invoke(GetCommandArgs());
        }

        public override string GetPickerInfo()
        {
            return "Choose target unit!";
        }

        public override string GetPickerState()
        {
            return "Choose target unit!";
        }

        private bool ContainsValidTarget(SMUBEVector2<int> targetPos)
        {
            if (BattleStateModel.BattleSceneState.Grid[targetPos.x, targetPos.y].UnitIdentifier == null)
            {
                return false;
            }
            
            var activeUnitTeamId = BattleStateModel.ActiveUnit.UnitData.UnitIdentifier.TeamId;
            var targetTeamId = BattleStateModel.BattleSceneState.Grid[targetPos.x, targetPos.y].UnitIdentifier.TeamId;
            
            if (Command.CommandArgsValidator.ArgsConstraint == ArgsConstraint.Ally)
            {
                return activeUnitTeamId == targetTeamId;
            }
            if (Command.CommandArgsValidator.ArgsConstraint == ArgsConstraint.Enemy)
            {
                return activeUnitTeamId != targetTeamId;
            }
            
            throw new ArgumentOutOfRangeException();
        }
        
        public override CommandArgs GetPseudoRandom()
        {
            var unit = BattleStateModel.ActiveUnit;
            
            // todo hardcoded 2 teams limit
            var enemyTeamId = unit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;
            var enemyTeamUnits = BattleStateModel.GetTeamUnits(enemyTeamId).Where(u => u.UnitData.UnitStats.IsAlive()).ToList();
            if (enemyTeamUnits.Any())
            {
                var targetUnitData = enemyTeamUnits.ElementAt(new Random().Next(enemyTeamUnits.Count() - 1)).UnitData;
                return new CommonArgs(unit.UnitData, new List<UnitData>() { targetUnitData }, BattleStateModel);
            }

            return null;
        }
    }
}