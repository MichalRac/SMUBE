using System;
using System.Collections.Generic;
using System.Linq;
using SMUBE.AI.QLearning.States;
using SMUBE.BattleState;
using SMUBE.Units;

namespace SMUBE.AI.QLearning
{
    public class QLearningState
    {
        private List<BaseQLearningStateComponent> _stateComponents;

        public QLearningState()
        {
            _stateComponents = new List<BaseQLearningStateComponent>()
            {
                // personal state components
                new QLearningUnitTypeStateComponent(0),
                //new QLearningHealthLevelStateComponent(1),
                //new QLearningEffectLevelStateComponent(2),
                //new QLearningPositionStateComponent(3),
                // team-related state components
                new QLearningTeamAdvantageStateComponent(4),
                //new QLearningTeamHealthLevelStateComponent(5),
                new QLearningTeamPositionStateComponent(6),
            };
        }

        public long GetStateNumber(BattleStateModel battleStateModel, Unit actor)
        {
            var sum = _stateComponents.Sum(s => s.GetValue(battleStateModel, actor));
            var prefixOrder = (int)Math.Pow(10, 7) * 9;
            return sum + prefixOrder;
        }

        public string GetStateNumberWithDescription(BattleStateModel battleStateModel, Unit actor)
        {
            var stateNumber = GetStateNumber(battleStateModel, actor);
            string result = $"\nStateID: {stateNumber:D7}\n";
            
            for (var index = _stateComponents.Count - 1; index >= 0; index--)
            {
                var stateComponent = _stateComponents[index];
                result += $"\n{stateComponent.GetValueWithDescriptions(battleStateModel, actor)}";
            }

            return result;
        }
    }
}