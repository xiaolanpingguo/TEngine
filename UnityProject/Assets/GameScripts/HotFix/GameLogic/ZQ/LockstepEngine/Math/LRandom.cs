using System;


namespace Lockstep.Framework
{
    public class LRandom
    {
        public ulong randSeed = 1;
        public LFloat value => new LFloat(true, Range(0, 1000));

        public LRandom(uint seed = 17)
        {
            randSeed = seed;
        }

        public uint Next()
        {
            randSeed = randSeed * 1103515245 + 36153;
            return (uint)(randSeed / 65536);
        }

        // range:[0 ~(max-1)]
        public uint Next(uint max)
        {
            return Next() % max;
        }

        public int Next(int max)
        {
            return (int)(Next() % max);
        }

        // range:[min~(max-1)]
        public uint Range(uint min, uint max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException("minValue", string.Format("'{0}' cannot be greater than {1}.", min, max));

            uint num = max - min;
            return this.Next(num) + min;
        }

        public int Range(int min, int max)
        {
            if (min >= max - 1)
                return min;

            int num = max - min;
            return this.Next(num) + min;
        }

        public LFloat Range(LFloat min, LFloat max)
        {
            if (min > max)
                throw new ArgumentOutOfRangeException("minValue", string.Format("'{0}' cannot be greater than {1}.", min, max));

            uint num = (uint)(max._val - min._val);
            return new LFloat(true, Next(num) + min._val);
        }
    }
}