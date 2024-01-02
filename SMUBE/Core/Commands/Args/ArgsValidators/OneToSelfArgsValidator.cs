using SMUBE.BattleState;

namespace SMUBE.Commands.Args.ArgsValidators
{
    public class OneToSelfArgsValidator : CommandArgsValidator
    {
        public ArgsConstraint ArgsConstraint => ArgsConstraint.None;

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
