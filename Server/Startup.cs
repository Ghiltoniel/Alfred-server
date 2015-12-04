using System.Web.Http;
using Alfred.Server.Properties;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.StaticFiles;
using Newtonsoft.Json;
using Owin;
using System;
using System.IO;
using System.Diagnostics;

namespace Alfred.Server
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder app)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();
            //config.Filters.Add(new AuthorizeAttribute());

            config.Formatters.XmlFormatter.SupportedMediaTypes.RemoveAt(0);
            config.Formatters.JsonFormatter.SerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
            //Minifier.MinifyJs();
            //Minifier.MinifyCss();

            // Turns on static files, directory browsing, and default files.
            
            var gitRepo = "https://github.com/Ghiltoniel/Alfred-ionic.git";
            var wwwDir = Path.Combine(Environment.CurrentDirectory, "www_app");
            string gitCloneArgument = string.Format(@"clone ""{0}"" ""{1}""", gitRepo, wwwDir);

            var directoryInfo = new DirectoryInfo(wwwDir);
            if (directoryInfo.Exists && DateTime.UtcNow.Subtract(directoryInfo.CreationTimeUtc).TotalDays > 1)
            {
                directoryInfo.Delete();
                var process = Process.Start("git", gitCloneArgument);
                process.WaitForExit(100000);
            }

            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(""),
                FileSystem = new PhysicalFileSystem(Path.Combine(wwwDir, "www")),
                EnableDirectoryBrowsing = true
            });

            var options = new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                LoginPath = CookieAuthenticationDefaults.LoginPath,
                LogoutPath = CookieAuthenticationDefaults.LogoutPath
            };

            app.UseCookieAuthentication(options);
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);
        }
    }
}