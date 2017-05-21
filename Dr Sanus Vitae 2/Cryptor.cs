using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace SanusVitae
{
    public static class Cryptor
    {
        public static string StrToMD5Hash(this string input)
        {
            using (MD5 hash = MD5.Create())
            {
                byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder str_builder = new StringBuilder();
                foreach (var data_byte in data)
                {
                    str_builder.Append(data_byte.ToString("x2"));
                }
                return str_builder.ToString();
            }
        }
        public static bool StrEqualsMD5Hash(this string input, string hash)
        {
            return input.StrToMD5Hash().Equals(hash);
        }
    }
}
