using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.DataStructures.Utils
{
    public partial class SMUBEVector2<T> : IEquatable<SMUBEVector2<T>> 
        where T : IEquatable<T>
    {
        public T x;
        public T y;

        public SMUBEVector2(T x, T y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(SMUBEVector2<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return EqualityComparer<T>.Default.Equals(x, other.x) && EqualityComparer<T>.Default.Equals(y, other.y);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SMUBEVector2<T>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(x) * 397) ^ EqualityComparer<T>.Default.GetHashCode(y);
            }
        }
    }
}
