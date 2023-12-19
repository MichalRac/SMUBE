using SMUBE.AI;
using SMUBE.Units;
using System.Collections.Generic;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Configurator
{
    internal class AutoGameSimulatorConfigurator : IGameSimulatorConfigurator
    {
        private readonly AIModel team1AiModel;
        private readonly AIModel team2AiModel;

        public AutoGameSimulatorConfigurator(AIModel team1AiModel, AIModel team2AiModel)
        {
            this.team1AiModel = team1AiModel;
            this.team2AiModel = team2AiModel;
        }

        public List<Unit> GetUnits(bool useSimpleBehaviour)
        {
            return new List<Unit>
                {
                    UnitHelper.CreateRandomUnit(0, team1AiModel, useSimpleBehaviour),
                    UnitHelper.CreateRandomUnit(0, team1AiModel, useSimpleBehaviour),
                    UnitHelper.CreateRandomUnit(0, team1AiModel, useSimpleBehaviour),

                    UnitHelper.CreateRandomUnit(1, team2AiModel, useSimpleBehaviour),
                    UnitHelper.CreateRandomUnit(1, team2AiModel, useSimpleBehaviour),
                    UnitHelper.CreateRandomUnit(1, team2AiModel, useSimpleBehaviour),
                };
        }
    }
}
