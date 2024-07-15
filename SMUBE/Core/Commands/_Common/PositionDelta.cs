using System.Collections.Generic;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;

namespace SMUBE.Commands._Common
{
    public class PositionDelta
    {
        public UnitIdentifier UnitIdentifier { get; }
        public SMUBEVector2<int> Start { get; }
        public SMUBEVector2<int> Target { get; }
        public bool IsPathless { get; }
        public List<SMUBEVector2<int>> Path;

        public PositionDelta(UnitIdentifier unitIdentifier, SMUBEVector2<int> start, SMUBEVector2<int> target, List<SMUBEVector2<int>> path, bool isPathless = false)
        {
            UnitIdentifier = unitIdentifier;
            Start = start;
            Target = target;
            Path = path;
            IsPathless = isPathless;
        }
    }
}
