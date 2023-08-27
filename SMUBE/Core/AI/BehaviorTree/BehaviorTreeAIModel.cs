using Commands;
using SMUBE.AI.GoalOrientedBehavior;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.DataStructures.Units;
using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.BehaviorTree
{
    public class BehaviorTreeAIModel : AIModel
    {

        public BehaviorTreeAIModel(bool useSimpleBehavior) : base(useSimpleBehavior) { }

        private BehaviorTreeTask _behaviorTree;
        private BehaviorTreeTask GetBehaviorTree(BaseCharacter baseCharacter)
        {
            if(_behaviorTree != null)
                return _behaviorTree;

            _behaviorTree = BehaviorTreeConfig.GetBehaviorTreeForArchetype(baseCharacter, UseSimpleBehavior);
            return _behaviorTree;
        }

        public override CommandArgs GetCommandArgs(ICommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            return null;
        }

        public override ICommand ResolveNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
            {
                var baseCharacter = unit.UnitData.UnitStats.BaseCharacter;
                var behaviorTree = GetBehaviorTree(baseCharacter);
                behaviorTree.Run(battleStateModel, activeUnitIdentifier, out var finalCommand);
                return finalCommand;            
            }

            Console.WriteLine($"Trying to fetch actions for unit {unit.UnitData.Name} that is not part of the battle!");
            return null;
        }
    }
}
