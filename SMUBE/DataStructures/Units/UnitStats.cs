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
        public int MaxHealth { get; private set; }
        public int MaxStamina { get; private set; }
        public int MaxMana { get; private set; }
        public int Defense { get; private set; }
        public int Speed { get; private set; }

        public UnitStats(int maxHealth, int maxStamina, int maxMana, int defense, int speed)
        {
            MaxHealth = maxHealth;
            MaxStamina = maxStamina;
            MaxMana = maxMana;
            Defense = defense;
            Speed = speed;
        }
    }
}
