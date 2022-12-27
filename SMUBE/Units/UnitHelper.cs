using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Units
{
    public static class UnitHelper
    {
        public static BaseCharacter CreateCharacter<T>() where T : BaseCharacter, new()
        {
            return new T();
        }
    }
}
