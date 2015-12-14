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
using System.Windows.Forms;

namespace AlfredModel
{
    public class TcpListener
    {
        const string server = "ws://veron.homedns.org:13000/channel";
        static bool isServerRunning;
        static WebSocketClient aClient;

        static TcpListener()
        {
            try
            {
                TcpClient client = new TcpClient();
                client.Connect("veron.homedns.org", 13000);
            }
            catch (SocketException)
            {
                MessageBox.Show("Le server n'est pas connecté");
                return;
            }

            aClient = new WebSocketClient(server)
            {
                OnReceive = OnReceive,
                ConnectTimeout = new TimeSpan(7, 0, 0, 0)
            };
            aClient.Connect();
            isServerRunning = true;
        }

        public static void SendCommand(Task task, string name)
        {
            task.fromName = name;
            Thread thread = new Thread(() => SendMessageTcp(task));
            thread.Start();
        }

        public static void SendLinesCommand(Task[] tasks, string name)
        {
            foreach (Task command in tasks)
            {
                SendCommand(command, name);
            }
        }

        static void SendMessageTcp(Task task)
        {
            try
            {
                if (aClient.Connected)
                {
                    aClient.Send(JsonConvert.SerializeObject(task));
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
            try
            {
            }
            catch (Exception)
            {
            }
        }
    }
}
