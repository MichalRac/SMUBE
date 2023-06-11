using SMUBE.AI.DecisionTree;
using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.BehaviorTree
{
    public static class BehaviorTreeConfig
    {

        public static BehaviorTreeTask GetBehaviorTreeForArchetype(BaseCharacter character)
        {
            switch (character)
            {
                case Hunter _:
                    return GetHunterBehaviorTree();
                case Scholar _:
                    return GetScholarBehaviorTree();
                case Squire _:
                    return GetSquireBehaviorTree();
                default:
                    return null;
            }
        }

        public static BehaviorTreeTask GetHunterBehaviorTree()
        {
            return new BehaviorTreeSelector(new List<BehaviorTreeTask>() { 
                new BehaviorTreeBasicAttack(), 
                new BehaviorTreeBasicBlock()});
        }
        public static BehaviorTreeTask GetScholarBehaviorTree()
        {
            return new BehaviorTreeSelector(new List<BehaviorTreeTask>() {
                new BehaviorTreeBasicAttack(),
                new BehaviorTreeBasicBlock()});
        }
        public static BehaviorTreeTask GetSquireBehaviorTree()
        {
            return new BehaviorTreeSelector(new List<BehaviorTreeTask>() {
                new BehaviorTreeBasicAttack(),
                new BehaviorTreeBasicBlock()});
        }

    }
}
