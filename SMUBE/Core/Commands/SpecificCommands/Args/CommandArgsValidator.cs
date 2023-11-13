using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands
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
        bool Validate(CommandArgs args);
    }
}
