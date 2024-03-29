using System.Collections.Generic;
using SMUBE.Commands._Common;
using SMUBE.Commands.Effects;
using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;

namespace SMUBE.Commands.Results
{
    public class CommandResults
    {
        public UnitData performer;
        public List<UnitData> targets = new List<UnitData>();
        public List<SMUBEVector2<int>> positionTargets = new List<SMUBEVector2<int>>();
        public List<Effect> effects = new List<Effect>();

        public List<PositionDelta> PositionDeltas { get; set; }

    }
}
