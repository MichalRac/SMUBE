using Commands;
using SMUBE.DataStructures.Units;
using SMUBE.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Commands.Effects
{
    public abstract class Effect
    {
        public virtual int GetPersistence() => 0;
        public virtual void Apply(UnitStats unitStats)
        {
        }
        public virtual void OnTurnStartEvaluate(UnitStats unitStats)
        {
        }
        public virtual void ModifyCommandResult(CommandResults commandResults)
        {
        }
    }
}
