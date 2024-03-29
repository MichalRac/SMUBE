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

        public PositionDelta(UnitIdentifier unitIdentifier, SMUBEVector2<int> start, SMUBEVector2<int> target, bool isPathless = false)
        {
            UnitIdentifier = unitIdentifier;
            Start = start;
            Target = target;
            IsPathless = isPathless;
        }
    }
}
