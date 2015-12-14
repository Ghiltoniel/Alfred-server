using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Windows;
using Microsoft.Ink;
using Newtonsoft.Json.Linq;

namespace Alfred.Client.Gui.Controllers.Ink
{
    public class InkEventHandlers
    {
        private readonly InkController _controller;
        public InkEventHandlers(InkController controller)
        {
            _controller = controller;
        }

        public void recognizeClick(object sender, RoutedEventArgs e)
        {
            _controller.Reset();
            if (Init.mainWindow.theInkCanvas.Strokes.Count > 0)
            {
                var strokeList = Init.mainWindow.theInkCanvas.Strokes;

                var ms = new MemoryStream();
                Init.mainWindow.theInkCanvas.Strokes.Save(ms);
                var myInkCollector = new InkCollector();
                var ink = new Microsoft.Ink.Ink();
                ink.Load(ms.ToArray());

                using (var myRecoContext = new RecognizerContext())
                {
                    RecognitionStatus status;
                    RecognitionResult recoResult;

                    myRecoContext.Strokes = ink.Strokes;
                    recoResult = myRecoContext.Recognize(out status);

                    var url = "http://tinysong.com/s/" + recoResult.TopString + "?format=json&key=91c9333f318105b007f6dc49ac07fc03";
                    var client = new WebClient();
                    var result = client.DownloadString(url);

                    _controller.musics = new ObservableCollection<GroovesharkMusic>();
                    var tokens = JArray.Parse(result);

                    foreach (var token in tokens)
                    {
                        var music = new GroovesharkMusic();
                        music.Url = token["Url"].Value<string>();
                        music.SongID = token["SongID"].Value<int>();
                        music.ArtistName = token["ArtistName"].Value<string>();
                        music.SongName = token["SongName"].Value<string>();
                        _controller.musics.Add(music);
                    }
                }
                UpdateResultGrid(_controller.musics);
            }
        }

        public void clearClick(object sender, RoutedEventArgs e)
        {
            _controller.Reset();
            Init.mainWindow.theInkCanvas.Strokes.Clear();
        }

        public void closeClick(object sender, RoutedEventArgs e)
        {
            Init.mainWindow.theRootCanvas.Visibility = Visibility.Hidden;
        }
        public void UpdateResultGrid(ObservableCollection<GroovesharkMusic> musics)
        {
            if (musics.Count > 0)
            {
                foreach (var music in musics)
                {
                    Init.mainWindow.resultGrid.Items.Add(music);
                }
                Init.mainWindow.resultGrid.Visibility = Visibility.Visible;
            }
            else
            {
                Init.mainWindow.labelInfo.Content = "Aucune musique n'a été trouvée !";
                Init.mainWindow.labelInfo.Visibility = Visibility.Visible;
            }
        }
    }
}