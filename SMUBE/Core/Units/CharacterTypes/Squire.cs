using SMUBE.Commands.SpecificCommands.DefendAll;
using SMUBE.DataStructures.Units;
using System.Collections.Generic;
using SMUBE.Commands;
using SMUBE.Commands.SpecificCommands.LowerEnemyDefense;
using SMUBE.Commands.SpecificCommands.Tackle;
using SMUBE.Commands.SpecificCommands.Taunt;

namespace SMUBE.Units.CharacterTypes
{
    public class Squire : BaseCharacter
    {
        public override UnitStats DefaultStats => new UnitStats(this,
                                                                UnitConsts.SquireInfo.MaxHealth,
                                                                UnitConsts.SquireInfo.MaxStamina,
                                                                UnitConsts.SquireInfo.MaxMana,
                                                                UnitConsts.SquireInfo.Power,
                                                                UnitConsts.SquireInfo.Defense,
                                                                UnitConsts.SquireInfo.Speed);

        public override List<ICommand> AvailableCommands => new List<ICommand>()
        {
            new DefendAll(),
            new Taunt(),
            new Tackle(),
        };
        public override BaseCharacterType BaseCharacterType => BaseCharacterType.Squire;
    }
}
