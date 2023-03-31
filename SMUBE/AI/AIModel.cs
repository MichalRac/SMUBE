using Commands;
using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AI
{
    public abstract class AIModel
    {
        public abstract ICommand GetNextCommand(BattleStateModel battleStateModel, UnitIdentifier unitIdentifier);
    }
}
