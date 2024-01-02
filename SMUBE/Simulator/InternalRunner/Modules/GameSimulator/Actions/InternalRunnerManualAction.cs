using System;
using System.Collections.Generic;
using Commands;
using SMUBE_Utils.Simulator.Utils;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Actions
{
    internal class InternalRunnerManualAction : InternalRunnerAction
    {
        public InternalRunnerManualAction(BattleCoreSimulationWrapper coreWrapper) : base(coreWrapper)
        {
        }

        public override void OnPicked()
        {
            var action = ChooseAction();
            if (action == default)
            {
                return;
            }
        }

        private ICommand ChooseAction()
        {
            var viableCommands = CoreWrapper.Core.currentStateModel.ActiveUnit.ViableCommands;

            var choiceList = new List<(string, ICommand)>();
            foreach (var viableCommand in viableCommands)
            {
                var description = Enum.GetName(typeof(CommandId), viableCommand.CommandId);
                var result = viableCommand;
                choiceList.Add((description, result));
            }

            return GenericChoiceUtils.GetListChoice("Choose action:", true, choiceList);
        }

        /*
        private CommandArgs GetCommandArgs(ICommand command)
        {
            return new CommonArgs();
        }
    */
    }
}