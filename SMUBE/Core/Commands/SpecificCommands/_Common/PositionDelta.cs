using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Commands.SpecificCommands.Args
{
    public class PositionDelta
    {
        public UnitIdentifier UnitIdentifier { get; }
        public SMUBEVector2<int> Start { get; }
        public SMUBEVector2<int> Target { get; }

        public PositionDelta(UnitIdentifier unitIdentifier, SMUBEVector2<int> start, SMUBEVector2<int> target)
        {
            UnitIdentifier = unitIdentifier;
            Start = start;
            Target = target;
        }
    }
}
