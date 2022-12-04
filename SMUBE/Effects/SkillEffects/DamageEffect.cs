using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Effects.SkillEffects
{
    public class DamageEffect : Effect
    {
        private int Value;

        public DamageEffect(int value)
        {
            Value = value;
        }
    }
}
