using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using SMUBE.Commands.Results;

namespace SMUBE.Commands.Effects
{
    public class BlockEffect : Effect
    {
        public readonly float Multiplier = 0.5f;
        private const int DEFAULT_PERSISTENCE = 1;
        private int currentPersistence = DEFAULT_PERSISTENCE;

        public BlockEffect(int currentPersistence = DEFAULT_PERSISTENCE)
        {
            this.currentPersistence = currentPersistence;
        }

        public override string GetDescriptor()
        {
            return "Blocking";
        }

        public override int GetPersistence() => currentPersistence;
        public override void OnOwnTurnStartEvaluate(BattleStateModel battleStateModel, UnitStats unitStats)
        {
            base.OnOwnTurnStartEvaluate(battleStateModel, unitStats);
            currentPersistence--;
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
    }
}
