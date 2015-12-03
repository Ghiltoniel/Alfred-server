using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Alfred.Model.Core.Music;
using HtmlAgilityPack;

namespace Alfred.Utils.Radios
{
    public class JazzRadio : ARadio
    {
        public JazzRadio()
        {
            DisplayName = "Jazz Radio";
            HasSubsetRadios = true;
        }

        public override IEnumerable<SubRadio> GetAllSubRadios()
        {
            var cacheSubRadios = GetCacheSubRadios();
            if (cacheSubRadios != null)
                return cacheSubRadios;

            var result = new HashSet<SubRadio>();
            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            var htmlCode = client.DownloadString(BaseUrl);

            var html = new HtmlDocument();
            html.LoadHtml(htmlCode);

            var webradios = html.GetElementbyId("header");
            var subHtml = new HtmlDocument();

            foreach (var radio in webradios.Descendants("a"))
            {
                if (!(radio.Attributes.Contains("class") && radio.Attributes["class"].Value.Contains("webradio")))
                    continue;

                var subRadio = new SubRadio();
                subRadio.ImgSrc = radio.FirstChild.Attributes["src"].Value;

                htmlCode = client.DownloadString(radio.Attributes["href"].Value);
                subHtml.LoadHtml(htmlCode);
                subRadio.Url = subHtml.GetElementbyId("flux").Attributes["value"].Value;
                subRadio.Name = subHtml.DocumentNode.Descendants("title").First()
                    .InnerText
                    .Replace("Vous écoutez ", "")
                    .Replace(" - Jazz Radio", "");

                subRadio.RadioName = DisplayName;
                result.Add(subRadio);
            }
            SaveCacheSubRadios(result);
            return result;
        }
    }
}