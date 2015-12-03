using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using Alfred.Model.Core;
using Alfred.Plugins.Managers;
using Alfred.Plugins.Ressources;
using Alfred.Utils.Managers;
using Alfred.Utils.Plugins;
using HtmlAgilityPack;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using Alfred.Plugins.Ressources;

namespace Alfred.Plugins
{
    public class Mail : BasePlugin
    {
        public static HashSet<MimeMessage> Messages;
        public static DateTime lastRead;

        static Mail()
        {
            Messages = new HashSet<MimeMessage>();
            lastRead = DateTime.Now.AddDays(-1);
        }

        public Mail()
        {
			getAllMails();
        }

        public void getAllMails()
		{
            if (DateTime.Now.Subtract(lastRead).TotalMinutes > 5)
            {
                try
                {
                    CommonPlugins.alfred.Speak("Veuillez patienter... Je récupère vos e-mail");
                    using (var client = new ImapClient())
                    {
                        var credentials = new NetworkCredential("guillaume.jacquart@live.fr", "Ghiltoniel1");
                        var uri = new Uri("imaps://imap-mail.outlook.com");

                        using (var cancel = new CancellationTokenSource())
                        {
                            client.Connect(uri, cancel.Token);
                            client.Authenticate(credentials, cancel.Token);

                            var inbox = client.Inbox;
                            inbox.Open(FolderAccess.ReadOnly, cancel.Token);

                            var query = SearchQuery.NotSeen;

                            foreach (var uid in inbox.Search(query, cancel.Token))
                            {
                                var message = inbox.GetMessage(uid, cancel.Token);
                                Messages.Add(message);
                            }

                            client.Disconnect(true, cancel.Token);
                            lastRead = DateTime.Now;
                        }
                    }
                } 
                catch(Exception)
                { 
                }
            }
		}

        public void Count()
        {
            result.toSpeakString = "Vous avez " + Messages.Count + " nouveaux messages !";
        }
        public void ReadLast()
        {
            var message = Messages.FirstOrDefault();
            if (message != null)
            {
                CommonPlugins.alfred.Speak("Message de : " + message.From.First().Name);
                Thread.Sleep(500);
                CommonPlugins.alfred.Speak("Sujet : " + message.Subject);
                Thread.Sleep(1500);

                foreach (var part in message.BodyParts)
                {
                    if (part.ContentType.MediaType == "text")
                    {
                        var filename = Paths.path_mails + message.MessageId + ".html";
                        using (var stream = File.Create(filename))
                        {
                            part.WriteTo(stream);
                            var process = new Process();

                            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
                            var file = path.Replace("file:\\", "") + "\\" + filename;

                            var task = new AlfredTask
                            {
                                Command = "DirectPlay",
                                Type = TaskType.ActionPlayer
                            };
                            task.Arguments = new Dictionary<string, string>();
                            task.Arguments.Add("file", file);
                            PlayerManager.BroadcastPlayersChannel(task);
                        }

                        var doc = new HtmlDocument();
                        doc.Load(filename);
                        result.toSpeakString = WebUtility.HtmlDecode(doc.DocumentNode.InnerText);
                    }
                }
            }
        }
    }
}
