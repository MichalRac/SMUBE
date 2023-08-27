using Commands;
using Commands.SpecificCommands.BaseAttack;
using SMUBE.AI;
using SMUBE.Commands.SpecificCommands.BaseBlock;
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
        private bool isDeepCopy = false;
        public UnitData UnitData { get; private set; }
        public List<ICommand> ViableCommands { get; private set; } = new List<ICommand>();
        public AIModel AiModel { get; }

        public Unit(UnitData argUnitData, AIModel aiModel, List<ICommand> argViableCommands = null)
        {
            UnitData = argUnitData;
            AiModel = aiModel;
            if (argViableCommands != null)
            {
                ViableCommands = argViableCommands;
            }
        }

        public Unit(string argName, UnitIdentifier argUnitIdentifier, UnitStats argUnitStats, AIModel aiModel, List<ICommand> argViableCommands = null)
            : this(new UnitData(argName, argUnitIdentifier, argUnitStats), aiModel) 
        {

            if (argViableCommands != null)
            {
                ViableCommands = argViableCommands;
            }

            ViableCommands.Add(new BaseAttack());
            ViableCommands.Add(new BaseBlock());
            ViableCommands.AddRange(argViableCommands);
        }

        private Unit(Unit sourceUnit)
        {
            UnitData = sourceUnit.UnitData.DeepCopy();
            ViableCommands = sourceUnit.ViableCommands;
            AiModel = sourceUnit.AiModel;
            isDeepCopy = true;
        }

        public Unit DeepCopy()
        {
            return new Unit(this);
        }
    }
}
