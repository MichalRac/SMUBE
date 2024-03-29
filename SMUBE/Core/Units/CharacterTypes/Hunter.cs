using SMUBE.Commands.SpecificCommands.HeavyAttack;
using SMUBE.DataStructures.Units;
using System.Collections.Generic;
using SMUBE.Commands;
using SMUBE.Commands.SpecificCommands.RaiseObstacle;
using SMUBE.Commands.SpecificCommands.Teleport;

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
            new RaiseObstacle(),
            new Teleport(),
        };

        public override BaseCharacterType BaseCharacterType => BaseCharacterType.Hunter;
    }
}
