using System.Collections.Concurrent;
using SMUBE.AI;
using SMUBE.Units;
using SMUBE.Units.CharacterTypes;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Configurator
{
    internal class PredefinedGameSimulatorConfigurator : IGameSimulatorConfigurator
    {
        private readonly AIModel team1AiModel;
        private readonly AIModel team2AiModel;

        public PredefinedGameSimulatorConfigurator(AIModel team1AiModel, AIModel team2AiModel)
        {
            this.team1AiModel = team1AiModel;
            this.team2AiModel = team2AiModel;
        }

        public ConcurrentBag<Unit> GetUnits(bool useSimpleBehaviour)
        {
            return new ConcurrentBag<Unit>
                {
                    UnitHelper.CreateUnit<Squire>(0, team1AiModel, useSimpleBehaviour),
                    //UnitHelper.CreateUnit<Hunter>(0, team1AiModel, useSimpleBehaviour),
                    //UnitHelper.CreateUnit<Scholar>(0, team1AiModel, useSimpleBehaviour),

                    UnitHelper.CreateUnit<Squire>(1, team2AiModel, useSimpleBehaviour),
                    //UnitHelper.CreateUnit<Hunter>(1, team2AiModel, useSimpleBehaviour),
                    //UnitHelper.CreateUnit<Scholar>(1, team2AiModel, useSimpleBehaviour),
                };
        }
    }
}
