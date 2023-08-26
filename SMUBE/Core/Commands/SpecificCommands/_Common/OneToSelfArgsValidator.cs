using Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Commands.SpecificCommands._Common
{
    public class OneToSelfArgsValidator : CommandArgsValidator
    {
        public ArgsConstraint ArgsConstraint => ArgsConstraint.None;

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
