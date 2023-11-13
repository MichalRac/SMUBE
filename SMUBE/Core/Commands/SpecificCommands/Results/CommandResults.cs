using SMUBE.Commands.Effects;
using SMUBE.Commands.SpecificCommands.Args;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands
{
    public class CommandResults
    {
        public UnitData performer;
        public List<UnitData> targets = new List<UnitData>();
        public List<Effect> effects = new List<Effect>();

        public List<PositionDelta> PositionDeltas { get; set; }

    }
}
