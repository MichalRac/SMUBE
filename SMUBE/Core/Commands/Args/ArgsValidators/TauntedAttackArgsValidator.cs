using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands.Args.ArgsPicker;

namespace SMUBE.Commands.Args.ArgsValidators
{
    public class TauntedAttackArgsValidator : BaseCommandArgsValidator
    {
        public override ArgsConstraint ArgsConstraint => ArgsConstraint.OtherUnit;
        public override ArgsPicker.ArgsPicker GetArgsPicker(ICommand command, BattleStateModel battleStateModel) 
            => new TauntedArgsPicker(command, battleStateModel);

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
            
            if (!ValidateTargetConstraint(ArgsConstraint, args.ActiveUnit.UnitIdentifier, targetUnitIdentifier))
            {
                return false;
            }
            
            return true;
        }
    }
}