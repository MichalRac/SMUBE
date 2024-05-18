using SMUBE.AI.BehaviorTree;
using SMUBE.Core;
using SMUBE.Units;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using SMUBE.AI;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.Commands.Args;
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
using SMUBE.Units.CharacterTypes;

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator
{
    internal class BattleCoreSimulationWrapper
    {
        public const int TURN_TIMEOUT_COUNT = 5000; 

        private BattleCore _core;
        public BattleCore Core => _core;
        
        private static bool prewarm = false;

        public SimulatorDebugData _simulatorDebugData = new SimulatorDebugData();

        private List<(ICommand, CommandArgs)> CurrentListOfActions = new List<(ICommand, CommandArgs)>();
        
        public void SetupSimulation(ConcurrentBag<Unit> initialUnits)
        {
            _simulatorDebugData.tempUnitList.Clear();
            
            _simulatorDebugData.totalSimulationCount++;
            _simulatorDebugData.turnCount = 0;

            _simulatorDebugData.teamOneAiName = initialUnits.First(u => u.UnitData.UnitIdentifier.TeamId == 0).AiModel.GetType().Name;
            _simulatorDebugData.teamTwoAiName = initialUnits.First(u => u.UnitData.UnitIdentifier.TeamId == 1).AiModel.GetType().Name;

            foreach (var unit in initialUnits)
            {
                switch (unit.UnitData.UnitStats.BaseCharacter.BaseCharacterType)
                {
                    case BaseCharacterType.None:
                        break;
                    case BaseCharacterType.Scholar:
                        _simulatorDebugData.ScholarCount++;
                        break;
                    case BaseCharacterType.Squire:
                        _simulatorDebugData.SquireCount++;
                        break;
                    case BaseCharacterType.Hunter:
                        _simulatorDebugData.HunterCount++;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            List<Unit> units = new List<Unit>();
            foreach (var initialUnit in initialUnits)
            {
                units.Add(initialUnit);
                _simulatorDebugData.tempUnitList.Add(initialUnit);
            }
            
            _core = new BattleCore(units);
        }

        public void RestartDebugCounters()
        {
            _simulatorDebugData.teamOneAICommandTime = 0;
            _simulatorDebugData.teamTwoAICommandTime = 0;

            _simulatorDebugData.teamOneAIArgsTime = 0;
            _simulatorDebugData.teamTwoAIArgsTime = 0;

            _simulatorDebugData.teamOneActions = 0;
            _simulatorDebugData.teamTwoActions = 0;
            _simulatorDebugData.turnCount = 0;

            _simulatorDebugData.totalSimulationCount = 0;
            _simulatorDebugData.team1WinCount = 0;
            _simulatorDebugData.team2WinCount = 0;

            _simulatorDebugData.ScholarCount = 0;
            _simulatorDebugData.HunterCount = 0;
            _simulatorDebugData.SquireCount = 0;

            _simulatorDebugData.FailedCommandExecutions = 0;
            
            _simulatorDebugData.BaseAttack_UseCounter = 0;
            _simulatorDebugData.BaseBlock_UseCounter = 0;
            _simulatorDebugData.BaseWalk_UseCounter = 0;
            
            _simulatorDebugData.HealAll_UseCounter = 0;
            _simulatorDebugData.LowerEnemyDefense_UseCounter = 0;
            _simulatorDebugData.ShieldPosition_UseCounter = 0;

            _simulatorDebugData.HeavyAttack_UseCounter = 0;
            _simulatorDebugData.RaiseObstacle_UseCounter = 0;
            _simulatorDebugData.Teleport_UseCounter = 0;

            _simulatorDebugData.Tackle_UseCounter = 0;
            _simulatorDebugData.Taunt_UseCounter = 0;
            _simulatorDebugData.DefendAll_UseCounter = 0;
            
            _simulatorDebugData.TauntedAttack_UseCounter = 0;
            _simulatorDebugData.Wait_UseCounter = 0;
        }
        
        public bool IsFinished(out int winningTeamId)
        {
            return _core.currentStateModel.IsFinished(out winningTeamId);
        }

        public void OnFinished()
        {
            CurrentListOfActions.Clear();
            if (!_core.currentStateModel.IsFinished(out var winningTeamId))
            {
                throw new NotSupportedException();
            }
            
            if (winningTeamId == 0)
                _simulatorDebugData.team1WinCount++;
            else
                _simulatorDebugData.team2WinCount++;

            foreach (var unit in _simulatorDebugData.tempUnitList)
            {
                if (unit.UnitData.UnitIdentifier.TeamId == winningTeamId)
                {
                    switch (unit.UnitData.UnitStats.BaseCharacter.BaseCharacterType)
                    {
                        case BaseCharacterType.Scholar:
                            _simulatorDebugData.ScholarWinCount++;
                            break;
                        case BaseCharacterType.Squire:
                            _simulatorDebugData.SquireWinCount++;
                            break;
                        case BaseCharacterType.Hunter:
                            _simulatorDebugData.HunterWinCount++;
                            break;
                        case BaseCharacterType.None:
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        public void OnFinishedLog(AIModel team1AI, AIModel team2AI, bool logToFile = false)
        {
            Console.WriteLine($"-- -- -- -- -- --");
            Console.WriteLine($"-- END OF GAME --");
            Console.WriteLine($"-- -- -- -- -- --");

            var team1AIName = team1AI.GetType().Name;
            var team2AIName = team2AI.GetType().Name;

            if (_simulatorDebugData.totalSimulationCount == 1)
            {
                if (_core.currentStateModel.IsFinished(out var winningTeamId))
                {
                    Console.WriteLine($"Winner team: {winningTeamId}");
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            
            Console.WriteLine($"team 1 ai ({team1AIName}) runtime: command {_simulatorDebugData.teamOneAICommandTime}ticks / args {_simulatorDebugData.teamOneAIArgsTime}ticks / total {_simulatorDebugData.teamOneAICommandTime + _simulatorDebugData.teamOneAIArgsTime}");
            Console.WriteLine($"team 2 ai ({team2AIName}) runtime: command {_simulatorDebugData.teamTwoAICommandTime}ticks / args {_simulatorDebugData.teamTwoAIArgsTime}ticks / total {_simulatorDebugData.teamTwoAICommandTime + _simulatorDebugData.teamTwoAIArgsTime}");
            Console.WriteLine($"team 1 actions: {_simulatorDebugData.teamOneActions}");
            Console.WriteLine($"team 2 actions: {_simulatorDebugData.teamTwoActions}");
            Console.WriteLine($"total actions: {_simulatorDebugData.teamOneActions + _simulatorDebugData.teamTwoActions}");

            Console.WriteLine($"team 1 ticks per action: {(float)(_simulatorDebugData.teamOneAICommandTime + _simulatorDebugData.teamOneAIArgsTime) / _simulatorDebugData.teamOneActions}");
            Console.WriteLine($"team 2 ticks per action: {(float)(_simulatorDebugData.teamTwoAICommandTime + _simulatorDebugData.teamTwoAIArgsTime) / _simulatorDebugData.teamTwoActions}");

            if (_simulatorDebugData.totalSimulationCount > 1)
            {
                Console.WriteLine($"team 1 win rate: {(float)_simulatorDebugData.team1WinCount/_simulatorDebugData.totalSimulationCount * 100}%");
                Console.WriteLine($"team 2 win rate: {(float)_simulatorDebugData.team2WinCount/_simulatorDebugData.totalSimulationCount * 100}%");
                
                Console.WriteLine($"total support character type count: {_simulatorDebugData.ScholarCount}");
                Console.WriteLine($"total offensive character type count: {_simulatorDebugData.HunterCount}");
                Console.WriteLine($"total defensive character type count: {_simulatorDebugData.SquireCount}");
            }
            
            Console.WriteLine($"{nameof(BaseAttack)} uses:\t{_simulatorDebugData.BaseAttack_UseCounter}");
            Console.WriteLine($"{nameof(BaseBlock)} uses:\t{_simulatorDebugData.BaseBlock_UseCounter}");
            Console.WriteLine($"{nameof(BaseWalk)} uses:\t{_simulatorDebugData.BaseWalk_UseCounter}");
            
            Console.WriteLine($"{nameof(HealAll)} uses:\t{_simulatorDebugData.HealAll_UseCounter}");
            Console.WriteLine($"{nameof(LowerEnemyDefense)} uses:\t{_simulatorDebugData.LowerEnemyDefense_UseCounter}");
            Console.WriteLine($"{nameof(ShieldPosition)} uses:\t{_simulatorDebugData.ShieldPosition_UseCounter}");

            Console.WriteLine($"{nameof(HeavyAttack)} uses:\t{_simulatorDebugData.HeavyAttack_UseCounter}");
            Console.WriteLine($"{nameof(RaiseObstacle)} uses:\t{_simulatorDebugData.RaiseObstacle_UseCounter}");
            Console.WriteLine($"{nameof(Teleport)} uses:\t{_simulatorDebugData.Teleport_UseCounter}");

            Console.WriteLine($"{nameof(Tackle)} uses:\t{_simulatorDebugData.Tackle_UseCounter}");
            Console.WriteLine($"{nameof(Taunt)} uses:\t{_simulatorDebugData.Taunt_UseCounter}");
            Console.WriteLine($"{nameof(DefendAll)} uses:\t{_simulatorDebugData.DefendAll_UseCounter}");

            Console.WriteLine($"{nameof(TauntedAttack)} uses:\t{_simulatorDebugData.TauntedAttack_UseCounter}");
            Console.WriteLine($"{nameof(Wait)} uses:\t{_simulatorDebugData.Wait_UseCounter}");

            var totalCommandUses = _simulatorDebugData.BaseAttack_UseCounter + _simulatorDebugData.BaseBlock_UseCounter + _simulatorDebugData.BaseWalk_UseCounter 
                                   + _simulatorDebugData.DefendAll_UseCounter + _simulatorDebugData.HealAll_UseCounter + _simulatorDebugData.HeavyAttack_UseCounter 
                                   + _simulatorDebugData.LowerEnemyDefense_UseCounter + _simulatorDebugData.RaiseObstacle_UseCounter + _simulatorDebugData.ShieldPosition_UseCounter 
                                   + _simulatorDebugData.Tackle_UseCounter + _simulatorDebugData.Taunt_UseCounter + _simulatorDebugData.Teleport_UseCounter 
                                   + _simulatorDebugData.TauntedAttack_UseCounter + _simulatorDebugData.Wait_UseCounter;
            
            Console.WriteLine($"Total uses:\t{totalCommandUses}");
            Console.WriteLine($"Failed turns: {BattleStateModel.FailedCommandExecutions}");
            Console.WriteLine($"(Total actions - total command use count) diff: {_simulatorDebugData.teamOneActions + _simulatorDebugData.teamTwoActions - totalCommandUses}");

            if(team1AIName == team2AIName)
            {
                var aiTotalRuntime = _simulatorDebugData.teamOneAICommandTime + _simulatorDebugData.teamTwoAICommandTime;
                var aiTotalArgsRuntime = _simulatorDebugData.teamOneAIArgsTime + _simulatorDebugData.teamTwoAIArgsTime;

                Console.WriteLine($"ai ({team1AIName}) runtime: command {_simulatorDebugData.teamOneAICommandTime}ticks / args {_simulatorDebugData.teamOneAIArgsTime}ticks / total {_simulatorDebugData.teamOneAICommandTime + _simulatorDebugData.teamTwoAIArgsTime}");
                Console.WriteLine($"ai ticks per action: {(float)(aiTotalRuntime + aiTotalArgsRuntime) / (_simulatorDebugData.teamOneActions + _simulatorDebugData.teamTwoActions)}");
            }

            if (logToFile)
            {
                var date = DateTime.UtcNow;
                var dateSuffix = $"{date.Year}_{date.Month}_{date.Day}_{date.Hour}_{date.Minute}_{date.Second}";
                var newFile = File.CreateText($"E:\\_RepositoryE\\SMUBE\\Output\\logs\\log_{dateSuffix}_{_simulatorDebugData.totalSimulationCount}sim.txt");
                
                newFile.WriteLine($"Simulation results");
                newFile.WriteLine($"simulations: {_simulatorDebugData.totalSimulationCount}");
                newFile.WriteLine($"date: {dateSuffix}");
                newFile.WriteLine($"\n");
                
                newFile.WriteLine($"team 1 ai ({team1AIName}) runtime: command {_simulatorDebugData.teamOneAICommandTime}ticks / args {_simulatorDebugData.teamOneAIArgsTime}ticks / total {_simulatorDebugData.teamOneAICommandTime + _simulatorDebugData.teamOneAIArgsTime}");
                newFile.WriteLine($"team 2 ai ({team2AIName}) runtime: command {_simulatorDebugData.teamTwoAICommandTime}ticks / args {_simulatorDebugData.teamTwoAIArgsTime}ticks / total {_simulatorDebugData.teamTwoAICommandTime + _simulatorDebugData.teamTwoAIArgsTime}");
                newFile.WriteLine($"team 1 actions: {_simulatorDebugData.teamOneActions}");
                newFile.WriteLine($"team 2 actions: {_simulatorDebugData.teamTwoActions}");
                newFile.WriteLine($"total actions: {_simulatorDebugData.teamOneActions + _simulatorDebugData.teamTwoActions}");
                newFile.WriteLine($"\n");

                newFile.WriteLine($"team 1 ticks per action: {(float)(_simulatorDebugData.teamOneAICommandTime + _simulatorDebugData.teamOneAIArgsTime) / _simulatorDebugData.teamOneActions}");
                newFile.WriteLine($"team 2 ticks per action: {(float)(_simulatorDebugData.teamTwoAICommandTime + _simulatorDebugData.teamTwoAIArgsTime) / _simulatorDebugData.teamTwoActions}");
                newFile.WriteLine($"\n");

                if (_simulatorDebugData.totalSimulationCount > 1)
                {
                    newFile.WriteLine($"team 1 win rate: {(float)_simulatorDebugData.team1WinCount/_simulatorDebugData.totalSimulationCount * 100}%");
                    newFile.WriteLine($"team 2 win rate: {(float)_simulatorDebugData.team2WinCount/_simulatorDebugData.totalSimulationCount * 100}%");
                    newFile.WriteLine($"\n");

                    newFile.WriteLine($"total support character type count: {_simulatorDebugData.ScholarCount}");
                    newFile.WriteLine($"total offensive character type count: {_simulatorDebugData.HunterCount}");
                    newFile.WriteLine($"total defensive character type count: {_simulatorDebugData.SquireCount}");
                }
                
                newFile.WriteLine($"{nameof(BaseAttack)} uses:\t{_simulatorDebugData.BaseAttack_UseCounter}");
                newFile.WriteLine($"{nameof(BaseBlock)} uses:\t{_simulatorDebugData.BaseBlock_UseCounter}");
                newFile.WriteLine($"{nameof(BaseWalk)} uses:\t{_simulatorDebugData.BaseWalk_UseCounter}");
                
                newFile.WriteLine($"{nameof(HealAll)} uses:\t{_simulatorDebugData.HealAll_UseCounter}");
                newFile.WriteLine($"{nameof(LowerEnemyDefense)} uses:\t{_simulatorDebugData.LowerEnemyDefense_UseCounter}");
                newFile.WriteLine($"{nameof(ShieldPosition)} uses:\t{_simulatorDebugData.ShieldPosition_UseCounter}");

                newFile.WriteLine($"{nameof(HeavyAttack)} uses:\t{_simulatorDebugData.HeavyAttack_UseCounter}");
                newFile.WriteLine($"{nameof(RaiseObstacle)} uses:\t{_simulatorDebugData.RaiseObstacle_UseCounter}");
                newFile.WriteLine($"{nameof(Teleport)} uses:\t{_simulatorDebugData.Teleport_UseCounter}");

                newFile.WriteLine($"{nameof(Tackle)} uses:\t{_simulatorDebugData.Tackle_UseCounter}");
                newFile.WriteLine($"{nameof(Taunt)} uses:\t{_simulatorDebugData.Taunt_UseCounter}");
                newFile.WriteLine($"{nameof(DefendAll)} uses:\t{_simulatorDebugData.DefendAll_UseCounter}");

                newFile.WriteLine($"{nameof(TauntedAttack)} uses:\t{_simulatorDebugData.TauntedAttack_UseCounter}");
                newFile.WriteLine($"{nameof(Wait)} uses:\t{_simulatorDebugData.Wait_UseCounter}");
                
                newFile.WriteLine($"Total uses:\t{totalCommandUses}");
                newFile.WriteLine($"Failed turns: {BattleStateModel.FailedCommandExecutions}");
                newFile.WriteLine($"(Total actions - total command use count) diff: {_simulatorDebugData.teamOneActions + _simulatorDebugData.teamTwoActions - totalCommandUses}");
                
                newFile.Close();
            }
        }

        public void LogTurnInfo()
        {
            Console.WriteLine($"\n-- -- -- -- -- --");
            Console.WriteLine($"-- TURN {_simulatorDebugData.turnCount} --");
            Console.WriteLine($"-- -- -- -- -- --");
        }

        public void AutoResolveTurn(bool log = true)
        {
            if (_simulatorDebugData.turnCount > TURN_TIMEOUT_COUNT)
            {
                throw new ApplicationException();
            }

            if (!_core.currentStateModel.GetNextActiveUnit(out var unit))
            {
                Console.WriteLine("ERROR - no units in the queue");
                return;
            }

            var commandStopwatch = new Stopwatch();
            var argsStopwatch = new Stopwatch();
            BaseCommand nextCommand = null;
            CommandArgs nextArgs = null;

            if (unit.AiModel is BehaviorTreeAIModel)
            {
                commandStopwatch.Start();
                nextCommand = unit.AiModel.ResolveNextCommand(_core.currentStateModel, unit.UnitData.UnitIdentifier);
                commandStopwatch.Stop();

                if (log)
                {
                    Console.WriteLine($"Unit {unit.UnitData.ToShortString()}");
                    Console.WriteLine($"Used {nextCommand.GetType().Name}");
                }
            }
            else
            {
                int attempts = 0;
                while (nextArgs == null)
                {
                    attempts++;
                    commandStopwatch.Start();
                    nextCommand = unit.AiModel.ResolveNextCommand(_core.currentStateModel, unit.UnitData.UnitIdentifier);
                    commandStopwatch.Stop();
                    argsStopwatch.Start();
                    nextArgs = unit.AiModel.GetCommandArgs(nextCommand, _core.currentStateModel, unit.UnitData.UnitIdentifier);
                    argsStopwatch.Stop();

                    var check = false;
                    if (check)
                    {
                        _core.currentStateModel.DebugReevaluateCommands();
                    }
                    if (attempts > TURN_TIMEOUT_COUNT)
                    {
                        _simulatorDebugData.FailedCommandExecutions++;
                        throw new ApplicationException();
                    }
                }

                if (log)
                {
                    if (nextArgs.TargetUnits != null && nextArgs.TargetUnits.Count > 0)
                    {
                        Console.WriteLine($"\nUnit {unit.UnitData.Name} Used {nextCommand.GetType().Name} on {nextArgs.TargetUnits[0]?.Name ?? "self"}\n");
                    }
                    else
                    {
                        Console.WriteLine($"\nUnit {unit.UnitData.Name} Used {nextCommand.GetType().Name}");
                    }
                }
            }


            // todo investigate why first lookup on state model from ai model causes additional processing time
            if (!prewarm)
            {
                prewarm = true;
            }
            else
            {
                if (unit.UnitData.UnitIdentifier.TeamId == 0)
                {
                    _simulatorDebugData.teamOneAICommandTime += commandStopwatch.ElapsedTicks;
                    _simulatorDebugData.teamOneAIArgsTime += argsStopwatch.ElapsedTicks;
                }
                else
                {
                    _simulatorDebugData.teamTwoAICommandTime += commandStopwatch.ElapsedTicks;
                    _simulatorDebugData.teamTwoAIArgsTime += argsStopwatch.ElapsedTicks;
                }
            }
            
            if (unit.UnitData.UnitIdentifier.TeamId == 0)
            {
                _simulatorDebugData.teamOneActions++;
            }
            else
            {
                _simulatorDebugData.teamTwoActions++;
            }

            if (!(unit.AiModel is BehaviorTreeAIModel))
            {
                _core.currentStateModel.ExecuteCommand(nextCommand, nextArgs);
            }
            
            _simulatorDebugData.UpdateCommandUse(nextCommand.CommandId);
            
            CurrentListOfActions.Add((nextCommand, nextArgs));

            if (log)
            {
                LogUnitSummary();
            }
            OnTurnEnded();
        }

        public void OnTurnEnded()
        {
            _simulatorDebugData.turnCount++;
        }

        public void LogUnitSummary()
        {
            Console.WriteLine("Unit Summary:");
            var team1Units = _core.currentStateModel.GetTeamUnits(0);
            var team2Units = _core.currentStateModel.GetTeamUnits(1);
            
            foreach (var unit in team1Units)
            {
                Console.WriteLine(write_unit_summary(unit));
            }
            foreach (var unit in team2Units)
            {
                Console.WriteLine(write_unit_summary(unit));
            }
            
            string write_unit_summary(Unit loggedUnit)
            {
                var unitCache = loggedUnit;
                var unitStats = unitCache.UnitData.UnitStats;
                return $"{unitCache.UnitData.Name} {unitCache.UnitData.UnitStats.BaseCharacter.GetType().Name}\t" +
                       $"hp{unitStats.CurrentHealth}/{unitStats.MaxHealth}\tsp{unitStats.CurrentStamina}/{unitStats.MaxStamina}   \t" +
                       $"mp{unitStats.CurrentMana}/{unitStats.MaxMana}\t pos: {unitCache.UnitData.BattleScenePosition}";
            }
        }
    }
}
