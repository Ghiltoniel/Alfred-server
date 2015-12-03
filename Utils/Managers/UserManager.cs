using System;
using System.Collections.Concurrent;
using System.Linq;
using Alfred.Model.Core;

namespace Alfred.Utils.Managers
{
    public class UserManager
    {
        static readonly ConcurrentDictionary<string, string> UserTokens = new ConcurrentDictionary<string, string>();
        
        public static string ValidateToken(string token)
        {
            var wsClient = UserTokens.FirstOrDefault(u => u.Value == token);
            if (wsClient.Key != null)
            {
                return wsClient.Value;
            }
            return null;
        }
        public static string GetUserLogin(string token)
        {
            var wsClient = UserTokens.FirstOrDefault(u => u.Value == token);
            if (wsClient.Key != null)
            {
                return wsClient.Key;
            }
            return null;
        }

        public static string SetUserToken(Client user)
        {
            string token = GenerateToken();
            string existingUserToken = string.Empty;
            UserTokens.TryGetValue(user.Name, out existingUserToken);
            if (!string.IsNullOrEmpty(existingUserToken))
                return existingUserToken;

            if (UserTokens.TryUpdate(user.Name, token, existingUserToken)
                    || UserTokens.TryAdd(user.Name, token))
            {
                return token;
            };
            return null;
        }

        private static string GenerateToken()
        {
            var time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            var key = Guid.NewGuid().ToByteArray();
            var token = Convert.ToBase64String(time.Concat(key).ToArray());
            return token;
        }

        public static bool LogoutUser(string token)
        {
            var user = UserTokens.SingleOrDefault(u => u.Value == token);
            string ret;
            if (user.Key != null)
                return UserTokens.TryRemove(user.Key, out ret);

            return true;
        }
    }
}
