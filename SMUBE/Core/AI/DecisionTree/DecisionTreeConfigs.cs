using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMUBE.Units.CharacterTypes;

namespace SMUBE.AI.DecisionTree
{
    public static class DecisionTreeConfigs
    {
        public static DecisionTreeNode GetDecisionTreeForArchetype(BaseCharacter character)
        {
            switch (character)
            {
                case Hunter _:
                    return GetHunterDecisionTree();
                case Scholar _:
                    return GetScholarDecisionTree();
                case Squire _:
                    return GetSquireDecisionTree();
                default:
                    return null;
            }    
        }

        public static DecisionTreeNode GetHunterDecisionTree()
        {
            return new DecisionTreeTestRandom(0.5D,
                new DecisionTreeActionAttack(),
                new DecisionTreeActionBlock());
        }
        public static DecisionTreeNode GetScholarDecisionTree()
        {
            return new DecisionTreeTestRandom(0.2D,
                new DecisionTreeActionAttack(),
                new DecisionTreeActionBlock());
        }
        public static DecisionTreeNode GetSquireDecisionTree()
        {
            return new DecisionTreeTestRandom(0.8D,
                new DecisionTreeActionAttack(),
                new DecisionTreeActionBlock());
        }

    }
}
