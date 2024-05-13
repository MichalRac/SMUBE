using SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator;
using SMUBE_Utils.Simulator.InternalRunner.Modules.Pathfinding;
using SMUBE_Utils.Simulator.Utils;
using System.Collections.Generic;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules
{
    internal class InternalRunnerModulePicker
    {
        public IInternalRunnerModule ChooseModule()
        {
            var result = GenericChoiceUtils.GetListChoice("Module:", true, new List<(string description, IInternalRunnerModule result)>
            {
                ("Game Simulator", new GameSimulatorModule()),
                ("Predefined Game Simulator", new PredefinedGameSimulatorModule()),
                ("Decision Tree Learning", new DecisionTreeLearningModule()),
                ("Pathfinding Simulator", new PathfindingSimulatorModule()),
            });

            return result;
        }
    }
}
