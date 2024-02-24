using System;
using SMUBE.BattleState;

namespace SMUBE.Commands.Args.ArgsPicker
{
    public abstract class ArgsPicker
    {
        protected readonly ICommand Command;
        protected readonly BattleStateModel BattleStateModel;

        protected Action OperationCancelled;
        protected Action<CommandArgs> ArgsUpdated;
        protected Action<CommandArgs> ArgsConfirmed;
        protected Action ArgsInvalid;

        public bool IsResolved = false;

        protected ArgsPicker(ICommand command, BattleStateModel battleStateModel)
        {
            Command = command;
            BattleStateModel = battleStateModel;
        }
        
        public abstract void Submit();
        public abstract void Return();

        protected abstract void SetDefaultTarget();

        public abstract bool IsAnyValid();

        public abstract CommandArgs GetCommandArgs();
        public abstract void Up();

        public abstract void Down();

        public abstract void Left();

        public abstract void Right();

        public abstract string GetPickerInfo();

        public abstract string GetPickerState();
    }
}