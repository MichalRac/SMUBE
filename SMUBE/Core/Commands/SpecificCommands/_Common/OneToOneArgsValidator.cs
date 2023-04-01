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
            if(!args.battleStateModel.TryGetUnit(args.TargetUnits.First().UnitIdentifier, out var _))
                return false;

            return true;
        }
    }
}
