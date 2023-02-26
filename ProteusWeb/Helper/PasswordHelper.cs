using System.Security.Cryptography;
using System.Text;

namespace ProteusWeb.Helper;

public class PasswordHelper
{
    public static string CreateHash(string password)
    {
        var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(StringToBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public static byte[] StringToBytes(string password)
    {
        return Encoding.UTF8.GetBytes(password);
    }
}