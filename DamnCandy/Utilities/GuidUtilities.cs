using System.Security.Cryptography;
using System.Text;

namespace DamnCandy.Utilities;

internal static class GuidUtilities
{
    public static Guid CreateGuid(this string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = MD5.HashData(bytes);
        return new Guid(hash);
    }
}