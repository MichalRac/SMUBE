using System;
using SMUBE.BattleState;
using SMUBE.Commands._Common;
using SMUBE.Commands.Results;
using SMUBE.Core;
using SMUBE.DataStructures.Units;

namespace SMUBE.Commands.Effects
{
    public class DamageAppliedMultiplier : Effect
    {
        private const int DEFAULT_PERSISTANCE = 3;
        
        public readonly float Multiplier;
        private readonly UnitRoundStartTrigger _persistenceUpdateTrigger;
        private int _currentPersistence = DEFAULT_PERSISTANCE;

        public DamageAppliedMultiplier(float multiplier, UnitRoundStartTrigger persistenceUpdateTrigger, int startingPersistence = DEFAULT_PERSISTANCE)
        {
            Multiplier = multiplier;
            _persistenceUpdateTrigger = persistenceUpdateTrigger;

            if (Math.Abs(Multiplier) <= float.Epsilon)
            {
                Multiplier = 1f;
            }
            
            _currentPersistence = startingPersistence;
        }
        public override int GetPersistence() => _currentPersistence;
        public override void OnOwnTurnStartEvaluate(BattleStateModel battleStateModel, UnitStats unitStats)
        {
            base.OnOwnTurnStartEvaluate(battleStateModel, unitStats);
            if (_persistenceUpdateTrigger == UnitRoundStartTrigger.All 
                || _persistenceUpdateTrigger == UnitRoundStartTrigger.OnActiveUnitRoundStart)
            {
                _currentPersistence--;
            }
        }

        public override void OnAnyTurnStartEvaluate(BattleStateModel battleStateModel, UnitStats unitStats)
        {
            base.OnAnyTurnStartEvaluate(battleStateModel, unitStats);
            if (_persistenceUpdateTrigger == UnitRoundStartTrigger.All 
                || _persistenceUpdateTrigger == UnitRoundStartTrigger.OnAnyUnitRoundStart)
            {
                _currentPersistence--;
            }
        }

        public override void ModifyCommandResult(CommandResults commandResults)
        {
            base.ModifyCommandResult(commandResults);

            foreach (var effect in commandResults.effects)
            {
                if(effect is DamageEffect damageEffect)
                {
                    damageEffect.Value = (int)(damageEffect.Value * Multiplier);
                }
            }
        }
        
        public override string GetDescriptor()
        {
            return $"AppliedDmg: x{Multiplier}";
        }
    }
}