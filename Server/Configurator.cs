using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using Alfred.Server.Properties;
using Alfred.Server.Ressources;
using Alfred.Utils.Utils;

namespace Alfred.Server
{
    public class Configurator
    {
        private string _appPath = Application.ExecutablePath;
        private readonly string _configPath = new FileInfo(Application.ExecutablePath).DirectoryName + "/config.ini";
        public Dictionary<string, object> Configs;

        private Dictionary<string, string> ExistingConfigs; 
        private Dictionary<string, object> ConfigDefaults = new Dictionary<string, object>
        {
            {"WebSocketServerPort", 13100},
            {"WebSocketServerHostname", "localhost"},
            {"WebApiServerHostname", "localhost"},
            {"WebApiServerPort", 80}
        };

        public Configurator()
        {
            Configs = new Dictionary<string, object>();
            if (File.Exists(_configPath))
            {
                Console.WriteLine("Welcome back, sir !");
                ReadConfigFile();
            }
            else
            {
                Console.WriteLine("Welcome to your new virtual butler, sir !");
                Console.WriteLine("My name is Alfred, and I will help you set a few things right...");
            }
            Console.WriteLine();
            Thread.Sleep(1500);

            foreach (var item in ConfigDefaults)
            {
                if (Configs.ContainsKey(item.Key))
                    continue;

                Console.Write(Errors.Configurator_Setting_Value, item.Key, item.Value);
                var casted = false;

                while (!casted)
                {
                    try
                    {
                        var value = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(value))
                            value = item.Value.ToString();

                        var type = item.Value.GetType();
                        var valueType = Convert.ChangeType(value, type);
                        Configs.Add(item.Key, valueType);
                        casted = true;
                    }
                    catch (InvalidCastException)
                    {
                        Console.WriteLine(Errors.Configurator_Settings_Error, item.Value.GetType().Name);
                    }
                }
            }

            FileUtils.WriteDictionaryToConfig(Configs, _configPath);

            Console.WriteLine();
            Console.WriteLine("Your settings have been saved.");
            Console.WriteLine("You can still change them by manually editing the config.ini file !");
            Console.WriteLine();

            SaveConfigsToSettings();
        }

        private void SaveConfigsToSettings()
        {
            foreach(var item in Configs)
            {
                Settings.Default[item.Key] = item.Value;
            }
            Settings.Default.Save();
        }

        private void ReadConfigFile()
        {
            ExistingConfigs = FileUtils.ReadConfig(_configPath);
            Configs = new Dictionary<string, object>();

            foreach (var item in ExistingConfigs)
            {
                if (!ConfigDefaults.ContainsKey(item.Key))
                    continue;

                var defaultValue = ConfigDefaults[item.Key];
                var casted = false;

                while (!casted)
                {
                    try
                    {
                        var value = ExistingConfigs[item.Key];
                        var type = defaultValue.GetType();
                        var valueType = Convert.ChangeType(value, type);
                        Configs.Add(item.Key, valueType);
                        casted = true;
                    }
                    catch (InvalidCastException)
                    {
                        Console.WriteLine(Errors.Configurator_Settings_Error, item.Value.GetType().Name);
                    }
                }
            }
        }
    }
}
