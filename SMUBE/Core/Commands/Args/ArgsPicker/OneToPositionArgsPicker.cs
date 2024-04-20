using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.DataStructures.Utils;

namespace SMUBE.Commands.Args.ArgsPicker
{
    public sealed class OneToPositionArgsPicker : ArgsPicker
    {
        private readonly bool _allowOccupied;
        private readonly bool _allowSpecial;
        private readonly bool _allowWalkable;
        private readonly bool _allowNonWalkable;

        private SMUBEVector2<int> _currentTargetCoordinates;

        public OneToPositionArgsPicker(ICommand command, BattleStateModel battleStateModel, 
            bool allowOccupied, bool allowSpecial, bool allowWalkable, bool allowNonWalkable) 
            : base(command, battleStateModel)
        {
            _allowOccupied = allowOccupied;
            _allowSpecial = allowSpecial;
            _allowWalkable = allowWalkable;
            _allowNonWalkable = allowNonWalkable;
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
            var validTargets = BattleStateModel.BattleSceneState.PathfindingHandler
                .ActiveUnitReachablePositions.Where(target => !target.Position.Coordinates.Equals(BattleStateModel.ActiveUnit.UnitData.BattleScenePosition.Coordinates));
            
            _currentTargetCoordinates = validTargets.First().Position.Coordinates;
        }

        public override bool IsAnyValid()
        {
            return BattleStateModel.BattleSceneState.PathfindingHandler.ActiveUnitReachablePositions.Count > 0;
        }

        public override CommandArgs GetCommandArgs()
        {
            var activeUnit = BattleStateModel.ActiveUnit;
            var targetPositions = new List<SMUBEVector2<int>>() { _currentTargetCoordinates };
            return new CommonArgs(activeUnit.UnitData, null, BattleStateModel, null, targetPositions);
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
            return "Pick any empty position!";
        }

        public override string GetPickerState()
        {
            return IsResolved
                ? "Resolved"
                : "Pick any empty position";
        }

        private bool IsValidTarget(SMUBEVector2<int> targetPos)
        {
            if (targetPos.x < 0 || targetPos.x >= BattleStateModel.BattleSceneState.Width)
                return false;
            if (targetPos.y < 0 || targetPos.y >= BattleStateModel.BattleSceneState.Height)
                return false;

            var targetPosition = BattleStateModel.BattleSceneState.Grid[targetPos.x, targetPos.y];

            
            if (!_allowSpecial && targetPosition.IsSpecial())
            {
                return false;
            }
            
            if (!_allowOccupied && targetPosition.IsOccupied())
            {
                return false;
            }

            if (!_allowWalkable && targetPosition.IsWalkable())
            {
                return false;
            }

            if (!_allowNonWalkable && !targetPosition.IsWalkable())
            {
                return false;
            }

            return true;
        }
        
        public override CommandArgs GetPseudoRandom()
        {
            var unit = BattleStateModel.ActiveUnit;
            var validTargets = BattleStateModel.BattleSceneState.AggregateAllPositions(true);

            if (validTargets.Count == 0)
            {
                return null;
            }
                
            validTargets.Shuffle();
            var targetPos = validTargets.First().Coordinates;
                    
            return new CommonArgs(unit.UnitData, null, BattleStateModel, null, new List<SMUBEVector2<int>>{targetPos});
        }
        
    }
}