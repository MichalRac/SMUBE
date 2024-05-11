using SMUBE.BattleState;
using System.Collections.Generic;
using SMUBE.Commands._Common;
using SMUBE.Commands.Args;
using SMUBE.Commands.Args.ArgsValidators;
using SMUBE.Commands.Results;
using SMUBE.Pathfinding;

namespace SMUBE.Commands.SpecificCommands.BaseWalk
{
    public class BaseWalk : BaseCommand
    {
        public override int StaminaCost => SpecificCommandCostConfiguration.Stamina_BaseWalk;
        public override int ManaCost => SpecificCommandCostConfiguration.Mana_BaseWalk;        
        public override CommandId CommandId => CommandId.BaseWalk;

        public override BaseCommandArgsValidator CommandArgsValidator => new OneMoveToPositionValidator(false);

        public override bool TryExecute(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            if (!CommandArgsValidator.Validate(commandArgs, battleStateModel))
            {
                return false;
            }

            var activeUnit = commandArgs.ActiveUnit;
            
            var startPos = commandArgs.BattleStateModel.BattleSceneState
                .Grid[activeUnit.BattleScenePosition.Coordinates.x, activeUnit.BattleScenePosition.Coordinates.y];
            startPos.Clear();

            var target = commandArgs.PositionDelta.Target;
            var targetPos = commandArgs.BattleStateModel.BattleSceneState.Grid[target.x, target.y];
            activeUnit.BattleScenePosition = targetPos;
            activeUnit.BattleScenePosition.ApplyUnit(activeUnit.UnitIdentifier);
            
            battleStateModel.BattleSceneState.PathfindingHandler.AggregatedDirtyPositionCache.Add((startPos.Coordinates, true));
            battleStateModel.BattleSceneState.PathfindingHandler.AggregatedDirtyPositionCache.Add((activeUnit.BattleScenePosition.Coordinates, false));

            activeUnit.UnitStats.TryUseAbility(this);
            var commandResults = GetCommandResults(commandArgs);
            activeUnit.UnitStats.AddEffects(commandResults);

            return true;
        }

        public override CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults();

            results.performer = commandArgs.ActiveUnit;
            results.PositionDeltas = new List<PositionDelta> { commandArgs.PositionDelta };

            return results;
        }
    }
}
