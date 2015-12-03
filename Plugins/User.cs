using System.Collections.Generic;
using System.Linq;
using Alfred.Model.Core;
using Alfred.Model.Db;
using Alfred.Utils.Managers;
using Alfred.Utils.Plugins;
using Alfred.Utils.Utils;
using System.Security.Cryptography;
using System;
using Alfred.Model.Db.Repositories;

namespace Alfred.Plugins
{
    public class User : BasePlugin
    {
        public override void Initialize()
        {
        }

        [WebSocketAuthorize]
        public void Login()
        {
            var login = arguments["login"];
            var password = arguments["password"];
            var repo = new UserRepository();
            var loginOk = repo.ValidateUser(login, password);

            if(!loginOk)
            {
                client.Send(new AlfredTask
                {
                    Command = "AuthenticationFailed",
                    Arguments = new Dictionary<string, string>
                    {
                            { "error", "Invalid credentials" }
                        }
                });
                return;
            }

            client.Name = login.ToLower();
            var token = UserManager.SetUserToken(client);
            if(!string.IsNullOrEmpty(token))
            {
                client.Send(new AlfredTask
                {
                    Command = "Authenticated",
                    Arguments = new Dictionary<string, string>
                    {
                            { "token", token },
                            { "login", login }
                        }
                });
            }
        }

        public void Logout()
        {
            var token = arguments["token"];

            var logout = UserManager.LogoutUser(token);
            if (logout)
            {
                client.Send(new AlfredTask
                {
                    Command = "Logout"
                });
            }
        }
    }
}
