using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.BattleScene
{
    public class BattleScene
    {
        public string id = "0";
        public string name = "debug_scene";
        public const int MAX_UNITS = 6;

        public BattleScene Setup()
        {
            return this;
        }
    }
}
