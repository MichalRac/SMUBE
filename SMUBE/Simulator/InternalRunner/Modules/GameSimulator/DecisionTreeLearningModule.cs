using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Configurator;
using SMUBE.AI.DecisionTree;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator
{
    internal class DecisionTreeLearningModule : GameSimulatorModule
    {
        private Random _rng;
        
        private const int MIN_START_WEIGHT = 0;
        private const int MAX_START_WEIGHT = 100;
        
        // for 24 genomes per generation
        private const int GENERATION_SIZE = 12;
        private const int GENERATIONS_TO_RUN = 250;
        private const int SIMULATIONS_PER_FITNESS_TEST = 1000;
        private const int IMMUNITY_RATE = 1;
        private const int ELITISM_RATE = 4;
        private const int RESSURECTION_RATE = 2;
        
        /*
        // for 12 genomes per generation
        private const int GENERATION_SIZE = 12;
        private const int GENERATIONS_TO_RUN = 100;
        private const int SIMULATIONS_PER_FITNESS_TEST = 100;
        
        private const int IMMUNITY_RATE = 2;
        private const int ELITISM_RATE = 6;
        private const int RESSURECTION_RATE = 2;
        */

        private const float CHANCE_TO_MUTATE_GENOME = 0.7f;
        private const float CHANCE_TO_MUTATE_PARAMETER = 0.25f;
        private const int MIN_PROBABILITY_MUTATION = 25;
        private const int MAX_PROBABILITY_MUTATION = 300;
        private const int MIN_WEIGHT_MUTATION = 2;
        private const int MAX_WEIGHT_MUTATION = 50;
        
        private const int MIN_WEIGHT = 0;
        private const int MAX_WEIGHT = 200;
        
        private const float CHANCE_TO_RANDOMIZE_GENOME = 0.15f;
        private const float CHANCE_TO_RANDOMIZE_PARAMETER = 0.5f;

        private List<DecisionTreeDataSet> _solutions;

        public DecisionTreeLearningModule()
        {
            _rng = new Random();
        }

        public override async void Run()
        {
            _solutions = InitializeFirstGeneration();

            Console.WriteLine("Give an unique id for the run:");
            var learningRunId = Console.ReadLine(); 
            var date = DateTime.UtcNow;
            var dateSuffix = $"{date.Year}_{date.Month}_{date.Day}_{date.Hour}_{date.Minute}";
            learningRunId = $"DTLearning\\{dateSuffix}-{learningRunId}";
            
            var results = new ConcurrentBag<SimulatorDebugData>();
            List<Task> tasks = new List<Task>();

            var solutionDebugResults = new ConcurrentDictionary<int, SimulatorDebugData>();

            (DecisionTreeDataSet best_solution, int fitness) bestSolutionTuple = (null, int.MinValue);
            List<string> allGenSummary = new List<string>();
            allGenSummary.Add($"generation,best-fitness,avg-win-rate");

            for (int i = 0; i < GENERATIONS_TO_RUN; i++)
            {
                results = new ConcurrentBag<SimulatorDebugData>();
                solutionDebugResults.Clear();
                
                // fitness
                int generation = i;
                for (var solutionIndex = 0; solutionIndex < _solutions.Count; solutionIndex++)
                {
                    var solutionId = solutionIndex;
                    var solution = _solutions[solutionId];

                    var gameConfigurator = new DecisionTreeLearningConfigurator(
                        () => new DecisionTreeAIModel((bc) => DecisionTreeConfigs.GetConditionalDecisionTree(bc, solution)));

                    tasks.Add(Task.Run(() => SingleSimulationWrapper(solutionId, SIMULATIONS_PER_FITNESS_TEST)));
                    continue;

                    Task SingleSimulationWrapper(int run, int simulationsPerThread)
                    {
                        var simulationWrapper = new BattleCoreSimulationWrapper();
                        int simulationsRun = 0;
                        while (simulationsRun++ < simulationsPerThread)
                        {
                            try
                            {
                                RunSingleSimulation(simulationWrapper, gameConfigurator, false, true);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Gen{generation}, thread/solution {run} simulation group was corrupted!");
                            }

                            if (simulationsRun % 25 == 0)
                            {
                                Console.WriteLine($"Gen{generation}, thread/solution {run} simulation group progress: {simulationsRun}/{SIMULATIONS_PER_FITNESS_TEST}");
                            }
                        }

                        results.Add(simulationWrapper._simulatorDebugData);
                        var debugDataForSolutionListed = simulationWrapper._simulatorDebugData.GetDebugDataListed();
                        debugDataForSolutionListed.Add("\n");
                        debugDataForSolutionListed.Add("Serialized Config Set:");
                        debugDataForSolutionListed.Add($"{JsonConvert.SerializeObject(solution).ToString()}");
                        solutionDebugResults.TryAdd(solutionId, simulationWrapper._simulatorDebugData);
                        
                        simulationWrapper._simulatorDebugData.SaveToFile(debugDataForSolutionListed, $"_gen{generation}_sol{solutionId}", learningRunId);
                        return Task.CompletedTask;
                    }
                }
                
                await Task.WhenAll(tasks);

                var aggregatedData = new SimulatorDebugData(results);
                var debugDataListed = aggregatedData.GetDebugDataListed();
                //aggregatedData.PrintToConsole(debugDataListed);
                Console.WriteLine($"Gen{i} completed!");
                aggregatedData.SaveToFile(debugDataListed, $"_gen{i}_aggregated",learningRunId);

                // Generation Processing
                var newSolutions = new List<DecisionTreeDataSet>();
                
                // Fitness
                var selectionList = new List<(DecisionTreeDataSet simulationSolution, int fitness)>();
                foreach (var solutionDebugResult in solutionDebugResults)
                {
                    selectionList.Add((_solutions[solutionDebugResult.Key], GetFitnessByWinRatio(solutionDebugResult.Value)));
                }
                selectionList = selectionList.OrderByDescending(s => s.fitness).ToList();

                // log generation summary
                var genBestSolution = selectionList.First();
                if (bestSolutionTuple.best_solution == null || genBestSolution.fitness > bestSolutionTuple.fitness)
                {
                    bestSolutionTuple = (genBestSolution.simulationSolution, genBestSolution.fitness);
                }
                var avgFitness = selectionList.Average(sol => sol.fitness);
                allGenSummary.Add($"{generation},{genBestSolution.fitness},{avgFitness}");
                var genSummary = new List<string>(allGenSummary);
                genSummary.Add("\n");
                genSummary.Add($"Best Fitness {bestSolutionTuple.fitness} Serialized Config Set:");
                genSummary.Add($"{JsonConvert.SerializeObject(bestSolutionTuple.best_solution).ToString()}");
                SimulatorDebugData.SaveToFileSummary(genSummary, $"gen{i}_summary",learningRunId);
                
                // early return on final loop
                if (i == GENERATIONS_TO_RUN - 1)
                {
                    break;
                }
                
                // Elitism
                for (int topId = 0; topId < ELITISM_RATE; topId++)
                {
                    newSolutions.Add(selectionList[topId].simulationSolution);
                }
                
                var totalFitness = selectionList.Sum(s => s.fitness);

                // Natural Selection
                while (newSolutions.Count < GENERATION_SIZE - RESSURECTION_RATE)
                {
                    // todo DRY it
                    var random = _rng.Next(totalFitness + 1);
                    var loopSum = 0;
                    var index1 = 0;
                    while (loopSum < random)
                    {
                        loopSum += selectionList[index1++].fitness;
                    }
                    var parent1 = selectionList[--index1].simulationSolution;

                    random = _rng.Next(totalFitness - selectionList[index1].fitness + 1);
                    loopSum = 0;
                    var index2 = 0;
                    while (loopSum < random)
                    {
                        if (index2 == index1)
                        {
                            index2++;
                            continue;
                        }
                        loopSum += selectionList[index2++].fitness;
                    }
                    var parent2 = selectionList[--index2].simulationSolution;
                    
                    // Crossover
                    var crossoverResult = SinglePointCrossover(parent1, parent2);
                    newSolutions.Add(crossoverResult.res1);
                    newSolutions.Add(crossoverResult.res2);
                }

                for (int resurrection = 0; resurrection < RESSURECTION_RATE; resurrection++)
                {
                    newSolutions.Add(GenerateDecisionTreeDataSet());
                }
                
                // Mutation
                int mutationIndex = 0;
                foreach (var newSolution in newSolutions)
                {
                    if (mutationIndex++ < IMMUNITY_RATE)
                    {
                        continue;
                    }
                    
                    if (_rng.NextDouble() < CHANCE_TO_MUTATE_GENOME)
                    {
                        List<(string key, float newValue)> probabilityChanges = new List<(string, float)>();
                        List<(string key, int newValue)> weightChanges = new List<(string, int)>();
                        
                        foreach (var probability in newSolution.Probabilities)
                        {
                            if (_rng.NextDouble() < CHANCE_TO_MUTATE_PARAMETER)
                            {
                                var delta = (float)_rng.Next(MIN_PROBABILITY_MUTATION, MAX_PROBABILITY_MUTATION) / 1_000;
                                if (_rng.Next(0, 2) == 0)
                                    delta *= -1;

                                var newValue = newSolution.Probabilities[probability.Key] + delta;
                                newValue = Math.Min(newValue, 1f);
                                newValue = Math.Max(newValue, 0f);
                                
                                probabilityChanges.Add((probability.Key, newValue));
                            }
                        }
                        foreach (var weight in newSolution.Weights)
                        {
                            if (_rng.NextDouble() < CHANCE_TO_MUTATE_PARAMETER)
                            {
                                var delta = _rng.Next(MIN_WEIGHT_MUTATION, MAX_WEIGHT_MUTATION);
                                if (_rng.Next(0, 2) == 0)
                                    delta *= -1;

                                var newValue = newSolution.Weights[weight.Key] + delta;
                                newValue = Math.Min(newValue, MAX_WEIGHT);
                                newValue = Math.Max(newValue, MIN_WEIGHT);
                                
                                weightChanges.Add((weight.Key, newValue));
                            }
                        }
                        foreach (var probabilityChange in probabilityChanges)
                        {
                            newSolution.Probabilities[probabilityChange.key] = probabilityChange.newValue;
                        }
                        foreach (var weightChange in weightChanges)
                        {
                            newSolution.Weights[weightChange.key] = weightChange.newValue;
                        }
                    }
                }
                
                // Randomization
                int randomizationIndex = 0;
                foreach (var newSolution in newSolutions)
                {
                    if (randomizationIndex++ < IMMUNITY_RATE)
                    {
                        continue;
                    }
                    
                    if (_rng.NextDouble() < CHANCE_TO_RANDOMIZE_GENOME)
                    {
                        List<(string key, float newValue)> probabilityChanges = new List<(string, float)>();
                        List<(string key, int newValue)> weightChanges = new List<(string, int)>();
                        
                        foreach (var probability in newSolution.Probabilities)
                        {
                            if (_rng.NextDouble() < CHANCE_TO_RANDOMIZE_PARAMETER)
                            {
                                probabilityChanges.Add((probability.Key, (float)_rng.NextDouble()));
                            }
                        }
                        foreach (var weight in newSolution.Weights)
                        {
                            if (_rng.NextDouble() < CHANCE_TO_RANDOMIZE_PARAMETER)
                            {
                                weightChanges.Add((weight.Key, _rng.Next(MIN_WEIGHT, MAX_WEIGHT)));
                            }
                        }
                        foreach (var probabilityChange in probabilityChanges)
                        {
                            newSolution.Probabilities[probabilityChange.key] = probabilityChange.newValue;
                        }
                        foreach (var weightChange in weightChanges)
                        {
                            newSolution.Weights[weightChange.key] = weightChange.newValue;
                        }
                    }
                }

                _solutions = newSolutions;
            }
        }

        private int GetFitnessByWinRatio(SimulatorDebugData simulationResult)
        {
            var turnAmountModifier = 1f; 
            var averageTurnsPerSimulation = (float)(simulationResult.teamOneActions + simulationResult.teamTwoActions) / simulationResult.totalSimulationCount;
            
            if (averageTurnsPerSimulation > 1000) turnAmountModifier = 0.01f;
            else if (averageTurnsPerSimulation > 500) turnAmountModifier = 0.1f;
            else if (averageTurnsPerSimulation > 250) turnAmountModifier = 0.2f;
            
            return (int)(((float)simulationResult.team1WinCount / simulationResult.totalSimulationCount) * 10000 * turnAmountModifier);
        }

        private (DecisionTreeDataSet res1, DecisionTreeDataSet res2) SinglePointCrossover(DecisionTreeDataSet sol1, DecisionTreeDataSet sol2)
        {
            var res1 = new DecisionTreeDataSet();
            var res2 = new DecisionTreeDataSet();
            
            var parameterSum = sol1.Probabilities.Count + sol2.Weights.Count;
            var crossoverPoint = _rng.Next(parameterSum);
            int currentIndex = 0;

            foreach (var probability in sol1.Probabilities)
            {
                var key = probability.Key;
                if (currentIndex < crossoverPoint)
                {
                    res1.Probabilities.Add(key, probability.Value);
                    res2.Probabilities.Add(key, sol2.Probabilities[key]);
                }
                else
                {
                    res1.Probabilities.Add(key, sol2.Probabilities[key]);
                    res2.Probabilities.Add(key, probability.Value);
                }
                currentIndex++;
            }
            foreach (var weight in sol1.Weights)
            {
                var key = weight.Key;
                if (currentIndex < crossoverPoint)
                {
                    res1.Weights.Add(key, weight.Value);
                    res2.Weights.Add(key, sol2.Weights[key]);
                }
                else
                {
                    res1.Weights.Add(key, sol2.Weights[key]);
                    res2.Weights.Add(key, weight.Value);
                }
                currentIndex++;
            }
            
            /*
            if (crossoverPoint < sol1.Probabilities.Count)
            {
                
                
                
                foreach (var probability1 in sol1.Probabilities)
                {
                    res1.Probabilities.Add(probability1.Key, probability1.Value);
                    currentIndex++;
                }
                foreach (var probability1 in sol2.Probabilities)
                {
                    res2.Probabilities.Add(probability1.Key, probability1.Value);
                    currentIndex++;
                }

                
                if (currentIndex < crossoverPoint)
                {

                }
                else
                {
                    foreach (var probability2 in sol2.Probabilities)
                    {
                        res1.Probabilities.Add(probability2.Key, probability2.Value);
                    }
                    foreach (var probability1 in sol1.Probabilities)
                    {
                        res2.Probabilities.Add(probability1.Key, probability1.Value);
                    }
                }

                foreach (var weight2 in sol2.Weights)
                {
                    res1.Weights.Add(weight2.Key, weight2.Value);
                }
                foreach (var weight1 in sol1.Weights)
                {
                    res2.Weights.Add(weight1.Key, weight1.Value);
                }
            }
            else
            {
                foreach (var weight1 in sol1.Weights)
                {
                    res1.Weights.Add(weight1.Key, weight1.Value);
                }
                foreach (var weight2 in sol2.Weights)
                {
                    res2.Weights.Add(weight2.Key, weight2.Value);
                }
                
                int currentIndex = 0;
                foreach (var weight in sol1.Weights)
                {
                    //var id = weight.Key;
                    /*
                    if (currentIndex < crossoverPoint)
                    {
                        res1.Weights[id] = sol1.Weights[id];
                        res2.Weights[id] = sol2.Weights[id];
                    }
                    else
                    {
                        res1.Weights[id] = sol2.Weights[id];
                        res2.Weights[id] = sol1.Weights[id];
                    }
                    #1#
                    if (currentIndex < crossoverPoint)
                    {
                        foreach (var weight1 in sol1.Weights)
                        {
                            res1.Weights.Add(weight1.Key, weight1.Value);
                        }
                        foreach (var weight2 in sol2.Weights)
                        {
                            res2.Weights.Add(weight2.Key, weight2.Value);
                        }
                    }
                    else
                    {
                        foreach (var weight2 in sol2.Weights)
                        {
                            res1.Weights.Add(weight2.Key, weight2.Value);
                        }
                        foreach (var weight1 in sol1.Weights)
                        {
                            res2.Weights.Add(weight1.Key, weight1.Value);
                        }
                    }

                    currentIndex++;
                }
            }
            */

            return (res1, res2);
        }

        public List<DecisionTreeDataSet> InitializeFirstGeneration()
        {
            _solutions = new List<DecisionTreeDataSet>();
            
            for (int i = 0; i < GENERATION_SIZE; i++)
            {
                var dtDataSet = GenerateDecisionTreeDataSet();
                _solutions.Add(dtDataSet);
            }

            return _solutions;
        }

        private DecisionTreeDataSet GenerateDecisionTreeDataSet()
        {
            var dtDataSet = new DecisionTreeDataSet()
            {
                Probabilities = new Dictionary<string, float>()
                {
                    { "HurtThreshold", (float)_rng.NextDouble() },
                
                    { "SpecialIfPossible-HealthyStatus", (float)_rng.NextDouble() },
                    { "SpecialIfPossible-HurtStatus", (float)_rng.NextDouble() },
                
                    { "BaseTree_EnemyInReach_BaseAttackChance-HealthyStatus", (float)_rng.NextDouble() },
                    { "BaseTree_EnemyInReach_BaseAttackChance-HurtStatus", (float)_rng.NextDouble() },
                },
                Weights = new Dictionary<string, int>()
                {
                    // BASE TREE < < WHEN HEALTHY > >
                    //   enemy in reach
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_None-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"BaseTree_EnemyInReach_BaseWalk-Pref_None-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"BaseTree_EnemyInReach_BaseBlock-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    //   enemy out of reach
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"BaseTree_EnemyOutOfReach_BaseBlock-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},


                    // EXTENSION TREES
                    //    hunter
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_None-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"ExtensionTree-Hunter-Teleport-Pref_None-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_None-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    //   scholar
                    {"ExtensionTree-Scholar-HealAll-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"ExtensionTree-Scholar-ShieldPosition-Pref_None-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    //   squire
                    {"ExtensionTree-Squire-DefendAll-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"ExtensionTree-Squire-Taunt-Pref_None-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Squire-Taunt-Pref_Closest-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"ExtensionTree-Squire-Tackle-Pref_None-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Squire-Tackle-Pref_Closest-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange-HealthyStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    // BASE TREE < < WHEN HURT > >
                    //   enemy in reach
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_None-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"BaseTree_EnemyInReach_BaseWalk-Pref_None-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"BaseTree_EnemyInReach_BaseBlock-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    //   enemy out of reach
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"BaseTree_EnemyOutOfReach_BaseBlock-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},


                    // EXTENSION TREES
                    //    hunter
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_None-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"ExtensionTree-Hunter-Teleport-Pref_None-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_None-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    //   scholar
                    {"ExtensionTree-Scholar-HealAll-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"ExtensionTree-Scholar-ShieldPosition-Pref_None-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    //   squire
                    {"ExtensionTree-Squire-DefendAll-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"ExtensionTree-Squire-Taunt-Pref_None-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Squire-Taunt-Pref_Closest-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},

                    {"ExtensionTree-Squire-Tackle-Pref_None-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Squire-Tackle-Pref_Closest-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                    {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange-HurtStatus", _rng.Next(MIN_START_WEIGHT, MAX_START_WEIGHT)},
                }
            };
            return dtDataSet;
        }
    }
}