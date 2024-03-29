using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands._Common;
using SMUBE.Commands.Args;
using SMUBE.Commands.Args.ArgsValidators;
using SMUBE.Commands.Effects;
using SMUBE.Commands.Results;
using SMUBE.DataStructures.BattleScene;

namespace SMUBE.Commands.SpecificCommands.Taunt
{
    public class TauntedAttack : BaseCommand
    {
        public override int StaminaCost => SpecificCommandCostConfiguration.Stamina_TauntedAttack;
        public override int ManaCost => SpecificCommandCostConfiguration.Mana_TauntedAttack;
        public override CommandId CommandId => CommandId.TauntedAttack;
        public override BaseCommandArgsValidator CommandArgsValidator => new TauntedAttackArgsValidator();

        public override bool TryExecute(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            if (!base.TryExecute(battleStateModel, commandArgs))
            {
                return false;
            }
            
            battleStateModel.TryGetUnit(commandArgs.ActiveUnit.UnitIdentifier, out var activeUnit);
            battleStateModel.TryGetUnit(commandArgs.TargetUnits.First().UnitIdentifier, out var targetUnit);

            var tauntEffect = activeUnit.UnitData.UnitStats.PersistentEffects.Find(e => e is TauntEffect) as TauntEffect;
            if (tauntEffect == null || !targetUnit.UnitData.UnitIdentifier.Equals(tauntEffect.Target))
            {
                return false;
            }
            
            var targetPosition = targetUnit.UnitData.BattleScenePosition;
            var surroundingPositions = battleStateModel.BattleSceneState.PathfindingHandler.GetSurroundingPositions(battleStateModel, targetPosition);

            bool actionReachesTarget = ActionReachesTarget(battleStateModel, commandArgs, out var optimalMove);
            
            if (!actionReachesTarget)
            {
                bool anyPathFound = false;
                var smallestPathCost = int.MaxValue;
                foreach (var surroundingPosition in surroundingPositions)
                {
                    if (battleStateModel.BattleSceneState.PathfindingHandler.GetLastReachableOnPath(battleStateModel, surroundingPosition, out var lastReachable, out int pathCost))
                    {
                        if (pathCost < smallestPathCost)
                        {
                            smallestPathCost = pathCost;
                            var positionDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, activeUnit.UnitData.BattleScenePosition.Coordinates, lastReachable.Coordinates);
                            commandArgs.PositionDelta = positionDelta;
                            anyPathFound = true;
                        }
                    }
                }
                
                if(!anyPathFound)
                {
                    // no valid path to target
                    return false;
                }
            }
            else if(optimalMove != null)
            {
                var positionDelta = new PositionDelta(activeUnit.UnitData.UnitIdentifier, activeUnit.UnitData.BattleScenePosition.Coordinates, optimalMove.Coordinates);
                commandArgs.PositionDelta = positionDelta;
            }

            var success = actionReachesTarget 
                ? TryUseCommand(commandArgs, activeUnit, targetUnit) 
                : TryUseCommand(commandArgs, activeUnit);

            if (!success)
            {
                return false;
            }

            if (commandArgs.PositionDelta != null)
            {
                MoveUnitToDelta(commandArgs, activeUnit);
            }

            return true;
        }


        public override CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var commandResults = new CommandResults();
            
            commandResults.performer = commandArgs.ActiveUnit;

            var battleStateModel = commandArgs.BattleStateModel;
            var actionReachesTarget = ActionReachesTarget(battleStateModel, commandArgs, out  _);
            if(actionReachesTarget)
            {
                commandResults.targets.Add(commandArgs.TargetUnits.First());
                commandResults.effects.Add(new DamageEffect(commandArgs.ActiveUnit.UnitStats.Power));
            }
            
            
            commandResults.PositionDeltas = new List<PositionDelta>() { commandArgs.PositionDelta };
            return commandResults;
        }
        
        private static bool ActionReachesTarget(BattleStateModel battleStateModel, CommandArgs commandArgs, out BattleScenePosition optimalMovePosition)
        {
            optimalMovePosition = null;
            var targetBattleScenePosition = commandArgs.TargetUnits[0].BattleScenePosition;
            var targetSurroundingPositions = battleStateModel.BattleSceneState.PathfindingHandler
                .GetSurroundingPositions(battleStateModel, targetBattleScenePosition);

            var alreadyNextTo =
                battleStateModel.BattleSceneState.PathfindingHandler.IsNextTo(battleStateModel, commandArgs.ActiveUnit.BattleScenePosition, targetBattleScenePosition);

            if (alreadyNextTo)
            {
                return true;
            }
            
            var allReachablePos = battleStateModel.BattleSceneState.PathfindingHandler.ActiveUnitReachablePositions;
            var validTargets =
                allReachablePos.Where(reachablePos =>
                    targetSurroundingPositions.Any(surroundingPos => surroundingPos.Coordinates.Equals(reachablePos.Position.Coordinates)))
                    .ToList();

            if (!validTargets.Any())
            {
                return false;
            }

            var minDistance = int.MaxValue;
            foreach (var validTarget in validTargets)
            {
                if (validTarget.ShortestDistance < minDistance)
                {
                    minDistance = validTarget.ShortestDistance;
                    optimalMovePosition = validTarget.Position;
                }
            }
            
            return true;
        }
    }
}