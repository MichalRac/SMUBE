using SMUBE.DataStructures.BattleScene;
using SMUBE.DataStructures.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands
{
    public abstract class CommandArgs
    {
        public UnitData ActiveUnit { get; }
        public UnitData UnitTarget { get; }
        public BattleSceneState battleSceneState { get; }
        public abstract CommandArgsValidator CommandArgsValidator { get; }
    }
}
