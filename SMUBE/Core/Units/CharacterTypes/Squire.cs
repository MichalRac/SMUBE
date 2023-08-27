using Commands;
using SMUBE.Commands.SpecificCommands.DefendAll;
using SMUBE.Commands.SpecificCommands.HealAll;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        };
        public override BaseCharacterType BaseCharacterType => BaseCharacterType.Squire;
    }
}
