using System;

namespace Utils
{
    public static class Extensions
    {
        private static Random _rnd = new Random();
        
        public static bool IsOneOf<T>(this T t, params T[] variants)
        {
            foreach (var variant in variants)
            {
                if (t.Equals(variant))
                    return true;
            }

            return false;
        }

        public static T GetRandom<T>(bool excludeZero = false) where T : Enum
        {
            var values = Enum.GetValues(typeof(T));
            var from = excludeZero ? 1 : 0;
            var i = _rnd.Next(from, values.Length);
            return (T) values.GetValue(i);
        }
    }
}