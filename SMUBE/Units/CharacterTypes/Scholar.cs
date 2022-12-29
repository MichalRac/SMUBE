using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Units.CharacterTypes
{
    public class Scholar : BaseCharacter
    {
        public override UnitStats UnitStats => UnitConsts.ScholarInfo;
    }
}
