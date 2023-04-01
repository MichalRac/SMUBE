using Commands;
using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI
{
    public abstract class AIModel
    {
        public abstract ICommand GetNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier);
        public abstract CommandArgs GetCommandArgs(ICommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier);
    }
}
