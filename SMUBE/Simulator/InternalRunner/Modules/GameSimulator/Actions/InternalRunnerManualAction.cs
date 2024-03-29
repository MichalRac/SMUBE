using System;
using System.Collections.Generic;
using SMUBE_Utils.Simulator.Utils;
using SMUBE_Utils.Simulator.Utils.MapPrinter;
using SMUBE.Commands;

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

            var argsPicker = action.CommandArgsValidator.GetArgsPicker(action, CoreWrapper.Core.currentStateModel);

            while (!argsPicker.IsResolved)
            {
                Console.Clear();
                GridMapPrinter.GridPrinterForCommandArgs(CoreWrapper.Core.currentStateModel, action, argsPicker.GetCommandArgs()).PrintMap();
                
                var argSelectionAction = GenericChoiceUtils.GetListChoiceKeyed("Choose next action:", true, new Dictionary<ConsoleKey, (string description, Action result)>()
                {
                    {ConsoleKey.UpArrow, ("above/next target", argsPicker.Up)},
                    {ConsoleKey.LeftArrow, ("previous target", argsPicker.Left)},
                    {ConsoleKey.RightArrow, ("next target", argsPicker.Right)},
                    {ConsoleKey.DownArrow, ("under/previous target", argsPicker.Down)},
                    {ConsoleKey.Enter, ("submit", argsPicker.Submit)},
                    {ConsoleKey.Backspace, ("return", argsPicker.Return)},
                }, false);

                if (argSelectionAction == default)
                {
                    return;
                }
                
                argSelectionAction?.Invoke();
            }

            if (argsPicker.IsResolved)
            {
                CoreWrapper.Core.currentStateModel.ExecuteCommand(action, argsPicker.GetCommandArgs());
                CoreWrapper.OnTurnEnded();
            }
            else
            {
                throw new ArgumentException();
            }
        }

        private ICommand ChooseAction()
        {
            var viableCommands = CoreWrapper.Core.currentStateModel.ActiveUnit.UnitCommandProvider.ViableCommands;

            var choiceList = new List<(string, ICommand)>();
            foreach (var viableCommand in viableCommands)
            {
                var description = Enum.GetName(typeof(CommandId), viableCommand.CommandId);
                var result = viableCommand;

                if (result.CommandArgsValidator.GetArgsPicker(result, CoreWrapper.Core.currentStateModel).IsAnyValid())
                {
                    choiceList.Add((description, result));
                }
            }

            return GenericChoiceUtils.GetListChoice("Choose action:", true, choiceList);
        }
    }
}