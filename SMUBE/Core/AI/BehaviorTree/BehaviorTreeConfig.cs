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

        public static BehaviorTreeTask GetBehaviorTreeForArchetype(BaseCharacter character, bool simpleSetup = false)
        {
            switch (character)
            {
                case Hunter _:
                    return simpleSetup ? GetSimpleHunterBehaviorTree() : GetHunterBehaviorTree();
                case Scholar _:
                    return simpleSetup ? GetSimpleScholarBehaviorTree() : GetScholarBehaviorTree();
                case Squire _:
                    return simpleSetup ? GetSimpleSquireBehaviorTree() : GetSquireBehaviorTree();
                default:
                    return null;
            }
        }

        public static BehaviorTreeTask GetHunterBehaviorTree()
        {
            return new BehaviorTreeSelector(new List<BehaviorTreeTask>() { 
                new BehaviorTreeHeavyAttack(),
                new BehaviorTreeBasicAttack(), 
                new BehaviorTreeBasicBlock()});
        }
        public static BehaviorTreeTask GetScholarBehaviorTree()
        {
            return new BehaviorTreeSelector(new List<BehaviorTreeTask>() {
                new BehaviorTreeHealAll(),
                new BehaviorTreeBasicAttack(),
                new BehaviorTreeBasicBlock()});
        }
        public static BehaviorTreeTask GetSquireBehaviorTree()
        {
            return new BehaviorTreeSelector(new List<BehaviorTreeTask>() {
                new BehaviorTreeDefendAll(),
                new BehaviorTreeBasicAttack(),
                new BehaviorTreeBasicBlock()});
        }

        public static BehaviorTreeTask GetSimpleHunterBehaviorTree()
        {
            return new BehaviorTreeSelector(new List<BehaviorTreeTask>() { 
                new BehaviorTreeBasicAttack(), 
                new BehaviorTreeBasicBlock()});
        }
        public static BehaviorTreeTask GetSimpleScholarBehaviorTree()
        {
            return new BehaviorTreeSelector(new List<BehaviorTreeTask>() {
                new BehaviorTreeBasicAttack(),
                new BehaviorTreeBasicBlock()});
        }
        public static BehaviorTreeTask GetSimpleSquireBehaviorTree()
        {
            return new BehaviorTreeSelector(new List<BehaviorTreeTask>() {
                new BehaviorTreeBasicAttack(),
                new BehaviorTreeBasicBlock()});
        }
    }
}
