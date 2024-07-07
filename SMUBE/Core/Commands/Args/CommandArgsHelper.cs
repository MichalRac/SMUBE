using System;
using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands._Common;
using SMUBE.Commands.Args.ArgsPicker;
using SMUBE.Commands.Args.ArgsValidators;
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
using SMUBE.Core;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;

namespace SMUBE.Commands.Args
{
    public static class CommandArgsHelper
    {

        public static CommandArgs GetDumbCommandArgs(ICommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if (command.CommandArgsValidator is OneToSelfArgsValidator)
            {
                if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
                {
                    return new CommonArgs(unit.UnitData, null, battleStateModel);
                }

                return null;
            }
            else if (command.CommandArgsValidator is OneToOneArgsValidator)
            {
                if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
                {
                    // todo hardcoded 2 teams limit
                    var enemyTeamId = unit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;
                    var enemyTeamUnits = battleStateModel.GetTeamUnits(enemyTeamId).Where(u => u.UnitData.UnitStats.IsAlive());
                    if (enemyTeamUnits != null && enemyTeamUnits.Count() > 0)
                    {
                        var targetUnitData = enemyTeamUnits.First().UnitData;
                        return new CommonArgs(unit.UnitData, new List<UnitData>() { targetUnitData }, battleStateModel);
                    }

                    return null;
                }

                return null;
            }
            else if (command.CommandArgsValidator is OneToEveryArgsValidator oneToEveryArgsValidator)
            {
                if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
                {
                    var constraint = oneToEveryArgsValidator.ArgsConstraint;

                    var viableTargets = new List<UnitData>();
                    switch (constraint)
                    {
                        case ArgsConstraint.None:
                            add_ally_targets();
                            add_enemy_targets();
                            break;
                        case ArgsConstraint.Ally:
                            add_ally_targets();
                            break;
                        case ArgsConstraint.Enemy:
                            add_enemy_targets();
                            break;
                    }

                    void add_ally_targets()
                    {
                        var targets = battleStateModel.GetTeamUnits(activeUnitIdentifier.TeamId == 0 ? 0 : 1).Where(u => u.UnitData.UnitStats.IsAlive());
                        foreach (var target in targets)
                        {
                            viableTargets.Add(target.UnitData);
                        }
                    }

                    void add_enemy_targets()
                    {
                        var targets = battleStateModel.GetTeamUnits(activeUnitIdentifier.TeamId == 0 ? 1 : 0).Where(u => u.UnitData.UnitStats.IsAlive());
                        foreach (var target in targets)
                        {
                            viableTargets.Add(target.UnitData);
                        }
                    }

                    return new CommonArgs(unit.UnitData, viableTargets, battleStateModel);
                }

                return null;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static List<CommandArgs> GetAllViableRandomCommandArgs(ICommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            var listViable = new List<CommandArgs>();

            if (command.CommandArgsValidator is OneToSelfArgsValidator)
            {
                listViable.Add(GetDumbCommandArgs(command, battleStateModel, activeUnitIdentifier));
                return listViable;
            }
            else if (command.CommandArgsValidator is OneToOneArgsValidator)
            {
                if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
                {
                    // todo hardcoded 2 teams limit
                    var enemyTeamId = unit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;
                    var enemyTeamUnits = battleStateModel.GetTeamUnits(enemyTeamId).Where(u => u.UnitData.UnitStats.IsAlive());
                    if (enemyTeamUnits != null && enemyTeamUnits.Count() > 0)
                    {
                        foreach (var enemyTeamUnit in enemyTeamUnits)
                        {
                            listViable.Add(new CommonArgs(unit.UnitData, new List<UnitData>() { enemyTeamUnit.UnitData }, battleStateModel));
                        }

                        return listViable;
                    }

                    return null;
                }

                return null;
            }
            else if (command.CommandArgsValidator is OneToEveryArgsValidator)
            {
                listViable.Add(GetDumbCommandArgs(command, battleStateModel, activeUnitIdentifier));
                return listViable;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static CommandArgs GetRandomCommandArgs(ICommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if (command.CommandArgsValidator is OneToSelfArgsValidator)
            {
                if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
                {
                    return new CommonArgs(unit.UnitData, null, battleStateModel);
                }

                return null;
            }
            else if (command.CommandArgsValidator is OneToOneArgsValidator)
            {
                if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
                {
                    // todo hardcoded 2 teams limit
                    var enemyTeamId = unit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;
                    var enemyTeamUnits = battleStateModel.GetTeamUnits(enemyTeamId).Where(u => u.UnitData.UnitStats.IsAlive());
                    if (enemyTeamUnits != null && enemyTeamUnits.Count() > 0)
                    {
                        var targetUnitData = enemyTeamUnits.ElementAt(RngProvider.Next(enemyTeamUnits.Count() - 1)).UnitData;
                        return new CommonArgs(unit.UnitData, new List<UnitData>() { targetUnitData }, battleStateModel);
                    }

                    return null;
                }

                return null;
            }
            else if (command.CommandArgsValidator is OneToEveryArgsValidator oneToEveryArgsValidator)
            {
                if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
                {
                    var constraint = oneToEveryArgsValidator.ArgsConstraint;

                    var viableTargets = new List<UnitData>();
                    switch (constraint)
                    {
                        case ArgsConstraint.None:
                            add_ally_targets();
                            add_enemy_targets();
                            break;
                        case ArgsConstraint.Ally:
                            add_ally_targets();
                            break;
                        case ArgsConstraint.Enemy:
                            add_enemy_targets();
                            break;
                    }

                    void add_ally_targets()
                    {
                        var targets = battleStateModel.GetTeamUnits(activeUnitIdentifier.TeamId == 0 ? 0 : 1).Where(u => u.UnitData.UnitStats.IsAlive());
                        foreach (var target in targets)
                        {
                            viableTargets.Add(target.UnitData);
                        }
                    }

                    void add_enemy_targets()
                    {
                        var targets = battleStateModel.GetTeamUnits(activeUnitIdentifier.TeamId == 0 ? 1 : 0).Where(u => u.UnitData.UnitStats.IsAlive());
                        foreach (var target in targets)
                        {
                            viableTargets.Add(target.UnitData);
                        }
                    }

                    return new CommonArgs(unit.UnitData, viableTargets, battleStateModel);
                }

                return null;
            }
            else if (command.CommandArgsValidator is OneMoveToOneArgsValidator)
            {
                if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
                {
                    var validTargets
                        = battleStateModel.GetTeamUnits(activeUnitIdentifier.TeamId == 0 ? 1 : 0)
                            .Where(u => u.UnitData.UnitStats.IsAlive()).ToList();

                    if (validTargets.Count == 0)
                    {
                        return null;
                    }

                    validTargets.Shuffle();

                    foreach (var validTarget in validTargets)
                    {
                        var reachableSurrounding = battleStateModel.BattleSceneState.PathfindingHandler
                            .GetReachableSurroundingPositions(battleStateModel, validTarget.UnitData.BattleScenePosition, validTarget.UnitData.UnitIdentifier);

                        if (reachableSurrounding.Count == 0)
                            continue;

                        reachableSurrounding.Shuffle();

                        var posDelta = new PositionDelta(activeUnitIdentifier, unit.UnitData.BattleScenePosition.Coordinates, reachableSurrounding.First().Coordinates);
                        var args = new CommonArgs(unit.UnitData, new List<UnitData>() { validTarget.UnitData }, battleStateModel, posDelta);

                        return args;
                    }

                    return null;
                }
            }
            else if (command.CommandArgsValidator is OneMoveToPositionValidator)
            {
                if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
                {
                    var reachablePositions = battleStateModel.BattleSceneState.PathfindingHandler.GetAllReachablePathsForActiveUnit(battleStateModel);
                    
                    if (reachablePositions.Count == 0)
                        return null;
                    
                    var target = reachablePositions.GetRandom();
                    var positionDelta = new PositionDelta(unit.UnitData.UnitIdentifier, unit.UnitData.BattleScenePosition.Coordinates, target.TargetPosition.Coordinates);
                    return new CommonArgs(unit.UnitData, null, battleStateModel, positionDelta);
                }
                return null;
            }
            else if (command.CommandArgsValidator is TauntedAttackArgsValidator)
            {
                if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
                {
                    var viableTargets = unit.UnitCommandProvider.ViableTargets;
                    if (viableTargets != null && viableTargets.Count > 0)
                    {
                        if (battleStateModel.TryGetUnit(viableTargets[0], out var targetUnit))
                        {
                            return new CommonArgs(unit.UnitData, new List<UnitData>() { targetUnit.UnitData }, battleStateModel);
                        }
                    }
                    return null;
                }
                return null;
            }
            else if (command.CommandArgsValidator is OneToPositionValidator)
            {
                if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
                {
                    var validTargets = battleStateModel.BattleSceneState.AggregateAllPositions(true);

                    if (validTargets.Count == 0)
                    {
                        return null;
                    }
                
                    validTargets.Shuffle();
                    var targetPos = validTargets.First().Coordinates;
                    
                    return new CommonArgs(unit.UnitData, null, battleStateModel, null, new List<SMUBEVector2<int>>{targetPos});
                }

                return null;
            }
            else
            {
                Console.WriteLine($"ERROR, unhandled CommandArgsValidator: {command.CommandArgsValidator.GetType().Name}");
                Console.WriteLine($"Tap anything to close.");
                Console.ReadKey();
                throw new NotImplementedException();
            }

            return null;
        }

        public static List<BaseCommand> GetAllCommandPreferencesVariants(List<BaseCommand> viableCommands)
        {
            var result = new List<BaseCommand>();

            foreach (var validUnitCommand in viableCommands)
            {
                switch (validUnitCommand.CommandId)
                {
                    case CommandId.BaseWalk:
                        result.Add(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)0));
                        result.Add(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)1));
                        result.Add(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)2));
                        result.Add(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)3));
                        result.Add(new BaseWalk().WithPreferences((ArgsMovementTargetingPreference)4));
                        break;
                    case CommandId.BaseAttack:
                        result.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.None));
                        result.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.Closest));
                        result.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints));
                        result.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage));
                        result.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt));
                        result.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange));
                        result.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn));
                        result.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn));
                        result.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn));
                        result.Add(new BaseAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn));
                        break;
                    case CommandId.BaseBlock:
                        result.Add(new BaseBlock());
                        break;
                    case CommandId.Wait:
                        result.Add(new Wait());
                        break;
                    case CommandId.DefendAll:
                        result.Add(new DefendAll());
                        break;
                    case CommandId.Taunt:
                        result.Add(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.None));
                        result.Add(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.Closest));
                        result.Add(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints));
                        result.Add(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage));
                        result.Add(new Taunt().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange));
                        break;
                    case CommandId.TauntedAttack:
                        result.Add(new TauntedAttack());
                        break;
                    case CommandId.Tackle:
                        result.Add(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.None));
                        result.Add(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.Closest));
                        result.Add(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints));
                        result.Add(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage));
                        result.Add(new Tackle().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange));
                        break;
                    case CommandId.RaiseObstacle:
                        result.Add(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.None));
                        result.Add(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy));
                        result.Add(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.InBetweenEnemies));
                        result.Add(new RaiseObstacle().WithPreferences(ArgsPositionTargetingPreference.InBetweenTeams));
                        break;
                    case CommandId.HeavyAttack:
                        result.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.None));
                        result.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.Closest));
                        result.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints));
                        result.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage));
                        result.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MostDmgDealt));
                        result.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange));
                        result.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimizeReachableEnemiesAfterTurn));
                        result.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximiseReachableEnemiesAfterTurn));
                        result.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MinimisePositionBuffAfterTurn));
                        result.Add(new HeavyAttack().WithPreferences(ArgsEnemyTargetingPreference.MaximisePositionBuffAfterTurn));
                        break;
                    case CommandId.Teleport:
                        result.Add(new Teleport().WithPreferences(ArgsMovementTargetingPreference.None)); 
                        result.Add(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetOutOfReach)); 
                        result.Add(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetCloserCarefully)); 
                        result.Add(new Teleport().WithPreferences(ArgsMovementTargetingPreference.GetCloserAggressively)); 
                        result.Add(new Teleport().WithPreferences(ArgsMovementTargetingPreference.OptimizeFortifiedPosition)); 
                        break;
                    case CommandId.HealAll:
                        result.Add(new HealAll());
                        break;
                    case CommandId.ShieldPosition:
                        result.Add(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.None));
                        result.Add(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnLeastHpPercentageAlly));
                        result.Add(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnMostHpPercentageAlly));
                        result.Add(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.NextToClosestEnemy));
                        result.Add(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.OnAllyWithMostEnemiesInReach));
                        result.Add(new ShieldPosition().WithPreferences(ArgsPositionTargetingPreference.InBetweenTeams));
                        break;
                    case CommandId.LowerEnemyDefense:
                        result.Add(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.None));
                        result.Add(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.Closest));
                        result.Add(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPoints));
                        result.Add(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.LeastHpPercentage));
                        result.Add(new LowerEnemyDefense().WithPreferences(ArgsEnemyTargetingPreference.EnemyWithMostAlliesInRange));
                        break;
                    case CommandId.None:
                    case CommandId.HealOne:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            return result;
        }
    }
}