using SMUBE.AI;
using SMUBE.AI.BehaviorTree;
using SMUBE.AI.DecisionTree;
using SMUBE.AI.GoalOrientedBehavior;
using SMUBE.AI.StateMachine;
using SMUBE.Units;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Configurator
{
    internal class DefaultGameSimulatorConfigurator : IGameSimulatorConfigurator
    {
        private Func<AIModel> team1AIModelProvider = null;
        private Func<AIModel> team2AIModelProvider = null;

        public ConcurrentBag<Unit> GetUnits(bool useSimpleBehaviour)
        {
            team1AIModelProvider = team1AIModelProvider ?? GetTeamAiProvider(1, useSimpleBehaviour);
            team2AIModelProvider = team2AIModelProvider ?? GetTeamAiProvider(2, useSimpleBehaviour);

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

        private Func<AIModel> GetTeamAiProvider(int teamNumber, bool useSimpleBehavior)
        {
            Console.Clear();
            PrintAiOptions(teamNumber);
            var key = Console.ReadKey(true);
            var result = GetAiProvider(key.Key, useSimpleBehavior);

            if (result == null)
            {
                Console.Clear();
                Console.WriteLine("Incorrect input, try again!");
                Console.ReadKey();
                return GetTeamAiProvider(teamNumber, useSimpleBehavior);
            }
            else
            {
                return result;
            }
        }

        private void PrintAiOptions(int teamNumber)
        {
            Console.WriteLine($"Team {teamNumber} AI:");
            Console.WriteLine("1. Random AI");
            Console.WriteLine("2. Decision Tree AI");
            Console.WriteLine("3. Goal Oriented Behavior AI");
            Console.WriteLine("4. Finite State Machine AI");
            Console.WriteLine("5. Behavior Tree AI");
            Console.WriteLine("\nChoice:");
        }

        private Func<AIModel> GetAiProvider(ConsoleKey input, bool useSimpleBehavior)
        {
            switch (input)
            {
                case ConsoleKey.D1:
                    return () => new RandomAIModel(useSimpleBehavior);
                case ConsoleKey.D2:
                    return () => new DecisionTreeAIModel(useSimpleBehavior);
                case ConsoleKey.D3:
                    return () => new GoalOrientedBehaviorAIModel(useSimpleBehavior);
                case ConsoleKey.D4:
                    return () => new StateMachineAIModel(null, useSimpleBehavior);
                case ConsoleKey.D5:
                    return () => new BehaviorTreeAIModel(useSimpleBehavior);
                default:
                    return null;
            }
        }
    }
}
