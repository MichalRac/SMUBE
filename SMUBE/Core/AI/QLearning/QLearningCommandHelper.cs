using System;
using System.Collections.Generic;
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
using SMUBE.Units;

namespace SMUBE.AI.QLearning
{
    public class QLearningCommandHelper
    {
        public List<BaseCommand> GetSubcommands(Unit actor, bool onlyViable = true)
        {
            var commands = onlyViable 
                ? actor.UnitCommandProvider.ViableCommands
                : actor.UnitCommandProvider.AllCommands;
            var activeUnitViableSubcommands = new List<BaseCommand>();
            
            foreach (var activeUnitCommand in commands)
            {
                switch (activeUnitCommand.CommandId)
                {
                    case CommandId.BaseWalk:
                        activeUnitViableSubcommands.Add(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)0));
                        activeUnitViableSubcommands.Add(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)1));
                        activeUnitViableSubcommands.Add(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)2));
                        activeUnitViableSubcommands.Add(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)3));
                        activeUnitViableSubcommands.Add(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)4));
                        break;
                    case CommandId.BaseAttack:
                        activeUnitViableSubcommands.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.None));
                        activeUnitViableSubcommands.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.Closest));
                        activeUnitViableSubcommands.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints));
                        activeUnitViableSubcommands.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage));
                        activeUnitViableSubcommands.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt));
                        activeUnitViableSubcommands.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange));
                        activeUnitViableSubcommands.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn));
                        activeUnitViableSubcommands.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn));
                        activeUnitViableSubcommands.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn));
                        activeUnitViableSubcommands.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn));
                        break;
                    case CommandId.BaseBlock:
                        activeUnitViableSubcommands.Add(new BaseBlock());
                        break;
                    case CommandId.Wait:
                        activeUnitViableSubcommands.Add(new Wait());
                        break;
                    case CommandId.DefendAll:
                        activeUnitViableSubcommands.Add(new DefendAll());
                        break;
                    case CommandId.Taunt:
                        activeUnitViableSubcommands.Add(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.None));
                        activeUnitViableSubcommands.Add(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.Closest));
                        activeUnitViableSubcommands.Add(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints));
                        activeUnitViableSubcommands.Add(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage));
                        activeUnitViableSubcommands.Add(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange));
                        break;
                    case CommandId.TauntedAttack:
                        activeUnitViableSubcommands.Add(new TauntedAttack());
                        break;
                    case CommandId.Tackle:
                        activeUnitViableSubcommands.Add(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.None));
                        activeUnitViableSubcommands.Add(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.Closest));
                        activeUnitViableSubcommands.Add(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints));
                        activeUnitViableSubcommands.Add(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage));
                        activeUnitViableSubcommands.Add(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange));
                        break;
                    case CommandId.RaiseObstacle:
                        activeUnitViableSubcommands.Add(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.None));
                        activeUnitViableSubcommands.Add(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy));
                        activeUnitViableSubcommands.Add(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.InBetweenEnemies));
                        activeUnitViableSubcommands.Add(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.InBetweenTeams));
                        break;
                    case CommandId.HeavyAttack:
                        activeUnitViableSubcommands.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.None));
                        activeUnitViableSubcommands.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.Closest));
                        activeUnitViableSubcommands.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints));
                        activeUnitViableSubcommands.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage));
                        activeUnitViableSubcommands.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt));
                        activeUnitViableSubcommands.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange));
                        activeUnitViableSubcommands.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn));
                        activeUnitViableSubcommands.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn));
                        activeUnitViableSubcommands.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn));
                        activeUnitViableSubcommands.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn));
                        break;
                    case CommandId.Teleport:
                        activeUnitViableSubcommands.Add(new Teleport().WithPreferences(ArgsMovementTargetingPreference.None)); 
                        activeUnitViableSubcommands.Add(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetOutOfReach)); 
                        activeUnitViableSubcommands.Add(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetCloserCarefully)); 
                        activeUnitViableSubcommands.Add(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetCloserAggressively)); 
                        activeUnitViableSubcommands.Add(new Teleport().WithPreferences(ArgsMovementTargetingPreference.OptimizeFortifiedPosition)); 
                        break;
                    case CommandId.HealAll:
                        activeUnitViableSubcommands.Add(new HealAll());
                        break;
                    case CommandId.ShieldPosition:
                        activeUnitViableSubcommands.Add(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.None));
                        activeUnitViableSubcommands.Add(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnLeastHpPercentageAlly));
                        activeUnitViableSubcommands.Add(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnMostHpPercentageAlly));
                        activeUnitViableSubcommands.Add(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy));
                        activeUnitViableSubcommands.Add(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnAllyWithMostEnemiesInReach));
                        activeUnitViableSubcommands.Add(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.InBetweenTeams));
                        break;
                    case CommandId.LowerEnemyDefense:
                        activeUnitViableSubcommands.Add(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.None));
                        activeUnitViableSubcommands.Add(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.Closest));
                        activeUnitViableSubcommands.Add(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints));
                        activeUnitViableSubcommands.Add(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage));
                        activeUnitViableSubcommands.Add(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange));
                        break;
                    case CommandId.None:
                    case CommandId.HealOne:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return activeUnitViableSubcommands;
        }
        
        
        public BaseCommand RetrieveCommand(CommandId commandId, ArgsPreferences argsPreferences)
        {
                switch (commandId)
                {
                    case CommandId.BaseWalk:
                        return new BaseWalk().WithPreferences(argsPreferences.MovementTargetingPreference);
                    case CommandId.BaseAttack:
                        return new BaseAttack().WithPreferences(argsPreferences.TargetingPreference);
                    case CommandId.BaseBlock:
                        return new BaseBlock();
                    case CommandId.Wait:
                        return new Wait();
                    case CommandId.DefendAll:
                        return new DefendAll();
                    case CommandId.Taunt:
                        return new Taunt().WithPreferences(argsPreferences.TargetingPreference);
                    case CommandId.TauntedAttack:
                        return new TauntedAttack();
                    case CommandId.Tackle:
                        return new Tackle().WithPreferences(argsPreferences.TargetingPreference);
                    case CommandId.RaiseObstacle:
                        return new RaiseObstacle().WithPreferences(argsPreferences.PositionTargetingPreference);
                    case CommandId.HeavyAttack:
                        return new HeavyAttack().WithPreferences(argsPreferences.TargetingPreference);
                    case CommandId.Teleport:
                        return new Teleport().WithPreferences(argsPreferences.PositionTargetingPreference);
                    case CommandId.HealAll:
                        return new HealAll();
                    case CommandId.ShieldPosition:
                        return new ShieldPosition().WithPreferences(argsPreferences.PositionTargetingPreference);
                    case CommandId.LowerEnemyDefense:
                        return new LowerEnemyDefense().WithPreferences(argsPreferences.TargetingPreference);
                    case CommandId.None:
                    case CommandId.HealOne:
                    default:
                        throw new ArgumentOutOfRangeException();
                }

        }
    }
}