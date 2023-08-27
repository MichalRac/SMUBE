using Commands;
using SMUBE.AI.BehaviorTree;
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
        public DecisionTreeAIModel(bool useSimpleBehavior) : base(useSimpleBehavior) { }
        private DecisionTreeNode _decisionTree;
        private DecisionTreeNode GetDecisionTree(BaseCharacter baseCharacter)
        {
            if (_decisionTree != null)
                return _decisionTree;

            _decisionTree = DecisionTreeConfigs.GetDecisionTreeForArchetype(baseCharacter, UseSimpleBehavior);
            return _decisionTree;
        }

        public override ICommand ResolveNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
            {
                var baseCharacter = unit.UnitData.UnitStats.BaseCharacter;
                var decisionTree = GetDecisionTree(baseCharacter);
                var decision = decisionTree.MakeDecision(battleStateModel);

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
