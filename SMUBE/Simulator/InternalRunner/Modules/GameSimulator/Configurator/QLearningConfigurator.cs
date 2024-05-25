using System;
using System.Collections.Concurrent;
using SMUBE.AI;
using SMUBE.AI.DecisionTree;
using SMUBE.Units;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Configurator
{
    public class QLearningConfigurator : IGameSimulatorConfigurator
    {
        private Func<AIModel> team1AIModelProvider;
        private Func<AIModel> team2AIModelProvider = 
            () => new DecisionTreeAIModel((bc) => DecisionTreeConfigs.GetConditionalDecisionTree(bc));

        public QLearningConfigurator(Func<AIModel> team1AiModelProvider)
        {
            team1AIModelProvider = team1AiModelProvider;
        }

        public ConcurrentBag<Unit> GetUnits(bool useSimpleBehaviour)
        {
            return new ConcurrentBag<Unit>
            {
                UnitHelper.CreateRandomUnit(0, team1AIModelProvider?.Invoke(), useSimpleBehaviour),
                UnitHelper.CreateRandomUnit(0, team1AIModelProvider?.Invoke(), useSimpleBehaviour),
                UnitHelper.CreateRandomUnit(0, team1AIModelProvider?.Invoke(), useSimpleBehaviour),

                UnitHelper.CreateRandomUnit(1, team2AIModelProvider?.Invoke(), useSimpleBehaviour),
                UnitHelper.CreateRandomUnit(1, team2AIModelProvider?.Invoke(), useSimpleBehaviour),
                UnitHelper.CreateRandomUnit(1, team2AIModelProvider?.Invoke(), useSimpleBehaviour),
            };
        }
    }
}