using SMUBE.BattleState;

namespace SMUBE.AI.StateMachine
{
    public class StateMachineHealthThresholdTransition : StateMachineTransition
    {
        private readonly float _threshold;
        private readonly bool _trueWhenAbove;

        public StateMachineHealthThresholdTransition(StateMachineState targetState, float threshold, bool trueWhenAbove) : base(targetState)
        {
            _threshold = threshold;
            _trueWhenAbove = trueWhenAbove;
        }

        public override bool IsTriggered(BattleStateModel battleStateModel)
        {
            if (battleStateModel.GetNextActiveUnit(out var nextUnit))
            {
                if (_trueWhenAbove)
                    return ((float)nextUnit.UnitData.UnitStats.CurrentHealth / nextUnit.UnitData.UnitStats.MaxHealth) >= _threshold;
                return ((float)nextUnit.UnitData.UnitStats.CurrentHealth / nextUnit.UnitData.UnitStats.MaxHealth) <= _threshold;
            }
            return false;
        }
    }
}