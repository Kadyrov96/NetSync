using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NetSync_WinDesktop
{
    /// <summary>
    /// Provides methods to get hashes of files and strings.
    /// </summary>
    class Hasher
    {
        /// <summary>
        /// Gets hash value of input array of bytes.
        /// </summary>
        private static string GetHash(byte[] byteInput)
        {
            MD5 md5hashFunc = MD5.Create();
            byte[] byted_data_hash = md5hashFunc.ComputeHash(byteInput);

            StringBuilder stringBuilder = new StringBuilder();
            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < byted_data_hash.Length; i++)
            {
                stringBuilder.Append(byted_data_hash[i].ToString("x2"));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets hash value of the file, which path is specified in param.
        /// </summary>
        internal static string GetFileHash(string filePath)
        {
            byte[] bytesOfFile = File.ReadAllBytes(filePath);
            return GetHash(bytesOfFile);
        }

        /// <summary>
        /// Gets hash value of the string specified in param.
        /// </summary>
        internal static string GetStringHash(string stringInput)
        {
            byte[] bytesOfString = Encoding.UTF8.GetBytes(stringInput);
            return GetHash(bytesOfString);
        }
    }
}
