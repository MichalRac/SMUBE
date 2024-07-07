﻿using SMUBE.Commands.Effects;
using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;
using System.Text;
using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.Commands.Results;

namespace SMUBE.DataStructures.Units
{
    [Serializable]
    public class UnitStats
    {
        public const int PER_TURN_HP_REPLENISH = 10;
        public const int PER_TURN_MANA_REPLENISH = 5;
        public const int PER_TURN_STAMINA_REPLENISH = 5;
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

        public UnitStats(UnitStats sourceUnitStats)
        {
            BaseCharacter = sourceUnitStats.BaseCharacter;

            CurrentHealth = sourceUnitStats.CurrentHealth;
            MaxHealth = sourceUnitStats.MaxHealth;

            CurrentStamina = sourceUnitStats.CurrentStamina;
            MaxStamina = sourceUnitStats.MaxStamina;

            CurrentMana = sourceUnitStats.CurrentMana;
            MaxMana = sourceUnitStats.MaxMana;

            Power = sourceUnitStats.Power;
            Defense = sourceUnitStats.Defense;
            Speed = sourceUnitStats.Speed;

            _persistentEffects = sourceUnitStats._persistentEffects;
        }

        public bool IsAlive()
        {
            return CurrentHealth > 0;
        }

        internal bool CanUseAbility(ICommand command)
        {
            return CurrentStamina >= command.StaminaCost && CurrentMana >= command.ManaCost;
        }

        internal bool TryUseAbility(ICommand command)
        {
            if(!CanUseAbility(command))
            {
                return false;
            }
            CurrentStamina -= command.StaminaCost;
            CurrentMana -= command.ManaCost;
            return true;
        }

        internal void DeltaHealth(int delta)
        {
            CurrentHealth += delta;
            if (CurrentHealth > MaxHealth)
                CurrentHealth = MaxHealth;
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
                effect.Apply(this, commandResults);

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

        internal void OnAnyTurnStartEvaluate(BattleStateModel battleStateModel)
        {
            for (int i = PersistentEffects.Count - 1; i >= 0; i--)
            {
                Effect effect = PersistentEffects[i];
                effect.OnAnyTurnStartEvaluate(battleStateModel, this);
                if(effect.GetPersistence() <= 0)
                {
                    PersistentEffects.RemoveAt(i);
                }
            }
        }

        internal void OnOwnTurnStartEvaluate(BattleStateModel battleStateModel)
        {
            PerOwnTurnReplenish();

            for (int i = PersistentEffects.Count - 1; i >= 0; i--)
            {
                Effect effect = PersistentEffects[i];
                effect.OnOwnTurnStartEvaluate(battleStateModel, this);
                if(effect.GetPersistence() <= 0)
                {
                    PersistentEffects.RemoveAt(i);
                }
            }
        }

        private void PerOwnTurnReplenish()
        {
            CurrentHealth += PER_TURN_HP_REPLENISH;
            CurrentMana += PER_TURN_MANA_REPLENISH;
            CurrentStamina += PER_TURN_STAMINA_REPLENISH;
            CurrentHealth = Math.Min(CurrentHealth, MaxHealth);
            CurrentMana = Math.Min(CurrentMana, MaxMana);
            CurrentStamina = Math.Min(CurrentStamina, MaxStamina);
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
