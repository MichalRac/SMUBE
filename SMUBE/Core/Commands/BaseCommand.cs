using SMUBE.BattleState;
using SMUBE.Commands.Args;
using SMUBE.Commands.Results;
using SMUBE.Pathfinding;
using SMUBE.Units;

namespace SMUBE.Commands
{
    public abstract class BaseCommand : ICommand
    {
        public abstract int StaminaCost { get; }
        public abstract int ManaCost { get; }
        public virtual CommandArgs ArgsCache { get; set; }

        public abstract CommandId CommandId { get; }
        public abstract BaseCommandArgsValidator CommandArgsValidator { get; }

        public virtual ArgsPreferences ArgsPreferences { get; private set; } = Args.ArgsPreferences.Default();

        public BaseCommand WithPreferences(ArgsPreferences argsPreferences)
        {
            ArgsPreferences = argsPreferences;
            return this;
        }
        
        public BaseCommand WithPreferences(ArgsEnemyTargetingPreference enemyTargetingPreference)
        {
            ArgsPreferences = new ArgsPreferences(enemyTargetingPreference);
            return this;
        }
        public BaseCommand WithPreferences(ArgsMovementTargetingPreference movementPreference)
        {
            ArgsPreferences = new ArgsPreferences(movementPreference);
            return this;
        }
        public BaseCommand WithPreferences(ArgsPositionTargetingPreference positionPreference)
        {
            ArgsPreferences = new ArgsPreferences(positionPreference);
            return this;
        }

        public virtual bool TryExecute(BattleStateModel battleStateModel, CommandArgs commandArgs)
        {
            if (!CommandArgsValidator.Validate(commandArgs, battleStateModel))
            {
                return false;
            }

            if (!commandArgs.ActiveUnit.UnitStats.CanUseAbility(this))
            {
                return false;
            }
            
            return true;
        }

        public abstract CommandResults GetCommandResults(CommandArgs commandArgs);
        
        protected bool TryUseCommand(CommandArgs _, Unit activeUnit)
        {
            return activeUnit.UnitData.UnitStats.TryUseAbility(this);
        }

        protected bool TryUseCommand(CommandArgs commandArgs, Unit activeUnit, Unit targetUnit)
        {
            if (activeUnit.UnitData.UnitStats.TryUseAbility(this))
            {
                targetUnit.UnitData.UnitStats.AffectByAbility(GetCommandResults(commandArgs));
                return true;
            }
            return false;
        }
        
        protected static void MoveUnitToDelta(BattleStateModel battleStateModel, CommandArgs commandArgs, Unit activeUnit)
        {
            var startPos = commandArgs.BattleStateModel.BattleSceneState
                .Grid[activeUnit.UnitData.BattleScenePosition.Coordinates.x, activeUnit.UnitData.BattleScenePosition.Coordinates.y];
            startPos.Clear();
            var target = commandArgs.PositionDelta.Target;
            var targetPos = commandArgs.BattleStateModel.BattleSceneState.Grid[target.x, target.y];
            activeUnit.UnitData.BattleScenePosition = targetPos;
            activeUnit.UnitData.BattleScenePosition.ApplyUnit(activeUnit.UnitData.UnitIdentifier);
            battleStateModel.BattleSceneState.PathfindingHandler.AggregatedDirtyPositionCache.Add((startPos.Coordinates, true));
            battleStateModel.BattleSceneState.PathfindingHandler.AggregatedDirtyPositionCache.Add((target, false));
        }

        internal virtual CommandArgs GetSuggestedPseudoRandomArgs(BattleStateModel battleStateModel)
        {
            return CommandArgsValidator.GetArgsPicker(this, battleStateModel).GetSuggestedArgs(ArgsPreferences);
        }
    }
}