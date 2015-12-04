using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Alfred.Model.Core.Interface;

namespace Alfred.Client.Gui.Controllers.Ink
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public class InkController : ControllerAbstract
    {
        public ObservableCollection<GroovesharkMusic> musics { get; set; }
        public InkEventHandlers eventHandler;

        public InkController()
        {
            eventHandler = new InkEventHandlers(this);
        }

        public override void Initialize()
        {
            var window = Init.mainWindow;
            window.WindowState = WindowState.Maximized;
            window.DataContext = this;

            window.resultGrid.Width = window.inkPanel.ActualWidth;
            window.resultGrid.Height = window.inkPanel.ActualHeight;
            window.theInkCanvas.Width = window.inkPanel.ActualWidth;
            window.theInkCanvas.Height = window.inkPanel.ActualHeight;

            var width = window.inkPanel.ActualWidth - 10;
            window.resultGrid.Columns[0].Width = width / 2;
            window.resultGrid.Columns[1].Width = width / 2;

            if(!window.resultGrid.CellStyle.Setters.IsSealed)
                window.resultGrid.CellStyle.Setters.Add(new Setter(DataGridCell.WidthProperty, window.resultGrid.Width / 2));
            
            window.resultGrid.MinRowHeight = window.resultGrid.Height / 5;
            window.inkPanel.Visibility = Visibility.Visible;

            var margin = window.inkButtonWrapper.Margin;
            margin.Top = window.inkPanel.ActualHeight - 100;
            window.inkButtonWrapper.Margin = margin;
            window.clearButton.Width = width / 3 - 10;
            window.closeButton.Width = width / 3 - 10;
            window.recognizeButton.Width = width / 3 - 10;
        }

        public override void StartDevice()
        {
            Init.alfredClient.Connect();
        }

        public override void StopDevice()
        {
            Init.mainWindow.inkPanel.Visibility = Visibility.Hidden;
            Init.mainWindow.WindowState = WindowState.Normal;
        }

        public override void StartListening()
        {
        
        }

        public override void StopListening()
        {
            
        }

        public override void RegisterEvents()
        {
            Init.mainWindow.StylusDown += mainWindow_StylusDown;
            Init.mainWindow.theInkCanvas.StylusDown += theInkCanvas_StylusDown;
            Init.mainWindow.resultGrid.SelectedCellsChanged += resultGrid_SelectedCellsChanged;
            Init.mainWindow.clearButton.Click += eventHandler.clearClick;
            Init.mainWindow.closeButton.Click += eventHandler.closeClick;
            Init.mainWindow.recognizeButton.Click += eventHandler.recognizeClick;
        }

        public override void UnregisterEvents()
        {
            Init.mainWindow.theInkCanvas.StylusDown -= theInkCanvas_StylusDown;
            Init.mainWindow.resultGrid.SelectedCellsChanged -= resultGrid_SelectedCellsChanged;
            Init.mainWindow.clearButton.Click -= eventHandler.clearClick;
            Init.mainWindow.closeButton.Click -= eventHandler.closeClick;
            Init.mainWindow.recognizeButton.Click -= eventHandler.recognizeClick;
        }

        void theInkCanvas_StylusDown(object sender, StylusDownEventArgs e)
        {
            Reset();
        }

        public void Reset()
        {
            Init.mainWindow.resultGrid.Items.Clear();
            Init.mainWindow.resultGrid.Visibility = Visibility.Hidden;
            Init.mainWindow.labelInfo.Visibility = Visibility.Hidden;
        }

        void resultGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (!e.AddedCells.Any())
                return;

            var focusedMusic = (GroovesharkMusic)e.AddedCells[0].Item;

            Init.alfredClient.Websocket.MediaManager.DirectPlay(
                focusedMusic.ArtistName,
                focusedMusic.AlbumName,
                focusedMusic.SongName,
                focusedMusic.Url);

            Init.mainWindow.inkPanel.Visibility = Visibility.Hidden;
            Init.mainWindow.resultGrid.Visibility = Visibility.Hidden;
            Init.mainWindow.WindowState = WindowState.Normal;
        }

        void mainWindow_StylusDown(object sender, StylusDownEventArgs e)
        {
            Init.mainWindow.inkPanel.Visibility = Visibility.Visible;
        }

    }
}
