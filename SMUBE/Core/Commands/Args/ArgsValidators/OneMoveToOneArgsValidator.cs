using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands.Args.ArgsPicker;

namespace SMUBE.Commands.Args.ArgsValidators
{
    public class OneMoveToOneArgsValidator : BaseCommandArgsValidator
    {
        private ArgsConstraint _argsConstraint;
        public override ArgsConstraint ArgsConstraint => _argsConstraint;

        public OneMoveToOneArgsValidator(ArgsConstraint argsConstraint)
        {
            _argsConstraint = argsConstraint;
        }

        public override ArgsPicker.ArgsPicker GetArgsPicker(ICommand command, BattleStateModel battleStateModel)
        {
            return new OneMoveToOneArgsPicker(command, battleStateModel);
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
            
            if (args.TargetUnits == null || args.TargetUnits.Count != 1 || args.TargetUnits.First().UnitIdentifier == null)
                return false;
            var targetUnitIdentifier = args.TargetUnits.First().UnitIdentifier;
            if (!args.BattleStateModel.TryGetUnit(targetUnitIdentifier, out var targetUnit))
                return false;

            if (!targetUnit.UnitData.UnitStats.IsAlive())
            {
                return false;
            }
            
            if (!ValidateTargetConstraint(_argsConstraint, args.ActiveUnit.UnitIdentifier, targetUnitIdentifier))
            {
                return false;
            }

            if (args.PositionDelta == null)
            {
                return false;
            }
            if (!args.PositionDelta.UnitIdentifier.Equals(args.ActiveUnit.UnitIdentifier))
            {
                return false;
            }
            
            var reachablePositions = battleStateModel.BattleSceneState.PathfindingHandler.ActiveUnitReachablePositions;
            if (!reachablePositions.Any(rp => rp.TargetPosition.Coordinates.Equals(args.PositionDelta.Target)))
            {
                return false;
                /*
                if (!_allowUnreachable)
                {
                    return false;
                }
                var targetScenePosition = battleStateModel.BattleSceneState.Grid[args.PositionDelta.Target.x, args.PositionDelta.Target.y];
                var pathExists = battleStateModel.BattleSceneState.PathfindingHandler
                    .PathExists(battleStateModel, args.ActiveUnit.BattleScenePosition, targetScenePosition, out _);
                if (!pathExists)
                {
                    return false;
                }
            */
            }
            
            return true;
        }
    }
}