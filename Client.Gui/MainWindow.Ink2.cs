using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Ink;
using Microsoft.Ink;
using Newtonsoft.Json.Linq;
using AlfredModel;

namespace AlfredNewInterface
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public ObservableCollection<GroovesharkMusic> musics { get; set; }

        public void InitializeInkWindow()
        {
            DataContext = this;
            theInkCanvas.StylusDown += theInkCanvas_StylusDown;

            resultGrid.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            resultGrid.Height = System.Windows.SystemParameters.PrimaryScreenHeight / 2;
            resultGrid.Columns[0].Width = resultGrid.Width / 2;
            resultGrid.Columns[1].Width = resultGrid.Width / 2;
            resultGrid.CellStyle.Setters.Add(new Setter(DataGridCell.WidthProperty, resultGrid.Width / 2));
            resultGrid.MinRowHeight = resultGrid.Height / 5;
            resultGrid.SelectedCellsChanged += resultGrid_SelectedCellsChanged;

            Thickness margin = clearButton.Margin;
            clearButton.Width = resultGrid.Width / 3 - 10;
            margin.Left = 5;
            clearButton.Margin = margin;
            closeButton.Width = resultGrid.Width / 3 - 10;
            margin.Left = resultGrid.Width / 3;
            closeButton.Margin = margin;
            recognizeButton.Width = resultGrid.Width / 3 - 10;
            margin.Left = resultGrid.Width / 3 * 2;
            recognizeButton.Margin = margin;

            theInkCanvas.Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            theInkCanvas.Height = System.Windows.SystemParameters.PrimaryScreenHeight;
        }

        void theInkCanvas_StylusDown(object sender, StylusDownEventArgs e)
        {
            Reset();
        }

        public void Reset()
        {
            resultGrid.Items.Clear();
            resultGrid.Visibility = System.Windows.Visibility.Hidden;
            labelInfo.Visibility = System.Windows.Visibility.Hidden;
        }

        void resultGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var focusedMusic = (GroovesharkMusic)e.AddedCells[0].Item;

            Task task = new Task() { baseCommand = "Media_DirectPlay" };
            task.arguments = new Dictionary<string, string>();
            task.arguments.Add("file", focusedMusic.Url);
            task.arguments.Add("artist", focusedMusic.ArtistName);
            task.arguments.Add("title", focusedMusic.SongName);
            task.arguments.Add("album", focusedMusic.AlbumName);

            Init.tcpClient.SendCommand(task);

            this.Close();
        }

        private void UpdateResultGrid(ObservableCollection<GroovesharkMusic> musics)
        {
            if (musics.Count > 0)
            {
                foreach (var music in musics)
                {
                    resultGrid.Items.Add(music);
                }
                resultGrid.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                labelInfo.Content = "Aucune musique n'a été trouvée !";
                labelInfo.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void recognizeClick(object sender, RoutedEventArgs e)
        {
            Reset();
            if (theInkCanvas.Strokes.Count > 0)
            {
                System.Windows.Ink.StrokeCollection strokeList = theInkCanvas.Strokes;

                MemoryStream ms = new MemoryStream();
                theInkCanvas.Strokes.Save(ms);
                InkCollector myInkCollector = new InkCollector();
                Ink ink = new Ink();
                ink.Load(ms.ToArray());

                using (RecognizerContext myRecoContext = new RecognizerContext())
                {
                    RecognitionStatus status;
                    RecognitionResult recoResult;

                    myRecoContext.Strokes = ink.Strokes;
                    recoResult = myRecoContext.Recognize(out status);

                    string url = "http://tinysong.com/s/" + recoResult.TopString + "?format=json&key=91c9333f318105b007f6dc49ac07fc03";
                    WebClient client = new WebClient();
                    string result = client.DownloadString(url);

                    musics = new ObservableCollection<GroovesharkMusic>();
                    JArray tokens = JArray.Parse(result);

                    foreach (JToken token in tokens)
                    {
                        GroovesharkMusic music = new GroovesharkMusic();
                        music.Url = token["Url"].Value<string>();
                        music.SongID = token["SongID"].Value<int>();
                        music.ArtistName = token["ArtistName"].Value<string>();
                        music.SongName = token["SongName"].Value<string>();
                        musics.Add(music);
                    }
                }
                UpdateResultGrid(musics);
            }
        }

        private void clearClick(object sender, RoutedEventArgs e)
        {
            Reset();
            theInkCanvas.Strokes.Clear();
        }

        private void closeClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
