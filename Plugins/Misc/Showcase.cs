using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Alfred.Utils.Plugins;
using HtmlAgilityPack;

namespace Alfred.Plugins
{
    public class Showcase : BasePlugin
	{
		public string DJ { get; set; }
		public DateTime Date { get; set; }
		public string Lien { get; set; }
		public string Prix { get; set; }
		public string PrixPrevente { get; set; }

		public static List<Showcase> GetProg()
		{
			var list = new List<Showcase>();
			var client = new WebClient();
			client.Encoding = Encoding.UTF8;

			var htmlCode = client.DownloadString("http://www.showcase.fr/agenda");
			client.Dispose();

			var doc = new HtmlDocument();
			doc.LoadHtml(htmlCode);

			for (var i = 0; i < 1000; i++)
			{
				var node = doc.GetElementbyId("node-" + i);
				if (node != null)
				{
					var soiree = new Showcase();
					var infos = node.ChildNodes[1];

					soiree.Lien = infos.ChildNodes[0].Attributes["href"].Value;
					soiree.DJ = infos.ChildNodes[0].InnerText;
					soiree.Date = DateTime.Parse(infos.ChildNodes[1].InnerText.Replace("Aout", "Août"));

					var infoSoiree = client.DownloadString("http://www.showcase.fr" + soiree.Lien);
					var docSoiree = new HtmlDocument();
					docSoiree.LoadHtml(infoSoiree);

					var noeud = docSoiree.GetElementbyId("node-" + i);
					noeud = noeud.ChildNodes[1];
					var noeudInfos = noeud.ChildNodes[noeud.ChildNodes.Count - 1].InnerHtml;

					var index = noeudInfos.IndexOf('€');
					soiree.Prix = noeudInfos.Substring(index - 3, 4);

					var indexPrevente = noeudInfos.IndexOf("Préventes", StringComparison.Ordinal);
					noeudInfos = noeudInfos.Substring(indexPrevente);
					index = noeudInfos.IndexOf('€');
					soiree.PrixPrevente = noeudInfos.Substring(index - 3, 4);

					list.Add(soiree);
				}
			}
			return list;
		}

		public static Showcase GetProgDay(DateTime day)
		{
			var list = GetProg();
			return list.SingleOrDefault(sc => (sc.Date.Year == day.Year && sc.Date.Day == day.Day && sc.Date.Month == day.Month));
		}

        public static string ProgListToString(List<Showcase> list)
        {
            var s = "";
            foreach (var sc in list)
            {
                s += "Le " + sc.Date.ToString("dddd") + " " + sc.Date.Day + " " + sc.Date.ToString("MMMM");
                s += " " + sc.DJ + " : ";
                s += sc.Prix + " l'entrée,";
                s += " ou " + sc.PrixPrevente + " en prévente; ";
            }
            return s;
        }

        public static string ProgEventToString(Showcase sc)
        { 
            var s = "";
            s += sc.DJ + " : ";
            s += sc.Prix + " l'entrée,";
            s += " ou " + sc.PrixPrevente + " en prévente";
            return s;
        }

		public void ShowcaseToday()
		{
			result.toSpeakString = ProgEventToString(GetProgDay(DateTime.Now));
		}

		public void ShowcaseAll()
		{
			result.toSpeakString = ProgListToString(GetProg());
		}
	}
}
