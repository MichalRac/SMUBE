using System;
using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands._Common;
using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;

namespace SMUBE.Commands.Args.ArgsPicker
{
    public sealed class OneMoveToOneArgsPicker : ArgsPicker
    {
        enum Stage
        {
            ChooseTargetUnit,
            ChooseMoveSpot,
        }

        private Stage _currentStage = Stage.ChooseTargetUnit;
        private UnitData _targetUnitData;
   
        private List<SMUBEVector2<int>> ValidTargets = new List<SMUBEVector2<int>>();
        private int _currentTargetIndex = 0;

        private List<SMUBEVector2<int>> ValidMoveTargets = new List<SMUBEVector2<int>>();
        private int _currentMoveIndex = 0;

        public OneMoveToOneArgsPicker(ICommand command, BattleStateModel battleStateModel)
            : base(command, battleStateModel)
        {
            SetDefaultTarget();
        }

        protected override void SetDefaultTarget()
        {
            ValidTargets.Clear();
            ValidMoveTargets.Clear();

            foreach (var potentialTargetUnit in BattleStateModel.Units)
            {
                if (IsStageOneValid(potentialTargetUnit.UnitData.BattleScenePosition.Coordinates))
                {
                    ValidTargets.Add(potentialTargetUnit.UnitData.BattleScenePosition.Coordinates);
                    _targetUnitData = potentialTargetUnit.UnitData;
                    ValidMoveTargets = GetReachableSurroundingPositions(_targetUnitData.BattleScenePosition.Coordinates)
                        .Select(p => p.Coordinates).ToList();
                }
            }
        }

        public override bool IsAnyValid()
        {
            foreach (var potentialTargetUnit in BattleStateModel.Units)
            {
                if (IsStageOneValid(potentialTargetUnit.UnitData.BattleScenePosition.Coordinates))
                {
                    return true;
                }
            }
            return false;
        }

        public override void Submit()
        {
            if (_currentStage == Stage.ChooseTargetUnit)
            {
                ValidMoveTargets.Clear();
                ValidMoveTargets = GetReachableSurroundingPositions(ValidTargets[_currentTargetIndex])
                    .Select(p => p.Coordinates).ToList();

                if (ValidMoveTargets.Count > 0)
                {
                    _currentMoveIndex = 0;
                    _currentStage = Stage.ChooseMoveSpot;
                }
                else
                {
                    ArgsInvalid?.Invoke();
                }
            }
            else if (_currentStage == Stage.ChooseMoveSpot)
            {
                var currentCommandArgs = GetCommandArgs();
                
                if (Command.CommandArgsValidator.Validate(currentCommandArgs, BattleStateModel))
                {
                    ArgsConfirmed?.Invoke(currentCommandArgs);
                    IsResolved = true;
                }
                else
                {
                    ArgsInvalid?.Invoke();
                }
            }
            else
            {
                ArgsInvalid?.Invoke();
            }
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

            PositionDelta posDelta = null;
            if (_currentStage == Stage.ChooseMoveSpot)
            {
                posDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, 
                    activeUnit.UnitData.BattleScenePosition.Coordinates,ValidMoveTargets[_currentMoveIndex]);
            }
            
            return new CommonArgs(activeUnit.UnitData, targetUnitData, BattleStateModel, posDelta);
        }

        public override void Return()
        {
            if (_currentStage == Stage.ChooseMoveSpot)
            {
                OperationCancelled?.Invoke();
            }
            else if (_currentStage == Stage.ChooseTargetUnit)
            {
                ValidMoveTargets.Clear();
                _currentStage = Stage.ChooseTargetUnit;
            }
            else
            {
                throw new ArgumentException();
            }
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
            switch (_currentStage)
            {
                case Stage.ChooseTargetUnit:
                    TryMoveToOffset(xDelta, yDelta);
                    break;
                case Stage.ChooseMoveSpot:
                    if (xDelta == 1 || yDelta == 1)
                    {
                        _currentMoveIndex++;

                        if (_currentMoveIndex >= ValidMoveTargets.Count)
                        {
                            _currentMoveIndex = 0;
                        }
                    }
                    else
                    {
                        _currentMoveIndex--;

                        if (_currentMoveIndex < 0)
                        {
                            _currentMoveIndex = ValidMoveTargets.Count -1;
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            ArgsUpdated?.Invoke(GetCommandArgs());
        }

        private void TryMoveToOffset(int xDelta, int yDelta)
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
        }

        private bool IsStageOneValid(SMUBEVector2<int> targetPos)
        {
            if (!ContainsValidTarget(targetPos))
            {
                return false;
            }
            
            if (!IsTargetReachable(targetPos))
            {
                return false;
            }
            
            return true;
        }

        private bool ContainsValidTarget(SMUBEVector2<int> targetPos)
        {
            if (BattleStateModel.BattleSceneState.Grid[targetPos.x, targetPos.y].UnitIdentifier == null)
            {
                return false;
            }
            
            if (Command.CommandArgsValidator.ArgsConstraint == ArgsConstraint.None)
            {
                return true;
            }
            if (Command.CommandArgsValidator.ArgsConstraint == ArgsConstraint.OtherUnit)
            {
                var sameId = BattleStateModel.ActiveUnit.UnitData.UnitIdentifier.Equals(BattleStateModel.BattleSceneState.Grid[targetPos.x, targetPos.y].UnitIdentifier);
                return !sameId;
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
        
        private bool IsTargetReachable(SMUBEVector2<int> targetPos)
        {
            var reachableSurroundingPositions = GetReachableSurroundingPositions(targetPos);
            return reachableSurroundingPositions.Count > 0;
        }

        private List<BattleScenePosition> GetReachableSurroundingPositions(SMUBEVector2<int> targetPos)
        {
            var targetBattleScenePosition = BattleStateModel.BattleSceneState.Grid[targetPos.x, targetPos.y];
            var reachableSurroundingPositions = BattleStateModel.BattleSceneState
                .PathfindingHandler.GetReachableSurroundingPositions(BattleStateModel, targetBattleScenePosition);
            return reachableSurroundingPositions;
        }
        
        public override string GetPickerInfo()
        {
            string argString;

            if (Command.CommandArgsValidator.ArgsConstraint == ArgsConstraint.Ally)
                argString = "ally";
            else if (Command.CommandArgsValidator.ArgsConstraint == ArgsConstraint.Enemy)
                argString = "enemy";
            else
                throw new ArgumentOutOfRangeException();
            
            switch (_currentStage)
            {
                case Stage.ChooseTargetUnit:
                    return $"Pick a reachable {argString} unit!";
                case Stage.ChooseMoveSpot:
                    return "Pick a reachable position next to target unit!";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override string GetPickerState()
        {
            switch (_currentStage)
            {
                case Stage.ChooseTargetUnit:
                    return "Valid Target! Confirm to select it!";
                case Stage.ChooseMoveSpot:
                    return "Confirm to select target attack position, or change to another one if available!";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}