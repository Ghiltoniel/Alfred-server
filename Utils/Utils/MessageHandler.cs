using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using Alchemy;
using Alchemy.Classes;
using AlfredModel;
using Newtonsoft.Json;

namespace AlfredModel
{
    public class MessageHandler
    {
        const string server = "ws://veron.homedns.org:13000/channel";
        static bool isServerRunning;

        static MessageHandler()
        {
            isServerRunning = true;
        }

        public static void SendCommand(Task task)
        {
            Thread thread = new Thread(() => SendMessageTcp(task));
            thread.Start();
        }

        public static void SendLinesCommand(Task[] tasks)
        {
            Thread thread = new Thread(() => SendMessagesTcp(tasks));
            thread.Start();
        }

        static void SendMessageTcp(Task task)
        {
            var aClient = new WebSocketClient(server)
            {
                OnReceive = OnReceive
            };

            try
            {
                aClient.Connect();
                if (aClient.Connected)
                {
                    aClient.Send(JsonConvert.SerializeObject(task)); // string or byte[]
                    aClient.Disconnect();
                }
                else
                {
                    isServerRunning = false;
                    return;
                }
            }
            catch (Exception)
            {
                isServerRunning = false;
            }
            finally
            {
            }
        }

        static void SendMessagesTcp(Task[] tasks)
        {
            var aClient = new WebSocketClient(server)
            {
                OnReceive = OnReceive
            };

            try
            {
                aClient.Connect();
                if (aClient.Connected)
                {
                    foreach(var task in tasks)
                        aClient.Send(JsonConvert.SerializeObject(task));

                    aClient.Disconnect();
                }
                else
                {
                    isServerRunning = false;
                    return;
                }
            }
            catch (Exception)
            {
                isServerRunning = false;
            }
            finally
            {
            }
        }

        static void OnReceive(UserContext context)
        {
            string response = context.DataFrame.ToString();
            try
            {
                Task task = JsonConvert.DeserializeObject<Task>(response);
                if (task != null && task.type == TaskType.WebsiteInfo)
                {
                    
                }
            }
            catch (Exception e)
            { }
        }
    }
}