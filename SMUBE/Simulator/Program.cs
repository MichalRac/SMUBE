using Commands;
using SMUBE.AI;
using SMUBE.AI.BehaviorTree;
using SMUBE.AI.DecisionTree;
using SMUBE.AI.GoalOrientedBehavior;
using SMUBE.AI.StateMachine;
using SMUBE.BattleState;
using SMUBE.Core;
using SMUBE.Units;
using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE_Utils.Simulator
{
    public class Program
    {
        static void Main(string[] args)
        {
            Init();
        }

        private static BattleCore _core;
        static Func<AIModel> team1AIModelProvider = null;
        static Func<AIModel> team2AIModelProvider = null;

        private static void Init(bool autoInit = false)
        {
            const bool useSimpleBehavior = true;

            List<Unit> initUnits;
            if (autoInit)
            {
                 initUnits = new List<Unit>
                {
                    UnitHelper.CreateUnit<Squire>(0, team1AIModelProvider?.Invoke(), useSimpleBehavior),
                    UnitHelper.CreateUnit<Hunter>(0, team1AIModelProvider?.Invoke(), useSimpleBehavior),
                    UnitHelper.CreateUnit<Scholar>(0, team1AIModelProvider?.Invoke(), useSimpleBehavior),

                    UnitHelper.CreateUnit<Squire>(1, team2AIModelProvider?.Invoke(), useSimpleBehavior),
                    UnitHelper.CreateUnit<Hunter>(1, team2AIModelProvider?.Invoke(), useSimpleBehavior),
                    UnitHelper.CreateUnit<Scholar>(1, team2AIModelProvider?.Invoke(), useSimpleBehavior),
                };

                _core = new BattleCore(initUnits);
                return;

            }

            Console.WriteLine("Team 1 AI:");
            Console.WriteLine("1. Random AI");
            Console.WriteLine("2. Decision Tree AI");
            Console.WriteLine("3. Goal Oriented Behavior AI");
            Console.WriteLine("4. Finite State Machine AI");
            Console.WriteLine("5. Behavior Tree AI");

            Console.WriteLine("\nChoice:");
            var key = Console.ReadKey(true);


            switch (key.Key)
            {
                case ConsoleKey.D1:
                    team1AIModelProvider = () => new RandomAIModel(useSimpleBehavior);
                    break;
                case ConsoleKey.D2:
                    team1AIModelProvider = () => new DecisionTreeAIModel(useSimpleBehavior);
                    break;
                case ConsoleKey.D3:
                    team1AIModelProvider = () => new GoalOrientedBehaviorAIModel(useSimpleBehavior);
                    break;
                case ConsoleKey.D4:
                    team1AIModelProvider = () => new StateMachineAIModel(null, useSimpleBehavior);
                    break;
                case ConsoleKey.D5:
                    team1AIModelProvider = () => new BehaviorTreeAIModel(useSimpleBehavior);
                    break;
                case ConsoleKey.D0:
                    return;
                default:
                    Init();
                    break;
            }
            /////////////////////////////////
            Console.WriteLine("Team 2 AI:");
            Console.WriteLine("1. Random AI");
            Console.WriteLine("2. Decision Tree AI");
            Console.WriteLine("3. Goal Oriented Behavior AI");
            Console.WriteLine("4. Finite State Machine AI");
            Console.WriteLine("5. Behavior Tree AI");
            Console.WriteLine("\nChoice:");
            var key2 = Console.ReadKey(true);

            switch (key2.Key)
            {
                case ConsoleKey.D1:
                    team2AIModelProvider = () => new RandomAIModel(useSimpleBehavior);
                    break;
                case ConsoleKey.D2:
                    team2AIModelProvider = () => new DecisionTreeAIModel(useSimpleBehavior);
                    break;
                case ConsoleKey.D3:
                    team2AIModelProvider = () => new GoalOrientedBehaviorAIModel(useSimpleBehavior);
                    break;
                case ConsoleKey.D4:
                    team2AIModelProvider = () => new StateMachineAIModel(null, useSimpleBehavior);
                    break;
                case ConsoleKey.D5:
                    team2AIModelProvider = () => new BehaviorTreeAIModel(useSimpleBehavior);
                    break;
                case ConsoleKey.D0:
                    return;
                default:
                    Init();
                    break;
            }

            initUnits =new List<Unit>
            {
                UnitHelper.CreateUnit<Squire>(0, team1AIModelProvider?.Invoke(), useSimpleBehavior),
                UnitHelper.CreateUnit<Hunter>(0, team1AIModelProvider?.Invoke(), useSimpleBehavior),
                UnitHelper.CreateUnit<Scholar>(0, team1AIModelProvider?.Invoke(), useSimpleBehavior),

                UnitHelper.CreateUnit<Squire>(1, team2AIModelProvider?.Invoke(), useSimpleBehavior),
                UnitHelper.CreateUnit<Hunter>(1, team2AIModelProvider?.Invoke(), useSimpleBehavior),
                UnitHelper.CreateUnit<Scholar>(1, team2AIModelProvider?.Invoke(), useSimpleBehavior),
            };

            _core = new BattleCore(initUnits);

            TurnChoice();

            return;
        }

        private static int turnCounter = 0;
        private static int simulationsRun = 0;
        private static void TurnChoice()
        {
            void turn_log()
            {
                Console.WriteLine($"\n-- -- -- -- -- --");
                Console.WriteLine($"-- TURN {turnCounter} --");
                Console.WriteLine($"-- -- -- -- -- --");
            }

            turn_log();
            Console.WriteLine("Options:");
            Console.WriteLine("1. Continue");
            Console.WriteLine("2. Auto-continue");
            Console.WriteLine("3. Show full unit summary");
            Console.WriteLine("4. Show short unit summary");
            Console.WriteLine("5. Show queue");
            Console.WriteLine("6. Run n simulations with this setup");
            Console.WriteLine("0. Close");

            Console.WriteLine("\nChoice:");
            var key = Console.ReadKey(true);
            Console.Write("\n");

            switch (key.Key)
            {
                case ConsoleKey.D1:
                    var gameResolved = ResolveTurn();
                    if(gameResolved)
                    {
                        Finish();
                    }
                    else
                    {
                        TurnChoice();
                    }
                    break;
                case ConsoleKey.D2:
                    var gameAutoResolved = false;
                    while (!gameAutoResolved)
                    {
                        gameAutoResolved = ResolveTurn();
                        if (!gameAutoResolved)
                        {
                            turn_log();
                        }
                    }
                    Finish();
                    break;
                case ConsoleKey.D3:
                    GetUnitSummary(0, false);
                    TurnChoice();
                    break;
                case ConsoleKey.D4:
                    GetUnitSummary(0, true);
                    TurnChoice();
                    break;
                case ConsoleKey.D5:
                    _core.currentStateModel.GetUnitQueueShallowCopy(out var queueCopy);
                    Console.WriteLine("Current queue:");
                    int place = 1;
                    foreach (var queueEntry in queueCopy)
                    {
                        Console.WriteLine($"\t{place++}. {queueEntry.UnitData.Name}, archetype {queueEntry.UnitData.UnitStats.BaseCharacter.GetType().Name}");
                    }
                    TurnChoice();
                    break;
                case ConsoleKey.D6:
                    int get_simulation_number()
                    {
                        Console.WriteLine("\nNumber of simulations to be run:");
                        var read = Console.ReadLine();
                        if(!int.TryParse(read, out var number))
                        {
                            Console.WriteLine("\nNon int value provided! Try again!");
                            return get_simulation_number();
                        }
                        return number;
                    }

                    var simulationNumber = get_simulation_number();

                    var simulationAutoResolved = false;
                    while (simulationsRun++ < simulationNumber)
                    {
                        while (!simulationAutoResolved)
                        {
                            simulationAutoResolved = ResolveTurn(false);
                        }
                        if(simulationsRun % 250 == 0)
                        {
                            Console.WriteLine($"simulation {simulationsRun}/{simulationNumber}");
                        }
                        simulationAutoResolved = false;
                        Init(true);
                    }

                    FinishSimulationSeries();
                    return;
                case ConsoleKey.D0:
                    return;
                default:
                    TurnChoice();
                    break;
            }
        }

        private static void Finish()
        {
            if(_core.currentStateModel.IsFinished(out var winnerTeam))
            {
                Console.WriteLine($"-- -- -- -- -- --");
                Console.WriteLine($"-- END OF GAME --");
                Console.WriteLine($"-- -- -- -- -- --");

                Console.WriteLine($"Winner team: {winnerTeam}");
                Console.WriteLine($"team 1 ai ({team1AIModelProvider.Invoke().GetType().Name}) runtime: command {teamOneAICommandTime}ticks / args {teamOneAIArgsTime}ticks / total {teamOneAICommandTime + teamOneAIArgsTime}");
                Console.WriteLine($"team 2 ai ({team2AIModelProvider.Invoke().GetType().Name}) runtime: command {teamTwoAICommandTime}ticks / args {teamTwoAIArgsTime}ticks / total {teamTwoAICommandTime + teamTwoAIArgsTime}");
                Console.WriteLine($"team 1 actions: {teamOneActions}");
                Console.WriteLine($"team 2 actions: {teamTwoActions}");

                Console.WriteLine($"team 1 ticks per action: {(float)(teamOneAICommandTime + teamOneAIArgsTime) / teamOneActions}");
                Console.WriteLine($"team 2 ticks per action: {(float)(teamTwoAICommandTime + teamTwoAIArgsTime) / teamTwoActions}");

                if(team1AIModelProvider.Invoke().GetType().Name == team2AIModelProvider.Invoke().GetType().Name)
                {
                    var aiTotalRuntime = teamOneAICommandTime + teamTwoAICommandTime;
                    var aiTotalArgsRuntime = teamOneAIArgsTime + teamTwoAIArgsTime;

                    Console.WriteLine($"ai ({team1AIModelProvider.Invoke().GetType().Name}) runtime: command {teamOneAICommandTime}ticks / args {teamOneAIArgsTime}ticks / total {teamOneAICommandTime + teamOneAIArgsTime}");
                    Console.WriteLine($"ai ticks per action: {(float)(aiTotalRuntime + aiTotalArgsRuntime) / (teamOneActions + teamTwoActions)}");
                }
            }
            else
            {
                Console.WriteLine($"ERROR, ended an unfinished game!");
            }

            Console.WriteLine("Press r to retry, press anything else to quit");
            var key = Console.ReadKey();

            if (key.Key == ConsoleKey.R)
            {
                Console.Clear();
                teamOneAICommandTime = 0;
                teamTwoAICommandTime = 0;

                teamOneAIArgsTime = 0;
                teamTwoAIArgsTime = 0;

                teamOneActions = 0;
                teamTwoActions = 0;
                turnCounter = 0;

                Init();
            }
            return;
        }

        private static void FinishSimulationSeries()
        {
            Console.WriteLine($"-- -- -- -- -- -- -- --");
            Console.WriteLine($"-- END OF SIMULATION --");
            Console.WriteLine($"-- -- -- -- -- -- -- --");

            Console.WriteLine($"team 1 ai ({team1AIModelProvider.Invoke().GetType().Name}) runtime: command {teamOneAICommandTime}ticks / args {teamOneAIArgsTime}ticks / total {teamOneAICommandTime + teamOneAIArgsTime}");
            Console.WriteLine($"team 2 ai ({team2AIModelProvider.Invoke().GetType().Name}) runtime: command {teamTwoAICommandTime}ticks / args {teamTwoAIArgsTime}ticks / total {teamTwoAICommandTime + teamTwoAIArgsTime}");
            Console.WriteLine($"team 1 actions: {teamOneActions}");
            Console.WriteLine($"team 2 actions: {teamTwoActions}");

            Console.WriteLine($"team 1 ticks per action: {(float)(teamOneAICommandTime + teamOneAIArgsTime) / teamOneActions}");
            Console.WriteLine($"team 2 ticks per action: {(float)(teamTwoAICommandTime + teamTwoAIArgsTime) / teamTwoActions}");

            if (team1AIModelProvider.Invoke().GetType().Name == team2AIModelProvider.Invoke().GetType().Name)
            {
                var aiTotalRuntime = teamOneAICommandTime + teamTwoAICommandTime;
                var aiTotalArgsRuntime = teamOneAIArgsTime + teamTwoAIArgsTime;

                Console.WriteLine($"ai ({team1AIModelProvider.Invoke().GetType().Name}) runtime: command {teamOneAICommandTime}ticks / args {teamOneAIArgsTime}ticks / total {teamOneAICommandTime + teamOneAIArgsTime}");
                Console.WriteLine($"ai ticks per action: {(float)(aiTotalRuntime + aiTotalArgsRuntime) / (teamOneActions + teamTwoActions)}");
            }


            Console.WriteLine("\nPress anything to quit");
            var key = Console.ReadKey();
        }

        private static long teamOneAICommandTime = 0;
        private static long teamTwoAICommandTime = 0;

        private static long teamOneAIArgsTime = 0;
        private static long teamTwoAIArgsTime = 0;

        private static long teamOneActions = 0;
        private static long teamTwoActions = 0;

        private static bool prewarm = false;

        private static bool ResolveTurn(bool log = true)
        {
            if (!_core.currentStateModel.GetNextActiveUnit(out var unit))
            {
                Console.WriteLine("ERROR - no units in the queue");
                return false;
            }

            var commandStopwatch = new Stopwatch();
            var argsStopwatch = new Stopwatch();
            ICommand nextCommand = null;
            CommandArgs nextArgs = null;

            if(unit.AiModel is BehaviorTreeAIModel)
            {
                commandStopwatch.Start();
                nextCommand = unit.AiModel.ResolveNextCommand(_core.currentStateModel, unit.UnitData.UnitIdentifier);
                commandStopwatch.Stop();

                if(log)
                {
                    Console.WriteLine($"Unit {unit.UnitData.ToShortString()}");
                    Console.WriteLine($"Used {nextCommand.GetType().Name}");
                }
            }
            else
            {
                commandStopwatch.Start();
                nextCommand = unit.AiModel.ResolveNextCommand(_core.currentStateModel, unit.UnitData.UnitIdentifier);
                commandStopwatch.Stop();
                argsStopwatch.Start();
                nextArgs = unit.AiModel.GetCommandArgs(nextCommand, _core.currentStateModel, unit.UnitData.UnitIdentifier);
                argsStopwatch.Stop();

                if (log)
                {
                    if (nextArgs.TargetUnits != null && nextArgs.TargetUnits.Count > 0)
                    {
                        Console.WriteLine($"\nUnit {unit.UnitData.Name} Used {nextCommand.GetType().Name} on {nextArgs.TargetUnits[0].Name}\n");
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
                    teamOneActions++;
                }
                else
                {
                    teamTwoAICommandTime += commandStopwatch.ElapsedTicks;
                    teamTwoAIArgsTime += argsStopwatch.ElapsedTicks;
                    teamTwoActions++;
                }
            }

            if (!(unit.AiModel is BehaviorTreeAIModel))
            {
                _core.currentStateModel.ExecuteCommand(nextCommand, nextArgs);
            }

            if(log)
            {
                Console.WriteLine("Unit Summary:");
                for (int i = 0; i < _core.currentStateModel.Units.Count; i++)
                {
                    Console.WriteLine(write_unit_summary(_core.currentStateModel.Units[i]));
                    string write_unit_summary(Unit loggedUnit)
                    {
                        var unitCache = loggedUnit;
                        var unitStats = unitCache.UnitData.UnitStats;
                        return $"{unitCache.UnitData.Name} {unitCache.UnitData.UnitStats.BaseCharacter.GetType().Name}\thp{unitStats.CurrentHealth}/{unitStats.MaxHealth}\tsp{unitStats.CurrentStamina}/{unitStats.MaxStamina}   \tmp{unitStats.CurrentMana}/{unitStats.MaxMana}";
                    }
                }
            }

            turnCounter++;
            return _core.currentStateModel.IsFinished(out var _);
        }

        private static void GetUnitSummary(int turn, bool showShortSummmary)
        {
            Console.WriteLine(
                "\n---\n" +
                $"Turn {turn}:" +
                "\n");


            for (int i = 0; i < 2; i++)
            {
                Console.WriteLine($"-- -- -- -- --");
                Console.WriteLine($"Team {i}:");
                foreach (var unit in _core.currentStateModel.GetTeamUnits(i))
                {
                    var summary = showShortSummmary ? unit.UnitData.ToShortString() : unit.UnitData.ToString();
                    Console.Write(summary);
                    Console.Write("Available commands:\n");
                    foreach (var viableCommand in unit.ViableCommands)
                    {
                        Console.Write($"\t{viableCommand.GetType().Name}\n");
                    }
                    Console.WriteLine($"-- -- --");
                }
            }
        }
    }
}