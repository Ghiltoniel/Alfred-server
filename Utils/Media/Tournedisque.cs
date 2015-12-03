using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Alfred.Model.Core.Music;
using Alfred.Model.Db;
using HtmlAgilityPack;
using Alfred.Model.Db.Repositories;

namespace Alfred.Utils.Media
{
    public class Tournedisque
    {
        const string baseUrl = "http://www.letournedisque.com/";
        const string baseName = "tournedisque";

        public static IDictionary<string, string> GetGenres()
        {
            var model = new Dictionary<string, string>();
            model.Add("new", "New");
            model.Add("mix", "Mix");
            model.Add("minimale", "Minimale");
            model.Add("hiphop", "Hip-Hop");
            model.Add("soft", "Soft");
            model.Add("newjazz", "New Jazz");
            model.Add("electro", "Electro");
            model.Add("electrorock", "Electro-Rock");
            return model;
        }

        public static IEnumerable<Song> GetSongsFromWeb(string type)
        {
            if (type == "new")
                type = string.Empty;

            var cacheSubRadios = GetCacheSongs(type);
            if (cacheSubRadios != null)
                return cacheSubRadios;

            var client = new WebClient();
            client.Encoding = Encoding.UTF8;

            var htmlCode = client.DownloadString(baseUrl + type);
            client.Dispose();
            var songs = new List<Song>();

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlCode);

            var sounds = doc.GetElementbyId("sounds");
            var ul = sounds.Descendants("ul").First();

            foreach (var li in ul.Descendants("li"))
            {
                if(li.Attributes.Contains("data-url"))
                {
                    var url = li.Attributes["data-url"].Value;
                    var name = li.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value == "name");
                    var artist = li.Descendants("div").Where(d => d.Attributes.Contains("class") && d.Attributes["class"].Value == "artist");
                    var img = li.Descendants("img");
                    songs.Add(new Song
                    {
                        Link = url,
                        Title = HtmlEntity.DeEntitize(name.First().InnerText),
                        Artist = HtmlEntity.DeEntitize(artist.First().InnerText),
                        Album = img.First().Attributes["src"].Value
                    });
                }
            }
            SaveCacheSongs(songs, type);
            return songs;
        }

        public static void SaveCacheSongs(IEnumerable<Song> songs, string type)
        {
            var ressourceFolder = new PathRepository().Get("WebsiteRessources").Value;
            ressourceFolder = ressourceFolder + "\\radios";
            if (!Directory.Exists(ressourceFolder))
                Directory.CreateDirectory(ressourceFolder);

            using (var writer = new StreamWriter(File.OpenWrite(string.Format("{0}\\{1}_{2}.txt", ressourceFolder, baseName, type))))
            {
                foreach (var song in songs)
                {
                    writer.WriteLine("{0}::{1}::{2}::{3}", song.Title, song.Artist, song.Link, song.Album);
                }
            }
        }

        public static IEnumerable<Song> GetCacheSongs(string type)
        {
            var ressourceFolder = new PathRepository().Get("WebsiteRessources").Value;
            ressourceFolder = ressourceFolder + "\\radios";
            if (!Directory.Exists(ressourceFolder))
                return null;

            var result = new HashSet<Song>();
            var path = string.Format("{0}\\{1}_{2}.txt", ressourceFolder, baseName, type);
            var fileInfo = new FileInfo(path);
            if (fileInfo.LastWriteTimeUtc.AddHours(1) < DateTime.UtcNow)
                return null;

            var lines = File.ReadAllLines(path);

            foreach (var line in lines)
            {
                var subradio = new Song();
                var items = line.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
                if (items.Length == 4)
                {
                    subradio.Title = items[0];
                    subradio.Artist = items[1];
                    subradio.Link = items[2];
                    subradio.Album = items[3];
                }
                result.Add(subradio);
            }
            return result;
        }
    }
}