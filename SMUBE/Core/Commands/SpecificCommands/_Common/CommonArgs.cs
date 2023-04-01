using SMUBE.BattleState;
using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands.SpecificCommands._Common
{
    public class CommonArgs : CommandArgs
    {
        public CommonArgs(UnitData activeUnit, List<UnitData> targetUnits, BattleStateModel battleStateModel) 
            : base(activeUnit, targetUnits, battleStateModel)
        {
        }
    }
}
