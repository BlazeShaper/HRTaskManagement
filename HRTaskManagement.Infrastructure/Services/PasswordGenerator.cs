using System.Security.Cryptography;
using HRTaskManagement.Application.Interfaces;

namespace HRTaskManagement.Infrastructure.Services
{
    public class PasswordGenerator : IPasswordGenerator
    {
        private const string Lower = "abcdefghijkmnopqrstuvwxyz";
        private const string Upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        private const string Digits = "0123456789";
        private const string Special = "!@#$%^&*";

        public string Generate(int length = 12)
        {
            if (length < 8) length = 8;

            var all = Lower + Upper + Digits + Special;
            var chars = new char[length];

            // Her kategoriden en az 1 karakter garanti et
            chars[0] = Lower[RandomNumberGenerator.GetInt32(Lower.Length)];
            chars[1] = Upper[RandomNumberGenerator.GetInt32(Upper.Length)];
            chars[2] = Digits[RandomNumberGenerator.GetInt32(Digits.Length)];
            chars[3] = Special[RandomNumberGenerator.GetInt32(Special.Length)];

            for (int i = 4; i < length; i++)
                chars[i] = all[RandomNumberGenerator.GetInt32(all.Length)];

            // Fisher-Yates karıştırma (ilk 4 karakter hep aynı sırada olmasın)
            for (int i = chars.Length - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (chars[i], chars[j]) = (chars[j], chars[i]);
            }

            return new string(chars);
        }
    }
}