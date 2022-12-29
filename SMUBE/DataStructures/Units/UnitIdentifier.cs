using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.DataStructures.Units
{
    public class UnitIdentifier : IEquatable<UnitIdentifier>
    {
        public int Id { get; private set; }
        public int TeamId { get; private set; }

        public UnitIdentifier(int id, int teamId)
        {
            Id = id;
            TeamId = teamId;
        }
     
        public bool Equals(UnitIdentifier other)
        {
            return Id == other.Id && TeamId == other.TeamId;
        }
    }
}
