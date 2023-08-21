﻿using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Units.CharacterTypes
{
    public class Hunter : BaseCharacter
    {
        public override UnitStats UnitStats => UnitConsts.HunterInfo;
    }
}