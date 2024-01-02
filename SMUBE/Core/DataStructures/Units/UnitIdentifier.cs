using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.DataStructures.Units
{
    [Serializable]
    public class UnitIdentifier : IEquatable<UnitIdentifier>
    {
        public int PersonalId { get; private set; }
        public int TeamId { get; private set; }

        public UnitIdentifier(int id, int teamId)
        {
            PersonalId = id;
            TeamId = teamId;
        }
     
        public bool Equals(UnitIdentifier other)
        {
            return PersonalId == other.PersonalId && TeamId == other.TeamId;
        }

        public override string ToString()
        {
            return $"T{TeamId}:ID{PersonalId}";
        }
    }
}
