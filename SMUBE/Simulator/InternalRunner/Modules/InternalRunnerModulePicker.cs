using System;
using SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator;
using SMUBE_Utils.Simulator.InternalRunner.Modules.Pathfinding;
using SMUBE_Utils.Simulator.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SMUBE.AI.QLearning;
using SMUBE.Commands;
using SMUBE.Commands.Args;
using SMUBE.Units.CharacterTypes;

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
                ("Decision Tree Learning - Win Rate Fitness", new DecisionTreeLearningModule()),
                ("Decision Tree Learning - Turns Survived Fitness", new DecisionTreeLearningModule(DecisionTreeLearningModule.DTLearningFitnessMode.TurnsSurvived)),
                ("Q Learning", new QLearningModule(false)),
                ("Q Learning - Restart Mode", new QLearningModule(true)),
                ("Pathfinding Simulator", new PathfindingSimulatorModule()),
                /*
                ("Test Process", new TestProcessModule()),
                */
                ("Process qTable to human-readable", new QTableToReadableModule())
            });

            return result;
        }
    }

    internal class QTableToReadableModule : IInternalRunnerModule
    {
        //private static string QTablePath = "E:\\_RepositoryE\\SMUBE\\Output\\JsonConfigs\\QTable\\";
        private static string QTablePath = "F:\\ThesisStuff\\_AllLogs\\_Results";
        public Task Run()
        {
            while(!Directory.Exists(QTablePath))
            {
                Console.Clear();
                Console.WriteLine("Path to configs not chosen! \n" +
                                  "Input path where your QTable json files are, or enter \"Q\" to leave");
                QTablePath = Console.ReadLine();
            }
                    
            var qTableFiles = Directory.GetFiles(QTablePath);
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
                    
            var fileContent = File.ReadAllText(result);
            var config = JsonConvert.DeserializeObject<QValueData>(fileContent);
            var readableLines = new List<string>();

            var qLearningState = new QLearningState();

            var ordered = config.QValueDataStorage.OrderBy(qTable => qTable.Key);

            // no filter
            //var filtered = ordered;
            // scholar filter
            //var filtered = ordered.Where(item => item.Key % 10 == 0).ToList();
            // squire filter
            var filtered = ordered.Where(item => item.Key % 10 == 1).ToList();
            // scholar filter
            //var filtered = ordered.Where(item => item.Key % 10 == 2).ToList();
            foreach (var qTableEntry in filtered)
            {
                readableLines.Add("------------------------");
                readableLines.Add("\n");
                readableLines.Add(qLearningState.ConvertStateToDescription(qTableEntry.Key));
                readableLines.Add("\n");

                
                readableLines.Add("qValue\tCommand\tSubcommand Preference");
                var unitTypeId = qTableEntry.Key % 10;
                BaseCharacterType unitType;
                switch (unitTypeId)
                {
                    case 0: unitType = BaseCharacterType.Scholar; break; 
                    case 1: unitType = BaseCharacterType.Squire; break;
                    case 2: unitType = BaseCharacterType.Hunter; break;
                    default: throw new Exception();
                }
                
                var orderedActions = qTableEntry.Value
                    .OrderBy(actionPair => GetCommandOrder(actionPair.CommandId))
                    .ThenBy(actionPair => actionPair.ArgsPreferences?.TargetingPreference)
                    .ThenBy(actionPair => actionPair.ArgsPreferences?.MovementTargetingPreference)
                    .ThenBy(actionPair => actionPair.ArgsPreferences?.PositionTargetingPreference);
                foreach (var qValueActionPair in orderedActions)
                {
                    switch (qValueActionPair.CommandId)
                    {
                        case CommandId.None:
                            break;
                        case CommandId.BaseWalk:
                            readableLines.Add($"{qValueActionPair.QValue:F2}\tBaseWalk \t {Enum.GetName(typeof(ArgsMovementTargetingPreference), qValueActionPair.ArgsPreferences.MovementTargetingPreference)}");
                            break;
                        case CommandId.BaseAttack:
                            readableLines.Add($"{qValueActionPair.QValue:F2}\tBaseAttack \t {Enum.GetName(typeof(ArgsEnemyTargetingPreference), qValueActionPair.ArgsPreferences.TargetingPreference)}");
                            break;
                        case CommandId.BaseBlock:
                            readableLines.Add($"{qValueActionPair.QValue:F2}\tBaseBlock");
                            break;
                        case CommandId.Wait:
                            break;
                        case CommandId.DefendAll:
                            if(unitType != BaseCharacterType.Squire) continue;
                            readableLines.Add($"{qValueActionPair.QValue:F2}\tDefendAll");
                            break;
                        case CommandId.Taunt:
                            if(unitType != BaseCharacterType.Squire) continue;
                            readableLines.Add($"{qValueActionPair.QValue:F2}\tTaunt \t {Enum.GetName(typeof(ArgsEnemyTargetingPreference), qValueActionPair.ArgsPreferences.TargetingPreference)}");
                            break;
                        case CommandId.TauntedAttack:
                            readableLines.Add($"{qValueActionPair.QValue:F2}\tTauntedAttack");
                            break;
                        case CommandId.Tackle:
                            if(unitType != BaseCharacterType.Squire) continue;
                            readableLines.Add($"{qValueActionPair.QValue:F2}\tTackle \t {Enum.GetName(typeof(ArgsEnemyTargetingPreference), qValueActionPair.ArgsPreferences.TargetingPreference)}");
                            break;
                        case CommandId.RaiseObstacle:
                            if(unitType != BaseCharacterType.Hunter) continue;
                            readableLines.Add($"{qValueActionPair.QValue:F2}\tRaiseObstacle \t {Enum.GetName(typeof(ArgsPositionTargetingPreference), qValueActionPair.ArgsPreferences.PositionTargetingPreference)}");
                            break;
                        case CommandId.HeavyAttack:
                            if(unitType != BaseCharacterType.Hunter) continue;
                            readableLines.Add($"{qValueActionPair.QValue:F2}\tHeavyAttack \t {Enum.GetName(typeof(ArgsEnemyTargetingPreference), qValueActionPair.ArgsPreferences.TargetingPreference)}");
                            break;
                        case CommandId.Teleport:
                            if(unitType != BaseCharacterType.Hunter) continue;
                            readableLines.Add($"{qValueActionPair.QValue:F2}\tTeleport \t {Enum.GetName(typeof(ArgsMovementTargetingPreference), qValueActionPair.ArgsPreferences.MovementTargetingPreference)}");
                            break;
                        case CommandId.HealOne:
                            if(unitType != BaseCharacterType.Scholar) continue;
                            readableLines.Add($"{qValueActionPair.QValue:F2}\tHealOne");
                            break;
                        case CommandId.HealAll:
                            if(unitType != BaseCharacterType.Scholar) continue;
                            readableLines.Add($"{qValueActionPair.QValue:F2}\tHealAll");
                            break;
                        case CommandId.ShieldPosition:
                            if(unitType != BaseCharacterType.Scholar) continue;
                            readableLines.Add($"{qValueActionPair.QValue:F2}\tShieldPosition \t {Enum.GetName(typeof(ArgsPositionTargetingPreference), qValueActionPair.ArgsPreferences.PositionTargetingPreference)}");
                            break;
                        case CommandId.LowerEnemyDefense:
                            if(unitType != BaseCharacterType.Scholar) continue;
                            readableLines.Add($"{qValueActionPair.QValue:F2}\tLowerEnemyDefense \t {Enum.GetName(typeof(ArgsEnemyTargetingPreference), qValueActionPair.ArgsPreferences.TargetingPreference)}");
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                
                readableLines.Add("\n");
            }
            
            SimulatorDebugData.SaveToFileSummary( readableLines, $"readableQTable","readableQTable", true);
            return Task.CompletedTask;
        }

        private int GetCommandOrder(CommandId commandId)
        {
            switch (commandId)
            {
                case CommandId.None:
                    return 0;
                case CommandId.BaseWalk:
                    return 1;
                case CommandId.BaseAttack:
                    return 2;
                case CommandId.BaseBlock:
                    return 3;
                case CommandId.Wait:
                    return 4;
                case CommandId.DefendAll:
                    return 5;
                case CommandId.Taunt:
                    return 6;
                case CommandId.Tackle:
                    return 8;
                case CommandId.RaiseObstacle:
                    return 9;
                case CommandId.HeavyAttack:
                    return 10;
                case CommandId.Teleport:
                    return 11;
                case CommandId.HealOne:
                    return 12;
                case CommandId.HealAll:
                    return 13;
                case CommandId.ShieldPosition:
                    return 14;
                case CommandId.LowerEnemyDefense:
                    return 15;
                case CommandId.TauntedAttack:
                    return 100;
                default:
                    throw new ArgumentOutOfRangeException(nameof(commandId), commandId, null);
            }
        }
    }

    internal class TestProcessModule : IInternalRunnerModule
    {
        public Task Run()
        {
            
            var fileContent = File.ReadAllText("E:\\_RepositoryE\\SMUBE\\Output\\JsonConfigs\\QTable\\log_2024_5_25_17_18_7_loopId0_winRate55,57407.txt");
            var config = JsonConvert.DeserializeObject<QValueData>(fileContent);
            var reserialized = JsonConvert.SerializeObject(config);
            
            SimulatorDebugData.SaveToFileSummary(new List<string>{fileContent}, $"reserialize-source","reserialized-group", true);
            SimulatorDebugData.SaveToFileSummary(new List<string>{reserialized}, $"reserialize-target","reserialized-group", true);

            Console.WriteLine("press anything to continue");
            Console.ReadKey();
            return Task.CompletedTask;
        }
    }
}
