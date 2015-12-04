using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Alfred.Utils;
using Alfred.Model.Core;
using Alfred.Model.Db.Repositories;

namespace Alfred.Client.Gui
{
    public partial class ConfigurationForm : Form
    {
        private PathRepository _pathRepo;
        private CommandRepository _commandRepo;

        public ConfigurationForm()
        {
            _pathRepo = new PathRepository();
            _commandRepo = new CommandRepository();
            InitializeComponent();
            FillGeneralTab();
            FillCommandTab();
        }

        public void FillGeneralTab()
        {
            var h = 0;
            var config = _pathRepo.GetPaths();
            foreach (var item in config)
            {
                h += 25;

                var label = new Label();
                var textbox = new TextBox();

                label.Text = item.Key;
                textbox.BackColor = Color.White;
                textbox.Name = item.Key;
                textbox.Text = item.Value;
                textbox.Width = 300;
                textbox.Click += textbox_Click;

                label.Location = new Point(10, h);
                textbox.Location = new Point(120, h);

                tabPage1.Controls.Add(label);
                tabPage1.Controls.Add(textbox);
            }
        }

        public void FillCommandTab()
        {
            var h = 0;
            foreach (var command in _commandRepo.GetCommands())
            {
                var label = new Label();
                var speechList = new DataGridView();
                label.Text = command.Name;
                speechList.Name = label.Text;
                speechList.Height = 60;

                speechList.DataSource = command.Items.Select(x => new { Value = x.Term }).ToList();
                speechList.RowHeadersVisible = false;
                speechList.ColumnHeadersVisible = false;
                label.Location = new Point(10, h);
                speechList.Location = new Point(120, h);

                speechList.Refresh();
                speechList.ReadOnly = false;
                speechList.AllowUserToAddRows = true;
                speechList.EditMode = DataGridViewEditMode.EditOnKeystroke;

                tabPage2.Controls.Add(label);
                tabPage2.Controls.Add(speechList);

                h += 65;
            }
            tabPage2.AutoScroll = true;
        }

        void textbox_Click(object sender, EventArgs e)
        {
            var textbox = (TextBox)sender;
            if (Directory.Exists(textbox.Text))
            {
                folderBrowserDialog1.RootFolder = Environment.SpecialFolder.Desktop;
                var result = folderBrowserDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    textbox.Text = folderBrowserDialog1.SelectedPath;
                }
            }
            else if (File.Exists(textbox.Text))
            {
                openFileDialog1.InitialDirectory = new FileInfo(textbox.Text).DirectoryName;
                var result = openFileDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    textbox.Text = openFileDialog1.FileName;
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            var config = new Dictionary<string, string>();
            foreach (var textbox in tabPage1.Controls.OfType<TextBox>())
            {
                config.Add(textbox.Name, textbox.Text);
            }
            _pathRepo.SavePaths(config);
        }

        private void saveCommands_Click(object sender, EventArgs e)
        {
            var commands = new List<CommandModel>();

            var listViews = tabPage2.Controls.OfType<DataGridView>();
            foreach (var label in tabPage2.Controls.OfType<Label>())
            {
                var command = new CommandModel();
                command.Name = label.Text;
                command.Id = _commandRepo.GetIdFromName(command.Name);

                var labelList = listViews.SingleOrDefault(l => l.Name == label.Text);
                if (labelList != null)
                {
                    command.Items = new List<CommandItemModel>();
                    foreach (DataGridViewRow row in labelList.Rows)
                        command.Items.Add(new CommandItemModel { Term = row.Cells[0].Value.ToString(), CommandId = command.Id });
                    _commandRepo.UpdateItemsCommand(command);
                }
            }
        }
    }
}
