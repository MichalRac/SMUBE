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
        bool Validate(CommandArgs args, BattleStateModel battleStateModel);
    }
}
