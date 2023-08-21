﻿using Commands;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.Commands.Effects
{
    public class BlockEffect : Effect
    {
        private const int DEFAULT_PERSISTANCE = 1;
        private int currentPersistance = DEFAULT_PERSISTANCE;

        public override int GetPersistence() => currentPersistance;
        public override void OnTurnStartEvaluate(UnitStats unitStats)
        {
            base.OnTurnStartEvaluate(unitStats);
            currentPersistance--;
        }
        public override void ModifyCommandResult(CommandResults commandResults)
        {
            base.ModifyCommandResult(commandResults);

            foreach (var effect in commandResults.effects)
            {
                if(effect is DamageEffect damageEffect)
                {
                    damageEffect.Value /= 2; 
                }
            }
        }
    }
}