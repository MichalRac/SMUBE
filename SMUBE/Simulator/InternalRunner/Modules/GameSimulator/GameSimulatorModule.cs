using System;
using System.Collections.Concurrent;
using SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Actions;
using SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Configurator;
using SMUBE_Utils.Simulator.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SMUBE.AI;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator
{
    internal class GameSimulatorModule : IInternalRunnerModule
    {
        public const int CURRENTLY_USED_THREADS = 10;
        protected BattleCoreSimulationWrapper _coreSimulator;
        private AIModel ai1;
        private AIModel ai2;

        protected virtual IGameSimulatorConfigurator GetGameSimulatorConfigurator() => new DefaultGameSimulatorConfigurator();
        
        public virtual async Task Run()
        {
            var useSimpleBehavior = GenericChoiceUtils.GetBooleanChoice("Setting up units AI, use simple behavior?");
            var gameConfigurator = GetGameSimulatorConfigurator();
            var initUnits = gameConfigurator.GetUnits(useSimpleBehavior);

            ai1 = initUnits.First(u => u.UnitData.UnitIdentifier.TeamId == 0).AiModel;
            ai2 = initUnits.First(u => u.UnitData.UnitIdentifier.TeamId == 1).AiModel;
            
            var simulationSeries = GenericChoiceUtils.GetBooleanChoice("Run N Simulation Series?");
            int simulationNumber = simulationSeries 
                ? GenericChoiceUtils.GetInt($"Number of simulations per used thread (currently configured to: {CURRENTLY_USED_THREADS}) to be run:") 
                : 1;

            if (simulationSeries)
            {
                await RunSimulationSeries(simulationNumber, gameConfigurator, useSimpleBehavior, simulationSeries);
            }
            else
            {
                var simulationWrapper = new BattleCoreSimulationWrapper();
                RunSingleSimulation(simulationWrapper, gameConfigurator, useSimpleBehavior, simulationSeries);
            }

            Finish();
        }

        private async Task RunSimulationSeries(int simulationNumber, IGameSimulatorConfigurator gameConfigurator, bool useSimpleBehavior, bool simulationSeries)
        {
            List<Task> tasks = new List<Task>();
            var results = new ConcurrentBag<SimulatorDebugData>();
            
            for (int repeats = 0; repeats < CURRENTLY_USED_THREADS; repeats++)
            {
                int repeat = repeats;
                tasks.Add(Task.Run(() => SingleSimulationWrapper(repeat, simulationNumber)));
                continue;

                Task SingleSimulationWrapper(int run, int simulationsPerThread)
                {
                    var simulationWrapper = new BattleCoreSimulationWrapper();
                    int simulationsRun = 0;
                    while (simulationsRun++ < simulationsPerThread)
                    {
                        /*
                                          try
                                          {
                                              */
                        //RunSingleSimulation(simulationWrapper, gameConfigurator, useSimpleBehavior, simulationSeries);
                        RunSingleSimulation(simulationWrapper, gameConfigurator, useSimpleBehavior, simulationSeries);
                        /*
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Simulation {simulationsRun} corrupted, press anything to continue;");
                            new InternalRunnerDisplayMap(_coreSimulator, true).OnPicked();
                            new InternalRunnerDisplayHeatmap(_coreSimulator).OnPicked();
                            Console.ReadKey();
                        }
                        */

                        if (simulationsRun % 25 == 0)
                        {
                            Console.WriteLine($"thread {run} simulation group progress: {simulationsRun}/{simulationNumber}");
                        }
                    }
                    
                    results.Add(simulationWrapper._simulatorDebugData);
                    //simulationWrapper.OnFinishedLog(ai1, ai2, true);
                    //simulationWrapper.RestartDebugCounters();
                    return Task.CompletedTask;
                }
            }

            await Task.WhenAll(tasks);

            var aggregatedData = new SimulatorDebugData(results);
            var debugDataListed = aggregatedData.GetDebugDataListed();
            aggregatedData.PrintToConsole(debugDataListed);
            aggregatedData.SaveToFile(debugDataListed, string.Empty, "RegularSimulations");
        }

        protected void RunSingleSimulation(BattleCoreSimulationWrapper simulationWrapper, IGameSimulatorConfigurator gameConfigurator, bool useSimpleBehavior, bool simulationSeries)
        {
            simulationWrapper.SetupSimulation(gameConfigurator.GetUnits(useSimpleBehavior));
                
            var roundAutoResolved = false;
            while (!roundAutoResolved)
            {
                if(!simulationSeries)
                {
                    ProcessCurrentTurn(simulationWrapper);
                }
                else
                {
                    new InternalRunnerSkipForward(simulationWrapper).OnPicked();
                }
                    
                roundAutoResolved = simulationWrapper.IsFinished(out _);
            }
                
            simulationWrapper.OnFinished();
        }

        private void ProcessCurrentTurn(BattleCoreSimulationWrapper simulator)
        {
            simulator.LogTurnInfo();
         
            var result = GenericChoiceUtils.GetListChoice("Options:", true, new List<(string description, InternalRunnerAction result)>
            {
                ("Continue", new InternalRunnerStepForward(simulator)),
                ("Auto-Continue", new InternalRunnerSkipForward(simulator, true)),
                ("Pick Action Manually", new InternalRunnerManualAction(simulator)),
                ("Display Map", new InternalRunnerDisplayMap(simulator, false)),
                ("Display Map With Descriptors", new InternalRunnerDisplayMap(simulator, true)),
                ("Display Ally Distance Heatmap", new InternalRunnerDisplayHeatmap(simulator)),
                ("Log Unit Summary", new InternalRunnerLogUnitSummary(simulator)),
            }, false);

            result.OnPicked();
        }

        protected async void Finish()
        {
            bool quit = false;
            Console.WriteLine("Press r to retry, press q to quit");

            while (!quit)
            {
                var key = Console.ReadKey();
            
                if (key.Key == ConsoleKey.R)
                {
                    Console.Clear();
                    _coreSimulator.RestartDebugCounters();
                    await Run();
                    quit = true;
                }
                else if (key.Key == ConsoleKey.Q)
                {
                    quit = true;
                }
            }
        }
    }
}
