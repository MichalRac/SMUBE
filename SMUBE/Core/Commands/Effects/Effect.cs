using SMUBE.DataStructures.Units;
using SMUBE.Units;
using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMUBE.Commands.Results;

namespace SMUBE.Commands.Effects
{
    public abstract class Effect
    {
        public virtual int GetPersistence() => 0;
        public virtual void Apply(UnitStats unitStats, CommandResults commandResults)
        {
        }
        public virtual void OnTurnStartEvaluate(UnitStats unitStats)
        {
        }
        public virtual void ModifyCommandResult(CommandResults commandResults)
        {
        }

        protected float GetEffectTypeEffectivness(UnitStats unitStats, CommandResults commandResults)
        {
            if (unitStats.BaseCharacter.BaseCharacterType == commandResults.performer.UnitStats.BaseCharacter.BaseCharacterType)
                return 1;

            switch (commandResults.performer.UnitStats.BaseCharacter.BaseCharacterType)
            {
                case BaseCharacterType.None:
                    return 1;
                case BaseCharacterType.Scholar:
                    if (unitStats.BaseCharacter.BaseCharacterType == BaseCharacterType.Hunter)
                        return 0.5f;
                    if (unitStats.BaseCharacter.BaseCharacterType == BaseCharacterType.Squire)
                        return 2f;
                    break;
                case BaseCharacterType.Squire:
                    if (unitStats.BaseCharacter.BaseCharacterType == BaseCharacterType.Scholar)
                        return 0.5f;
                    if (unitStats.BaseCharacter.BaseCharacterType == BaseCharacterType.Hunter)
                        return 2;
                    break;
                case BaseCharacterType.Hunter:
                    if (unitStats.BaseCharacter.BaseCharacterType == BaseCharacterType.Squire)
                        return 0.5f;
                    if (unitStats.BaseCharacter.BaseCharacterType == BaseCharacterType.Scholar)
                        return 2;
                    break;
            }

            Console.WriteLine($"Type effectivness {commandResults.performer.UnitStats.BaseCharacter.BaseCharacterType} on {unitStats.BaseCharacter.BaseCharacterType} is not covered!");

            return 1;
        }
    }
}
