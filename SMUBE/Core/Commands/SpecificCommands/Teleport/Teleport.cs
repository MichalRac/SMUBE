using System.Collections.Generic;
using SMUBE.BattleState;
using SMUBE.BattleState.Heatmap.GeneralHeatmaps;
using SMUBE.Commands._Common;
using SMUBE.Commands.Args;
using SMUBE.Commands.Args.ArgsValidators;
using SMUBE.Commands.Results;
using SMUBE.DataStructures.Utils;

namespace SMUBE.Commands.SpecificCommands.Teleport
{
    public class Teleport : BaseCommand
    {
        public static int UseCounter = 0;
        public override int StaminaCost => SpecificCommandCostConfiguration.Stamina_Teleport;
        public override int ManaCost => SpecificCommandCostConfiguration.Mana_Teleport;
        public override CommandId CommandId => CommandId.Teleport;
        public override BaseCommandArgsValidator CommandArgsValidator => new OneMoveToPositionValidator(true);
        
        public override bool TryExecute(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            if (!base.TryExecute(battleStateModel, commandArgs))
            {
                return false;
            }

            battleStateModel.TryGetUnit(commandArgs.ActiveUnit.UnitIdentifier, out var activeUnit);
            
            var startPos = commandArgs.BattleStateModel.BattleSceneState
                .Grid[activeUnit.UnitData.BattleScenePosition.Coordinates.x, activeUnit.UnitData.BattleScenePosition.Coordinates.y];
            startPos.Clear();

            var target = commandArgs.PositionDelta.Target;
            var targetPos = commandArgs.BattleStateModel.BattleSceneState.Grid[target.x, target.y];
            activeUnit.UnitData.BattleScenePosition = targetPos;
            activeUnit.UnitData.BattleScenePosition.ApplyUnit(activeUnit.UnitData.UnitIdentifier);

            var success = TryUseCommand(commandArgs, activeUnit);
            if (success)
            {
                UseCounter++;
            }
            return success;
        }

        public override CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults();

            results.performer = commandArgs.ActiveUnit;
            results.PositionDeltas = new List<PositionDelta>{ commandArgs.PositionDelta };
            
            return results;
        }

        internal override CommandArgs GetSuggestedPseudoRandomArgs(BattleStateModel battleStateModel)
        {
            var activeUnitStats = battleStateModel.ActiveUnit.UnitData.UnitStats;
            var healthPercentage = (float)activeUnitStats.CurrentHealth / activeUnitStats.MaxHealth;

            const float aggressionThreshold = 0.5f;

            var targetPos = new SMUBEVector2<int>(-1, -1);
            var opponentTeamId = battleStateModel.ActiveUnit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;
            var distanceToUnitOfTeamIdHeatmap = new DistanceToUnitOfTeamIdHeatmap(opponentTeamId, false, battleStateModel);
            distanceToUnitOfTeamIdHeatmap.ProcessHeatmap(battleStateModel);
            
            if (healthPercentage > aggressionThreshold)
            {
                var maxScore = distanceToUnitOfTeamIdHeatmap.GetMinScoreCoordinates();
                targetPos.x = maxScore.x;
                targetPos.y = maxScore.y;
            }
            else
            {
                var minScore = distanceToUnitOfTeamIdHeatmap.GetMaxScoreCoordinates();
                targetPos.x = minScore.x;
                targetPos.y = minScore.y;
            }

            var positionDelta = new PositionDelta(
                battleStateModel.ActiveUnit.UnitData.UnitIdentifier, 
                battleStateModel.ActiveUnit.UnitData.BattleScenePosition.Coordinates, targetPos);
            
            return new CommonArgs(battleStateModel.ActiveUnit.UnitData, null, 
                battleStateModel, positionDelta, new List<SMUBEVector2<int>> {targetPos});
        }
    }
}