using System.Dynamic;
using System.Linq;
using SMUBE.Commands.Args;
using SMUBE.DataStructures.Units;
using SMUBE.Commands.Results;

namespace SMUBE.Commands.Effects
{
    public class DamageEffect : Effect
    {
        public int Value { get; set; }

        public DamageEffect(int value)
        {
            Value = value;
        }

        public override string GetDescriptor()
        {
            return "Damaged";
        }

        public override void Apply(UnitStats unitStats, CommandResults commandResults)
        {            
            unitStats.DeltaHealth((int)(Value * -1 * GetEffectTypeEffectivness(unitStats, commandResults)));
        }

        public int GetFinalValue(CommandArgs args, CommandResults commandResults)
        {
            return (int)(Value * -1 * GetEffectTypeEffectivness(args.TargetUnits.First().UnitStats, commandResults));
        }
    }
}
