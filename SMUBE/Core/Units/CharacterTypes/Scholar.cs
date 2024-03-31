using SMUBE.Commands.SpecificCommands.HealAll;
using SMUBE.DataStructures.Units;
using System.Collections.Generic;
using SMUBE.Commands;
using SMUBE.Commands.SpecificCommands.LowerEnemyDefense;
using SMUBE.Commands.SpecificCommands.ShieldPosition;

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

        public override List<BaseCommand> AvailableCommands => new List<BaseCommand>()
        {
            new HealAll(),
            new ShieldPosition(),
            new LowerEnemyDefense(),
        };
        public override BaseCharacterType BaseCharacterType => BaseCharacterType.Scholar;

    }
}
