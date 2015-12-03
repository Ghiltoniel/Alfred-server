using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Alfred.Model.Core.Music;
using Alfred.Model.Db;
using Alfred.Utils.Properties;
using Alfred.Model.Db.Repositories;

namespace Alfred.Utils.Radios
{
    public abstract class ARadio : Radio
    {
        public virtual IEnumerable<SubRadio> GetAllSubRadios()
        {
            return null;
        }

        public void SaveCacheSubRadios(IEnumerable<SubRadio> subradios)
        {
            var ressourceFolder = new PathRepository().Get("WebsiteRessources").Value;
            ressourceFolder = ressourceFolder + "\\radios";
            if (!Directory.Exists(ressourceFolder))
                Directory.CreateDirectory(ressourceFolder);

            using (var writer = new StreamWriter(File.OpenWrite(string.Format("{0}\\{1}.txt", ressourceFolder, BaseName))))
            {
                foreach (var radio in subradios)
                {
                    writer.WriteLine("{0}::{1}::{2}", radio.Name, radio.Url, radio.ImgSrc);
                }
            }
        }

        public IEnumerable<SubRadio> GetCacheSubRadios()
        {
            var ressourceFolder = new PathRepository().Get("WebsiteRessources").Value;
            ressourceFolder = ressourceFolder + "\\radios";
            if (!Directory.Exists(ressourceFolder))
                return null;

            var result = new HashSet<SubRadio>();
            var path = string.Format("{0}\\{1}.txt", ressourceFolder, BaseName);
            var fileInfo = new FileInfo(path);
            if (fileInfo.LastWriteTimeUtc.AddDays(3) < DateTime.UtcNow)
                return null;

            var lines = File.ReadAllLines(path);

            foreach (var line in lines)
            {
                var subradio = new SubRadio();
                var items = line.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length == 3)
                {
                    subradio.Name = items[0];
                    subradio.Url = items[1];
                    subradio.ImgSrc = items[2];
                    subradio.RadioName = DisplayName;
                }
                result.Add(subradio);
            }
            return result;
        }

        public static ARadio GetARadio(string name)
        {
            try
            {
                if (!name.EndsWith("Radio"))
                    return null;

                var url = Settings.Default[name].ToString();

                // Create a new object of plugin type
                var model = Type.GetType(string.Format("AlfredUtils.Radios.{0}", name));
                var objectModel = (ARadio)Activator.CreateInstance(model);

                // Set the arguments for the plugin call
                objectModel.BaseName = name;
                objectModel.BaseUrl = url;

                return objectModel;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}