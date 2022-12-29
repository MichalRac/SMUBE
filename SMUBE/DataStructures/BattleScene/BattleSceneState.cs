﻿using SMUBE.DataStructures.Units;
using SMUBE.DataStructures.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.DataStructures.BattleScene
{
    public class BattleSceneState
    {
        public Dictionary<BattleScenePosition, BattleScenePositionContent> positions = new Dictionary<BattleScenePosition, BattleScenePositionContent>();
    }
}
