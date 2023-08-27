using Commands;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Commands.Effects
{
    public class HealEffect : Effect
    {
        public int Value { get; set; }

        public HealEffect(int value)
        {
            Value = value;
        }

        public override void Apply(UnitStats unitStats, CommandResults commandResults)
        {
            unitStats.DeltaHealth(Value);
        }
    }
}
