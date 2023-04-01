using Commands;
using Commands.SpecificCommands._Common;
using SMUBE.BattleState;
using SMUBE.Commands.SpecificCommands._Common;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SMUBE.AI
{
    // always pick random option
    public class RandomAIModel : AIModel
    {
        public override ICommand GetNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if(battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
            {
                var viableCommands = unit.ViableCommands;
                
                if(viableCommands == null || viableCommands.Count == 0) 
                {
                    Console.WriteLine($"Unit {unit.UnitData.Name} has no viable actions!");
                    return null;
                }

                return viableCommands[new Random().Next(viableCommands.Count)];
            }

            Console.WriteLine($"Trying to fetch actions for unit {unit.UnitData.Name} that is not part of the battle!");
            return null;
        }

        public override CommandArgs GetCommandArgs(ICommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if(command.CommandArgsValidator is OneToZeroArgsValidator)
            {
                if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
                {
                    return new CommonArgs(unit.UnitData, null, battleStateModel);
                }
                return null;
            }
            else if(command.CommandArgsValidator is OneToOneArgsValidator)
            {
                if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
                {
                    // todo hardcoded 2 teams limit
                    var enemyTeamId = unit.UnitData.UnitIdentifier.TeamId == 0 ? 1 : 0;
                    var enemyTeamUnits = battleStateModel.GetTeamUnits(enemyTeamId);
                    if(enemyTeamUnits != null && enemyTeamUnits.Count > 0)
                    {
                        var targetUnitData = enemyTeamUnits[new Random().Next(enemyTeamUnits.Count - 1)].UnitData;
                        return new CommonArgs(unit.UnitData, new List<UnitData>() { targetUnitData }, battleStateModel);
                    }

                    return null;
                }
                return null;

            }

            return null;
        }
    }
}
