using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace charlie.security
{
    public class Helper
    {
        public static string Encrypt(string data, string key)
        {
            using (Aes csp = Aes.Create())
            {
                byte[] keyArr = Convert.FromBase64String(key);
                byte[] keyArrBytes32 = new byte[32];
                Array.Copy(keyArr, keyArrBytes32, 32);
                csp.Key = keyArr;
                csp.Padding = PaddingMode.PKCS7;
                csp.Mode = CipherMode.ECB;
                ICryptoTransform encrypter = csp.CreateEncryptor();
                return Convert.ToBase64String(encrypter.TransformFinalBlock(ASCIIEncoding.UTF8.GetBytes(data), 0, ASCIIEncoding.UTF8.GetBytes(data).Length));
            }
        }

        public static string Decrypt(string data, string key)
        {
            using (Aes csp = Aes.Create())
            {
                byte[] keyArr = Convert.FromBase64String(key);
                byte[] keyArrBytes32 = new byte[32];
                Array.Copy(keyArr, keyArrBytes32, 32);
                csp.Key = keyArr;
                csp.Padding = PaddingMode.PKCS7;
                csp.Mode = CipherMode.ECB;
                ICryptoTransform decrypter = csp.CreateDecryptor();
                return Convert.ToBase64String(decrypter.TransformFinalBlock(ASCIIEncoding.UTF8.GetBytes(data), 0, ASCIIEncoding.UTF8.GetBytes(data).Length));
            }
        }

        public static string PinToAesKey(string pin)
        {
            byte[] salt = new byte[0];

            string firstHash = ByteArrayToHexString(
                KeyDerivation.Pbkdf2(
                    password: pin,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 1024,
                    numBytesRequested: 16
                )
            );

            byte[] key = KeyDerivation.Pbkdf2(
                password: firstHash.ToLower(),
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1024,
                numBytesRequested: 32
            );

            return Convert.ToBase64String(key);
        }

        public static byte[] SHA256_Hash(byte[] value)
        {
            return SHA256.Create().ComputeHash(value);
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            return BitConverter.ToString(ba).Replace("-", "").ToLower();
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

    }

}
