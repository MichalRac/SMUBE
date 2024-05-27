using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Configurator;
using SMUBE_Utils.Simulator.Utils;
using SMUBE.AI.QLearning;
using SMUBE.Core;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator
{
    internal class QLearningModule : GameSimulatorModule
    {
        private static string QLearningLogPath = "E:\\_RepositoryE\\SMUBE\\Output\\logs\\QLearning\\";

        // 100_800 * 250 = 25 200 000 simulations = ~ 1-1.5 Billion QLearning iterations (assuming each simulation is around 50 turns then 1.25B)
        // 50_400 * 100 = 5 040 000 simulations = ~ 0.25 Billion QLearning iterations (assuming each simulation is around 50 turns)
        // 27_000 * 250 = 6 750 000 simulations = ~ 337.5 Million QLearning iterations (assuming each simulation is around 50 turns then 1.25B)
        // 27_000 * 100 = 2 700 000 simulations = ~ 135 Million QLearning iterations (assuming each simulation is around 50 turns then 1.25B)
        // Workstation
        protected override int CURRENTLY_USED_THREADS => 18;
        private const int LOOPS_TO_RUN = 250;
        private const int SIMULATIONS_BETWEEN_VALIDATION = 13_500;
        
        private const int SIMULATIONS_PER_VALIDATION = 2_700;
        private const int VALIDATION_SUB_THREADING_RATE = 18;
        private const bool PRE_VALIDATE_INIT_STATE = true;

        
        // PC
        /*
        protected override int CURRENTLY_USED_THREADS => 10;
        private const int LOOPS_TO_RUN = 250;
        private const int SIMULATIONS_BETWEEN_VALIDATION = 2_700;
        
        private const int SIMULATIONS_PER_VALIDATION = 2_700;
        private const int VALIDATION_SUB_THREADING_RATE = 10;
        
        private const bool PRE_VALIDATE_INIT_STATE = true;
        */
        
        public List<string> AggregatedSummary = new List<string>();
        private bool _restartMode;
        
        public QLearningModule(bool restartMode)
        {
            _restartMode = restartMode;
        }
        
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

            int startLoopNumber = 0;
            qLearningModel = ProcessRestartMode(qLearningModel, ref startLoopNumber);
            
            var gameConfigurator = new QLearningConfigurator(() => qLearningModel);
            
            List<Task> tasks = new List<Task>();

            if (PRE_VALIDATE_INIT_STATE)
            {
                qLearningModel.EnableLearning(false);
                await ValidateScore(-1, tasks, qLearningModel, learningRunId, gameConfigurator);
            }

            for (int i = startLoopNumber; i < LOOPS_TO_RUN; i++)
            {
                RngProvider.RestartGlobalRandom();
                
                qLearningModel.EnableLearning(true);
                var loop = i;
                Console.WriteLine($"Running loop {loop}");
                await RunSimulationSeries(SIMULATIONS_BETWEEN_VALIDATION, gameConfigurator, false, true, learningRunId, $"_TrainingLoop{loop}");
               
                qLearningModel.EnableLearning(false);
                
                await ValidateScore(loop, tasks, qLearningModel, learningRunId, gameConfigurator);
            }
            var serializedQLearningData = JsonConvert.SerializeObject(qLearningModel._qValueData);
            SimulatorDebugData.SaveToFileSummary(new List<string>{serializedQLearningData}, $"final-config",learningRunId, true);

        }

        private QLearningModel ProcessRestartMode(QLearningModel qLearningModel, ref int startLoopNumber)
        {
            if (_restartMode)
            {
                while(!Directory.Exists(QLearningLogPath))
                {
                    Console.Clear();
                    Console.WriteLine("Path to QLearning not chosen not chosen! \n" +
                                      "Input path where your QLearning log files are, or enter \"Q\" to leave");
                    QLearningLogPath = Console.ReadLine();
                }
                var qTableDirectories = Directory.GetDirectories(QLearningLogPath);
                var directoryChoice = new List<(string msg, string path)>();
                foreach (var dictionary in qTableDirectories)
                {
                    directoryChoice.Add((dictionary, dictionary));
                }
                var dictionaryResult = GenericChoiceUtils.GetListChoice("choose QLearning results dictionary", false, directoryChoice);

                var qTableFiles = Directory.GetFiles(dictionaryResult);
                var choice = new List<(string msg, string path)>();
                foreach (var file in qTableFiles)
                {
                    var filename = Path.GetFileName(file);
                    if (filename.Contains("winRate"))
                    {
                        choice.Add((filename, file));
                    }
                }
                var result = GenericChoiceUtils.GetListChoice("choose config file to load as QTable", false, choice);
                qLearningModel = new QLearningModel(GetJsonQTable(result));

                int.TryParse(Regex.Match(result, "(?<=loopId)\\d+(?=_winRate)").ToString(), out var number);
                startLoopNumber = number;

                foreach (var qTableFile in qTableFiles)
                {
                    if (qTableFile.Contains($"{startLoopNumber}_summary"))
                    {
                        var fileContent = File.ReadAllLines(qTableFile);
                        AggregatedSummary = new List<string>();
                        bool beginningFound = false;
                        foreach (var line in fileContent)
                        {
                            if (!beginningFound && line != "generation;best-fitness")
                            {
                                continue;
                            }
                            beginningFound = true;
                            AggregatedSummary.Add(line);
                        }
                    }
                }
                startLoopNumber++;
            }

            return qLearningModel;
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
                            catch (TimeoutException e)
                            {
                                Console.WriteLine(sequence == -1
                                    ? $"Loop id {sequence} (PreValidation), thread/solution {argSubThread}, simulation group timed out!"
                                    : $"Loop id {sequence}, thread/solution {argSubThread}, simulation group timed out!");
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

                var serializedQLearningData = JsonConvert.SerializeObject(qLearningModel._qValueData);
                SimulatorDebugData.SaveToFileSummary(new List<string>{serializedQLearningData}, $"loopId{loop}_winRate{groupedMatchingData.GetAverageWinRateTeam1()}",learningRunId, true);
            }
        }
        
        private QValueData GetJsonQTable(string filePath)
        {
            var fileContent = File.ReadAllText(filePath);
            var config = JsonConvert.DeserializeObject<QValueData>(fileContent);
            return config;
        }
    }
}