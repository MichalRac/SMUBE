using SMUBE.DataStructures.Units;
using SMUBE.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Commands.Effects
{
    public class DamageEffect : Effect
    {
        public int Value { get; set; }

        public DamageEffect(int value)
        {
            Value = value;
        }

        public override void Apply(UnitStats unitStats)
        {
            unitStats.DeltaHealth(Value * -1);
        }
    }
}
