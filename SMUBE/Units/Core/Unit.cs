using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Units.Core
{
    [Serializable]
    public class Unit
    {
        public string name;
        public int health;
        public int maxHealth;

        public int posX;
        public int posY;
    }
}
