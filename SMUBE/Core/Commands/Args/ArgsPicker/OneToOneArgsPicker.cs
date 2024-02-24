using SMUBE.BattleState;

namespace SMUBE.Commands.Args.ArgsPicker
{
    public sealed class OneToOneArgsPicker : ArgsPicker
    {
        public OneToOneArgsPicker(ICommand command, BattleStateModel battleStateModel) : base(command, battleStateModel)
        {
        }

        public override void Submit()
        {
        }

        public override void Return()
        {
        }

        protected override void SetDefaultTarget()
        {
        }

        public override bool IsAnyValid()
        {
            return false;
        }

        public override CommandArgs GetCommandArgs()
        {
            return null;
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
            return string.Empty;
        }

        public override string GetPickerState()
        {
            return string.Empty;
        }
    }
}