using Commands;
using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Commands.SpecificCommands._Common
{
    public class OneToOneArgsValidator : CommandArgsValidator
    {
        private ArgsConstraint _argsConstraint;
        public ArgsConstraint ArgsConstraint => _argsConstraint;

        public OneToOneArgsValidator(ArgsConstraint argsConstraint)
        {
            _argsConstraint = argsConstraint;
        }


        public bool Validate(CommandArgs args)
        {
            if (args == null)
            {
                return false;
            }
            
            if (args.battleStateModel == null)
            {
                return false;
            }

            if (args.ActiveUnit == null || args.ActiveUnit.UnitIdentifier == null)
                return false;
            if (!args.battleStateModel.TryGetUnit(args.ActiveUnit.UnitIdentifier, out var _))
                return false;

            if (args.TargetUnits == null || args.TargetUnits.Count != 1 || args.TargetUnits.First().UnitIdentifier == null)
                return false;
            var targetUnitIdentifier = args.TargetUnits.First().UnitIdentifier;
            if (!args.battleStateModel.TryGetUnit(targetUnitIdentifier, out var targetUnit))
                return false;

            if (!targetUnit.UnitData.UnitStats.IsAlive())
            {
                return false;
            }

            if (_argsConstraint != ArgsConstraint.None)
            {
                if(_argsConstraint == ArgsConstraint.Ally)
                {
                    return args.ActiveUnit.UnitIdentifier.TeamId == targetUnitIdentifier.TeamId;
                }
                else if(_argsConstraint == ArgsConstraint.Enemy)
                {
                    return args.ActiveUnit.UnitIdentifier.TeamId != targetUnitIdentifier.TeamId;
                }
            }

            return true;
        }
    }
}
