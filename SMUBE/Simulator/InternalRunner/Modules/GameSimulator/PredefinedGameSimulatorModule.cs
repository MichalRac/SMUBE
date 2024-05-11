using SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Configurator;
using SMUBE.AI;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator
{
    internal class PredefinedGameSimulatorModule : GameSimulatorModule
    {
        private static AIModel AiModel => new RandomAIModel(true);
        protected override IGameSimulatorConfigurator GetGameSimulatorConfigurator()
        {
            return new PredefinedGameSimulatorConfigurator(AiModel, AiModel);
        }

        public override void Run()
        {
            _coreSimulator = new BattleCoreSimulationWrapper();

            var gameConfigurator = GetGameSimulatorConfigurator();
            var initUnits = gameConfigurator.GetUnits(false);
            RunSingleSimulation(_coreSimulator, gameConfigurator, false, false);
            
            _coreSimulator.OnFinishedLog(AiModel, AiModel);
            Finish();
        }
    }
}