using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.DataStructures.Units;
using SMUBE.Units.CharacterTypes;
using System;
using SMUBE.AI.DecisionTree.EndNodes;
using SMUBE.Commands.Args;

namespace SMUBE.AI.DecisionTree
{
    public class DecisionTreeAIModel : AIModel
    {
        private Func<BaseCharacter, DecisionTreeNode> _decisionTreeProvider;
        public DecisionTreeAIModel(bool useSimpleBehavior) : base(useSimpleBehavior) { }
        
        public DecisionTreeAIModel(Func<BaseCharacter, DecisionTreeNode> decisionTreeProvider) 
            : base(false)
        {
            _decisionTreeProvider = decisionTreeProvider;
        }

        private DecisionTreeNode _decisionTree;
        private DecisionTreeNode GetDecisionTree(BaseCharacter baseCharacter)
        {
            if (_decisionTree != null)
                return _decisionTree;

            if (_decisionTreeProvider != null)
            {
                _decisionTree = _decisionTreeProvider?.Invoke(baseCharacter);
                return _decisionTree;
            }

            _decisionTree = DecisionTreeConfigs.GetDecisionTreeForArchetype(baseCharacter, UseSimpleBehavior);
            return _decisionTree;
        }

        public override BaseCommand ResolveNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
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

        public override CommandArgs GetCommandArgs(BaseCommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            return command.GetSuggestedPseudoRandomArgs(battleStateModel);
        }

    }
}
