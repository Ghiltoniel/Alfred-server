using System;
using System.Diagnostics;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace AlfredPlayer
{
    public partial class Player : Form
    {
        public Player()
        {
            Initialize();
            Init.media = new MediaManager();
            Init.browser = browser;
            Init.form = this;
            Init.connectionLabel = connectionStatusLabel;
            Init.connectionButton = connectionButton;

            var timer = new Timer(1000);
            timer.Elapsed += timer_Elapsed;
            timer.Enabled = true;
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var procStartInfo = new ProcessStartInfo("cmd", "/c net time \\\\SERVER /set /yes");

            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            // Do not create the black window.
            procStartInfo.CreateNoWindow = true;
            // Now we create a process, assign its ProcessStartInfo and start it
            var proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();

            while (proc.StandardOutput.ReadLine() != null)
            {

                var test = proc.StandardOutput.ReadLine();

            }
        }

        #region Init
        public void Initialize()
        {
            InitializeComponent();
            SetEvents();
            InitializeProperties();
            SetDisplay();
        }

        private void SetDisplay()
        {
            browser.Width = Width - 40;
            playerWmp.Width = Width - 40;
        }

        private void SetEvents()
        {
            FormClosing += IHM_FormClosing;
            Resize += Player_Resize;
            GotFocus += Player_Shown;
        }

        private void InitializeProperties()
        {
            SetDisplay();
        }
        #endregion

        #region Events

        private void IHM_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon1.Visible = false;
            Init.media.SendUpdateStatusSignal(5, 0, 0);
            Application.Exit();
        }

        void Player_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                notifyIcon1.Visible = true;
                try
                {
                    notifyIcon1.ShowBalloonTip(500);
                }
                catch(Exception)
                {

                }
                Hide();
            }
            else if (FormWindowState.Normal == WindowState)
            {
                notifyIcon1.Visible = false;
            }
        }

        void Player_Shown(object sender, EventArgs e)
        {
            Activate();
            BringToFront();
        }

        #endregion

        private void connectionButton_Click(object sender, EventArgs e)
        {
            Init.alfredClient.Connect();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Config().Show();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            Focus();
            WindowState = FormWindowState.Normal;
        }

        #region Media
        #endregion
    }
}
