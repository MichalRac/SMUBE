using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Configurator;
using SMUBE.AI.DecisionTree;
using SMUBE.Core;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator
{
    internal class DecisionTreeLearningModule : GameSimulatorModule
    {
        private const int GENERATION_SIZE = 10;
        private const int GENERATIONS_TO_RUN = 250;
        private const int SIMULATIONS_PER_FITNESS_TEST = 5000;
        private const int IMMUNITY_RATE = 2;
        private const int ELITISM_RATE = 3;
        private const int RESSURECTION_RATE = 1;
        
        // thread groups per solution simulation set, preferably valid divisor for sims per fitness test
        private const int SUB_THREADING_RATE = 2;

        private const float CHANCE_TO_MUTATE_GENOME = 0.7f;
        private const float CHANCE_TO_MUTATE_PARAMETER = 0.25f;
        private const int MIN_PROBABILITY_MUTATION = 25;
        private const int MAX_PROBABILITY_MUTATION = 300;
        
        private const float CHANCE_TO_RANDOMIZE_GENOME = 0.15f;
        private const float CHANCE_TO_RANDOMIZE_PARAMETER = 0.5f;

        private List<DecisionTreeDataSet> _solutions;
        
        public override async Task Run()
        {
            SimulatorDebugData.EnsurePath();
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
            
            int simulationsPerThread = SIMULATIONS_PER_FITNESS_TEST / SUB_THREADING_RATE;

            for (int i = 0; i < GENERATIONS_TO_RUN; i++)
            {
                results = new ConcurrentBag<SimulatorDebugData>();
                solutionDebugResults.Clear();
                
                RngProvider.RestartGlobalRandom();
                
                // fitness
                int generation = i;
                for (var solutionIndex = 0; solutionIndex < _solutions.Count; solutionIndex++)
                {
                    var solutionId = solutionIndex;
                    var solution = _solutions[solutionId];

                    var gameConfigurator = new DecisionTreeLearningConfigurator(
                        () => new DecisionTreeAIModel((bc) => DecisionTreeConfigs.GetComplexDecisionTree(bc, solution)));

                    for (int subThreadId = 0; subThreadId < SUB_THREADING_RATE; subThreadId++)
                    {
                        var subThread = subThreadId;
                        tasks.Add(Task.Run(() => SingleSimulationWrapper(solutionId, subThread, simulationsPerThread)));
                    }
                    
                    continue;

                    Task SingleSimulationWrapper(int run, int subThread, int simCount)
                    {
                        var simulationWrapper = new BattleCoreSimulationWrapper();
                        int simulationsRun = 0;
                        while (simulationsRun++ < simCount)
                        {
                            try
                            {
                                RunSingleSimulation(simulationWrapper, gameConfigurator, false, true);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine($"Gen{generation}, thread/solution {run}, group {subThread}, simulation group was corrupted!");
                            }

                            if (simulationsRun % 50 == 0)
                            {
                                Console.WriteLine($"Gen{generation}, thread/solution {run}, group {subThread}, simulation group progress: {simulationsRun}/{simCount}");
                            }
                        }

                        simulationWrapper._simulatorDebugData.solutionId = run;
                        results.Add(simulationWrapper._simulatorDebugData);
                        
                        return Task.CompletedTask;
                    }
                }
                
                await Task.WhenAll(tasks);

                // group subthreaded data and print results of each
                var preAggregatedData = new ConcurrentBag<SimulatorDebugData>();
                for (var solutionIndex = 0; solutionIndex < _solutions.Count; solutionIndex++)
                {
                    var matchingData = new ConcurrentBag<SimulatorDebugData>();
                    foreach (var result in results)
                    {
                        if (result.solutionId == solutionIndex)
                        {
                            matchingData.Add(result);
                        }
                    }

                    var groupedMatchingData = new SimulatorDebugData(matchingData);
                    preAggregatedData.Add(groupedMatchingData);
                    
                    var debugDataForSolutionListed = groupedMatchingData.GetDebugDataListed();
                    debugDataForSolutionListed.Add("\n");
                    debugDataForSolutionListed.Add("Serialized Config Set:");
                    debugDataForSolutionListed.Add($"{JsonConvert.SerializeObject(_solutions[solutionIndex]).ToString()}");
                    
                    solutionDebugResults.TryAdd(solutionIndex, groupedMatchingData);
                    groupedMatchingData.SaveToFile(debugDataForSolutionListed, $"_gen{generation}_sol{solutionIndex}", learningRunId);
                }
                
                

                // aggregate all and print results
                var aggregatedData = new SimulatorDebugData(preAggregatedData);
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
                var bestSolution = JsonConvert.SerializeObject(bestSolutionTuple.best_solution).ToString();
                genSummary.Add($"{bestSolution}");
                SimulatorDebugData.SaveToFileSummary(genSummary, $"gen{i}_summary",learningRunId);
                SimulatorDebugData.SaveToFileSummary(new List<string>{bestSolution}, $"gen{i}_{bestSolutionTuple.fitness}fit",learningRunId, true);
                
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
                    var random = RngProvider.Next(totalFitness + 1);
                    var loopSum = 0;
                    var index1 = 0;
                    while (loopSum < random)
                    {
                        loopSum += selectionList[index1++].fitness;
                    }
                    var parent1 = selectionList[--index1].simulationSolution;

                    random = RngProvider.Next(totalFitness - selectionList[index1].fitness + 1);
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
                    if (newSolutions.Count < GENERATION_SIZE - RESSURECTION_RATE)
                    {
                        newSolutions.Add(crossoverResult.res1);
                    }
                    if (newSolutions.Count < GENERATION_SIZE - RESSURECTION_RATE)
                    {
                        newSolutions.Add(crossoverResult.res2);
                    }
                }

                for (int resurrection = 0; resurrection < RESSURECTION_RATE; resurrection++)
                {
                    newSolutions.Add(GenerateComplexDecisionTreeDataSet());
                }
                
                // Mutation
                int mutationIndex = 0;
                foreach (var newSolution in newSolutions)
                {
                    if (mutationIndex++ < IMMUNITY_RATE)
                    {
                        continue;
                    }
                    
                    if (RngProvider.NextDouble() < CHANCE_TO_MUTATE_GENOME)
                    {
                        List<(string key, float newValue)> probabilityChanges = new List<(string, float)>();
                        List<(string key, int newValue)> weightChanges = new List<(string, int)>();
                        
                        foreach (var probability in newSolution.Probabilities)
                        {
                            if (RngProvider.NextDouble() < CHANCE_TO_MUTATE_PARAMETER)
                            {
                                var delta = (float)RngProvider.Next(MIN_PROBABILITY_MUTATION, MAX_PROBABILITY_MUTATION) / 1_000;
                                if (RngProvider.Next(0, 2) == 0)
                                    delta *= -1;

                                var newValue = newSolution.Probabilities[probability.Key] + delta;
                                newValue = Math.Min(newValue, 1f);
                                newValue = Math.Max(newValue, 0f);
                                
                                probabilityChanges.Add((probability.Key, newValue));
                            }
                        }
                        foreach (var weight in newSolution.Weights)
                        {
                            if (RngProvider.NextDouble() < CHANCE_TO_MUTATE_PARAMETER)
                            {
                                weight.Value.RandomizeAssignedWeight();
                                
                                /*
                                var delta = RngProvider.Next(MIN_WEIGHT_MUTATION, MAX_WEIGHT_MUTATION);
                                if (RngProvider.Next(0, 2) == 0)
                                    delta *= -1;

                                var newValue = newSolution.Weights[weight.Key] + delta;
                                newValue = Math.Min(newValue, MAX_WEIGHT);
                                newValue = Math.Max(newValue, MIN_WEIGHT);
                                
                                weightChanges.Add((weight.Key, newValue));
                            */
                            }
                        }
                        foreach (var probabilityChange in probabilityChanges)
                        {
                            newSolution.Probabilities[probabilityChange.key] = probabilityChange.newValue;
                        }
                        /*
                        foreach (var weightChange in weightChanges)
                        {
                            newSolution.Weights[weightChange.key] = weightChange.newValue;
                        }
                    */
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
                    
                    if (RngProvider.NextDouble() < CHANCE_TO_RANDOMIZE_GENOME)
                    {
                        List<(string key, float newValue)> probabilityChanges = new List<(string, float)>();
                        List<(string key, int newValue)> weightChanges = new List<(string, int)>();
                        
                        foreach (var probability in newSolution.Probabilities)
                        {
                            if (RngProvider.NextDouble() < CHANCE_TO_RANDOMIZE_PARAMETER)
                            {
                                probabilityChanges.Add((probability.Key, (float)RngProvider.NextDouble()));
                            }
                        }
                        foreach (var weight in newSolution.Weights)
                        {
                            if (RngProvider.NextDouble() < CHANCE_TO_RANDOMIZE_PARAMETER)
                            {
                                weight.Value.RandomizeAssignedWeight();
                                //weightChanges.Add((weight.Key, RngProvider.Next(MIN_WEIGHT, MAX_WEIGHT)));
                            }
                        }
                        foreach (var probabilityChange in probabilityChanges)
                        {
                            newSolution.Probabilities[probabilityChange.key] = probabilityChange.newValue;
                        }
                        /*
                        foreach (var weightChange in weightChanges)
                        {
                            newSolution.Weights[weightChange.key];
                        }
                        */
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
            var crossoverPoint = RngProvider.Next(parameterSum);
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
                //var dtDataSet = GenerateConditionalDecisionTreeDataSet();
                var dtDataSet = GenerateComplexDecisionTreeDataSet();
                _solutions.Add(dtDataSet);
            }

            return _solutions;
        }

        private DecisionTreeDataSet GenerateConditionalDecisionTreeDataSet()
        {
            var dtDataSet = new DecisionTreeDataSet()
            {
                Probabilities = new Dictionary<string, float>()
                {
                    { "HurtThreshold", (float)RngProvider.NextDouble() },
                
                    { "SpecialIfPossible-HealthyStatus", (float)RngProvider.NextDouble() },
                    { "SpecialIfPossible-HurtStatus", (float)RngProvider.NextDouble() },
                
                    { "BaseTree_EnemyInReach_BaseAttackChance-HealthyStatus", (float)RngProvider.NextDouble() },
                    { "BaseTree_EnemyInReach_BaseAttackChance-HurtStatus", (float)RngProvider.NextDouble() },
                },
                Weights = new Dictionary<string, DecisionTreeDataSetWeight>()
                {
                    // BASE TREE < < WHEN HEALTHY > >
                    //   enemy in reach
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn-HealthyStatus", DecisionTreeDataSetWeight.Random()},

                    {"BaseTree_EnemyInReach_BaseWalk-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition-HealthyStatus", DecisionTreeDataSetWeight.Random()},

                    {"BaseTree_EnemyInReach_BaseBlock-HealthyStatus", DecisionTreeDataSetWeight.Random()},

                    //   enemy out of reach
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition-HealthyStatus", DecisionTreeDataSetWeight.Random()},

                    {"BaseTree_EnemyOutOfReach_BaseBlock-HealthyStatus", DecisionTreeDataSetWeight.Random()},


                    // EXTENSION TREES
                    //    hunter
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn-HealthyStatus", DecisionTreeDataSetWeight.Random()},

                    {"ExtensionTree-Hunter-Teleport-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition-HealthyStatus", DecisionTreeDataSetWeight.Random()},

                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams-HealthyStatus", DecisionTreeDataSetWeight.Random()},

                    //   scholar
                    {"ExtensionTree-Scholar-HealAll-HealthyStatus", DecisionTreeDataSetWeight.Random()},

                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange-HealthyStatus", DecisionTreeDataSetWeight.Random()},

                    {"ExtensionTree-Scholar-ShieldPosition-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams-HealthyStatus", DecisionTreeDataSetWeight.Random()},

                    //   squire
                    {"ExtensionTree-Squire-DefendAll-HealthyStatus", DecisionTreeDataSetWeight.Random()},

                    {"ExtensionTree-Squire-Taunt-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_Closest-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange-HealthyStatus", DecisionTreeDataSetWeight.Random()},

                    {"ExtensionTree-Squire-Tackle-Pref_None-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_Closest-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange-HealthyStatus", DecisionTreeDataSetWeight.Random()},

                    // BASE TREE < < WHEN HURT > >
                    //   enemy in reach
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_None-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn-HurtStatus", DecisionTreeDataSetWeight.Random()},

                    {"BaseTree_EnemyInReach_BaseWalk-Pref_None-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition-HurtStatus", DecisionTreeDataSetWeight.Random()},

                    {"BaseTree_EnemyInReach_BaseBlock-HurtStatus", DecisionTreeDataSetWeight.Random()},

                    //   enemy out of reach
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition-HurtStatus", DecisionTreeDataSetWeight.Random()},

                    {"BaseTree_EnemyOutOfReach_BaseBlock-HurtStatus", DecisionTreeDataSetWeight.Random()},


                    // EXTENSION TREES
                    //    hunter
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_None-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn-HurtStatus", DecisionTreeDataSetWeight.Random()},

                    {"ExtensionTree-Hunter-Teleport-Pref_None-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition-HurtStatus", DecisionTreeDataSetWeight.Random()},

                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_None-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams-HurtStatus", DecisionTreeDataSetWeight.Random()},

                    //   scholar
                    {"ExtensionTree-Scholar-HealAll-HurtStatus", DecisionTreeDataSetWeight.Random()},

                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange-HurtStatus", DecisionTreeDataSetWeight.Random()},

                    {"ExtensionTree-Scholar-ShieldPosition-Pref_None-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams-HurtStatus", DecisionTreeDataSetWeight.Random()},

                    //   squire
                    {"ExtensionTree-Squire-DefendAll-HurtStatus", DecisionTreeDataSetWeight.Random()},

                    {"ExtensionTree-Squire-Taunt-Pref_None-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_Closest-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange-HurtStatus", DecisionTreeDataSetWeight.Random()},

                    {"ExtensionTree-Squire-Tackle-Pref_None-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_Closest-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange-HurtStatus", DecisionTreeDataSetWeight.Random()},
                }
            };
            return dtDataSet;
        }
        
        private DecisionTreeDataSet GenerateComplexDecisionTreeDataSet()
        {
            var dtDataSet = new DecisionTreeDataSet()
            {
                Probabilities = new Dictionary<string, float>()
                {
                    { "HurtThreshold", (float)RngProvider.NextDouble() },
                
                    { "SpecialIfPossible-Advantage-HealthyStatus", (float)RngProvider.NextDouble() },
                    { "SpecialIfPossible-Advantage-HurtStatus", (float)RngProvider.NextDouble() },
                    { "SpecialIfPossible-Disadvantage-HealthyStatus", (float)RngProvider.NextDouble() },
                    { "SpecialIfPossible-Disadvantage-HurtStatus", (float)RngProvider.NextDouble() },
                
                    { "BaseTree_EnemyInReach_BaseAttackChance-Advantage-HealthyStatus", (float)RngProvider.NextDouble() },
                    { "BaseTree_EnemyInReach_BaseAttackChance-Advantage-HurtStatus", (float)RngProvider.NextDouble() },
                
                    { "BaseTree_EnemyInReach_BaseAttackChance-Disadvantage-HealthyStatus", (float)RngProvider.NextDouble() },
                    { "BaseTree_EnemyInReach_BaseAttackChance-Disadvantage-HurtStatus", (float)RngProvider.NextDouble() },
                },
                Weights = new Dictionary<string, DecisionTreeDataSetWeight>()
                {
                    // BASE TREE < < TEAM ADVANTAGE + HEALTHY > >
                    //   enemy in reach
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"BaseTree_EnemyInReach_BaseBlock-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    //   enemy out of reach
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"BaseTree_EnemyOutOfReach_BaseBlock-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    
                    // EXTENSION TREES
                    //    hunter
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Hunter-Teleport-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    //   scholar
                    {"ExtensionTree-Scholar-HealAll-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    //   squire
                    {"ExtensionTree-Squire-DefendAll-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Squire-Taunt-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_Closest-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Squire-Tackle-Pref_None-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_Closest-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange-Advantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    // BASE TREE < < TEAM ADVANTAGE + UNHEALTHY > >
                    //   enemy in reach
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"BaseTree_EnemyInReach_BaseBlock-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    //   enemy out of reach
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"BaseTree_EnemyOutOfReach_BaseBlock-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    
                    // EXTENSION TREES
                    //    hunter
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Hunter-Teleport-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    //   scholar
                    {"ExtensionTree-Scholar-HealAll-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    //   squire
                    {"ExtensionTree-Squire-DefendAll-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Squire-Taunt-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_Closest-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Squire-Tackle-Pref_None-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_Closest-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange-Advantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    
                    // BASE TREE < < TEAM DISADVANTAGE + HEALTHY > >
                    //   enemy in reach
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"BaseTree_EnemyInReach_BaseBlock-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    //   enemy out of reach
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"BaseTree_EnemyOutOfReach_BaseBlock-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    
                    // EXTENSION TREES
                    //    hunter
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},   
                    {"ExtensionTree-Hunter-Teleport-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    //   scholar
                    {"ExtensionTree-Scholar-HealAll-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    //   squire
                    {"ExtensionTree-Squire-DefendAll-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Squire-Taunt-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_Closest-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Squire-Tackle-Pref_None-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_Closest-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange-Disadvantage-HealthyStatus", DecisionTreeDataSetWeight.Random()},
                    
                    // BASE TREE < < TEAM DISADVANTAGE + UNHEALTHY > >
                    //   enemy in reach
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_Closest-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPoints-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_LeastHpPercentage-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MostDmgDealt-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_EnemyWithMostAlliesInRange-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimizeReachableEnemiesAfterTurn-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximiseReachableEnemiesAfterTurn-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MinimisePositionBuffAfterTurn-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_AttackWeight-Pref_MaximisePositionBuffAfterTurn-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetOutOfReach-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserCarefully-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_GetCloserAggressively-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyInReach_BaseWalk-Pref_OptimizeFortifiedPosition-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"BaseTree_EnemyInReach_BaseBlock-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    //   enemy out of reach
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetOutOfReach-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserCarefully-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_GetCloserAggressively-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"BaseTree_EnemyOutOfReach_BaseWalk-Pref_OptimizeFortifiedPosition-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"BaseTree_EnemyOutOfReach_BaseBlock-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    
                    // EXTENSION TREES
                    //    hunter
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_Closest-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPoints-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_LeastHpPercentage-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MostDmgDealt-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_EnemyWithMostAlliesInRange-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimizeReachableEnemiesAfterTurn-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximiseReachableEnemiesAfterTurn-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MinimisePositionBuffAfterTurn-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-HeavyAttack-Pref_MaximisePositionBuffAfterTurn-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Hunter-Teleport-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetOutOfReach-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetCloserCarefully-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_GetCloserAggressively-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-Teleport-Pref_OptimizeFortifiedPosition-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_NextToClosestEnemy-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenEnemies-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Hunter-RaiseObstacle-Pref_InBetweenTeams-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    //   scholar
                    {"ExtensionTree-Scholar-HealAll-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_Closest-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPoints-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_LeastHpPercentage-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-LowerEnemyDefense-Pref_EnemyWithMostAlliesInRange-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnLeastHpPercentageAlly-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnMostHpPercentageAlly-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_NextToClosestEnemy-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_OnAllyWithMostEnemiesInReach-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Scholar-ShieldPosition-Pref_InBetweenTeams-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    //   squire
                    {"ExtensionTree-Squire-DefendAll-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Squire-Taunt-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_Closest-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_LeastHpPoints-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_LeastHpPercentage-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Taunt-Pref_EnemyWithMostAlliesInRange-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    
                    {"ExtensionTree-Squire-Tackle-Pref_None-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_Closest-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_LeastHpPoints-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_LeastHpPercentage-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                    {"ExtensionTree-Squire-Tackle-Pref_EnemyWithMostAlliesInRange-Disadvantage-HurtStatus", DecisionTreeDataSetWeight.Random()},
                }
            };
            return dtDataSet;
        }
    }
}