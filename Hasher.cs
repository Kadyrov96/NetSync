using System.Security.Cryptography;
using System.Text;

namespace TEST2
{
    class Hasher
    {
        private static MD5 md5hashFunc;
        private static byte[] byted_data_hash;
        private static StringBuilder stringBuilder;

        internal static string GetMd5Hash(byte[] byteInput)
        {
            md5hashFunc = MD5.Create();
            byted_data_hash = md5hashFunc.ComputeHash(byteInput);

            stringBuilder = new StringBuilder();
            for (int i = 0; i < byted_data_hash.Length; i++)
            {
                stringBuilder.Append(byted_data_hash[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        internal static string GetMd5Hash(string stringInput)
        {
            md5hashFunc = MD5.Create();
            byted_data_hash = md5hashFunc.ComputeHash(Encoding.UTF8.GetBytes(stringInput));

            stringBuilder = new StringBuilder();
            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < byted_data_hash.Length; i++)
            {
                stringBuilder.Append(byted_data_hash[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }
    }
}
