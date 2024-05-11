using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands._Common;
using SMUBE.Commands.Args;
using SMUBE.Commands.Args.ArgsValidators;
using SMUBE.Commands.Effects;
using SMUBE.Commands.Results;
using SMUBE.Pathfinding;

namespace SMUBE.Commands.SpecificCommands.BaseAttack
{
    public class BaseAttack : BaseCommand
    {
        public override CommandId CommandId => CommandId.BaseAttack;

        public override int StaminaCost => SpecificCommandCostConfiguration.Stamina_BaseAttack;

        public override int ManaCost => SpecificCommandCostConfiguration.Mana_BaseAttack;

        public override BaseCommandArgsValidator CommandArgsValidator => new OneMoveToOneArgsValidator(ArgsConstraint.Enemy);

        public override bool TryExecute(BattleStateModel battleStateModel, CommandArgs commandArgs)
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
            
            battleStateModel.BattleSceneState.PathfindingHandler.AggregatedDirtyPositionCache.Add((startPos.Coordinates, true));
            battleStateModel.BattleSceneState.PathfindingHandler.AggregatedDirtyPositionCache.Add((targetCoords, false));

            return true;
            
        }

        public override CommandResults GetCommandResults(CommandArgs commandArgs)
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
