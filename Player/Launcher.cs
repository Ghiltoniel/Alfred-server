using System;
using System.Reflection;
using Alfred.Model.Core;

namespace AlfredPlayer
{
    class Launcher
    {
        /// <summary>
        /// Execute a plugin class method
        /// </summary>
        /// <param name="modelName">The name of the plugin class</param>
        /// <param name="commandName">The name of the plugin class method</param>
        /// <param name="arguments">The arguments to pass to the method</param>
        /// <param name="ihm">The interface form in case of media command </param>
        /// <param name="alfred">The alfred object</param>
        /// <returns>The result object</returns>
        public static int ExecutePlayer(AlfredTask task)
        {
            Type model;
            MethodInfo methodInfo;

            var media = Init.media;

            try
            {
                model = media.GetType();
                methodInfo = model.GetMethod(task.Command);

                // Set the arguments for the plugin call
                var argumentProp = model.GetField("arguments");
                if (null != argumentProp)
                    argumentProp.SetValue(media, task.Arguments);

                // Run the plugin method if it exists
                if (methodInfo != null)
                    methodInfo.Invoke(media, null);

                // Get the result of the plugin call
                var resultProp = model.GetField("status");
                return (int)resultProp.GetValue(media);
            }
            catch(Exception)
            {
                return -1;
            }
        }
    }
}
