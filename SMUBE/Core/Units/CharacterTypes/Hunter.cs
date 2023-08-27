using Commands;
using SMUBE.Commands.SpecificCommands.HeavyAttack;
using SMUBE.DataStructures.Units;
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
        public override UnitStats DefaultStats => new UnitStats(this,
                                                                UnitConsts.HunterInfo.MaxHealth,
                                                                UnitConsts.HunterInfo.MaxStamina,
                                                                UnitConsts.HunterInfo.MaxMana,
                                                                UnitConsts.HunterInfo.Power,
                                                                UnitConsts.HunterInfo.Defense,
                                                                UnitConsts.HunterInfo.Speed);

        public override List<ICommand> AvailableCommands => new List<ICommand>()
        {
            new HeavyAttack(),
        };
    }
}
