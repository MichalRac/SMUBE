using Commands.SpecificCommands._Common;
using SMUBE.BattleState;
using SMUBE.Commands.Effects;
using SMUBE.Commands.SpecificCommands._Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands.SpecificCommands.BaseAttack
{
    public class BaseAttack : ICommand
    {
        public CommandId CommandId => CommandId.BaseAttack;

        public int StaminaCost => 0;

        public int ManaCost => 0;

        public CommandArgsValidator CommandArgsValidator => new OneToOneArgsValidator(ArgsConstraint.Enemy);

        private CommandArgs _argsCache;
        public CommandArgs ArgsCache { get => _argsCache; set => _argsCache = value; }

        public bool Execute(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            if (!CommandArgsValidator.Validate(commandArgs))
            {
                return false;
            }


            battleStateModel.TryGetUnit(commandArgs.ActiveUnit.UnitIdentifier, out var activeUnit);
            battleStateModel.TryGetUnit(commandArgs.TargetUnits.First().UnitIdentifier, out var targetUnit);

            if(activeUnit == null || targetUnit == null)
            {
                return false;
            }

            activeUnit.UnitData.UnitStats.UseAbility(this);
            targetUnit.UnitData.UnitStats.AffectByAbility(GetCommandResults(commandArgs));

            return true;
            
        }

        public CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults();
            results.effects.Add(new DamageEffect(commandArgs.ActiveUnit.UnitStats.Power, commandArgs.TargetUnits[0].UnitIdentifier));
            return results;
        }
    }
}
