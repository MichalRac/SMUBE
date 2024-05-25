using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Configurator;
using SMUBE.AI.QLearning;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator
{
    internal class QLearningModule : GameSimulatorModule
    {
        protected override int CURRENTLY_USED_THREADS => 20;

        // 100_000 * 250 = 25 000 000 simulations = ~ 1-1.5 Billion QLearning iterations (assuming each simulation is around 50 turns)
        private const int LOOPS_TO_RUN = 250;
        private const int SIMULATIONS_BETWEEN_VALIDATION = 100_000;
        
        private const int SIMULATIONS_PER_VALIDATION = 10000;
        private const int VALIDATION_SUB_THREADING_RATE = 20;
        
        private const bool PRE_VALIDATE_INIT_STATE = false;

        public List<string> AggregatedSummary = new List<string>();
        
        public override async Task Run()
        {
            SimulatorDebugData.EnsurePath();
            
            Console.WriteLine("Give an unique id for the run:");
            var learningRunId = Console.ReadLine(); 
            var date = DateTime.UtcNow;
            var dateSuffix = $"{date.Year}_{date.Month}_{date.Day}_{date.Hour}_{date.Minute}";
            learningRunId = $"QLearning\\{dateSuffix}-{learningRunId}";
            
            AggregatedSummary.Add($"generation;best-fitness");

            var qLearningModel = new QLearningModel();
            var gameConfigurator = new QLearningConfigurator(() => qLearningModel);
            
            List<Task> tasks = new List<Task>();

            if (PRE_VALIDATE_INIT_STATE)
            {
                qLearningModel.EnableLearning(false);
                await ValidateScore(-1, tasks, qLearningModel, learningRunId, gameConfigurator);
            }

            for (int i = 0; i < LOOPS_TO_RUN; i++)
            {
                qLearningModel.EnableLearning(true);
                var loop = i;
                Console.WriteLine($"Running loop {loop}");
                await RunSimulationSeries(SIMULATIONS_BETWEEN_VALIDATION, gameConfigurator, false, true, learningRunId, $"_TrainingLoop{loop}");
               
                qLearningModel.EnableLearning(false);
                
                await ValidateScore(loop, tasks, qLearningModel, learningRunId, gameConfigurator);
            }

        }

        private async Task ValidateScore(int loop, List<Task> tasks, QLearningModel qLearningModel, string learningRunId, QLearningConfigurator gameConfigurator)
        {
            {
                if (loop == -1)
                    Console.WriteLine($"Validating pre-training score");
                else
                    Console.WriteLine($"Validating loop {loop}");

                var validationResults = new ConcurrentBag<SimulatorDebugData>();
                for (int subThreadId = 0; subThreadId < VALIDATION_SUB_THREADING_RATE; subThreadId++)
                {
                    var subThread = subThreadId;
                    tasks.Add(Task.Run(() => SingleSimulationWrapper(loop, subThread, SIMULATIONS_PER_VALIDATION / VALIDATION_SUB_THREADING_RATE)));
                    
                    continue;
                    
                    Task SingleSimulationWrapper(int sequence, int argSubThread, int simCount)
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
                                Console.WriteLine(sequence == -1
                                    ? $"Loop id {sequence} (PreValidation), thread/solution {argSubThread}, simulation group was corrupted!"
                                    : $"Loop id {sequence}, thread/solution {argSubThread}, simulation group was corrupted!");
                            }

                            if (simulationsRun % 50 == 0)
                            {
                                Console.WriteLine(sequence == -1
                                    ? $"Loop id {sequence} (PreValidation), thread/solution {argSubThread}, simulation group progress: {simulationsRun}/{simCount}"
                                    : $"Loop id {sequence}, thread/solution {argSubThread}, simulation group progress: {simulationsRun}/{simCount}");
                            }
                        }

                        simulationWrapper._simulatorDebugData.solutionId = sequence;
                        validationResults.Add(simulationWrapper._simulatorDebugData);
                        return Task.CompletedTask;
                    }
                }
                    
                await Task.WhenAll(tasks);
                
                var matchingData = new ConcurrentBag<SimulatorDebugData>();
                foreach (var result in validationResults)
                {
                    matchingData.Add(result);
                }
                var groupedMatchingData = new SimulatorDebugData(matchingData);
                var debugDataForSolutionListed = groupedMatchingData.GetDebugDataListed();
                var nameSuffix = loop == -1
                    ? "PreTrainingValidation"
                    : $"TrainingLoopId{loop}-aggregated";
                groupedMatchingData.SaveToFile(debugDataForSolutionListed, nameSuffix, learningRunId);
                
                AggregatedSummary.Add($"{loop};{groupedMatchingData.GetAverageWinRateTeam1().ToString()}");
                
                // config save
                SimulatorDebugData.SaveToFileSummary(AggregatedSummary, $"loopId{loop}_summary",learningRunId);
                
                if (loop % 10 == 0)
                {
                    var serializedQLearningData = JsonConvert.SerializeObject(qLearningModel._qValueData);
                    SimulatorDebugData.SaveToFileSummary(new List<string>{serializedQLearningData}, $"loopId{loop}_winRate{groupedMatchingData.GetAverageWinRateTeam1()}",learningRunId, true);
                }
            }
        }
    }
}