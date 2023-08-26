using Commands;
using SMUBE.Commands.Effects;
using SMUBE.DataStructures.Utils;
using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.DataStructures.Units
{
    [Serializable]
    public class UnitStats
    {
        public BaseCharacter BaseCharacter { get; }
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; private set; }

        public int CurrentStamina { get; private set; }
        public int MaxStamina { get; private set; }

        public int CurrentMana { get; private set; }
        public int MaxMana { get; private set; }

        public int Power { get; private set; }
        public int Defense { get; private set; }
        public int Speed { get; private set; }

        private List<Effect> _persistentEffects = new List<Effect>();
        public List<Effect> PersistentEffects => _persistentEffects;

        public UnitStats(
                         BaseCharacter baseCharacter,
                         int maxHealth,
                         int maxStamina,
                         int maxMana,
                         int power,
                         int defense,
                         int speed)
        {
            BaseCharacter = baseCharacter;

            CurrentHealth = maxHealth;
            MaxHealth = maxHealth;

            CurrentStamina = maxStamina;
            MaxStamina = maxStamina;

            CurrentMana = maxMana;
            MaxMana = maxMana;

            Power = power;
            Defense = defense;
            Speed = speed;
        }

        private UnitStats(UnitStats sourceUnitStats)
        {
            BaseCharacter = sourceUnitStats.BaseCharacter;

            CurrentHealth = sourceUnitStats.MaxHealth;
            MaxHealth = sourceUnitStats.MaxHealth;

            CurrentStamina = sourceUnitStats.MaxStamina;
            MaxStamina = sourceUnitStats.MaxStamina;

            CurrentMana = sourceUnitStats.MaxMana;
            MaxMana = sourceUnitStats.MaxMana;

            Power = sourceUnitStats.Power;
            Defense = sourceUnitStats.Defense;
            Speed = sourceUnitStats.Speed;
        }

        public UnitStats Copy()
        {
            return new UnitStats(this);
        }

        public bool IsAlive()
        {
            return CurrentHealth > 0;
        }

        internal void UseAbility(ICommand command)
        {
            CurrentStamina -= command.StaminaCost;
            CurrentMana -= command.ManaCost;
        }

        internal void DeltaHealth(int delta)
        {
            CurrentHealth += delta;
        }

        internal void AddEffects(CommandResults commandResults)
        {
            foreach(var newEffect in commandResults.effects)
            {
                for (int i = 0; i < PersistentEffects.Count; i++)
                {
                    if (PersistentEffects[i].GetType() == newEffect.GetType())
                    {
                        PersistentEffects.RemoveAt(i);
                        break;
                    }
                }

                PersistentEffects.Add(newEffect);
            }
        }

        internal void AffectByAbility(CommandResults commandResults)
        {
            ModifyByPersistantEffects(commandResults);
            foreach (var effect in commandResults.effects)
            {
                effect.Apply(this);

                if(effect.GetPersistence() > 0)
                {
                    PersistentEffects.Add(effect);
                }
            }
        }

        internal void ModifyByPersistantEffects(CommandResults commandResults)
        {
            foreach (var effect in PersistentEffects)
            {
                effect.ModifyCommandResult(commandResults);
            }
        }

        internal void OnTurnStartEvaluate()
        {
            for (int i = PersistentEffects.Count - 1; i >= 0; i--)
            {
                Effect effect = PersistentEffects[i];
                effect.OnTurnStartEvaluate(this);
                if(effect.GetPersistence() <= 0)
                {
                    PersistentEffects.RemoveAt(i);
                }
            }
        }

        public override string ToString()
        {
            var fullStatus = new StringBuilder();
            fullStatus.AppendLine($"Archetype: {BaseCharacter.GetType().Name}");
            fullStatus.AppendLine($"Health: {CurrentHealth}/{MaxHealth}");
            fullStatus.AppendLine($"Stamina: {CurrentStamina}/{MaxStamina}");
            fullStatus.AppendLine($"Mana: {CurrentMana}/{MaxMana}");
            fullStatus.AppendLine($"Defense: {Defense}");
            fullStatus.AppendLine($"Speed: {Speed}");

            return fullStatus.ToString();
        }

        public string ToShortString()
        {
            var shortStatus = new StringBuilder();

            shortStatus.AppendLine($"Health: {CurrentHealth}/{MaxHealth}");
            if(MaxStamina > 0)
            {
                shortStatus.AppendLine($"Stamina: {CurrentStamina}/{MaxStamina}");
            }
            if (MaxMana > 0)
            {
                shortStatus.AppendLine($"Mana: {CurrentMana}/{MaxMana}");
            }

            return shortStatus.ToString();
        }
    }
}
