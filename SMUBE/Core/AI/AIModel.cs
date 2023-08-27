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
        public bool UseSimpleBehavior { get; }

        protected AIModel(bool useSimpleBehavior)
        {
            this.UseSimpleBehavior = useSimpleBehavior;
        }

        public abstract ICommand ResolveNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier);
        public abstract CommandArgs GetCommandArgs(ICommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier);

        public void SimulationPrewarm(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            battleStateModel.TryGetUnit(activeUnitIdentifier, out var _);
        }
    }
}
