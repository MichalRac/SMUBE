using SMUBE.BattleState;
using SMUBE.Commands.Args.ArgsPicker;

namespace SMUBE.Commands.Args.ArgsValidators
{
    public class OneMoveToPositionValidator : BaseCommandArgsValidator
    {
        public override ArgsConstraint ArgsConstraint => ArgsConstraint.Position;

        private bool Pathless { get; }
        
        public OneMoveToPositionValidator(bool pathless)
        {
            Pathless = pathless;
        }
        
        public override ArgsPicker.ArgsPicker GetArgsPicker(ICommand command, BattleStateModel battleStateModel)
        {
            return new OneMoveToPositionArgsPicker(command, battleStateModel, Pathless);
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
            if (args.PositionDelta == null)
                return false;
            if (!args.PositionDelta.UnitIdentifier.Equals(args.ActiveUnit.UnitIdentifier))
                return false;
            
            var battleScene = args.BattleStateModel.BattleSceneState;
            if (!battleScene.IsValid(args.PositionDelta.Target) || !battleScene.IsEmpty(args.PositionDelta.Target))
            {
                return false;
            }

            return true;
        }
    }
}
