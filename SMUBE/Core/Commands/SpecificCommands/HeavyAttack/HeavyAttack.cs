using SMUBE.BattleState;
using SMUBE.Commands.Effects;
using System.Collections.Generic;
using System.Linq;
using SMUBE.Commands._Common;
using SMUBE.Commands.Args;
using SMUBE.Commands.Args.ArgsValidators;
using SMUBE.Commands.Results;

namespace SMUBE.Commands.SpecificCommands.HeavyAttack
{
    public class HeavyAttack : BaseCommand
    {
        public static int UseCounter = 0;
        public const int HEAVY_ATTACK_POWER_MULTIPLIER = 2;

        public override int StaminaCost => SpecificCommandCostConfiguration.Stamina_HeavyAttack;
        public override int ManaCost => SpecificCommandCostConfiguration.Mana_HeavyAttack;
        public override CommandId CommandId => CommandId.HeavyAttack;

        public override BaseCommandArgsValidator CommandArgsValidator => new OneMoveToOneArgsValidator(ArgsConstraint.Enemy);

        public override bool TryExecute(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            if (!CommandArgsValidator.Validate(commandArgs, battleStateModel))
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

            if (activeUnit == null || targetUnit == null)
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

            UseCounter++;
            return true;
        }

        public override CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults { performer = commandArgs.ActiveUnit };
            results.targets.Add(commandArgs.TargetUnits.First());
            results.effects.Add(new DamageEffect(commandArgs.ActiveUnit.UnitStats.Power * HEAVY_ATTACK_POWER_MULTIPLIER));
            results.PositionDeltas = new List<PositionDelta>() { commandArgs.PositionDelta };
            return results;
        }
    }
}
