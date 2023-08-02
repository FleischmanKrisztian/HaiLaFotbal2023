using RestApi.Constants;
using System.Security.Cryptography;
using System.Text;

namespace RestApi.Utils
{
    public class GetHashedString
    {
        public static string Execute(string inputString)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(Secrets.LOGIN_API_FIRST_SALT + inputString + Secrets.LOGIN_API_SECOND_SALT))
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }

        public static byte[] GetHash(string inputString)
        {
            using (HashAlgorithm algorithm = SHA256.Create())
                return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }
    }
}