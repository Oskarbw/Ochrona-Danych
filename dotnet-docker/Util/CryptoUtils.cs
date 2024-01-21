using BCrypt.Net;
using dotnet_docker.Model;
using System.Security.Cryptography;
using System.Text;

namespace dotnet_docker.Util
{
    public class CryptoUtils
    {
        private readonly Random _rand;

        public CryptoUtils()
        {
            _rand = new Random();
        }

        public static string Encryption(string strText)
        {
            var publicKey = "<RSAKeyValue><Modulus>rfzlz5vARPlfFHSPfit9GoJ3uQ7u7pGZj9PIYQcJjM+600ZyOh1ofz+zCBx6RodcpYevk8Vb2PMk4Z1ePzghLpGep08yhASUspzyOtPwDdXqhX7Mf0ySdUF2ICpypkAu9/QYjvw1/fUm+aFGOAqqeFsJpXaDX6jl+SgPahwCrl0=</Modulus><Exponent>AQAB</Exponent><P>4LNv8IfQEaapvSk/6xW6BH9JZa0WqL3CoeVT9n4ySq8S2GYE9XmbFte28LK98eW+N8v7hhiCK8WWY4vb1cSPpw==</P><Q>xjkYPna3HuwWSav4/48Q2WHMcT5zBxAkGYxWTiZSUtHiXC735K627ELYtX4ZaWUVqX1w14s0SOBLRlY3FuMyWw==</Q><DP>dV8ldLXsiJvPBCEc4zZJIXo/o53DPUdJ+Hkq35HRwVMr+99mbbckvMzXIWmscEO6lbi2XLhGnoiqYrs2jLYM9w==</DP><DQ>XM6Gh1hVzGiE1uFpp114ag7cBXlTqc7o1/1YuyY+DQCvlrF25t7WTi/N/suXYj0tszlEB+bpB+Xb2IatLE4bWQ==</DQ><InverseQ>SGLSknLn0hzB9qCcCGLyk3UHRlut98wN2s5riNjmclUQODxgNr0x6ak0HbsRVnPiR+BzGgmyGG8hTB1EZIyolQ==</InverseQ><D>OYN/9EDoLeTBKWHejTaTBFBcgzAMi5BV0tWPR4OsBIAmofCHke5mvKmx5NyFDwtv9MgFojN7SRwW9P2wSfWkAdUTTHa4uLrcafR1YkxcNKcJd39nPcm0r+hdURvGKBg+rWnhdE0Nd+lrcR0u0+clFpmokTdHuActqJZtJoTg6YE=</D></RSAKeyValue>";
            var testData = Encoding.UTF8.GetBytes(strText);

            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    // client encrypting data with public key issued by server                    
                    rsa.FromXmlString(publicKey.ToString());

                    var encryptedData = rsa.Encrypt(testData, true);

                    var base64Encrypted = Convert.ToBase64String(encryptedData);

                    return base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        public static string Decryption(string strText)
        {
            var privateKey = Environment.GetEnvironmentVariable("cipher_key");
            var testData = Encoding.UTF8.GetBytes(strText);

            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    var base64Encrypted = strText;

                    // server decrypting data with private key                    
                    rsa.FromXmlString(privateKey);

                    var resultBytes = Convert.FromBase64String(base64Encrypted);
                    var decryptedBytes = rsa.Decrypt(resultBytes, true);
                    var decryptedData = Encoding.UTF8.GetString(decryptedBytes);
                    return decryptedData.ToString();
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        public int randomNumber(int min, int max)
        {
            return _rand.Next(min, max);
        }

        public bool verifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);      
        }
        public List<Password> createTenPasswordEntities(string password, string username)
        {   
            
            // Utworz 10 patternów na zasadzie _XX_X__XX

            int passwordLength = password.Length;
            int fragmentLength = passwordLength / 2;

            List<string> patterns = new List<string>();
            List<string> passwordFragments = new List<string>();

            for (int i = 1; i < 11; i++)
            {
                List<int> indexes = new List<int>();
                for (int k = 0; k < fragmentLength; k++)
                {
                    int randomInt = _rand.Next(1, passwordLength + 1);
                    while (indexes.Contains(randomInt))
                    {
                        randomInt = _rand.Next(1, passwordLength + 1);
                    }
                    indexes.Add(randomInt);
                }

                string pattern = "";
                string passwordFragment = "";
                for (int j = 1; j < passwordLength + 1; j++)
                {
                    if (indexes.Contains(j)) { pattern = pattern + "_ "; passwordFragment = passwordFragment + password[j - 1]; }
                    else { pattern = pattern + "X "; }
                }
                patterns.Add(pattern.TrimEnd());
                passwordFragments.Add(passwordFragment);
            }

            List<Password> passwordEntities = new List<Password>();
            List<string> passwordHashes = new List<string>();

            for (int i = 1; i < 11; i++)
            {

                var hash = BCrypt.Net.BCrypt.HashPassword(passwordFragments[i - 1]);
                passwordHashes.Add(hash);
            }


            for (int i = 1; i < 11; i++)
            {
                Password passwordEntity = new Password
                {
                    Id = Guid.NewGuid(),
                    Variant = i,
                    Username = username,
                    Pattern = patterns[i - 1],
                    Hash = passwordHashes[i - 1]
                };
                passwordEntities.Add(passwordEntity);
            }


            return passwordEntities;
        }

        public static double CalculateEntropy(string input)
        {
            var inputCount = input.Length;
            var valueCounts = input.GroupBy(x => x)
                                   .ToDictionary(x => x.Key, x => x.Count());

            double entropy = 0;
            foreach (var valueCount in valueCounts)
            {
                var frequency = (double)valueCount.Value / inputCount;
                entropy -= frequency * (Math.Log(frequency) / Math.Log(2));
            }

            return entropy;
        }

    }
}
