using System.IO;
using System.Net.Http;
using Microsoft.Ajax.Utilities;

namespace Alfred.Server.Web
{
    class Minifier
    {
        public static void MinifyJs()
        {            
            var minifier = new Microsoft.Ajax.Utilities.Minifier();

            var rootDirectory = @"C:\Users\guillaume\Documents\nodejs\express\dashboard\public\";
            var outputFile = rootDirectory + @"javascripts\compiled.js";

            var jsFiles = new[]{
                "http://ajax.googleapis.com/ajax/libs/jquery/2.0.2/jquery.min.js",
                "/javascripts/third-party/jquery-ui-1.10.3.min.js",
                "/javascripts/third-party/jquery.cookie.js",
                "/javascripts/third-party/bootstrap.min.js",
                "http://cdnjs.cloudflare.com/ajax/libs/raphael/2.1.0/raphael-min.js",
                "/javascripts/third-party/plugins/morris/morris.min.js",
                "/javascripts/third-party/plugins/sparkline/jquery.sparkline.min.js",
                "/javascripts/third-party/plugins/jvectormap/jquery-jvectormap-1.2.2.min.js",
                "/javascripts/third-party/plugins/jvectormap/jquery-jvectormap-world-mill-en.js",
                "/javascripts/third-party/plugins/fullcalendar/fullcalendar.min.js",
                "/javascripts/third-party/plugins/jqueryKnob/jquery.knob.js",
                "/javascripts/third-party/plugins/daterangepicker/daterangepicker.js",
                "/javascripts/third-party/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js",
                "/javascripts/third-party/plugins/iCheck/icheck.min.js",
                "/javascripts/third-party/AdminLTE/app.js",
                "/javascripts/third-party/AdminLTE/dashboard.js",
                "https://ajax.googleapis.com/ajax/libs/angularjs/1.2.21/angular.min.js",
                "https://ajax.googleapis.com/ajax/libs/angularjs/1.2.21/angular-cookies.min.js",
                "/javascripts/third-party/angular-route.min.js",
                "/javascripts/core.js",
                "/javascripts/routes.js",
                "/javascripts/services/websocket.js",
                "/javascripts/services/authentication.js",
                "/javascripts/models/light.js",
                "/javascripts/models/user.js",
                "/javascripts/models/sensor.js",
                "/javascripts/models/scenario.js",
                "/javascripts/models/mouvement.js",
                "/javascripts/models/texttospeech.js",
                "/javascripts/models/notification.js",
                "/javascripts/models/chat.js",
                "/javascripts/models/audioplayer.js",
                "/javascripts/controllers/global.js",
                "/javascripts/controllers/light.js",
                "/javascripts/controllers/user.js",
                "/javascripts/controllers/sensor.js",
                "/javascripts/controllers/scenario.js",
                "/javascripts/controllers/mouvement.js",
                "/javascripts/controllers/texttospeech.js",
                "/javascripts/controllers/notification.js",
                "/javascripts/controllers/chat.js",
                "/javascripts/controllers/audioplayer.js"
            };
            
            if(File.Exists(outputFile))
                File.Delete(outputFile);

            foreach(var file in jsFiles)
            {
                string script = null;
                if(File.Exists(rootDirectory + file))
                    script = File.ReadAllText(rootDirectory + file);
                else
                {
                    var result = new HttpClient().GetAsync(file).Result;
                    if(result.IsSuccessStatusCode)
                        script = result.Content.ReadAsStringAsync().Result;
                }

                if(!string.IsNullOrEmpty(script))
                {
                    File.AppendAllText(outputFile, minifier.MinifyJavaScript(script, new CodeSettings
                    {
                        MinifyCode = false,
                        TermSemicolons = true
                    }));
                }
            }
        }

        public static void MinifyCss()
        {
            var minifier = new Microsoft.Ajax.Utilities.Minifier();

            var rootDirectory = @"C:\Users\guillaume\Documents\nodejs\express\dashboard\public\";
            var outputFile = rootDirectory + @"stylesheets\compiled.css";

            var jsFiles = new[]{
                "/stylesheets/style.css",
                "/stylesheets/third-party/bootstrap.min.css",
                "/stylesheets/third-party/font-awesome.min.css",
                "/stylesheets/third-party/ionicons.min.css",
                "/stylesheets/third-party/morris/morris.css",
                "/stylesheets/third-party/jvectormap/jquery-jvectormap-1.2.2.css",
                "/stylesheets/third-party/fullcalendar/fullcalendar.css",
                "/stylesheets/third-party/daterangepicker/daterangepicker-bs3.css",
                "/stylesheets/third-party/bootstrap-wysihtml5/bootstrap3-wysihtml5.min.css",
                "/stylesheets/third-party/AdminLTE.css",
                "/stylesheets/switch.css",
                "/stylesheets/style.css"
            };

            if (File.Exists(outputFile))
                File.Delete(outputFile);

            foreach (var file in jsFiles)
            {
                string script = null;
                if (File.Exists(rootDirectory + file))
                    script = File.ReadAllText(rootDirectory + file);
                else
                {
                    var result = new HttpClient().GetAsync(file).Result;
                    if (result.IsSuccessStatusCode)
                        script = result.Content.ReadAsStringAsync().Result;
                }

                if (!string.IsNullOrEmpty(script))
                {
                    File.AppendAllText(outputFile, minifier.MinifyStyleSheet(script));
                }
            }
        }
    }
}
