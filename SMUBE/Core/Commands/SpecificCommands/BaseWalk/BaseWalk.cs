using Commands;
using SMUBE.BattleState;
using SMUBE.Commands.Effects;
using SMUBE.Commands.SpecificCommands._Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (!CommandArgsValidator.Validate(commandArgs))
            {
                return false;
            }

            var activeUnit = commandArgs.ActiveUnit;

            var target = commandArgs.PositionDeltas[0].Target;
            var targetPos = commandArgs.BattleStateModel.BattleSceneState.Grid[target.x, target.y];
            activeUnit.BattleScenePosition = targetPos;

            activeUnit.UnitStats.TryUseAbility(this);
            activeUnit.UnitStats.AddEffects(GetCommandResults(commandArgs));

            return true;
        }

        public CommandResults GetCommandResults(CommandArgs commandArgs)
        {
            var results = new CommandResults();

            results.performer = commandArgs.ActiveUnit;
            results.PositionDeltas = commandArgs.PositionDeltas;

            return results;
        }
    }
}
