﻿using SMUBE.BattleState;
using SMUBE.Commands.Effects;
using SMUBE.Commands.SpecificCommands._Common;
using System.Linq;

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

            if (!battleStateModel.BattleSceneState.Pathfinding.CanGetNextTo(battleStateModel.BattleSceneState, activeUnit.UnitData.BattleScenePosition, 
                targetUnit.UnitData.BattleScenePosition, out _, activeUnit.UnitData.UnitStats.Speed))
            {
                return false;
            }

            activeUnit.UnitData.UnitStats.TryUseAbility(this);
            targetUnit.UnitData.UnitStats.AffectByAbility(GetCommandResults(commandArgs));

            return true;
            
        }

        public CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults();
            results.performer = commandArgs.ActiveUnit;
            results.targets.Add(commandArgs.TargetUnits.First());
            results.effects.Add(new DamageEffect(commandArgs.ActiveUnit.UnitStats.Power));
            return results;
        }
    }
}
