using SMUBE.BattleState;
using SMUBE.Units;
using System;
using System.Collections.Generic;

namespace SMUBE.Core
{
    public class BattleCore
    {
        public BattleStateModel currentStateModel { get; private set; }

        public BattleCore(List<Unit> initialUnits) 
        {
            if(initialUnits == null || initialUnits.Count == 0)
            {
                throw new ArgumentException(nameof(initialUnits), $"Cannot construct object of type {nameof(BattleCore)} with empty or null list of initial units");
            }

            currentStateModel = new BattleStateModel(initialUnits);
        }
    }
}
