using Commands;
using SMUBE.BattleState;

namespace SMUBE.Commands.SpecificCommands._Common
{
    public class OneToPositionValidator : CommandArgsValidator
    {
        public ArgsConstraint ArgsConstraint => ArgsConstraint.Position;

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
