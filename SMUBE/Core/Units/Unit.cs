using SMUBE.AI;
using SMUBE.Commands.SpecificCommands.BaseBlock;
using SMUBE.DataStructures.Units;
using System.Collections.Generic;
using SMUBE.Commands;
using SMUBE.Commands.SpecificCommands.BaseAttack;
using SMUBE.Commands.SpecificCommands.BaseWalk;

namespace SMUBE.Units
{
    public class Unit
    {
        private bool isDeepCopy = false;
        public UnitData UnitData { get; private set; }
        public UnitCommandProvider UnitCommandProvider;
        public AIModel AiModel { get; }


        public Unit(string argName, UnitIdentifier argUnitIdentifier, UnitStats argUnitStats, AIModel aiModel, List<ICommand> argViableCommands = null)
        {
            UnitData = new UnitData(argName, argUnitIdentifier, argUnitStats);
            AiModel = aiModel;

            var viableCommands = new List<ICommand>();

            viableCommands.Add(new BaseAttack());
            viableCommands.Add(new BaseBlock());
            viableCommands.Add(new BaseWalk());

            if (argViableCommands != null)
            {
                foreach (var argViableCommand in argViableCommands)
                {
                    viableCommands.Add(argViableCommand);
                }
            }

            UnitCommandProvider = new UnitCommandProvider(UnitData, viableCommands);
        }

        private Unit(Unit sourceUnit)
        {
            UnitData = sourceUnit.UnitData.DeepCopy();
            UnitCommandProvider = sourceUnit.UnitCommandProvider;
            AiModel = sourceUnit.AiModel;
            isDeepCopy = true;
        }

        public Unit DeepCopy()
        {
            return new Unit(this);
        }
    }
}
