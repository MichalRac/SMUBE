using SMUBE.BattleState;
using SMUBE.DataStructures.Units;

namespace SMUBE.Commands.Effects
{
    public class TauntEffect : Effect
    {
        public UnitIdentifier Target;
        
        private const int DEFAULT_PERSISTENCE = 3;
        private int _currentPersistence = DEFAULT_PERSISTENCE;

        public TauntEffect(UnitIdentifier target, int currentPersistence = DEFAULT_PERSISTENCE)
        {
            Target = target;
            _currentPersistence = currentPersistence;
        }
        
        public override void OnOwnTurnStartEvaluate(BattleStateModel battleStateModel, UnitStats unitStats)
        {
            base.OnOwnTurnStartEvaluate(battleStateModel, unitStats);

            if (!battleStateModel.TryGetUnit(Target, out _))
            {
                _currentPersistence = 0;
            }
            else
            {
                _currentPersistence--;
            }
        }

        public override int GetPersistence() => _currentPersistence;
        
        public override string GetDescriptor()
        {
            return "Taunted";
        }
    }
}