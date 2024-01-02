using SMUBE.BattleState;
using System.Collections.Generic;
using SMUBE.Commands._Common;
using SMUBE.Commands.Args;
using SMUBE.Commands.Args.ArgsValidators;
using SMUBE.Commands.Results;

namespace SMUBE.Commands.SpecificCommands.BaseWalk
{
    public class BaseWalk : ICommand
    {
        public int StaminaCost => 0;

        public int ManaCost => 0;

        private CommandArgs _argsCache;
        public CommandArgs ArgsCache { get => _argsCache; set => _argsCache = value; }

        public CommandId CommandId => CommandId.BaseWalk;

        public CommandArgsValidator CommandArgsValidator => new OneToPositionValidator();

        public bool Execute(BattleStateModel battleStateModel, CommandArgs commandArgs)
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

            activeUnit.UnitStats.TryUseAbility(this);
            activeUnit.UnitStats.AddEffects(GetCommandResults(commandArgs));

            return true;
        }

        public CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults();

            results.performer = commandArgs.ActiveUnit;
            results.PositionDeltas = new List<PositionDelta> { commandArgs.PositionDelta };

            return results;
        }
    }
}
