using System;
using System.Security.Cryptography;
using System.Text;

namespace DamnCandy.Utilities
{
    internal static class GuidUtilities
    {
        public static Guid CreateGuid(this string input)
        {
#if UNITY_5_3_OR_NEWER
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return new Guid(hash);
#else
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = MD5.HashData(bytes);
            return new Guid(hash);
#endif
        }
    }
}