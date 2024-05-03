using System;
using SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Actions;
using SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator.Configurator;
using SMUBE_Utils.Simulator.Utils;
using System.Collections.Generic;
using System.Linq;
using SMUBE.AI;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator
{
    internal class GameSimulatorModule : IInternalRunnerModule
    {
        protected BattleCoreSimulationWrapper _coreSimulator;
        private AIModel ai1;
        private AIModel ai2;

        protected virtual IGameSimulatorConfigurator GetGameSimulatorConfigurator() => new DefaultGameSimulatorConfigurator();
        
        public virtual void Run()
        {
            var useSimpleBehavior = GenericChoiceUtils.GetBooleanChoice("Setting up units AI, use simple behavior?");
            var gameConfigurator = GetGameSimulatorConfigurator();
            var initUnits = gameConfigurator.GetUnits(useSimpleBehavior);

            ai1 = initUnits.First(u => u.UnitData.UnitIdentifier.TeamId == 0).AiModel;
            ai2 = initUnits.First(u => u.UnitData.UnitIdentifier.TeamId == 1).AiModel;

            _coreSimulator = new BattleCoreSimulationWrapper();

            var simulationSeries = GenericChoiceUtils.GetBooleanChoice("Run N Simulation Series?");
            int simulationNumber = simulationSeries 
                ? GenericChoiceUtils.GetInt("Number of simulations to be run:") 
                : 1;

            int simulationsRun = 0;
            while (simulationsRun++ < simulationNumber)
            {
                /*
                try
                {
                    */
                    RunSingleSimulation(gameConfigurator, useSimpleBehavior, simulationSeries);
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

                if (simulationsRun % 100 == 0)
                {
                    Console.WriteLine($"simulation progress: {simulationsRun}/{simulationNumber}");
                }
            }
            
            _coreSimulator.OnFinishedLog(ai1, ai2);
            Finish();
        }

        protected void RunSingleSimulation(IGameSimulatorConfigurator gameConfigurator, bool useSimpleBehavior, bool simulationSeries)
        {
            _coreSimulator.SetupSimulation(gameConfigurator.GetUnits(useSimpleBehavior));
                
            var roundAutoResolved = false;
            while (!roundAutoResolved)
            {
                if(!simulationSeries)
                {
                    ProcessCurrentTurn();
                }
                else
                {
                    new InternalRunnerSkipForward(_coreSimulator).OnPicked();
                }
                    
                roundAutoResolved = _coreSimulator.IsFinished(out _);
            }
                
            _coreSimulator.OnFinished();
        }

        private void ProcessCurrentTurn()
        {
            _coreSimulator.LogTurnInfo();
         
            var result = GenericChoiceUtils.GetListChoice("Options:", true, new List<(string description, InternalRunnerAction result)>
            {
                ("Continue", new InternalRunnerStepForward(_coreSimulator)),
                ("Auto-Continue", new InternalRunnerSkipForward(_coreSimulator, true)),
                ("Pick Action Manually", new InternalRunnerManualAction(_coreSimulator)),
                ("Display Map", new InternalRunnerDisplayMap(_coreSimulator, false)),
                ("Display Map With Descriptors", new InternalRunnerDisplayMap(_coreSimulator, true)),
                ("Display Ally Distance Heatmap", new InternalRunnerDisplayHeatmap(_coreSimulator)),
                ("Log Unit Summary", new InternalRunnerLogUnitSummary(_coreSimulator)),
            }, false);

            result.OnPicked();
        }

        protected void Finish()
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
                    Run();
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
