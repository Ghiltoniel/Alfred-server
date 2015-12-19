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
using System.Windows.Forms;
using Ionic.Zip;
using log4net;

namespace Alfred.Server
{
    public class Startup
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Startup));

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
            var wwwDir = Path.Combine(new FileInfo(Application.ExecutablePath).DirectoryName, "www_app");
            var tempDir = Path.Combine(new FileInfo(Application.ExecutablePath).DirectoryName, "temp_www");

            log.InfoFormat("www_dir = {0}", wwwDir);

            var gitAppUrl = "https://github.com/Ghiltoniel/Alfred-ionic/archive/master.zip";
            var gitAdminUrl = "https://github.com/Ghiltoniel/Alfred-angular-admin-client/archive/master.zip";
            var gitPlayerUrl = "https://github.com/Ghiltoniel/Alfred-angular-web-player/archive/master.zip";
            var zipAppFile = "app.zip";
            var zipAdminFile = "admin.zip";
            var zipPlayerFile = "player.zip";

            var wwwDirInfo = new DirectoryInfo(wwwDir);

            if (!wwwDirInfo.Exists || DateTime.UtcNow.Subtract(wwwDirInfo.LastWriteTimeUtc).TotalDays > 1)
            {
                try {
                    if (wwwDirInfo.Exists)
                        wwwDirInfo.Delete(true);

                    log.Info("Downloading web app files ...");
                    new WebClient().DownloadFile(gitAppUrl, zipAppFile);

                    log.Info("Downloading web admin panel files ...");
                    new WebClient().DownloadFile(gitAdminUrl, zipAdminFile);

                    log.Info("Downloading web player files ...");
                    new WebClient().DownloadFile(gitPlayerUrl, zipPlayerFile);

                    wwwDirInfo.Create();
                    using (ZipFile zip = ZipFile.Read(zipAppFile))
                    {
                        log.Info("Extracting web app files ...");
                        zip.ExtractAll(tempDir, ExtractExistingFileAction.OverwriteSilently);

                        var tempDirInfo = new DirectoryInfo(Path.Combine(tempDir, "Alfred-ionic-master", "www"));
                        tempDirInfo.MoveTo(Path.Combine(wwwDir, "app/"));
                    }
                    File.Delete(zipAppFile);
                    Directory.Delete(tempDir, true);

                    using (ZipFile zip = ZipFile.Read(zipAdminFile))
                    {
                        log.Info("Extracting web admin panel files ...");
                        zip.ExtractAll(tempDir, ExtractExistingFileAction.OverwriteSilently);

                        var tempDirInfo = new DirectoryInfo(Path.Combine(tempDir, "Alfred-angular-admin-client-master"));
                        tempDirInfo.MoveTo(Path.Combine(wwwDir, "admin/"));
                    }
                    File.Delete(zipAdminFile);
                    Directory.Delete(tempDir, true);

                    using (ZipFile zip = ZipFile.Read(zipPlayerFile))
                    {
                        log.Info("Extracting web player files ...");
                        zip.ExtractAll(tempDir, ExtractExistingFileAction.OverwriteSilently);

                        var tempDirInfo = new DirectoryInfo(Path.Combine(tempDir, "Alfred-angular-web-player-master"));
                        tempDirInfo.MoveTo(Path.Combine(wwwDir, "player/"));
                    }
                    File.Delete(zipPlayerFile);
                    Directory.Delete(tempDir, true);
                }
                catch(Exception e)
                {
                    log.WarnFormat("Error when downloading web files : {0}", e);
                    if(Directory.Exists(tempDir))
                        Directory.Delete(tempDir, true);
                    if(wwwDirInfo.Exists)
                        wwwDirInfo.Delete(true);
                }
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