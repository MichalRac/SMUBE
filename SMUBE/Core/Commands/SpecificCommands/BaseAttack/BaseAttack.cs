using System.Collections.Generic;
using SMUBE.BattleState;
using SMUBE.Commands.Effects;
using SMUBE.Commands.SpecificCommands._Common;
using System.Linq;
using SMUBE.Commands.SpecificCommands.Args;

namespace Commands.SpecificCommands.BaseAttack
{
    public class BaseAttack : ICommand
    {
        public CommandId CommandId => CommandId.BaseAttack;

        public int StaminaCost => 0;

        public int ManaCost => 0;

        public CommandArgsValidator CommandArgsValidator => new OneMoveToOneArgsValidator(ArgsConstraint.Enemy);

        private CommandArgs _argsCache;
        public CommandArgs ArgsCache { get => _argsCache; set => _argsCache = value; }

        public bool Execute(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            if (!CommandArgsValidator.Validate(commandArgs, battleStateModel))
            {
                return false;
            }
            
            battleStateModel.TryGetUnit(commandArgs.ActiveUnit.UnitIdentifier, out var activeUnit);
            battleStateModel.TryGetUnit(commandArgs.TargetUnits.First().UnitIdentifier, out var targetUnit);

            if(activeUnit == null || targetUnit == null)
            {
                return false;
            }
            
            activeUnit.UnitData.UnitStats.TryUseAbility(this);
            var startPos = commandArgs.BattleStateModel.BattleSceneState
                .Grid[activeUnit.UnitData.BattleScenePosition.Coordinates.x, activeUnit.UnitData.BattleScenePosition.Coordinates.y];
            startPos.Clear();
            var targetCoords = commandArgs.PositionDelta.Target;
            var targetPos = commandArgs.BattleStateModel.BattleSceneState.Grid[targetCoords.x, targetCoords.y];
            activeUnit.UnitData.BattleScenePosition = targetPos;
            activeUnit.UnitData.BattleScenePosition.ApplyUnit(activeUnit.UnitData.UnitIdentifier);
            targetUnit.UnitData.UnitStats.AffectByAbility(GetCommandResults(commandArgs));

            return true;
            
        }

        public CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults();
            results.performer = commandArgs.ActiveUnit;
            results.targets.Add(commandArgs.TargetUnits.First());
            results.effects.Add(new DamageEffect(commandArgs.ActiveUnit.UnitStats.Power));
            results.PositionDeltas = new List<PositionDelta>() { commandArgs.PositionDelta };
            return results;
        }
    }
}
