using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Units.CharacterTypes
{
    public abstract class BaseCharacter
    {
        public abstract UnitStats UnitStats { get; }
    }
}
