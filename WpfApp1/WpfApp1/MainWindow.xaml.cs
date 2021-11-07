using System;
using System.Threading.Tasks;
using System.Windows;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;
using System.Threading;
using Microsoft.Win32;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // OxyPlot のモデルとコントローラー
        PlotModel plotModel { get; } = new PlotModel();
        LineSeries lineSeries { get; } = new LineSeries();
        bool canelFrag = false;
        public MainWindow()
        {
            InitializeComponent();
            GraphSetup();
        }

        //グラフの見た目をつくる
        void GraphSetup()
        {
            // X軸とY軸の設定
            var AxisX = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                TitleFontSize = 16,
                Title = "X軸"
            };

            var AxisY = new LinearAxis()
            {
                Position = AxisPosition.Left,
                TitleFontSize = 16,
                Title = "Y軸"
            };

            plotModel.Axes.Add(AxisX);
            plotModel.Axes.Add(AxisY);
            plotModel.Background = OxyColors.White;

            //折れ線グラフの設定
            lineSeries.StrokeThickness = 1.5;
            lineSeries.Color = OxyColor.FromRgb(0, 100, 205);

            plotModel.Series.Add(lineSeries);

            PlotView.Model = plotModel;
        }

        void Draw_Button(object sender, RoutedEventArgs e)
        {
            //描画されているグラフをクリア
            lineSeries.Points.Clear();
            canelFrag = false;
            DrawButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
            StopButton.IsEnabled = true;

            using(var tokenSource = new CancellationTokenSource())
            {
                Draw(tokenSource);
            }
        }

        void Stop_Button(object sender, RoutedEventArgs e)
        {
            canelFrag = true;
            StopButton.IsEnabled = false;
            SaveButton.IsEnabled = true;
            DrawButton.IsEnabled = true;
        }

        void Save_Button(object sender, RoutedEventArgs e)
        {
            var dlg = new SaveFileDialog
            {
                Filter = "BMP形式|*.bmp",
                DefaultExt = ".bmp"
            };
            if (dlg.ShowDialog(this).Value)
            {
                var ext = System.IO.Path.GetExtension(dlg.FileName).ToLower();
                switch (ext)
                {
                    case ".bmp":
                        PlotView.SaveBitmap(dlg.FileName, 0, 0);
                        break;
                    //他の拡張子があるならば追加
                }
            }
        }

        async Task Draw(CancellationTokenSource tokenSource)
        {
            int deg = 0;
            while (true)
            {
                if (canelFrag)
                {
                    tokenSource.Cancel();
                    return;
                }

                //データ数が 720 を超えたらデキューしていく
                if(lineSeries.Points.Count > 720)
                {
                    lineSeries.Points.RemoveAt(0);
                }

                //とりあえず皆大好き sin 波
                var val = Math.Sin(deg * (Math.PI / 180));
                lineSeries.Points.Add(new DataPoint(deg, val));
                plotModel.InvalidatePlot(true);
                deg++;
                await Task.Delay(1);
            }
        }
    }
}
