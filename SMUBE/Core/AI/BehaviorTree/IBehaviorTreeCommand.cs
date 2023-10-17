using Commands;
using SMUBE.BattleState;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.AI.BehaviorTree
{
    public interface IBehaviorTreeCommand
    {
        CommandArgs CommandArgsCache { get; }
        CommandArgs GetCommandArgs(ICommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier);
    }
}
