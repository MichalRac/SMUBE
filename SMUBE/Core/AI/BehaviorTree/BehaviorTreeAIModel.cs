using Commands;
using SMUBE.AI.GoalOrientedBehavior;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.BehaviorTree
{
    public class BehaviorTreeAIModel : AIModel
    {
        // Todo Replace with Behavior Tree implementation
        public override CommandArgs GetCommandArgs(ICommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            return command.ArgsCache;
        }

        public override ICommand GetNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var unit))
            {
                var baseCharacter = unit.UnitData.UnitStats.BaseCharacter;
                var goals = GOPConfig.GetGoalsForArchetype(baseCharacter);

                var viableActions = unit.ViableCommands;

                var battleStateModelDeepCopy = battleStateModel.DeepCopy();
                ICommand bestAction = null;
                CommandArgs bestArgs = null;
                float minDiscontentment = float.MaxValue;

                foreach (var action in viableActions)
                {
                    var commandArgs = CommandArgsHelper.GetRandomCommandArgs(action, battleStateModelDeepCopy, activeUnitIdentifier);
                    action.Execute(battleStateModelDeepCopy, commandArgs);

                    var discontentment = GetDiscontentment(goals, battleStateModelDeepCopy, activeUnitIdentifier);
                    if (discontentment < minDiscontentment)
                    {
                        minDiscontentment = discontentment;
                        bestAction = action;
                        bestArgs = commandArgs;
                    }
                }

                bestAction.ArgsCache = bestArgs;
                return bestAction;
            }

            Console.WriteLine($"Trying to fetch actions for unit {unit.UnitData.Name} that is not part of the battle!");
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
