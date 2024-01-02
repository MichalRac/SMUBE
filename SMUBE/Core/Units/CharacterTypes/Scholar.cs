using SMUBE.Commands.SpecificCommands.HealAll;
using SMUBE.Commands.SpecificCommands.HeavyAttack;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMUBE.Commands;

namespace SMUBE.Units.CharacterTypes
{
    public class Scholar : BaseCharacter
    {
        public override UnitStats DefaultStats => new UnitStats(this,
                                                                UnitConsts.ScholarInfo.MaxHealth,
                                                                UnitConsts.ScholarInfo.MaxStamina,
                                                                UnitConsts.ScholarInfo.MaxMana,
                                                                UnitConsts.ScholarInfo.Power,
                                                                UnitConsts.ScholarInfo.Defense,
                                                                UnitConsts.ScholarInfo.Speed);

        public override List<ICommand> AvailableCommands => new List<ICommand>()
        {
            new HealAll(),
        };
        public override BaseCharacterType BaseCharacterType => BaseCharacterType.Scholar;

    }
}
