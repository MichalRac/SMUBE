using Commands;
using Commands.SpecificCommands._Common;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.GoalOrientedBehavior
{
    public class GoalOrientedBehaviorAIModel : AIModel
    {
        public override CommandArgs GetCommandArgs(ICommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            return command.ArgsCache;
        }

        public override ICommand ResolveNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            if (battleStateModel.TryGetUnit(activeUnitIdentifier, out var activeUnit))
            {
                var baseCharacter = activeUnit.UnitData.UnitStats.BaseCharacter;
                var goals = GOPConfig.GetGoalsForArchetype(baseCharacter);

                var viableActions = activeUnit.ViableCommands;

                ICommand bestAction = null;
                CommandArgs bestArgs = null;
                float minDiscontentment = float.MaxValue;

                foreach (var action in viableActions)
                {
                    var battleStateModelDeepCopy = battleStateModel.DeepCopy();
                    var validCommandArgs = CommandArgsHelper.GetAllViableRandomCommandArgs(action, battleStateModelDeepCopy, activeUnitIdentifier);

                    foreach (var commandArgs in validCommandArgs)
                    {
                        var battleStateModelArgDeepCopy = battleStateModelDeepCopy.DeepCopy();
                        bool success = action.Execute(battleStateModelDeepCopy, commandArgs);

                        if (!success)
                        {
                            continue;
                        }

                        var discontentment = GetDiscontentment(goals, battleStateModelDeepCopy, activeUnitIdentifier);
                        if (discontentment < minDiscontentment)
                        {
                            minDiscontentment = discontentment;
                            bestAction = action;
                            bestArgs = commandArgs;
                        }
                    }
                }

                var targetUnits = new List<UnitData>();
                if(bestArgs.TargetUnits?.Count > 0)
                {
                    foreach (var deepCopyTargetUnit in bestArgs.TargetUnits)
                    {
                        if (battleStateModel.TryGetUnit(deepCopyTargetUnit.UnitIdentifier, out var targetUnit))
                        {
                            targetUnits.Add(targetUnit.UnitData);
                            continue;
                        }
                        return null;
                    }
                }

                bestAction.ArgsCache = new CommonArgs(activeUnit.UnitData, targetUnits, battleStateModel);
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
