﻿using Commands;
using Commands.SpecificCommands.BaseAttack;
using SMUBE.DataStructures;
using SMUBE.DataStructures.Units;
using SMUBE.Units.CharacterTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Units
{
    public class Unit
    {
        public UnitData UnitData { get; private set; }
        public List<Command> ViableCommands { get; private set; } = new List<Command>();

        public Unit(UnitData argUnitData, List<Command> argViableCommands = null)
        {
            UnitData = argUnitData;
            if (argViableCommands != null)
            {
                ViableCommands = argViableCommands;
            }
        }

        public Unit(string argName, UnitIdentifier argUnitIdentifier, UnitStats argUnitStats, List<Command> argViableCommands = null)
            : this(new UnitData(argName, argUnitIdentifier, argUnitStats)) 
        {
            if (argViableCommands != null)
            {
                ViableCommands = argViableCommands;
            }

            ViableCommands.Add(new BaseAttack());
        }
    }
}
