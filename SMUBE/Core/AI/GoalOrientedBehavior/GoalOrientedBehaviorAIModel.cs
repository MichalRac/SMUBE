using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.DataStructures.Units;
using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;
using SMUBE.Commands.Args;

namespace SMUBE.AI.GoalOrientedBehavior
{
    public class GoalOrientedBehaviorAIModel : AIModel
    {
        public GoalOrientedBehaviorAIModel(bool useSimpleBehavior) : base(useSimpleBehavior)
        {
        }

        private List<Goal> _goals = new List<Goal>();
        private List<Goal> GetGoals(BaseCharacter baseCharacter)
        {
            if (_goals != null && _goals.Count > 0)
                return _goals;

            _goals = GOPConfig.GetGoalsForArchetype(baseCharacter, UseSimpleBehavior);
            return _goals;
        }


        public override CommandArgs GetCommandArgs(BaseCommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            return command.ArgsCache;
        }

        public override BaseCommand ResolveNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var activeUnit))
            {
                var baseCharacter = activeUnit.UnitData.UnitStats.BaseCharacter;
                var goals = GetGoals(baseCharacter);

                var viableActions = activeUnit.UnitCommandProvider.ViableCommands;

                BaseCommand bestAction = null;
                CommandArgs bestArgs = null;
                float minDiscontentment = float.MaxValue;

                var validCommandPreferencesVariants = CommandArgsHelper.GetAllCommandPreferencesVariants(viableActions);
                
                foreach (var action in validCommandPreferencesVariants)
                {
                    var battleStateModelDeepCopy = battleStateModel.DeepCopy();

                    var battleStateModelArgDeepCopy = battleStateModelDeepCopy.DeepCopy();
                    var args = action.GetSuggestedPseudoRandomArgs(battleStateModelDeepCopy);
                    bool success = action.TryExecute(battleStateModelArgDeepCopy, args);

                    if (!success)
                    {
                        continue;
                    }

                    var discontentment = GetDiscontentment(goals, battleStateModelArgDeepCopy, activeUnitIdentifier);
                    if (discontentment < minDiscontentment)
                    {
                        minDiscontentment = discontentment;
                        bestAction = action;
                        bestArgs = args;
                    }
                }
                
                bestAction.ArgsCache = bestArgs.DeepCopyWithNewBattleStateModel(battleStateModel);
                return bestAction;
            }

            Console.WriteLine($"Trying to fetch actions for unit {activeUnit.UnitData.Name} that is not part of the battle!");
            return null;
        }

        public float GetDiscontentment(List<Goal> goals, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            float totalDiscontentment = 0f;

            foreach (var goal in goals)
            {
                totalDiscontentment += goal.GetDiscontentment(battleStateModel, activeUnitIdentifier);
            }

            return totalDiscontentment;
        }
    }
}
