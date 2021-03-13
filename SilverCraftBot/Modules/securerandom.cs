using System;
using System.Security.Cryptography;

namespace SilverCraftBot.Modules
{
    public class RandomGenerator
    {
        private readonly RNGCryptoServiceProvider csp;

        public RandomGenerator()
        {
            csp = new RNGCryptoServiceProvider();
        }

        public int Next(int minValue, int maxExclusiveValue)
        {
            if (minValue >= maxExclusiveValue)
            {
                ArgumentOutOfRangeException argumentOutOfRangeException = new ArgumentOutOfRangeException(nameof(minValue), "minValue must be lower than maxExclusiveValue");
                throw argumentOutOfRangeException;
            }

            long diff = (long)maxExclusiveValue - minValue;
            long upperBound = uint.MaxValue / diff * diff;

            uint ui;
            do
            {
                ui = GetRandomUInt();
            } while (ui >= upperBound);
            return (int)(minValue + (ui % diff));
        }

        private uint GetRandomUInt()
        {
            byte[] randomBytes = GenerateRandomBytes(sizeof(uint));
            return BitConverter.ToUInt32(randomBytes, 0);
        }

        private byte[] GenerateRandomBytes(int bytesNumber)
        {
            byte[] buffer = new byte[bytesNumber];
            csp.GetBytes(buffer);
            return buffer;
        }
    }
}