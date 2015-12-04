using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Alfred.Model.Core.Interface;
using Newtonsoft.Json;

namespace Alfred.Client.Gui.Controllers.MindWave
{
    public class MindWaveController : ControllerAbstract
    {
        public string appName;
        public string appKey;
        public bool isAuthorized;
        public bool formatSent;

        public TcpClient client;
        public NetworkStream tcpStream;
        public Thread listeningThread;

        delegate void RawEventMethod(int[] values);
        delegate void PoorSignalEventMethod(int level);
        delegate void MeditationEventMethod(int level);
        delegate void EegEventMethod(int delta, int theta, int low_alpha, int high_alpha, int low_beta, int high_beta, int low_gamma, int mid_gamma);
        delegate void BlinkEventMethod(int level);
        delegate void AttentionEventMethod(int level);

        private RawEventMethod rawEventMethod;
        private PoorSignalEventMethod poorSignalEventMethod;
        private MeditationEventMethod meditationEventMethod;
        private EegEventMethod eegEventMethod;
        private BlinkEventMethod blinkEventMethod;
        private AttentionEventMethod attentionEventMethod;

        private ThinkGearEventsHandlers EventHandlers;

        public override void Initialize()
        {
            client = new TcpClient();
            SHA1 sha = new SHA1CryptoServiceProvider();
            // This is one implementation of the abstract class SHA1.
            appName = "Alfred";
            appKey = string.Empty;
            var appKeyBytes = sha.ComputeHash(Encoding.ASCII.GetBytes(appName));
            for(var i=0; i<appKeyBytes.Length; i++)
                appKey += string.Format("{0:X2}", appKeyBytes[i]).ToLower();

            isAuthorized = false;
            formatSent = false;

            EventHandlers = new ThinkGearEventsHandlers();
            meditationEventMethod = EventHandlers.MeditationEventHandler;
            eegEventMethod = EventHandlers.EegEventHandler;
            blinkEventMethod = EventHandlers.BlinkEventHandler;
            attentionEventMethod = EventHandlers.AttentionEventHandler;
            poorSignalEventMethod = EventHandlers.PoorSignalEventHandler;
        }

        public override void StartDevice()
        {
            try
            {
                client.Connect("127.0.0.1", 13854);
                if (client.Connected)
                {
                    tcpStream = client.GetStream();

                    if (tcpStream.CanWrite)
                    {
                        if (!string.IsNullOrEmpty(appName) && !string.IsNullOrEmpty(appKey))
                        {
                            var json = JsonConvert.SerializeObject(new
                            {
                                appName, appKey
                            });

                            var bytesToSend = ASCIIEncoding.ASCII.GetBytes(json);
                            tcpStream.Write(bytesToSend, 0, bytesToSend.Length);
                        }
                    }
                }
            }
            catch (SocketException)
            {
                Init.mainWindow.SetLeapLabelContent("Cannot reach MindWave data server");
            }
        }

        public override void StopDevice()
        {
            try
            {
                client.Close();
            }
            catch(SocketException)
            {

            }
        }

        public override void StartListening()
        {
            listeningThread = new Thread(SocketListen);
            listeningThread.Start();
        }

        public override void StopListening()
        {
            if (listeningThread.ThreadState == ThreadState.Running)
                listeningThread.Abort();
        }

        public void SocketListen()
        {
            if (client.Connected && tcpStream.CanRead)
            {
                try
                {
                    byte[] block = new byte[8196];
                    while (tcpStream.Read(block, 0, block.Length) != 0)
                    {
                        string packetString = ASCIIEncoding.UTF8.GetString(block);
                        Init.mainWindow.SetLeapLabelContent(packetString);
                        block = new byte[8196];
                        if (!formatSent)
                        {
                            Thread.Sleep(1500);
                            var json = JsonConvert.SerializeObject(new
                            {
                                format = "Json"
                            });

                            var bytesToSend = ASCIIEncoding.ASCII.GetBytes(json);
                            tcpStream.Write(bytesToSend, 0, bytesToSend.Length);
                            formatSent = true;
                            continue;
                        }

                        var packets = packetString.Split('\r');
                        for (var s = 0; s < packets.Length; s++)
                        {
                            if ((packets[s]).IndexOf('{') > -1)
                            {
                                var obj = JsonConvert.DeserializeObject<ThinkGearStatus>(packets[s]);
                                parsePacket(obj);
                            }
                        }
                    }
                }
                catch (SocketException e)
                {
                    Init.mainWindow.SetLeapLabelContent(e.Message);
                }
                catch (IOException)
                {

                }
                catch (JsonException)
                {
                }
            }
        }

        private void triggerAttentionEvent(int attentionLevel)
        {
            if (attentionEventMethod != null)
            {
                try
                {
                    attentionEventMethod.BeginInvoke(attentionLevel, null, null );
                }
                catch (Exception)
                {
                    attentionEventMethod = null;
                }
            }
        }

        private void triggerMeditationEvent(int meditationLevel)
        {
            if (meditationEventMethod != null)
            {
                try
                {
                    meditationEventMethod.BeginInvoke(meditationLevel, null, null);
                }
                catch (Exception)
                {
                    meditationEventMethod = null;
                }
            }
        }

        private void triggerPoorSignalEvent(int poorSignalLevel)
        {
            if (poorSignalEventMethod != null)
            {
                try
                {
                    poorSignalEventMethod.Invoke(poorSignalLevel);
                    //println("Attention: " + attention);
                }
                catch (Exception)
                {
                    poorSignalEventMethod = null;
                }
            }
        }

        private void triggerBlinkEvent(int blinkStrength)
        {
            if (blinkEventMethod != null)
            {
                try
                {
                    blinkEventMethod.Invoke(blinkStrength);
                }
                catch (Exception)
                {
                    blinkEventMethod = null;
                }
            }
        }

        private void triggerEEGEvent(int delta, int theta, int low_alpha, int high_alpha, int low_beta, int high_beta, int low_gamma, int mid_gamma)
        {
            if (eegEventMethod != null)
            {
                try
                {
                    eegEventMethod.Invoke(delta, theta, low_alpha, high_alpha, low_beta, high_beta, low_gamma, mid_gamma);
                }
                catch (Exception)
                {
                    eegEventMethod = null;
                }
            }
        }


        private void triggerRawEvent(int[] values)
        {
            if (rawEventMethod != null)
            {
                try
                {
                    rawEventMethod.Invoke(values);
                }
                catch (Exception)
                {
                    rawEventMethod = null;
                }
            }
        }
        private void parsePacket(ThinkGearStatus data)
        {
            try
            {
                if (data.isAuthorized.HasValue)
                    isAuthorized = true;

                if (ThinkGearEventsHandlers.RaiseEvents)
                    {
                    if (data.blinkStrength != null)
                    {
                        triggerBlinkEvent(data.blinkStrength.Value);
                    }

                    if (data.eSense != null)
                    {
                        var esense = data.eSense;
                        triggerAttentionEvent(esense.attention);
                        triggerMeditationEvent(esense.meditation);
                    }
                }
            }
            catch (Exception)
            {

            }
        }

    }
}
