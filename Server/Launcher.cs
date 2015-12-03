using System;
using System.Collections.Generic;
using Alfred.Model.Core;
using Alfred.Model.Core.Plugins;
using Alfred.Plugins;
using Alfred.Plugins.Manager;
using Alfred.Plugins.Managers;
using Alfred.Utils;
using Alfred.Utils.Ressources;
using log4net;
using Alfred.Utils.Managers;
using Alfred.Server.Ressources;

namespace Alfred.Server
{
    public class Launcher
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Launcher));
        public static PluginManager PluginManager;

        public static TaskPool Watcher;

        static Launcher()
        {
            Watcher = new TaskPool();
        }

        public static void LoadPlugins()
        {
            PluginManager = new PluginManager();
            PluginManager.LoadPlugins();
        }

        public static void EnqueueTask(AlfredTask task)
        {
            Watcher.EnqueueTask(task);
        }

        /// <summary>
        /// Execute a plugin class method
        /// </summary>
        /// <param name="task">The task object</param>
        /// <returns>The result object</returns>
        private static PluginResult ExecutePlugin(AlfredTask task)
        {
            // Command is parse this way : PluginClassName_PluginClassMethod
            var infoCommand = task.Command.Split('_');
            if (infoCommand.Length < 2)
                return null;

            var modelName = infoCommand[0];
            var commandName = infoCommand[1];

            var plugin = PluginManager.GetPlugin(modelName);
            var model = plugin.GetType();

            try
            {
                plugin.client = task.Client;

                // Set the arguments for the plugin call
                var argumentProp = model.GetField("arguments");
                if (null != argumentProp)
                    argumentProp.SetValue(plugin, task.Arguments);

                // Heavy loading of media current objects
                if (model == CommonPlugins.media.GetType())
                    CommonPlugins.media.LoadObjectsFromArguments(task.Arguments);


                // Run the plugin method if it exists
                var methodInfo = model.GetMethod(commandName, new Type[] { });
                if (methodInfo != null)
                {
                    if (!plugin.BeforeExecute(task, methodInfo))
                        return null;

                    methodInfo.Invoke(plugin, null);
                }

                CommonPlugins.alfred.lastCommand = task;
                log.Info(string.Format("{0} : {1} : Done !", task.FromName, task.Command));

                // Get the result of the plugin call
                var resultProp = model.GetField("result");
                return (PluginResult)resultProp.GetValue(plugin);
            }
            catch (Exception e)
            {
                log.Info(string.Format("Error executing plugin command `{0}` : {1}", commandName, e.Message));
                return null;
            }
        }

        public static void ExecuteServer(AlfredTask task)
        {
            // Do nothing if voice control and not started or starting command
            if(task.FromName == "Speech"
                && CommonPlugins.alfred.state == Alfred.Plugins.Alfred.State.Sleep
                && task.Command != "Alfred_Start")
                return;

            if (task.SpeakBeforeExecute)
                SpeakBegin(task.Command);

            var result = ExecutePlugin(task);
            SendDoneSignal(task, result);

            if (task.SpeakAfterExecute &&
                (task.Type != TaskType.Alfred
                 || (CommonPlugins.alfred.state == Alfred.Plugins.Alfred.State.Awake
                     || task.Command == "Alfred_StopListening")))
            {
                SpeakAfter(task, result);
            }
        }

        private static void SpeakBegin(string command)
        {
            var alfredResponse = RessourcesReader.ReadResourceValue(typeof(AlfredDialogBegin), command);
            if (alfredResponse == "") return;

            var toSpeakString = "";
            var choices = alfredResponse.Split(';');

            var index = new Random().Next(0, choices.Length);
            toSpeakString = choices[index];
            CommonPlugins.alfred.Speak(toSpeakString);
        }

        private static void SpeakAfter(AlfredTask task, PluginResult result)
        {
            // Try to get the return string
            try
            {
                var toSpeakString = "";
                if (result != null && result.toSpeakString != null)
                    toSpeakString = result.toSpeakString;
                // If no return value, then get the string to speak from the ressource file
                else
                {
                    var alfredResponse = RessourcesReader.ReadResourceValue(typeof(AlfredDialogBegin), task.Command);
                    if (alfredResponse != "")
                    {
                        var choices = alfredResponse.Split(';');
                        var r = new Random();
                        var index = r.Next(0, choices.Length);
                        toSpeakString = choices[index];
                    }
                }
                // Speak the string and remember the last command and last said sentence
                CommonPlugins.alfred.Speak(toSpeakString);
                CommonPlugins.alfred.lastResponse = toSpeakString;
                CommonPlugins.alfred.RestartTimer();

                log.Info(string.Format("{0} : {1} : Done !", task.FromName, task.Command));
            }
            catch (Exception ex)
            {
                log.Info(string.Format("Error  `{0}` : {1}", task.Command, ex.Message));
            }
        }

        private static void SendDoneSignal(AlfredTask task, PluginResult result)
        {
            if (result == null)
                return;

            var taskDone = new AlfredTask
            {
                Type = TaskType.Server,
                Command = "Done",
                Arguments = new Dictionary<string, string>
                { 
                    { "text", result.toSpeakString },
                    { "task", task.Command },
                    { "user", task.Client != null ? task.Client.Name : null }
                }
            };
            Init.WebSocketServer.Broadcast(taskDone);
        }
    }
}
