using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands.SpecificCommands.BaseAttack
{
    public class BaseAttackArgsValidator : CommandArgsValidator
    {
        public bool Validate(CommandArgs args)
        {
            if (args.battleSceneState == null)
            {
                return false;
            }

            if (args.battleSceneState.TryFindUnitPosition(args.ActiveUnit.UnitIdentifier, out var basePos)
                && args.battleSceneState.TryFindUnitPosition(args.UnitTarget.UnitIdentifier, out var targetPos))
            {
                return args.battleSceneState.BattleSceneType.IsReachable(basePos, targetPos, args.battleSceneState);
            }
            return false;
        }
    }
}
