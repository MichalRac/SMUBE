using SMUBE.BattleState;
using SMUBE.Commands;
using SMUBE.Commands.Args;
using SMUBE.DataStructures.Units;

namespace SMUBE.AI
{
    public abstract class AIModel
    {
        public bool UseSimpleBehavior { get; }

        protected AIModel(bool useSimpleBehavior)
        {
            this.UseSimpleBehavior = useSimpleBehavior;
        }

        public abstract BaseCommand ResolveNextCommand(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier);
        public abstract CommandArgs GetCommandArgs(BaseCommand command, BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier);

        public void SimulationPrewarm(BattleStateModel battleStateModel, UnitIdentifier activeUnitIdentifier)
        {
            battleStateModel.TryGetUnit(activeUnitIdentifier, out var _);
        }
    }
}
