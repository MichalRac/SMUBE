using SMUBE.BattleState;

namespace SMUBE.Commands.Args
{
    public enum ArgsConstraint
    {
        None = 0,

        Ally = 1,
        Enemy = 2,
        Position = 3,
    }

    public interface CommandArgsValidator
    {
        ArgsConstraint ArgsConstraint { get; }
        ArgsPicker.ArgsPicker GetArgsPicker(ICommand command, BattleStateModel battleStateModel);
        bool Validate(CommandArgs args, BattleStateModel battleStateModel);
    }
}
