using SMUBE.BattleState;

namespace SMUBE.Commands.Args.ArgsPicker
{
    public sealed class OneToSelfArgsPicker : ArgsPicker
    {
        public OneToSelfArgsPicker(ICommand command, BattleStateModel battleStateModel) : base(command, battleStateModel)
        {
        }

        public override void Submit()
        {
            var args = GetCommandArgs();
            IsResolved = true;
            ArgsConfirmed?.Invoke(args);
        }

        public override void Return()
        {
            OperationCancelled?.Invoke();
        }

        protected override void SetDefaultTarget()
        {
        }

        public override bool IsAnyValid()
        {
            return true;
        }

        public override CommandArgs GetCommandArgs()
        {
            var state = BattleStateModel;
            var args = new CommonArgs(state.ActiveUnit.UnitData, null, state);
            return args;
        }

        public override void Up()
        {
        }

        public override void Down()
        {
        }

        public override void Left()
        {
        }

        public override void Right()
        {
        }

        public override string GetPickerInfo()
        {
            return "Confirm or cancel command on self";
        }

        public override string GetPickerState()
        {
            return "Confirm or cancel command on self";
        }
    }
}