using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Alfred.Model.Core.Music;
using HtmlAgilityPack;

namespace Alfred.Utils.Radios
{
    public class DIRadio : ARadio
    {
        public DIRadio()
        {
            DisplayName = "Digitaly Imported";
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

            var webradios = html.GetElementbyId("root");
            var list = webradios.Descendants("div").Where(d => d.Attributes["class"].Value == "lists").First();
            var subHtml = new HtmlDocument();

            foreach (var radio in list.Descendants("a"))
            {
                var subRadio = new SubRadio();
                subRadio.ImgSrc = radio.Descendants("img").First().Attributes["src"].Value;
                subRadio.Url = BaseUrl + radio.Attributes["href"].Value;
                subRadio.Name = radio.InnerText.Trim();
                subRadio.RadioName = DisplayName;
                result.Add(subRadio);
            }
            SaveCacheSubRadios(result);
            return result;
        }
    }
}