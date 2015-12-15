using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Alfred.Utils.Plugins;
using HtmlAgilityPack;

namespace Alfred.Plugins
{
    class ParisBouge : BasePlugin
	{
		public string Type { get; set; }
		public string Nom { get; set; }
		public DateTime Date { get; set; }
		public string Lieu { get; set; }
		public string Lien { get; set; }
		public string Musique { get; set; }
		public string Prix { get; set; }
		public string PrixPrevent { get; set; }

		public static List<ParisBouge> GetEventList(DateTime day)
		{
			var list = new List<ParisBouge>();

			var client = new WebClient();
			client.Encoding = Encoding.UTF8;

			var dateTonight = String.Format("{0:yyyy/MM/dd}", day); 

			var htmlCode = client.DownloadString("http://www.parisbouge.com/events/"+dateTonight+"/");
			client.Dispose();

			var doc = new HtmlDocument();
			doc.LoadHtml(htmlCode);

			var soirees = doc.GetElementbyId("tab-soiree");

			foreach (var soiree in soirees.ChildNodes)
			{
				var pb = new ParisBouge();
				pb.Date = day;
				if (soiree.Name == "#text")
					continue;

				foreach (var soireeInfo in soiree.ChildNodes)
				{
					if (soireeInfo.Name != "ul")
						continue;

					var linePosition = 10000;
					foreach (var soireeInfo2 in soireeInfo.ChildNodes)
					{
						if (soireeInfo2.Name != "li")
							continue;

						if (soireeInfo2.ChildNodes.Count == 1 && soireeInfo2.FirstChild.Name == "#text")
							pb.Musique = soireeInfo2.LastChild.InnerText;

						
						foreach (var soireeInfo3 in soireeInfo2.ChildNodes)
						{
							
							if (soireeInfo3.Name == "#text")
								continue;
							if (soireeInfo3.Name == "h2")
							{
								pb.Nom = soireeInfo3.InnerText;
								pb.Lien = soireeInfo3.FirstChild.Attributes["href"].Value;
							}

							if (soireeInfo3.Name == "a" && soireeInfo3.LinePosition < linePosition)
							{
								pb.Type = soireeInfo3.InnerText;
								linePosition = soireeInfo3.LinePosition;
							}

							if (soireeInfo3.Name == "a" && soireeInfo3.LinePosition > linePosition)
								pb.Lieu = soireeInfo3.InnerText;

							break;
						}
					}
					list.Add(pb);
				}
			}
			var aux = list.Where(sc => sc.Lien != null).ToList();
			return list;
		}
	}
}
