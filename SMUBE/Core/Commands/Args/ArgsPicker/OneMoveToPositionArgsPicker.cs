using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands._Common;
using SMUBE.DataStructures.Utils;

namespace SMUBE.Commands.Args.ArgsPicker
{
    public sealed class OneMoveToPositionArgsPicker : ArgsPicker
    {
        private bool IsPathless { get; }
        private SMUBEVector2<int> _currentTargetCoordinates;
        public OneMoveToPositionArgsPicker(ICommand command, BattleStateModel battleStateModel, bool isPathless) : base(command, battleStateModel)
        {
            IsPathless = isPathless;
            SetDefaultTarget();
        }
        
        public override void Submit()
        {
            var currentArgs = GetCommandArgs();
            if (Command.CommandArgsValidator.Validate(currentArgs, BattleStateModel))
            {
                ArgsConfirmed?.Invoke(currentArgs);
                IsResolved = true;
            }
            else
            {
                ArgsInvalid?.Invoke();
            }
        }

        public override void Return()
        {
            OperationCancelled?.Invoke();
        }

        protected override void SetDefaultTarget()
        {
            _currentTargetCoordinates = BattleStateModel.BattleSceneState.PathfindingHandler.ActiveUnitReachablePositions.First().Position.Coordinates;
        }

        public override bool IsAnyValid()
        {
            return BattleStateModel.BattleSceneState.PathfindingHandler.ActiveUnitReachablePositions.Count > 0;
        }

        public override CommandArgs GetCommandArgs()
        {
            var activeUnit = BattleStateModel.ActiveUnit;
            var positionDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, 
                activeUnit.UnitData.BattleScenePosition.Coordinates, _currentTargetCoordinates, IsPathless);
            return new CommonArgs(activeUnit.UnitData, null, BattleStateModel, positionDelta);
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
            TryMoveToOffset(xDelta, yDelta);
            ArgsUpdated?.Invoke(GetCommandArgs());
        }
        
        private void TryMoveToOffset(int xDelta, int yDelta)
        {
            var targetPos = new SMUBEVector2<int>(_currentTargetCoordinates.x + xDelta, _currentTargetCoordinates.y + yDelta);
            if (IsValidTarget(targetPos))
            {
                _currentTargetCoordinates = targetPos;
            }
        }

        public override string GetPickerInfo()
        {
            return "Pick any empty reachable position!";
        }

        public override string GetPickerState()
        {
            return IsResolved 
                ? "Resolved"
                : "Pick any empty reachable position";
        }

        private bool IsValidTarget(SMUBEVector2<int> targetPos)
        {
            if (targetPos.x < 0 || targetPos.x >= BattleStateModel.BattleSceneState.Width)
                return false;
            if (targetPos.y < 0 || targetPos.y >= BattleStateModel.BattleSceneState.Height)
                return false;
            
            if (IsPathless)
            {
                return BattleStateModel.BattleSceneState.IsEmpty(targetPos);
            }
            else
            {
                foreach (var reachablePosition in BattleStateModel.BattleSceneState.PathfindingHandler.ActiveUnitReachablePositions)
                {
                    if (reachablePosition.Position.Coordinates.Equals(targetPos))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        public override CommandArgs GetPseudoRandom()
        {
            var unit = BattleStateModel.ActiveUnit;
            var reachablePositions = BattleStateModel.BattleSceneState.PathfindingHandler.ActiveUnitReachablePositions;
                    
            if (reachablePositions.Count == 0)
                return null;
                    
            var target = reachablePositions.GetRandom();
            var positionDelta = new PositionDelta(unit.UnitData.UnitIdentifier, unit.UnitData.BattleScenePosition.Coordinates, target.Position.Coordinates);
            return new CommonArgs(unit.UnitData, null, BattleStateModel, positionDelta);
        }
    }
}