using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands.Args.ArgsPicker;
using SMUBE.Units;

namespace SMUBE.Commands.Args.ArgsValidators
{
    public class OneToEveryArgsValidator : CommandArgsValidator
    {
        private ArgsConstraint _argsConstraint;
        public ArgsConstraint ArgsConstraint => _argsConstraint;

        public OneToEveryArgsValidator(ArgsConstraint argsConstraint)
        {
            _argsConstraint = argsConstraint;
        }
        public ArgsPicker.ArgsPicker GetArgsPicker(ICommand command, BattleStateModel battleStateModel)
        {
            return new OneToEveryArgsPicker(command, battleStateModel);
        }
        
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


            var viableUnits = new List<Unit>();
            switch (ArgsConstraint)
            {
                case ArgsConstraint.None:
                    append_ally_units();
                    append_enemy_units();
                    break;
                case ArgsConstraint.Ally:
                    append_ally_units();
                    break;
                case ArgsConstraint.Enemy:
                    append_enemy_units();
                    break;
                default:
                    break;

                    void append_ally_units()
                    {
                        viableUnits.AddRange(args.BattleStateModel
                            .GetTeamUnits(args.ActiveUnit.UnitIdentifier.TeamId)
                            .Where(unit => unit.UnitData.UnitStats.IsAlive()));
                    }
                    void append_enemy_units()
                    {
                        viableUnits.AddRange(args.BattleStateModel
                            .GetTeamUnits(args.ActiveUnit.UnitIdentifier.TeamId == 0 ? 1 : 0)
                            .Where(unit => unit.UnitData.UnitStats.IsAlive()));
                    }
            }

            if(args.TargetUnits.Count != viableUnits.Count)
            {
                return false;
            }

            foreach(var targetUnit in args.TargetUnits)
            {
                if (!args.BattleStateModel.TryGetUnit(targetUnit.UnitIdentifier, out var _))
                    return false;
            }

            return true;
        }
    }
}
