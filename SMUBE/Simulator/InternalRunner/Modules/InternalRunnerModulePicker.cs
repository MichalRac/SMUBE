using System;
using SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator;
using SMUBE_Utils.Simulator.InternalRunner.Modules.Pathfinding;
using SMUBE_Utils.Simulator.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SMUBE.AI.QLearning;
using SMUBE.Commands.Args;
using SMUBE.Commands.SpecificCommands.Tackle;
using SMUBE.Commands.SpecificCommands.Teleport;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules
{
    internal class InternalRunnerModulePicker
    {
        public IInternalRunnerModule ChooseModule()
        {
            var result = GenericChoiceUtils.GetListChoice("Module:", true, new List<(string description, IInternalRunnerModule result)>
            {
                ("Game Simulator", new GameSimulatorModule()),
                ("Predefined Game Simulator", new PredefinedGameSimulatorModule()),
                ("Decision Tree Learning", new DecisionTreeLearningModule()),
                ("Q Learning", new QLearningModule()),
                ("Pathfinding Simulator", new PathfindingSimulatorModule()),
                ("Test Process", new TestProcessModule())
            });

            return result;
        }
    }

    internal class TestProcessModule : IInternalRunnerModule
    {
        public Task Run()
        {
            var qValuePair = new QValueActionPair(0.5f, new Teleport().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange));
            var serializeObject = JsonConvert.SerializeObject(qValuePair);
            Console.WriteLine(serializeObject);
            Console.WriteLine("\n");
            
            var qValueData = new QValueData();
            
            var serializeObject3 = JsonConvert.SerializeObject(qValueData.QValueDataStorage);
            Console.WriteLine(serializeObject3);
            Console.WriteLine("\n");
            
            qValueData.SetQValue(10000,new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetCloserCarefully), 0.6f );
            qValueData.SetQValue(10002,new Tackle().WithPreferences(ArgsEnemyTargetingPreference.None), 0.2f );
            var serializeObject2 = JsonConvert.SerializeObject(qValueData);
            Console.WriteLine(serializeObject2);
            Console.WriteLine("\n");

            Console.WriteLine("press anything to continue");
            Console.ReadKey();
            return Task.CompletedTask;
        }
    }
}
