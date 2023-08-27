using Commands;
using SMUBE.BattleState;
using SMUBE.Commands.Effects;
using SMUBE.Commands.SpecificCommands._Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Commands.SpecificCommands.HealAll
{
    public class HealAll : ICommand
    {
        public int StaminaCost => 0;

        public int ManaCost => 25;

        public CommandId CommandId => CommandId.HealAll;
        public CommandArgsValidator CommandArgsValidator => new OneToEveryArgsValidator(ArgsConstraint.Ally);


        private CommandArgs _argsCache;
        public CommandArgs ArgsCache { get => _argsCache; set => _argsCache = value; }


        public bool Execute(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            if (!CommandArgsValidator.Validate(commandArgs))
            {
                return false;
            }

            battleStateModel.TryGetUnit(commandArgs.ActiveUnit.UnitIdentifier, out var activeUnit);

            if (activeUnit == null)
            {
                return false;
            }


            var success = commandArgs.ActiveUnit.UnitStats.CanUseAbility(this);
            if (!success)
            {
                return false;
            }

            foreach (var targetUnitData in commandArgs.TargetUnits)
            {
                battleStateModel.TryGetUnit(targetUnitData.UnitIdentifier, out var targetUnit);

                if (targetUnit == null)
                {
                    return false;
                }

                targetUnit.UnitData.UnitStats.AddEffects(GetCommandResults(commandArgs));
            }

            activeUnit.UnitData.UnitStats.TryUseAbility(this);
            return true;
        }

        public CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults();
            results.performer = commandArgs.ActiveUnit;
            results.targets.AddRange(commandArgs.TargetUnits);
            results.effects.Add(new HealEffect(commandArgs.ActiveUnit.UnitStats.Power));
            return results;
        }
    }
}
