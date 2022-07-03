using charlie.bll.interfaces;
using System;

namespace charlie.bll.providers
{
    public class RandomNumberProvider : IRandomNumberProvider
    {
        private ulong[] state = new ulong[16];
        private uint index = 0;
        private ILogWriter _logger;

        public RandomNumberProvider(ILogWriter logger)
        {
            _logger = logger;
            ReSeed((ulong)DateTime.UtcNow.Ticks);
        }

        public void ReSeed(ulong seed)
        {
            _logger.ServerLogInfo("reseeding random");
            state[0] = seed;

            for (int i = 1; i < state.Length; i++)
            {
                state[i] = state[i - 1] * 47;
            }

            index = 0;
        }

        private double CalcInRange(double min, double max, double val)
        {
            if (max < min)
            {
                var temp = max;
                max = min;
                min = temp;
            }

            var diff = max - min;
            return min + diff * val;
        }

        public double Next()
        {
            ulong a, b, c, d;

            a = state[index];
            c = state[(index + 13) & 15];
            b = a ^ c ^ (a << 16) ^ (c << 15);
            c = state[(index + 9) & 15];
            c ^= (c >> 11);
            a = state[index] = b ^ c;
            d = a ^ ((a << 5) & 0xDA442D20UL);
            index = (index + 15) & 15;
            a = state[index];
            state[index] = a ^ b ^ d ^ (a << 2) ^ (b << 18) ^ (c << 28);

            return (state[index] / (double)ulong.MaxValue);
        }


        public double Next(double min, double max)
        {
            var val = Next();

            return CalcInRange(min, max, val);
        }
    }
}
