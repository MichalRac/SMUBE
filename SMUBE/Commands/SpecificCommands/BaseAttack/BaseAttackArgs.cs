using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands.SpecificCommands.BaseAttack
{
    public class BaseAttackArgs : CommandArgs
    {
        public override CommandArgsValidator CommandArgsValidator => new BaseAttackArgsValidator();
    }
}
