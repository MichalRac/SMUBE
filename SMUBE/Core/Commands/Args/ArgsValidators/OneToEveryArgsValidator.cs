using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands.Args.ArgsPicker;
using SMUBE.Units;

namespace SMUBE.Commands.Args.ArgsValidators
{
    public class OneToEveryArgsValidator : BaseCommandArgsValidator
    {
        private ArgsConstraint _argsConstraint;
        public override  ArgsConstraint ArgsConstraint => _argsConstraint;

        public OneToEveryArgsValidator(ArgsConstraint argsConstraint)
        {
            _argsConstraint = argsConstraint;
        }
        public override  ArgsPicker.ArgsPicker GetArgsPicker(ICommand command, BattleStateModel battleStateModel)
        {
            return new OneToEveryArgsPicker(command, battleStateModel);
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
