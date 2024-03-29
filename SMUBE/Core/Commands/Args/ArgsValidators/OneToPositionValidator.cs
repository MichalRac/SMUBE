using SMUBE.BattleState;
using SMUBE.Commands.Args.ArgsPicker;

namespace SMUBE.Commands.Args.ArgsValidators
{
    public class OneToPositionValidator : BaseCommandArgsValidator
    {
        private readonly bool _allowOccupied;
        private readonly bool _allowSpecial;
        private readonly bool _allowWalkable;
        private readonly bool _allowNonWalkable;
        public override ArgsConstraint ArgsConstraint => ArgsConstraint.Position;

        public OneToPositionValidator(bool allowOccupied, bool allowSpecial, bool allowWalkable, bool allowNonWalkable)
        {
            _allowOccupied = allowOccupied;
            _allowSpecial = allowSpecial;
            _allowWalkable = allowWalkable;
            _allowNonWalkable = allowNonWalkable;
        }
        
        public override ArgsPicker.ArgsPicker GetArgsPicker(ICommand command, BattleStateModel battleStateModel)
        {
            return new OneToPositionArgsPicker(command, battleStateModel, _allowOccupied, _allowSpecial, _allowWalkable, _allowNonWalkable);
        }
        
        public override bool Validate(CommandArgs args, BattleStateModel battleStateModel)
        {
            if (!base.Validate(args, battleStateModel))
            {
                return false;
            }

            if (!ValidateActiveUnit(args, out _))
            {
                return false;
            }

            if (args.TargetUnits != null)
                return false;
            if (args.PositionDelta != null)
                return false;
            if (args.TargetPositions == null)
                return false;
            if (args.TargetPositions.Count != 1)
                return false;

            var targetCoordinates = args.TargetPositions[0];
            var targetPosition = battleStateModel.BattleSceneState.Grid[targetCoordinates.x, targetCoordinates.y];
            
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
    }
}