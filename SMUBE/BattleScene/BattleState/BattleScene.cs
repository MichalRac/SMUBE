using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.BattleState
{
    public class BattleScene
    {
        public string id = "0";
        public string name = "debug_scene";
        public const int MAX_UNITS = 6;

        public void Setup()
        {
            Console.WriteLine("SMUBE connection defined");
        }
    }
}
