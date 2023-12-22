using Commands;
using Commands.SpecificCommands.BaseAttack;
using SMUBE.AI;
using SMUBE.Commands.SpecificCommands.BaseBlock;
using SMUBE.DataStructures.Units;
using System.Collections.Generic;
using SMUBE.Commands.SpecificCommands.BaseWalk;

namespace SMUBE.Units
{
    public class Unit
    {
        private bool isDeepCopy = false;
        public UnitData UnitData { get; private set; }
        public List<ICommand> ViableCommands { get; private set; } = new List<ICommand>();
        public AIModel AiModel { get; }


        public Unit(string argName, UnitIdentifier argUnitIdentifier, UnitStats argUnitStats, AIModel aiModel, List<ICommand> argViableCommands = null)
        {
            UnitData = new UnitData(argName, argUnitIdentifier, argUnitStats);
            AiModel = aiModel;


            ViableCommands.Add(new BaseAttack());
            ViableCommands.Add(new BaseBlock());
            ViableCommands.Add(new BaseWalk());

            if (argViableCommands != null)
            {
                foreach (var argViableCommand in argViableCommands)
                {
                    ViableCommands.Add(argViableCommand);
                }
            }
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
