using SMUBE.DataStructures.Units;
using SMUBE.Commands.Results;

namespace SMUBE.Commands.Effects
{
    public class HealEffect : Effect
    {
        public int Value { get; set; }

        public HealEffect(int value)
        {
            Value = value;
        }

        public override string GetDescriptor()
        {
            return "Heal";
        }

        public override void Apply(UnitStats unitStats, CommandResults commandResults)
        {
            unitStats.DeltaHealth(Value);
        }
    }
}
