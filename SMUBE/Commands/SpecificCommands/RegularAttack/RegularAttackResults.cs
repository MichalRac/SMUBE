using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands.SpecificCommands.RegularAttack
{
    public class RegularAttackResults : CommandResults
    {
        public int targetXPosition { get; private set; }
        public int targetYPosition { get; private set; }
        public int damageDealt { get; private set; }
    }
}
