﻿using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands._Common;
using SMUBE.Commands.Args;
using SMUBE.Commands.Args.ArgsValidators;
using SMUBE.Commands.Effects;
using SMUBE.Commands.Results;
using SMUBE.DataStructures.Utils;

namespace SMUBE.Commands.SpecificCommands.Tackle
{
    public class Tackle : BaseCommand
    {
        public static int UseCounter = 0;
        public const float TACKLE_ATTACK_POWER_MULTIPLIER = 1.5f;
        public override int StaminaCost => SpecificCommandCostConfiguration.Stamina_Tackle;
        public override int ManaCost => SpecificCommandCostConfiguration.Mana_Tackle;
        public override CommandId CommandId => CommandId.Tackle;
        public override BaseCommandArgsValidator CommandArgsValidator => new OneMoveToOneArgsValidator(ArgsConstraint.OtherUnit);

        public override bool TryExecute(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            if (!base.TryExecute(battleStateModel, commandArgs))
            {
                return false;
            }

            var success = commandArgs.ActiveUnit.UnitStats.CanUseAbility(this);
            if (!success)
            {
                return false;
            }

            battleStateModel.TryGetUnit(commandArgs.ActiveUnit.UnitIdentifier, out var activeUnit);
            battleStateModel.TryGetUnit(commandArgs.TargetUnits.First().UnitIdentifier, out var targetUnit);
            
            TryUseCommand(commandArgs, activeUnit);

            var commandResults = GetCommandResults(commandArgs);
            
            var startPos = commandArgs.BattleStateModel.BattleSceneState
                .Grid[activeUnit.UnitData.BattleScenePosition.Coordinates.x, activeUnit.UnitData.BattleScenePosition.Coordinates.y];
            startPos.Clear();
            var targetCoords = commandArgs.PositionDelta.Target;
            var targetPos = commandArgs.BattleStateModel.BattleSceneState.Grid[targetCoords.x, targetCoords.y];
            activeUnit.UnitData.BattleScenePosition = targetPos;
            activeUnit.UnitData.BattleScenePosition.ApplyUnit(activeUnit.UnitData.UnitIdentifier);
            if (commandResults.PositionDeltas.Count == 2)
            {
                targetUnit.UnitData.BattleScenePosition.Clear();
                var targetMoveCoords = commandResults.PositionDeltas[1].Target;
                var targetMovePosition = commandArgs.BattleStateModel.BattleSceneState.Grid[targetMoveCoords.x, targetMoveCoords.y];
                targetUnit.UnitData.BattleScenePosition = targetMovePosition;
                targetUnit.UnitData.BattleScenePosition.ApplyUnit(targetUnit.UnitData.UnitIdentifier);;
            }
            targetUnit.UnitData.UnitStats.AffectByAbility(GetCommandResults(commandArgs));
            
            UseCounter++;
            return true;
        }

        public override CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults { performer = commandArgs.ActiveUnit };

            var target = commandArgs.TargetUnits.First();
            results.targets.Add(target);

            if (commandArgs.ActiveUnit.UnitIdentifier.TeamId != target.UnitIdentifier.TeamId)
            {
                results.effects.Add(new DamageEffect((int)(commandArgs.ActiveUnit.UnitStats.Power * TACKLE_ATTACK_POWER_MULTIPLIER)));
            }

            var positionDeltas = new List<PositionDelta> { commandArgs.PositionDelta };

            var activeUnitPos = commandArgs.PositionDelta.Target;
            var targetUnitPos = target.BattleScenePosition.Coordinates;
            var positionDiff = new SMUBEVector2<int>(targetUnitPos.x - activeUnitPos.x, targetUnitPos.y - activeUnitPos.y);
            var potentialTargetMoveCoordinates = new SMUBEVector2<int>(targetUnitPos.x + positionDiff.x, targetUnitPos.y + positionDiff.y);
            var canMove = commandArgs.BattleStateModel.BattleSceneState.IsValid(potentialTargetMoveCoordinates)
                && commandArgs.BattleStateModel.BattleSceneState.IsEmpty(potentialTargetMoveCoordinates);
            if (canMove)
            {
                positionDeltas.Add(new PositionDelta(target.UnitIdentifier, targetUnitPos, potentialTargetMoveCoordinates));
            }
            
            results.PositionDeltas = positionDeltas;
            return results;
        }
    }
}