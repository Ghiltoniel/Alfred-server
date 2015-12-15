using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using AlfredPlayer.Properties;

namespace AlfredPlayer.Controllers.Players
{
    class VlcPlayer : APlayer
    {
        const UInt32 WM_KEYDOWN = 0x0100;
        const int VK_F = 0x46;
        const int VK_SPACE = 0x20;
        const int VK_S = 0x53;

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        private readonly Process vlc;
        private double currentDuration;

        public VlcPlayer()
        {
            vlc = new Process();
            vlc.StartInfo = new ProcessStartInfo(Settings.Default.VlcPath);
        }

        public override void Play(string file)
        {
            if (File.Exists(file))
            {
                try
                {
                    vlc.Kill();
                }
                catch(Exception){}

                vlc.StartInfo.Arguments = string.Format("\"{0}\"", file);
                vlc.Start();
                Thread.Sleep(200);
                PostMessage(vlc.MainWindowHandle, WM_KEYDOWN, VK_F, 0);

                TimeSpan duration;
                if (!GetDuration(file, out duration))
                    duration = new TimeSpan(0);

                currentDuration = duration.TotalSeconds;
                Init.media.SendUpdateStatusSignal(3, currentDuration, 0);
            }
        }

        public override void Pause()
        {
            PlayPause();
            Init.media.SendUpdateStatusSignal(4, currentDuration, 0);
        }

        public override void PlayPause()
        {
            if (vlc.Responding)
            {
                PostMessage(vlc.MainWindowHandle, WM_KEYDOWN, VK_SPACE, 0);
            }
        }

        public override void Stop()
        {
            if (vlc.Responding)
            {
                PostMessage(vlc.MainWindowHandle, WM_KEYDOWN, VK_S, 0);
                Init.media.SendUpdateStatusSignal(5, currentDuration, 0);
            }
        }

        public override void SetPosition(double position)
        {
            if (vlc.Responding)
            {
                SetForegroundWindow(vlc.MainWindowHandle);
                SendKeys.SendWait("{ESC}");
                SendKeys.Flush();
                SendKeys.SendWait("^{t}");

                var h_int = Math.Round(position / 3600);
                var h = (h_int.ToString()).PadLeft(2, '0');

                var m_int = Math.Round(position - h_int * 3600);
                var m = (m_int.ToString()).PadLeft(2, '0');

                var s_int = Math.Round(position - h_int * 3600 - m_int * 60);
                var s = (s_int.ToString()).PadLeft(2, '0');

                SendKeys.SendWait(string.Format("{0}{1}{2}", h, m, s));
                SendKeys.Flush();
                SendKeys.SendWait("{ENTER}");
                SendKeys.Flush();
                SendKeys.SendWait("F");
            }
        }

        public override void SetVolume(float volume)
        {
            throw new NotImplementedException();
        }

        public static bool GetDuration(string filename, out TimeSpan duration)
        {
            try
            {
                var t = Type.GetTypeFromProgID("Shell.Application");
                dynamic shell = Activator.CreateInstance(t);

                var fldr = shell.NameSpace(Path.GetDirectoryName(filename));
                var itm = fldr.ParseName(Path.GetFileName(filename));
                var propValue = fldr.GetDetailsOf(itm, 27);

                return TimeSpan.TryParse(propValue, out duration);
            }
            catch (Exception)
            {
                duration = new TimeSpan();
                return false;
            }
        }
    }
}
