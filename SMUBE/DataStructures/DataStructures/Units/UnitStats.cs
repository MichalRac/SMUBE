using SMUBE.DataStructures.Utils;
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
        public int CurrentHealth { get; set; }
        public int MaxHealth { get; private set; }

        public int CurrentStamina { get; private set; }
        public int MaxStamina { get; private set; }

        public int CurrentMana { get; private set; }
        public int MaxMana { get; private set; }

        public int Defense { get; private set; }
        public int Speed { get; private set; }

        public UnitStats(int maxHealth, int maxStamina, int maxMana, int defense, int speed)
        {
            CurrentHealth = maxHealth;
            MaxHealth = maxHealth;

            CurrentStamina = maxStamina;
            MaxStamina = maxStamina;

            CurrentMana = maxMana;
            MaxMana = maxMana;

            Defense = defense;
            Speed = speed;
        }

        public override string ToString()
        {
            var fullStatus = new StringBuilder();

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
