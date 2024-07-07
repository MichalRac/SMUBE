﻿using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.DataStructures.Units;
using SMUBE.Units.CharacterTypes;
using System;
using SMUBE.Commands.Args;

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

        public override CommandArgs GetCommandArgs(BaseCommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            return null;
        }

        public override BaseCommand ResolveNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
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
    
    public class CompetentBehaviorTreeAIModel : AIModel
    {
        public CompetentBehaviorTreeAIModel() : base(false) { }

        private BehaviorTreeTask _behaviorTree;
        private BehaviorTreeTask GetBehaviorTree()
        {
            if(_behaviorTree != null)
                return _behaviorTree;

            _behaviorTree = BehaviorTreeConfig.GetCompetentBehaviorTree();
            return _behaviorTree;
        }

        public override CommandArgs GetCommandArgs(BaseCommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            return null;
        }

        public override BaseCommand ResolveNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
            {
                var behaviorTree = GetBehaviorTree();
                behaviorTree.Run(battleStateModel, activeUnitIdentifier, out var finalCommand);
                return finalCommand;            
            }

            Console.WriteLine($"Trying to fetch actions for unit {unit.UnitData.Name} that is not part of the battle!");
            return null;
        }
    }

}
