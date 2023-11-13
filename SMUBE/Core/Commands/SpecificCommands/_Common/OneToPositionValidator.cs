using Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Commands.SpecificCommands._Common
{
    public class OneToPositionValidator : CommandArgsValidator
    {
        public ArgsConstraint ArgsConstraint => ArgsConstraint.Position;

        public bool Validate(CommandArgs args)
        {
            if (args == null)
            {
                return false;
            }

            if (args.BattleStateModel == null)
            {
                return false;
            }

            if (args.ActiveUnit == null || args.ActiveUnit.UnitIdentifier == null)
                return false;
            if (!args.BattleStateModel.TryGetUnit(args.ActiveUnit.UnitIdentifier, out var _))
                return false;
            if (args.TargetUnits != null)
                return false;

            var activeUnitMoveDeltas = args.PositionDeltas.Where(posDelta => posDelta.UnitIdentifier.Equals(args.ActiveUnit.UnitIdentifier));
            if (activeUnitMoveDeltas.Count() != 1)
            {
                return false;
            }
            var activeUnitMoveDelta = activeUnitMoveDeltas.First();
            if(activeUnitMoveDelta == null)
            {
                return false;
            }

            var battleScene = args.BattleStateModel.BattleSceneState;
            if (!battleScene.IsValid(activeUnitMoveDelta.Target) || !battleScene.IsEmpty(activeUnitMoveDelta.Target))
            {
                return false;
            }

            return true;
        }
    }
}
