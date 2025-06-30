using System.Security.Cryptography;

namespace MobileAppsAPIS.DataAccess
{
    public static class OtpGenerator
    {
        public static string GenerateOtp(int length = 6)
        {
            if (length <= 0 || length > 9)
                throw new ArgumentOutOfRangeException(nameof(length), "OTP length must be between 1 and 9.");

            int min = (int)Math.Pow(10, length - 1);
            int max = (int)Math.Pow(10, length) - 1;

            return GetSecureRandomNumber(min, max).ToString();
        }

        private static int GetSecureRandomNumber(int minValue, int maxValue)
        {
            if (minValue >= maxValue)
                throw new ArgumentOutOfRangeException("minValue must be less than maxValue");

            byte[] uint32Buffer = new byte[4];

            using (var rng = RandomNumberGenerator.Create())
            {
                while (true)
                {
                    rng.GetBytes(uint32Buffer);
                    uint random = BitConverter.ToUInt32(uint32Buffer, 0);

                    const long range = (long)uint.MaxValue + 1;
                    long remainder = range % (maxValue - minValue + 1);
                    if (random < range - remainder)
                    {
                        return (int)(minValue + (random % (maxValue - minValue + 1)));
                    }
                }
            }
        }
    }
}
