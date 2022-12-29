using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMUBE.DataStructures.Utils
{
    sealed public partial class SMUBEVector2<T> : IEquatable<SMUBEVector2<T>> 
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
            return x.Equals(other.x) && y.Equals(other.y);
        }
    }
}
