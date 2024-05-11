﻿using SMUBE.BattleState;
using SMUBE.Commands.Args;
using SMUBE.Commands.Args.ArgsValidators;
using SMUBE.Commands.Effects;
using SMUBE.Commands.Results;

namespace SMUBE.Commands.SpecificCommands.HealAll
{
    public class HealAll : BaseCommand
    {
        public const float HEAL_POWER_MULTIPIER = 1.5f;
        public override int StaminaCost => SpecificCommandCostConfiguration.Stamina_HealAll;
        public override int ManaCost => SpecificCommandCostConfiguration.Mana_HealAll;
        public override CommandId CommandId => CommandId.HealAll;
        public override BaseCommandArgsValidator CommandArgsValidator => new OneToEveryArgsValidator(ArgsConstraint.Ally);

        public override bool TryExecute(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            if (!CommandArgsValidator.Validate(commandArgs, battleStateModel))
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
                targetUnit.UnitData.UnitStats.AffectByAbility(GetCommandResults(commandArgs));
            }

            activeUnit.UnitData.UnitStats.TryUseAbility(this);
            return true;
        }

        public override CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults();
            results.performer = commandArgs.ActiveUnit;
            results.targets.AddRange(commandArgs.TargetUnits);
            results.effects.Add(new HealEffect((int)(commandArgs.ActiveUnit.UnitStats.Power * HEAL_POWER_MULTIPIER)));
            return results;
        }
    }
}
