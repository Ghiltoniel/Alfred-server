using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Windows.Forms;
using AlfredPlayer.Properties;

namespace AlfredPlayer
{
    public partial class Config : Form
    {
        readonly Dictionary<string, TextBox> textboxes = new Dictionary<string, TextBox>();
        TextBox currentTextbox;

        public Config()
        {
            InitializeComponent();
            FillEditors();
        }

        private void FillEditors()
        {
            var count = Settings.Default.Properties.Count;
            TextBox input;
            Label label;
            tableLayoutPanel1.RowCount = count;
            var row = 0;
            foreach (SettingsProperty currentProperty in Settings.Default.Properties)
            {
                var value = Settings.Default[currentProperty.Name];
                input = new TextBox();
                input.Text = value.ToString();

                if (File.Exists(value.ToString()))
                {
                    input.Click += input_Click;
                }

                textboxes.Add(currentProperty.Name, input);
                tableLayoutPanel1.Controls.Add(input, 1, row);

                label = new Label();
                label.Text = currentProperty.Name;
                tableLayoutPanel1.Controls.Add(label, 0, row);
                row++;
            }
        }

        void input_Click(object sender, EventArgs e)
        {
            currentTextbox = (TextBox)sender;
            openFileDialog1.InitialDirectory = ((TextBox)sender).Text;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (File.Exists(openFileDialog1.FileName))
                    {
                        currentTextbox.Text = openFileDialog1.FileName;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var item in textboxes)
            {
                Settings.Default[item.Key] = item.Value.Text;
            }
            Settings.Default.Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string error = null;
            foreach (var item in textboxes)
            {
                if (item.Key.ToLower().Contains("path") && !File.Exists(item.Value.Text))
                {
                    error = "Le fichier " + item.Value.Text + " n'existe pas !" + Environment.NewLine;
                }

                if (item.Key == "AlchemyHost")
                {
                    int port;
                    var portOK = int.TryParse(textboxes["AlchemyPort"].Text, out port);

                    if (!portOK)
                    {
                        error = "Le port pour le server Alchemy doit être un entier !" + Environment.NewLine;
                    }


                    try
                    {
                        var client = new TcpClient(
                            item.Value.Text,
                            port);
                        client.Close();
                    }
                    catch (SocketException ex)
                    {
                        error = "Impossible de se connecter au server Alchemy : " + ex.Message + Environment.NewLine;
                    }
                }
            }
            if (string.IsNullOrEmpty(error))
                MessageBox.Show("Votre configuration sembe être correcte !");
            else
                MessageBox.Show(error);
        }
    }
}
