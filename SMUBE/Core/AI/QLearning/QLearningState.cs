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
                new QLearningUnitTypeStateComponent(0), // 3 states: scholar / hunter / squire 
                new QLearningHealthLevelStateComponent(1), // 3 states
                new QLearningEffectLevelStateComponent(2), // 3 states
                new QLearningPositionStateComponent(3), // 4 states: Far from enemy / Barely Outside enemy range / In Enemy Range / Among Enemies
                // team-related state components
                new QLearningTeamAdvantageStateComponent(4), // 3 states: Losing / Even / Winning
                new QLearningTeamHealthLevelStateComponent(5), // 3 states
                new QLearningTeamPositionStateComponent(6), // 5 states: Far / Independent Battles / Under Assault / Assaulting / Full Battle
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

        public string ConvertStateToDescription(long state)
        {
            string description = string.Empty;
            
            description += $"state: {state}";
            var id = state % 10;
            
            description += $"\n{new QLearningUnitTypeStateComponent(0).ValueToDescription(id)}";
            state = remove_last_digit(state); id = state % 10;
            description += $"\n{new QLearningHealthLevelStateComponent(1).ValueToDescription(id)}";
            state = remove_last_digit(state); id = state % 10;
            description += $"\n{new QLearningEffectLevelStateComponent(2).ValueToDescription(id)}";
            state = remove_last_digit(state); id = state % 10;
            description += $"\n{new QLearningPositionStateComponent(3).ValueToDescription(id)}";
            state = remove_last_digit(state); id = state % 10;
            description += $"\n{new QLearningTeamAdvantageStateComponent(4).ValueToDescription(id)}";
            state = remove_last_digit(state); id = state % 10;
            description += $"\n{new QLearningTeamHealthLevelStateComponent(5).ValueToDescription(id)}";
            state = remove_last_digit(state); id = state % 10;
            description += $"\n{new QLearningTeamPositionStateComponent(6).ValueToDescription(id)}";
            
            long remove_last_digit(long arg)
            {
                arg -= id; 
                arg /= 10; 
                return arg;
            }
            
            return description;
        }
    }
}