using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.DataStructures.Units
{
    public class UnitInfo
    {
        public string Name { get; private set; }
        public int MaxHealth { get; private set; }
        public int MaxStamina { get; private set; }
        public int MaxMana { get; private set; }
        public int Defense { get; private set; }
        public int Speed { get; private set; }

        public UnitInfo(string name, int maxHealth, int maxStamina, int maxMana, int defense, int speed)
        {
            Name = name;
            MaxHealth = maxHealth;
            MaxStamina = maxStamina;
            MaxMana = maxMana;
            Defense = defense;
            Speed = speed;
        }
    }
}
