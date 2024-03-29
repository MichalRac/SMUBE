using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands.Args.ArgsPicker;

namespace SMUBE.Commands.Args.ArgsValidators
{
    public class OneToOneArgsValidator : BaseCommandArgsValidator
    {
        private ArgsConstraint _argsConstraint;
        public override ArgsConstraint ArgsConstraint => _argsConstraint;

        public OneToOneArgsValidator(ArgsConstraint argsConstraint)
        {
            _argsConstraint = argsConstraint;
        }

        public override ArgsPicker.ArgsPicker GetArgsPicker(ICommand command, BattleStateModel battleStateModel)
        {
            return new OneToOneArgsPicker(command, battleStateModel);
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

            return true;
        }
    }
}
