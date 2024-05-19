using System;
using System.Collections.Generic;
using System.Linq;
using SMUBE.BattleState;
using SMUBE.Commands._Common;
using SMUBE.Commands.Args.ArgsPicker;
using SMUBE.Commands.Args.ArgsValidators;
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
    }
}