using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace WebAPI.Extensions
{
    public static class StringExtention
    {
        public static string ToMD5Hash(this string text)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] textBytes = Encoding.ASCII.GetBytes(text);
                byte[] hashBytes = md5.ComputeHash(textBytes);
                return Convert.ToHexString(hashBytes);   
            }
        }
    }
}
