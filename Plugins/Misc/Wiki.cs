using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml;
using Alfred.Utils.Plugins;

namespace Alfred.Plugins
{
    public class Wiki : BasePlugin
	{
		private List<String> keywords;

		public string title { get; set; }
		public string content { get; set; }
        
		public void LoadFromQuery(string query)
		{
			var result = TryDirectSearch(query);
			if (result == "no")
				AddKeywords(query);
			else
				ParseFromText(result);
		}

		public void ParseFromText(string result)
		{
			var doc = new XmlDocument();
			doc.LoadXml(result);
			var rev = doc.GetElementsByTagName("rev")[0];
			
			var r = new Regex(@"\{\{([^{|]+)\|([^{|]+)\}\}");
			var newS = r.Replace(rev.InnerText, "$2");

			r = new Regex(@"\{\{([^{]+)\}\}");
			newS = r.Replace(newS, "$1");

			r = new Regex(@"\[\[([^[]+)\|([^[]+)\]\]");
			newS = r.Replace(newS, "$2");

			r = new Regex(@"\[\[([^[]+)\]\]");
			newS = r.Replace(newS, "$1");

			r = new Regex(@"\[([^[]+)\]");
			newS = r.Replace(newS, "");

			content = newS;
		}

		public void AddKeywords(string s)
		{
			var myRequest = (HttpWebRequest)WebRequest.Create("http://fr.wikipedia.org//w/api.php?action=query&list=search&format=xml&prop=&srsearch=" + s + "&srlimit=5");
			myRequest.Timeout = 10000; // 10 secs
			myRequest.UserAgent = "Code Sample Web Client";
			myRequest.Accept = "text/xml";
			var response = (HttpWebResponse)myRequest.GetResponse();
		    using (var reader = new StreamReader(response.GetResponseStream()))
			{
				var responseText = DecodeEncodedNonAsciiCharacters(reader.ReadToEnd());
				var doc = new XmlDocument();
				doc.LoadXml(responseText);
				var l = doc.GetElementsByTagName("p");
				keywords = new List<string>();
				foreach (XmlNode x in l)
				    if (x.Attributes != null) keywords.Add(x.Attributes["title"].Value);
			}
		}

		public string TryDirectSearch(string s)
		{
			var myRequest = (HttpWebRequest)WebRequest.Create("http://fr.wikipedia.org/w/api.php?action=query&prop=revisions&format=xml&redirects&titles=" + s + "&rvprop=content");
			myRequest.Timeout = 10000; // 10 secs
			myRequest.UserAgent = "Code Sample Web Client";
			myRequest.Accept = "text/xml";
			var response = (HttpWebResponse)myRequest.GetResponse();
		    using (var reader = new StreamReader(response.GetResponseStream()))
		    {
		        var responseText = DecodeEncodedNonAsciiCharacters(reader.ReadToEnd());
		        return responseText.Length > 200 ? responseText : "no";
		    }
		}

		static string DecodeEncodedNonAsciiCharacters(string value)
		{
			return Regex.Replace(
				value,
				@"\\u(?<Value>[a-zA-Z0-9]{4})",
				m => ((char)int.Parse(m.Groups["Value"].Value, NumberStyles.HexNumber)).ToString(CultureInfo.InvariantCulture));
		}
	}
}
