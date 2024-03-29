using SMUBE.DataStructures.Units;
using System.Collections.Generic;
using SMUBE.Commands;

namespace SMUBE.Units.CharacterTypes
{
    public enum BaseCharacterType
    {
        None = 0,

        Scholar = 1,
        Squire = 2,
        Hunter = 3,
    }

    public abstract class BaseCharacter
    {
        public abstract UnitStats DefaultStats { get; }
        public abstract BaseCharacterType BaseCharacterType { get; }

        public abstract List<ICommand> AvailableCommands { get; }
    }
}
