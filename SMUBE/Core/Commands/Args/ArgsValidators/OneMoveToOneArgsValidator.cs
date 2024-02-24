using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands.Args.ArgsPicker;

namespace SMUBE.Commands.Args.ArgsValidators
{
    public class OneMoveToOneArgsValidator : CommandArgsValidator
    {
        private ArgsConstraint _argsConstraint;
        public ArgsConstraint ArgsConstraint => _argsConstraint;

        public OneMoveToOneArgsValidator(ArgsConstraint argsConstraint)
        {
            _argsConstraint = argsConstraint;
        }

        public ArgsPicker.ArgsPicker GetArgsPicker(ICommand command, BattleStateModel battleStateModel)
        {
            return new OneMoveToOneArgsPicker(command, battleStateModel);
        }

        public bool Validate(CommandArgs args, BattleStateModel battleStateModel)
        {
            if (args?.BattleStateModel == null)
            {
                return false;
            }

            if (args.ActiveUnit == null || args.ActiveUnit.UnitIdentifier == null)
                return false;
            if (!args.BattleStateModel.TryGetUnit(args.ActiveUnit.UnitIdentifier, out var _))
                return false;

            if (args.TargetUnits == null || args.TargetUnits.Count != 1 || args.TargetUnits.First().UnitIdentifier == null)
                return false;
            var targetUnitIdentifier = args.TargetUnits.First().UnitIdentifier;
            if (!args.BattleStateModel.TryGetUnit(targetUnitIdentifier, out var targetUnit))
                return false;

            if (!targetUnit.UnitData.UnitStats.IsAlive())
            {
                return false;
            }

            if (_argsConstraint != ArgsConstraint.None)
            {
                if (_argsConstraint == ArgsConstraint.Ally)
                {
                    return args.ActiveUnit.UnitIdentifier.TeamId == targetUnitIdentifier.TeamId;
                }
                else if (_argsConstraint == ArgsConstraint.Enemy)
                {
                    return args.ActiveUnit.UnitIdentifier.TeamId != targetUnitIdentifier.TeamId;
                }
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
            if (!reachablePositions.Any(rp => rp.Position.Coordinates.Equals(args.PositionDelta.Target)))
            {
                return false;
            }
            
            return true;
        }
    }
}