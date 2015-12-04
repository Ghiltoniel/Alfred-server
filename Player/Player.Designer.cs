using System.ComponentModel;
using System.Windows.Forms;
using AxWMPLib;

namespace AlfredPlayer
{
	partial class Player
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Player));
            this.browser = new System.Windows.Forms.WebBrowser();
            this.playerWmp = new AxWMPLib.AxWindowsMediaPlayer();
            this.connectionButton = new System.Windows.Forms.Button();
            this.connectionStatusLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.playerWmp)).BeginInit();
            this.SuspendLayout();
            // 
            // browser
            // 
            this.browser.Location = new System.Drawing.Point(12, 263);
            this.browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.browser.Name = "browser";
            this.browser.ScriptErrorsSuppressed = true;
            this.browser.Size = new System.Drawing.Size(696, 497);
            this.browser.TabIndex = 1;
            // 
            // playerWmp
            // 
            this.playerWmp.Enabled = true;
            this.playerWmp.Location = new System.Drawing.Point(12, 46);
            this.playerWmp.Name = "playerWmp";
            this.playerWmp.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("playerWmp.OcxState")));
            this.playerWmp.Size = new System.Drawing.Size(696, 211);
            this.playerWmp.TabIndex = 2;
            // 
            // connectionButton
            // 
            this.connectionButton.Location = new System.Drawing.Point(158, 12);
            this.connectionButton.Name = "connectionButton";
            this.connectionButton.Size = new System.Drawing.Size(75, 23);
            this.connectionButton.TabIndex = 3;
            this.connectionButton.Text = "Connecter";
            this.connectionButton.UseVisualStyleBackColor = true;
            this.connectionButton.Click += new System.EventHandler(this.connectionButton_Click);
            // 
            // connectionStatusLabel
            // 
            this.connectionStatusLabel.AutoSize = true;
            this.connectionStatusLabel.Location = new System.Drawing.Point(12, 17);
            this.connectionStatusLabel.Name = "connectionStatusLabel";
            this.connectionStatusLabel.Size = new System.Drawing.Size(75, 13);
            this.connectionStatusLabel.TabIndex = 4;
            this.connectionStatusLabel.Text = "Non connecté";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(256, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(80, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Configuration";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipText = "Alfred Player is still running !";
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "Alfred Player";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // Player
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 741);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.connectionStatusLabel);
            this.Controls.Add(this.connectionButton);
            this.Controls.Add(this.playerWmp);
            this.Controls.Add(this.browser);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Player";
            this.Text = "Player";
            ((System.ComponentModel.ISupportInitialize)(this.playerWmp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        public WebBrowser browser;
        public AxWindowsMediaPlayer playerWmp;
        private Button connectionButton;
        private Label connectionStatusLabel;
        private Button button1;
        private NotifyIcon notifyIcon1;
	}
}