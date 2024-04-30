using SMUBE.AI.BehaviorTree;
using SMUBE.Core;
using SMUBE.Units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace SMUBE_Utils.Simulator.InternalRunner.Modules.GameSimulator
{
    internal class BattleCoreSimulationWrapper
    {
        private BattleCore _core;
        public BattleCore Core => _core;

        private int turnCounter = 0;

        private static long teamOneAICommandTime = 0;
        private static long teamTwoAICommandTime = 0;

        private static long teamOneAIArgsTime = 0;
        private static long teamTwoAIArgsTime = 0;

        private static long teamOneActions = 0;
        private static long teamTwoActions = 0;

        private static int totalSimulationCount = 0;
        private static int team1WinCount = 0;
        private static int team2WinCount = 0;
        
        private static bool prewarm = false;

        private List<(ICommand, CommandArgs)> CurrentListOfActions = new List<(ICommand, CommandArgs)>();
        
        public void SetupSimulation(List<Unit> initialUnits)
        {
            totalSimulationCount++;
            turnCounter = 0;
            _core = new BattleCore(initialUnits);
        }

        public void RestartDebugCounters()
        {
            teamOneAICommandTime = 0;
            teamTwoAICommandTime = 0;

            teamOneAIArgsTime = 0;
            teamTwoAIArgsTime = 0;

            teamOneActions = 0;
            teamTwoActions = 0;
            turnCounter = 0;

            totalSimulationCount = 0;
            team1WinCount = 0;
            team2WinCount = 0;

            UnitHelper.ScholarCount = 0;
            UnitHelper.HunterCount = 0;
            UnitHelper.SquireCount = 0;

            BattleStateModel.FailedCommandExecutions = 0;
            
            BaseAttack.UseCounter = 0;
            BaseBlock.UseCounter = 0;
            BaseWalk.UseCounter = 0;
            
            HealAll.UseCounter = 0;
            LowerEnemyDefense.UseCounter = 0;
            ShieldPosition.UseCounter = 0;

            HeavyAttack.UseCounter = 0;
            RaiseObstacle.UseCounter = 0;
            Teleport.UseCounter = 0;

            Tackle.UseCounter = 0;
            Taunt.UseCounter = 0;
            DefendAll.UseCounter = 0;
            
            TauntedAttack.UseCounter = 0;
            Wait.UseCounter = 0;
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
                team1WinCount++;
            else
                team2WinCount++;

        }

        public void OnFinishedLog(AIModel team1AI, AIModel team2AI)
        {
            Console.WriteLine($"-- -- -- -- -- --");
            Console.WriteLine($"-- END OF GAME --");
            Console.WriteLine($"-- -- -- -- -- --");

            var team1AIName = team1AI.GetType().Name;
            var team2AIName = team2AI.GetType().Name;

            if (totalSimulationCount == 1)
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
            Console.WriteLine($"team 1 ai ({team1AIName}) runtime: command {teamOneAICommandTime}ticks / args {teamOneAIArgsTime}ticks / total {teamOneAICommandTime + teamOneAIArgsTime}");
            Console.WriteLine($"team 2 ai ({team2AIName}) runtime: command {teamTwoAICommandTime}ticks / args {teamTwoAIArgsTime}ticks / total {teamTwoAICommandTime + teamTwoAIArgsTime}");
            Console.WriteLine($"team 1 actions: {teamOneActions}");
            Console.WriteLine($"team 2 actions: {teamTwoActions}");
            Console.WriteLine($"total actions: {teamOneActions + teamTwoActions}");

            Console.WriteLine($"team 1 ticks per action: {(float)(teamOneAICommandTime + teamOneAIArgsTime) / teamOneActions}");
            Console.WriteLine($"team 2 ticks per action: {(float)(teamTwoAICommandTime + teamTwoAIArgsTime) / teamTwoActions}");

            if (totalSimulationCount > 1)
            {
                Console.WriteLine($"team 1 win rate: {(float)team1WinCount/totalSimulationCount * 100}%");
                Console.WriteLine($"team 2 win rate: {(float)team2WinCount/totalSimulationCount * 100}%");
                
                Console.WriteLine($"total support character type count: {UnitHelper.ScholarCount}");
                Console.WriteLine($"total offensive character type count: {UnitHelper.HunterCount}");
                Console.WriteLine($"total defensive character type count: {UnitHelper.SquireCount}");
            }
            
            Console.WriteLine($"{nameof(BaseAttack)} uses:\t{BaseAttack.UseCounter}");
            Console.WriteLine($"{nameof(BaseBlock)} uses:\t{BaseBlock.UseCounter}");
            Console.WriteLine($"{nameof(BaseWalk)} uses:\t{BaseWalk.UseCounter}");
            
            Console.WriteLine($"{nameof(HealAll)} uses:\t{HealAll.UseCounter}");
            Console.WriteLine($"{nameof(LowerEnemyDefense)} uses:\t{LowerEnemyDefense.UseCounter}");
            Console.WriteLine($"{nameof(ShieldPosition)} uses:\t{ShieldPosition.UseCounter}");

            Console.WriteLine($"{nameof(HeavyAttack)} uses:\t{HeavyAttack.UseCounter}");
            Console.WriteLine($"{nameof(RaiseObstacle)} uses:\t{RaiseObstacle.UseCounter}");
            Console.WriteLine($"{nameof(Teleport)} uses:\t{Teleport.UseCounter}");

            Console.WriteLine($"{nameof(Tackle)} uses:\t{Tackle.UseCounter}");
            Console.WriteLine($"{nameof(Taunt)} uses:\t{Taunt.UseCounter}");
            Console.WriteLine($"{nameof(DefendAll)} uses:\t{DefendAll.UseCounter}");

            Console.WriteLine($"{nameof(TauntedAttack)} uses:\t{TauntedAttack.UseCounter}");
            Console.WriteLine($"{nameof(Wait)} uses:\t{Wait.UseCounter}");

            var totalCommandUses = BaseAttack.UseCounter + BaseBlock.UseCounter + BaseWalk.UseCounter 
                                   + DefendAll.UseCounter + HealAll.UseCounter + HeavyAttack.UseCounter 
                                   + LowerEnemyDefense.UseCounter + RaiseObstacle.UseCounter + ShieldPosition.UseCounter 
                                   + Tackle.UseCounter + Taunt.UseCounter + Teleport.UseCounter 
                                   + TauntedAttack.UseCounter + Wait.UseCounter;
            
            Console.WriteLine($"Total uses:\t{totalCommandUses}");
            Console.WriteLine($"Failed turns: {BattleStateModel.FailedCommandExecutions}");
            Console.WriteLine($"(Total actions - total command use count) diff: {teamOneActions + teamTwoActions - totalCommandUses}");

            if(team1AIName == team2AIName)
            {
                var aiTotalRuntime = teamOneAICommandTime + teamTwoAICommandTime;
                var aiTotalArgsRuntime = teamOneAIArgsTime + teamTwoAIArgsTime;

                Console.WriteLine($"ai ({team1AIName}) runtime: command {teamOneAICommandTime}ticks / args {teamOneAIArgsTime}ticks / total {teamOneAICommandTime + teamOneAIArgsTime}");
                Console.WriteLine($"ai ticks per action: {(float)(aiTotalRuntime + aiTotalArgsRuntime) / (teamOneActions + teamTwoActions)}");
            }
        }

        public void LogTurnInfo()
        {
            Console.WriteLine($"\n-- -- -- -- -- --");
            Console.WriteLine($"-- TURN {turnCounter} --");
            Console.WriteLine($"-- -- -- -- -- --");
        }

        public void AutoResolveTurn(bool log = true)
        {
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
                while (nextArgs == null)
                {
                    commandStopwatch.Start();
                    nextCommand = unit.AiModel.ResolveNextCommand(_core.currentStateModel, unit.UnitData.UnitIdentifier);
                    commandStopwatch.Stop();
                    argsStopwatch.Start();
                    nextArgs = unit.AiModel.GetCommandArgs(nextCommand, _core.currentStateModel, unit.UnitData.UnitIdentifier);
                    argsStopwatch.Stop();
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
                    teamOneAICommandTime += commandStopwatch.ElapsedTicks;
                    teamOneAIArgsTime += argsStopwatch.ElapsedTicks;
                }
                else
                {
                    teamTwoAICommandTime += commandStopwatch.ElapsedTicks;
                    teamTwoAIArgsTime += argsStopwatch.ElapsedTicks;
                }
            }
            
            if (unit.UnitData.UnitIdentifier.TeamId == 0)
            {
                teamOneActions++;
            }
            else
            {
                teamTwoActions++;
            }

            if (!(unit.AiModel is BehaviorTreeAIModel))
            {
                _core.currentStateModel.ExecuteCommand(nextCommand, nextArgs);
            }
            
            CurrentListOfActions.Add((nextCommand, nextArgs));

            if (log)
            {
                LogUnitSummary();
            }
            OnTurnEnded();
        }

        public void OnTurnEnded()
        {
            turnCounter++;
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
