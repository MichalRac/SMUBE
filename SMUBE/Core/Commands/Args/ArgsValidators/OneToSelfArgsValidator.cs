using SMUBE.BattleState;
using SMUBE.Commands.Args.ArgsPicker;

namespace SMUBE.Commands.Args.ArgsValidators
{
    public class OneToSelfArgsValidator : BaseCommandArgsValidator
    {
        public override ArgsConstraint ArgsConstraint => ArgsConstraint.None;

        public override ArgsPicker.ArgsPicker GetArgsPicker(ICommand command, BattleStateModel battleStateModel)
        {
            return new OneToSelfArgsPicker(command, battleStateModel);
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
            {
                if (args.TargetUnits.Count != 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
