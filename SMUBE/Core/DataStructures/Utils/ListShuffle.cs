using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace SMUBE.DataStructures.Utils
{
    public static class ListShuffle
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            if(list.Count <= 1)
                return;
            
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
    
    public static class ListRandomPick
    {
        public static T GetRandom<T>(this IReadOnlyList<T> list)
        {
            if (list.Count == 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (list.Count == 1)
            {
                return list[0];
            }
            
            Random random = new Random();
            return list[random.Next(0, list.Count)];
        }
    }

}
