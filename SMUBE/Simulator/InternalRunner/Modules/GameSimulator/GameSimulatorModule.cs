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
        private BattleCoreSimulationWrapper _coreSimulator;
        private AIModel ai1;
        private AIModel ai2;

        private IGameSimulatorConfigurator GetGameSimulatorConfigurator() => new DefaultGameSimulatorConfigurator();
        
        public void Run()
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
                
                if (simulationsRun % 250 == 0)
                {
                    Console.WriteLine($"simulation progress: {simulationsRun}/{simulationNumber}");
                }
            }
            
            _coreSimulator.OnFinishedLog(ai1, ai2);
            Finish();
        }

        private void ProcessCurrentTurn()
        {
            _coreSimulator.LogTurnInfo();
         
            var result = GenericChoiceUtils.GetListChoice("Options:", true, new List<(string description, InternalRunnerAction result)>
            {
                ("Continue", new InternalRunnerStepForward(_coreSimulator)),
                ("Auto-Continue", new InternalRunnerSkipForward(_coreSimulator, true)),
                ("Display Map", new InternalRunnerDisplayMap(_coreSimulator))
            }, false);

            result.OnPicked();
        }

        private void Finish()
        {
            Console.WriteLine("Press r to retry, press anything else to quit");
            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.R)
            {
                Console.Clear();
                _coreSimulator.Restart();
                Run();
            }
        }
    }
}
