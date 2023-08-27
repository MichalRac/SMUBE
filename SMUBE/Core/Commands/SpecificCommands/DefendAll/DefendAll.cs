using Commands;
using SMUBE.BattleState;
using SMUBE.Commands.Effects;
using SMUBE.Commands.SpecificCommands._Common;
using System;

namespace SMUBE.Commands.SpecificCommands.DefendAll
{
    public class DefendAll : ICommand
    {
        private const int DEFEND_ALL_PERSISTANCE = 2;

        public int StaminaCost => 25;

        public int ManaCost => 0;

        public CommandId CommandId => CommandId.DefendAll;
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
            if(!success)
            { 
                return false;
            }

            foreach(var targetUnitData in commandArgs.TargetUnits)
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
            results.targets.AddRange(commandArgs.TargetUnits);
            results.effects.Add(new BlockEffect(DEFEND_ALL_PERSISTANCE));
            return results;
        }
    }
}
