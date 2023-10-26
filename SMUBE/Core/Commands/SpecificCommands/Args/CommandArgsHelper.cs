using Commands;
using Commands.SpecificCommands._Common;
using SMUBE.BattleState;
using SMUBE.Commands.SpecificCommands._Common;
using SMUBE.DataStructures.Units;
using SMUBE.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Commands
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
            else if(command.CommandArgsValidator is OneToEveryArgsValidator)
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
                        var targetUnitData = enemyTeamUnits.ElementAt(new Random().Next(enemyTeamUnits.Count() - 1)).UnitData;
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

    }
}
