using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TicketSystem
{
    public class AuthOptions
    {
        public const string ISSUER = "ITLServer"; // издатель токена
        public const string AUDIENCE = "ITLUser"; // потребитель токена
        const string KEY = "vgj45fv@HGuv%g3t561!@#HJG1246#@$))))af213***";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
