using System;
using SMUBE.AI.QLearning;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Actions
{
    internal class InternalRunnerPrintQLearningStateResults : InternalRunnerAction
    {
        public InternalRunnerPrintQLearningStateResults(BattleCoreSimulationWrapper coreWrapper) : base(coreWrapper)
        {
        }

        public override void OnPicked()
        {
            var qLearningState = new QLearningState();
            Console.WriteLine(qLearningState.GetStateNumberWithDescription(CoreWrapper.Core.currentStateModel, CoreWrapper.Core.currentStateModel.ActiveUnit));
        }
    }
}