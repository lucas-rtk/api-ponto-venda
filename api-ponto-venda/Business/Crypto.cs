using System;
using System.Security.Cryptography;

namespace api_ponto_venda.Business
{
    public class Crypto
    {
        private const int BYTE_SIZE = 24;
        private const int HASING_ITERATIONS = 10101;
        private const string CHAVE = "0febc9373cd766198c3f7fc011b848e3323c67ac7419d7fa";

        public static byte[] CalcularHash(string senha)
        {
            Rfc2898DeriveBytes hashGenerator = new Rfc2898DeriveBytes(senha, Convert.FromBase64String(CHAVE))
            {
                IterationCount = HASING_ITERATIONS
            };
            return hashGenerator.GetBytes(BYTE_SIZE);
        }

        public static bool CompararHashes(byte[] hash1, byte[] hash2)
        {
            int tamanhoMenorHash = hash1.Length <= hash2.Length ? hash1.Length : hash2.Length;
            int xor = hash1.Length ^ hash2.Length;

            for (int i = 0; i < tamanhoMenorHash; i++)
                xor |= hash1[i] ^ hash2[i];

            return 0 == xor;
        }
    }
}