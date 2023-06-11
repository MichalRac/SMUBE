using Commands;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.DataStructures.Units;
using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.DecisionTree
{
    public class DecisionTreeAIModel : AIModel
    {

        public override ICommand GetNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
            {
                var baseCharacter = unit.UnitData.UnitStats.BaseCharacter;
                var decisionTree = DecisionTreeConfigs.GetDecisionTreeForArchetype(baseCharacter);
                var decision = decisionTree.MakeDecision();

                if(decision is DecisionTreeAction action)
                {
                    return action.GetCommand();
                }
                Console.WriteLine($"Trying to make an action with non Action end node in decision tree!");
                return null;
            }

            Console.WriteLine($"Trying to fetch actions for unit {unit.UnitData.Name} that is not part of the battle!");
            return null;
        }

        public override CommandArgs GetCommandArgs(ICommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            return CommandArgsHelper.GetRandomCommandArgs(command, battleStateModel, activeUnitIdentifier);
        }

    }
}
