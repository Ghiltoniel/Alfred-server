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
using System.Net;
using Ionic.Zip;

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
            var wwwDir = Path.Combine(Environment.CurrentDirectory, "www_app");
            var tempDir = Path.Combine(Environment.CurrentDirectory, "temp_www");

            var gitUrl = "https://github.com/Ghiltoniel/Alfred-ionic/archive/master.zip";
            var zipFile = "app.zip";

            var wwwDirInfo = new DirectoryInfo(wwwDir);

            if (!wwwDirInfo.Exists || DateTime.UtcNow.Subtract(wwwDirInfo.LastWriteTimeUtc).TotalDays > 1)
            {
                new WebClient().DownloadFile(gitUrl, zipFile);

                if (wwwDirInfo.Exists)
                    wwwDirInfo.Delete(true);

                using (ZipFile zip = ZipFile.Read(zipFile))
                {
                    zip.ExtractAll(tempDir, ExtractExistingFileAction.OverwriteSilently);

                    var tempDirInfo = new DirectoryInfo(Path.Combine(tempDir, "Alfred-ionic-master", "www"));
                    tempDirInfo.MoveTo(wwwDir);
                }
                File.Delete(zipFile);
                Directory.Delete(tempDir, true);
            }

            app.UseFileServer(new FileServerOptions
            {
                RequestPath = new PathString(""),
                FileSystem = new PhysicalFileSystem(wwwDir),
                EnableDirectoryBrowsing = true
            });
            
            app.UseCors(CorsOptions.AllowAll);
            app.UseWebApi(config);
        }
    }
}