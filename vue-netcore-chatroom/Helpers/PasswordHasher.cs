using System;
using System.Security.Cryptography;
using System.Text;

namespace vue_netcore_chatroom.Helpers
{
    public class PasswordHasher
    {
        private static readonly int _saltSize = 16; // 128 bits
        private static readonly int _hashSize = 32; // 256 bits
        private static readonly int _iterations = 100000;
        private static readonly HashAlgorithmName _algorithmName = HashAlgorithmName.SHA256;

        private const char segmentDelimiter = ':';

        public static string Hash(string password)
        {
            // Create salt
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[_saltSize]);

            // Create hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, _iterations);
            var hash = pbkdf2.GetBytes(_hashSize);

            // Combine salt and hash
            var hashBytes = new byte[_saltSize + _hashSize];
            Array.Copy(salt, 0, hashBytes, 0, _saltSize);
            Array.Copy(hash, 0, hashBytes, _saltSize, _hashSize);

            // Convert to base64
            var base64Hash = Convert.ToBase64String(hashBytes);


            // Format hash with extra information
            return string.Format("$MYHASH$V1${0}${1}", _iterations, base64Hash);
        }

        private static bool IsHashSupported(string hashString)
        {
            return hashString.Contains("$MYHASH$V1$");
        }

        public static bool Verify(string password, string hashedPassword)
        {
            // Check hash
            if (!IsHashSupported(hashedPassword))
            {
                throw new NotSupportedException("The hashtype is not supported");
            }

            // Extract iteration and Base64 string
            var splittedHashString = hashedPassword.Replace("$MYHASH$V1$", "").Split('$');
            var iterations = int.Parse(splittedHashString[0]);
            var base64Hash = splittedHashString[1];


            // Get hash bytes
            var hashBytes = Convert.FromBase64String(base64Hash);

            // Get salt
            var salt = new byte[_saltSize];
            Array.Copy(hashBytes, 0, salt, 0, _saltSize);

            // Create hash with given salt
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(_hashSize);

            // Get result
            for (var i = 0; i < _hashSize; i++)
            {
                if (hashBytes[i + _saltSize] != hash[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
