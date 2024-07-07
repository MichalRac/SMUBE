using SMUBE.AI;
using SMUBE.AI.BehaviorTree;
using SMUBE.AI.DecisionTree;
using SMUBE.AI.GoalOrientedBehavior;
using SMUBE.AI.StateMachine;
using SMUBE.Units;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SMUBE_Utils.Simulator.Utils;
using SMUBE.AI.QLearning;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Configurator
{
    internal class DefaultGameSimulatorConfigurator : IGameSimulatorConfigurator
    {
        private static string DTConfigSetPath = "E:\\_RepositoryE\\SMUBE\\Output\\JsonConfigs\\DT\\";
        private static string QTablePath = "E:\\_RepositoryE\\SMUBE\\Output\\JsonConfigs\\QTable\\";
        
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
            Console.WriteLine($"Team {teamNumber} AI:");
            var result = GetAiProvider(useSimpleBehavior);

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
        
        private Func<AIModel> GetAiProvider(bool useSimpleBehavior)
        {
            var provider = GenericChoiceUtils.GetListChoice(String.Empty, false, new List<(string, Func<AIModel>)>()
            {
                ("Random AI", () => new RandomAIModel(useSimpleBehavior)),
                ("Decision Tree AI", () => new DecisionTreeAIModel(useSimpleBehavior)),
                ("Goal Oriented Behavior AI", () => new GoalOrientedBehaviorAIModel(useSimpleBehavior)),
                ("Finite State Machine AI", () => new StateMachineAIModel(null, useSimpleBehavior)),
                ("Behavior Tree AI", () => new BehaviorTreeAIModel(useSimpleBehavior)),
                
                ("Decision Tree - Extended Conditional AI", () => new DecisionTreeAIModel((bc) => DecisionTreeConfigs.GetConditionalDecisionTree(bc))),
                ("Decision Tree - Extended Complex AI", () => new DecisionTreeAIModel((bc) => DecisionTreeConfigs.GetComplexDecisionTree(bc))),
                // todo a way to provide these? 
                ("Decision Tree - json config", () => GetDTbyJsonConfig().Invoke()),
                
                ("QTable - json config", () => GetQTableByJsonConfig().Invoke()),
                
                ("Finite State Machine - Competent AI", () => new StateMachineAIModel(null, useSimpleBehavior).AsCompetent()),
                ("Competent Behavior Tree AI", () => new CompetentBehaviorTreeAIModel()),
            }, false);

            return provider;

            Func<AIModel> GetDTbyJsonConfig()
            {
                DecisionTreeDataSet jsonDataSet = null;

                while(!Directory.Exists(DTConfigSetPath))
                {
                    Console.Clear();
                    Console.WriteLine("Path to configs not chosen! \n" +
                                      "Input path where your DecisionTreeConfigs are, or enter \"Q\" to leave");
                    DTConfigSetPath = Console.ReadLine();
                }
                    
                var files = Directory.GetFiles(DTConfigSetPath);
                var choice = new List<(string msg, string path)>();
                foreach (var file in files)
                {
                    var filename = Path.GetFileName(file);
                    choice.Add((filename, file));
                }
                var result = GenericChoiceUtils.GetListChoice("choose config file to load as decision tree config set", false, choice);
                    
                return () => new DecisionTreeAIModel((bc) => DecisionTreeConfigs.GetComplexDecisionTree(bc, GetJsonDataSet(result)));
            }

            Func<AIModel> GetQTableByJsonConfig()
            {
                while(!Directory.Exists(QTablePath))
                {
                    Console.Clear();
                    Console.WriteLine("Path to configs not chosen! \n" +
                                      "Input path where your QTable json files are, or enter \"Q\" to leave");
                    DTConfigSetPath = Console.ReadLine();
                }
                    
                var qTableFiles = Directory.GetFiles(QTablePath);
                var choice = new List<(string msg, string path)>();
                foreach (var file in qTableFiles)
                {
                    var filename = Path.GetFileName(file);
                    choice.Add((filename, file));
                }
                var result = GenericChoiceUtils.GetListChoice("choose config file to load as QTable", false, choice);
                    
                var fileContent = File.ReadAllText(result);
                SimulatorDebugData.SaveToFileSummary(new List<string>{fileContent}, $"reserialize-source","reserialized-group", true);

                return () => new QLearningModel(GetJsonQTable(result));
            }

            /*
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
                case ConsoleKey.D6:
                    return () => new DecisionTreeAIModel((bc) => DecisionTreeConfigs.GetConditionalDecisionTree(bc));
                case ConsoleKey.D7:
                    return () => new DecisionTreeAIModel((bc) => DecisionTreeConfigs.GetComplexDecisionTree(bc));
                case ConsoleKey.D8:
                {
                    DecisionTreeDataSet jsonDataSet = null;

                    while(!Directory.Exists(DTConfigSetPath))
                    {
                        Console.Clear();
                        Console.WriteLine("Path to configs not chosen! \n" +
                                          "Input path where your DecisionTreeConfigs are, or enter \"Q\" to leave");
                        DTConfigSetPath = Console.ReadLine();
                    }
                    
                    var files = Directory.GetFiles(DTConfigSetPath);
                    var choice = new List<(string msg, string path)>();
                    foreach (var file in files)
                    {
                        var filename = Path.GetFileName(file);
                        choice.Add((filename, file));
                    }
                    var result = GenericChoiceUtils.GetListChoice("choose config file to load as decision tree config set", false, choice);
                    
                    return () => new DecisionTreeAIModel((bc) => DecisionTreeConfigs.GetComplexDecisionTree(bc, GetJsonDataSet(result)));
                }
                case ConsoleKey.D9:
                {
                    while(!Directory.Exists(QTablePath))
                    {
                        Console.Clear();
                        Console.WriteLine("Path to configs not chosen! \n" +
                                          "Input path where your QTable json files are, or enter \"Q\" to leave");
                        DTConfigSetPath = Console.ReadLine();
                    }
                    
                    var qTableFiles = Directory.GetFiles(QTablePath);
                    var choice = new List<(string msg, string path)>();
                    foreach (var file in qTableFiles)
                    {
                        var filename = Path.GetFileName(file);
                        choice.Add((filename, file));
                    }
                    var result = GenericChoiceUtils.GetListChoice("choose config file to load as QTable", false, choice);
                    
                    var fileContent = File.ReadAllText(result);
                    SimulatorDebugData.SaveToFileSummary(new List<string>{fileContent}, $"reserialize-source","reserialized-group", true);

                    return () => new QLearningModel(GetJsonQTable(result));
                }

                default:
                    return null;
            }
        */
        }

        private DecisionTreeDataSet GetJsonDataSet(string filePath)
        {
            var fileContent = File.ReadAllText(filePath);
            var config = JsonConvert.DeserializeObject<DecisionTreeDataSet>(fileContent);
            return config;
        }
        
        private QValueData GetJsonQTable(string filePath)
        {
            var fileContent = File.ReadAllText(filePath);
            var config = JsonConvert.DeserializeObject<QValueData>(fileContent);
            return config;
        }
    }
}
