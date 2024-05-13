using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.Commands.SpecificCommands.BaseAttack;
using SMUBE.Commands.SpecificCommands.BaseBlock;
using SMUBE.Commands.SpecificCommands.BaseWalk;
using SMUBE.Commands.SpecificCommands.DefendAll;
using SMUBE.Commands.SpecificCommands.HealAll;
using SMUBE.Commands.SpecificCommands.HeavyAttack;
using SMUBE.Commands.SpecificCommands.LowerEnemyDefense;
using SMUBE.Commands.SpecificCommands.RaiseObstacle;
using SMUBE.Commands.SpecificCommands.ShieldPosition;
using SMUBE.Commands.SpecificCommands.Tackle;
using SMUBE.Commands.SpecificCommands.Taunt;
using SMUBE.Commands.SpecificCommands.Teleport;
using SMUBE.Commands.SpecificCommands.Wait;
using SMUBE.Units;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator
{
    public class SimulatorDebugData
    {
        public string teamOneAiName = "unknown";
        public string teamTwoAiName = "unknown";
        
        // general
        public int totalSimulationCount = 0;

        public long teamOneActions = 0;
        public long teamTwoActions = 0;
        
        public int team1WinCount = 0;
        public int team2WinCount = 0;
        
        public int turnCount = 0;

        // performance
        public long teamOneAICommandTime = 0;
        public long teamTwoAICommandTime = 0;

        public long teamOneAIArgsTime = 0;
        public long teamTwoAIArgsTime = 0;
        
        // setup
        public int ScholarCount = 0;
        public int HunterCount = 0;
        public int SquireCount = 0;
        
        public int ScholarWinCount = 0;
        public int HunterWinCount = 0;
        public int SquireWinCount = 0;
        
        // commands
        public int FailedCommandExecutions = 0;
            
        public int BaseAttack_UseCounter = 0;
        public int BaseBlock_UseCounter = 0;
        public int BaseWalk_UseCounter = 0;
            
        public int HealOne_UseCounter = 0;
        public int HealAll_UseCounter = 0;
        public int LowerEnemyDefense_UseCounter = 0;
        public int ShieldPosition_UseCounter = 0;

        public int HeavyAttack_UseCounter = 0;
        public int RaiseObstacle_UseCounter = 0;
        public int Teleport_UseCounter = 0;

        public int Tackle_UseCounter = 0;
        public int Taunt_UseCounter = 0;
        public int DefendAll_UseCounter = 0;
            
        public int TauntedAttack_UseCounter = 0;
        public int Wait_UseCounter = 0;
        
        private long GetTotalCommandUseCount => BaseAttack_UseCounter + BaseBlock_UseCounter + BaseWalk_UseCounter 
                                               + DefendAll_UseCounter + HealAll_UseCounter + HeavyAttack_UseCounter + HealOne_UseCounter
                                               + LowerEnemyDefense_UseCounter + RaiseObstacle_UseCounter + ShieldPosition_UseCounter 
                                               + Tackle_UseCounter + Taunt_UseCounter + Teleport_UseCounter 
                                               + TauntedAttack_UseCounter + Wait_UseCounter;

        public List<Unit> tempUnitList = new List<Unit>();
        
        public SimulatorDebugData(){}

        public SimulatorDebugData(ConcurrentBag<SimulatorDebugData> aggregatedDebugData)
        {
            teamOneAiName = aggregatedDebugData.First().teamOneAiName;
            teamTwoAiName = aggregatedDebugData.First().teamTwoAiName;
            
            foreach (var debugData in aggregatedDebugData)
            {
                if (teamOneAiName != debugData.teamOneAiName || teamTwoAiName != debugData.teamTwoAiName)
                {
                    Console.WriteLine($"Incompatible data attempted to be aggregated!");
                    throw new ArgumentException();
                }
                
                totalSimulationCount += debugData.totalSimulationCount;

                teamOneActions += debugData.teamOneActions;
                teamTwoActions += debugData.teamTwoActions;
                
                team1WinCount += debugData.team1WinCount;
                team2WinCount += debugData.team2WinCount;
                
                turnCount += debugData.turnCount;

                // performance
                teamOneAICommandTime += debugData.teamOneAICommandTime;
                teamTwoAICommandTime += debugData.teamTwoAICommandTime;

                teamOneAIArgsTime += debugData.teamOneAIArgsTime;
                teamTwoAIArgsTime += debugData.teamTwoAIArgsTime;
                
                // setup
                ScholarCount += debugData.ScholarCount;
                HunterCount += debugData.HunterCount;
                SquireCount += debugData.SquireCount;
                
                ScholarWinCount += debugData.ScholarWinCount;
                HunterWinCount += debugData.HunterWinCount;
                SquireWinCount += debugData.SquireWinCount;
                
                // commands
                FailedCommandExecutions += debugData.FailedCommandExecutions;
                    
                BaseAttack_UseCounter += debugData.BaseAttack_UseCounter;
                BaseBlock_UseCounter += debugData.BaseBlock_UseCounter;
                BaseWalk_UseCounter += debugData.BaseWalk_UseCounter;
                    
                HealOne_UseCounter += debugData.HealOne_UseCounter;
                HealAll_UseCounter += debugData.HealAll_UseCounter;
                LowerEnemyDefense_UseCounter += debugData.LowerEnemyDefense_UseCounter;
                ShieldPosition_UseCounter += debugData.ShieldPosition_UseCounter;

                HeavyAttack_UseCounter += debugData.HeavyAttack_UseCounter;
                RaiseObstacle_UseCounter += debugData.RaiseObstacle_UseCounter;
                Teleport_UseCounter += debugData.Teleport_UseCounter;

                Tackle_UseCounter += debugData.Tackle_UseCounter;
                Taunt_UseCounter += debugData.Taunt_UseCounter;
                DefendAll_UseCounter += debugData.DefendAll_UseCounter;
                    
                TauntedAttack_UseCounter += debugData.TauntedAttack_UseCounter;
                Wait_UseCounter += debugData.Wait_UseCounter;
            }
        }

        public void UpdateCommandUse(CommandId commandId)
        {
            switch (commandId)
            {
                case CommandId.None:
                    break;
                case CommandId.BaseWalk:
                    BaseWalk_UseCounter++;
                    break;
                case CommandId.BaseAttack:
                    BaseAttack_UseCounter++;
                    break;
                case CommandId.BaseBlock:
                    BaseBlock_UseCounter++;
                    break;
                case CommandId.Wait:
                    Wait_UseCounter++;
                    break;
                case CommandId.DefendAll:
                    DefendAll_UseCounter++;
                    break;
                case CommandId.Taunt:
                    Taunt_UseCounter++;
                    break;
                case CommandId.TauntedAttack:
                    TauntedAttack_UseCounter++;
                    break;
                case CommandId.Tackle:
                    Tackle_UseCounter++;
                    break;
                case CommandId.RaiseObstacle:
                    RaiseObstacle_UseCounter++;
                    break;
                case CommandId.HeavyAttack:
                    HeavyAttack_UseCounter++;
                    break;
                case CommandId.Teleport:
                    Teleport_UseCounter++;
                    break;
                case CommandId.HealOne:
                    HealOne_UseCounter++;
                    break;
                case CommandId.HealAll:
                    HealAll_UseCounter++;
                    break;
                case CommandId.ShieldPosition:
                    ShieldPosition_UseCounter++;
                    break;
                case CommandId.LowerEnemyDefense:
                    LowerEnemyDefense_UseCounter++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(commandId), commandId, null);
            }
        }

        public float GetAverageWinRateTeam1()
        {
            return (float)team1WinCount / totalSimulationCount * 100;
        }
        
        public List<string> GetDebugDataListed()
        {
            var results = new List<string>();
            
            var team1AiRuntime = $"team 1 ai ({teamOneAiName}) runtime: command {teamOneAICommandTime}ticks / args {teamOneAIArgsTime}ticks / total {teamOneAICommandTime + teamOneAIArgsTime}";
            results.Add(team1AiRuntime);
            var team2AiRuntime = $"team 2 ai ({teamTwoAiName}) runtime: command {teamTwoAICommandTime}ticks / args {teamTwoAIArgsTime}ticks / total {teamTwoAICommandTime + teamTwoAIArgsTime}";
            results.Add(team2AiRuntime);
            var team1ActionCount = $"team 1 actions: {teamOneActions}";
            results.Add(team1ActionCount);
            var team2ActionCount = $"team 2 actions: {teamTwoActions}";
            results.Add(team2ActionCount);
            var totalActionCount = $"total actions: {teamOneActions + teamTwoActions}";
            results.Add(totalActionCount);
            
            results.Add("\n");

            var team1TicksPerAction = $"team 1 ticks per action: {(float)(teamOneAICommandTime + teamOneAIArgsTime) / teamOneActions}";
            results.Add(team1TicksPerAction);
            var team2TicksPerAction = $"team 2 ticks per action: {(float)(teamTwoAICommandTime + teamTwoAIArgsTime) / teamTwoActions}";
            results.Add(team2TicksPerAction);
            var team1WinRate = $"team 1 win rate: {(float)team1WinCount/totalSimulationCount * 100}% ({team1WinCount}/{totalSimulationCount})";
            results.Add(team1WinRate);
            var team2WinRate = $"team 2 win rate: {(float)team2WinCount/totalSimulationCount * 100}% ({team2WinCount}/{totalSimulationCount})";
            results.Add(team2WinRate);

            results.Add("\n");
            
            var totalSupportCount = $"total support character type count: {ScholarCount}";
            results.Add(totalSupportCount);
            var totalOffensiveCount = $"total offensive character type count: {HunterCount}";
            results.Add(totalOffensiveCount);
            var totalDefensiveCount = $"total defensive character type count: {SquireCount}";
            results.Add(totalDefensiveCount);
            
            var totalSupportWinCount = $"total support character type win count: {ScholarWinCount} win percentage {(float)ScholarWinCount / ScholarCount * 100}";
            results.Add(totalSupportWinCount);
            var totalOffensiveWinCount = $"total offensive character type win count: {HunterWinCount} win percentage {(float)HunterWinCount / HunterCount * 100}";
            results.Add(totalOffensiveWinCount);
            var totalDefensiveWinCount = $"total defensive character type win count: {SquireWinCount} win percentage {(float)SquireWinCount / SquireCount * 100}";
            results.Add(totalDefensiveWinCount);
                      
            results.Add("\n");
            
            var baseAttackCount = $"{nameof(BaseAttack)} uses:\t{BaseAttack_UseCounter}";
            results.Add(baseAttackCount);
            var baseBlockCount = $"{nameof(BaseBlock)} uses:\t{BaseBlock_UseCounter}";
            results.Add(baseBlockCount);
            var baseWalkCount = $"{nameof(BaseWalk)} uses:\t{BaseWalk_UseCounter}";
            results.Add(baseWalkCount);
            var healAllCount = $"{nameof(HealAll)} uses:\t{HealAll_UseCounter}";
            results.Add(healAllCount);
            var lowerEnemyDefenseCount = $"{nameof(LowerEnemyDefense)} uses:\t{LowerEnemyDefense_UseCounter}";
            results.Add(lowerEnemyDefenseCount);
            var shieldPositionCount = $"{nameof(ShieldPosition)} uses:\t{ShieldPosition_UseCounter}";
            results.Add(shieldPositionCount);
            var heavyAttackCount = $"{nameof(HeavyAttack)} uses:\t{HeavyAttack_UseCounter}";
            results.Add(heavyAttackCount);
            var raiseObstacleCount = $"{nameof(RaiseObstacle)} uses:\t{RaiseObstacle_UseCounter}";
            results.Add(raiseObstacleCount);
            var teleportCount = $"{nameof(Teleport)} uses:\t{Teleport_UseCounter}";
            results.Add(teleportCount);
            var tackleCount = $"{nameof(Tackle)} uses:\t{Tackle_UseCounter}";
            results.Add(tackleCount);
            var tauntCount = $"{nameof(Taunt)} uses:\t{Taunt_UseCounter}";
            results.Add(tauntCount);
            var defendAllCount = $"{nameof(DefendAll)} uses:\t{DefendAll_UseCounter}";
            results.Add(defendAllCount);
            var tauntedAttackCount = $"{nameof(TauntedAttack)} uses:\t{TauntedAttack_UseCounter}";
            results.Add(tauntedAttackCount);
            var waitCount = $"{nameof(Wait)} uses:\t{Wait_UseCounter}";
            results.Add(waitCount);

            results.Add("\n");

            var totalCommandUses = GetTotalCommandUseCount;
            var totalUsesCount = $"Total uses:\t{totalCommandUses}";
            results.Add(totalUsesCount);
            var failedTurnsCount = $"Failed turns: {BattleStateModel.FailedCommandExecutions}";
            results.Add(failedTurnsCount);
            var diffCount = $"(Total actions - total command use count) diff: {teamOneActions + teamTwoActions - totalCommandUses}";
            results.Add(diffCount);

            return results;
        }

        public void PrintToConsole(List<string> results)
        {
            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }

        public void SaveToFile(List<string> results, string nameSuffix, string group)
        {
            var date = DateTime.UtcNow;
            var dateSuffix = $"{date.Year}_{date.Month}_{date.Day}_{date.Hour}_{date.Minute}_{date.Second}";
            var path = $"E:\\_RepositoryE\\SMUBE\\Output\\logs\\";

            if (!Directory.Exists(path + group))
            {
                path = $"{path}\\{group}";
                Directory.CreateDirectory(path);
            }
            else
            {
                path = $"{path}\\{group}";
            }
            
            var newFile = File.CreateText($"{path}\\log_{dateSuffix}_{totalSimulationCount}sim{nameSuffix}.txt");
                
            newFile.WriteLine($"Simulation results \"{group}\"");
            newFile.WriteLine($"simulations: {totalSimulationCount}");
            newFile.WriteLine($"date: {dateSuffix}");
            newFile.WriteLine($"\n");

            foreach (var result in results)
            {
                newFile.WriteLine($"{result}");
            }
            
            newFile.Close();
        }
        
        public static void SaveToFileSummary(List<string> results, string name, string group)
        {
            var date = DateTime.UtcNow;
            var dateSuffix = $"{date.Year}_{date.Month}_{date.Day}_{date.Hour}_{date.Minute}_{date.Second}";
            var path = $"E:\\_RepositoryE\\SMUBE\\Output\\logs\\";

            if (!Directory.Exists(path + group))
            {
                path = $"{path}\\{group}";
                Directory.CreateDirectory(path);
            }
            else
            {
                path = $"{path}\\{group}";
            }
            
            var newFile = File.CreateText($"{path}\\log_{dateSuffix}_{name}.txt");
                
            newFile.WriteLine($"Simulation results \"{group}\"");
            newFile.WriteLine($"date: {dateSuffix}");
            newFile.WriteLine($"\n");

            foreach (var result in results)
            {
                newFile.WriteLine($"{result}");
            }
            
            newFile.Close();
        }
    }
}