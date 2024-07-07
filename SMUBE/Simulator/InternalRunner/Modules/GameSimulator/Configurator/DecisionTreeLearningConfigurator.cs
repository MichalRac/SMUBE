using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SMUBE_Utils.Simulator.Utils;
using SMUBE.AI;
using SMUBE.AI.DecisionTree;
using SMUBE.Units;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Configurator
{
    internal class DecisionTreeLearningConfigurator : IGameSimulatorConfigurator
    {
        private Func<AIModel> team1AIModelProvider;
        private Func<AIModel> team2AIModelProvider;
        private readonly DecisionTreeLearningModule.DTLearningFitnessMode _dtLearningFitnessMode;

        public DecisionTreeLearningConfigurator(Func<AIModel> team1AiModelProvider)
        {
            team1AIModelProvider = team1AiModelProvider;
            team2AIModelProvider =
                () => new DecisionTreeAIModel((bc) => DecisionTreeConfigs.GetConditionalDecisionTree(bc));
        }
        public DecisionTreeLearningConfigurator(Func<AIModel> team1AiModelProvider, Func<AIModel> team2AiModelProvider)
        {
            team1AIModelProvider = team1AiModelProvider;
            team2AIModelProvider = team2AiModelProvider;
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