using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using System.Drawing;
using Microsoft.Win32;

namespace GTAllPaintEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CarColorLoader CarColorLoader { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 1 || !files[0].EndsWith(".bin"))
                    return;

                if (!File.Exists(files[0]))
                    return;

                LoadAllPaintsFile(files[0]);
            }
        }

        private void btn_LoadAllPaint_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.CheckPathExists = true;
            dialog.Filter = "allpaint.bin (*.bin)|*.bin";

            if (dialog.ShowDialog() is true)
                LoadAllPaintsFile(dialog.FileName);
        }

        private void btn_SaveAllPaint_Click(object sender, RoutedEventArgs e)
        {
            if (CarColorLoader is null)
                return;

            var dialog = new SaveFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;

            if (dialog.ShowDialog() is true)
            {
                CarColorLoader.Save(dialog.FileName);
            }
        }

        private void lb_Paints_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lb_Paints.SelectedIndex == -1)
                return;

            var paint = lb_Paints.SelectedItem as PaintEntry;
            using (Bitmap paintImage = CarColorLoader.GetPaintImageBuffer(paint.ID))
            {
                img_PaintImage.Source = GetImageSourceFromBitmap(paintImage);
            }

            using (Bitmap paintImage2 = CarColorLoader.GetPaintImageBuffer2(paint.ID))
            {
                img_PaintImage2.Source = GetImageSourceFromBitmap(paintImage2);
            }

            this.DataContext = paint;

            OnLoadedPaint();
        }

        private ImageSource GetImageSourceFromBitmap(Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        private void img_LoadPaintImage_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Paints.SelectedIndex == -1)
                return;

            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            dialog.Filter = "PNG Image (*.png)|*.png|JPG Image (*.jpg)|*.jpg";

            if (dialog.ShowDialog() is true)
                LoadImage(dialog.FileName);
        }

        public void img_PaintImage_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 1)
                    return;

                if (!File.Exists(files[0]))
                    return;

                LoadImage(files[0]);
            }
        }

        public void img_PaintImage2_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 1)
                    return;

                if (!File.Exists(files[0]))
                    return;

                LoadImage2(files[0]);
            }
        }

        private void img_LoadPaintImage2_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Paints.SelectedIndex == -1)
                return;

            var dialog = new OpenFileDialog();
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;

            if (dialog.ShowDialog() is true)
                LoadImage2(dialog.FileName);
        }

        private void img_SavePaintImage_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Paints.SelectedIndex == -1)
                return;

            var paint = lb_Paints.SelectedItem as PaintEntry;

            var dialog = new SaveFileDialog();
            dialog.FileName = $"{paint.ID}.png";

            if (dialog.ShowDialog() is true)
            {
                using (Bitmap paintImage = CarColorLoader.GetPaintImageBuffer(paint.ID))
                {
                    paintImage.Save(dialog.FileName);
                }
            }
        }

        private void img_SavePaintImage2_Click(object sender, RoutedEventArgs e)
        {
            if (lb_Paints.SelectedIndex == -1)
                return;

            var paint = lb_Paints.SelectedItem as PaintEntry;

            var dialog = new SaveFileDialog();
            dialog.FileName = $"{paint.ID}.png";
            if (dialog.ShowDialog() is true)
            {
                using (Bitmap paintImage = CarColorLoader.GetPaintImageBuffer2(paint.ID))
                {
                    paintImage.Save(dialog.FileName);
                }
            }
        }

        private void OnLoadedPaint()
        {
            gp_PaintImage.IsEnabled = true;
            gp_PaintImage2.IsEnabled = true;
            gb_PaintProperties.IsEnabled = true;
        }

        private void LoadAllPaintsFile(string fileName)
        {
            try
            {
                CarColorLoader = new CarColorLoader(fileName);
                CarColorLoader.Load();
            }
            catch (Exception e)
            {
                MessageBox.Show("Errored", $"Unable to load file: {e.Message}", MessageBoxButton.OK, MessageBoxImage.Error);
                btn_SaveAllPaint.IsEnabled = false;

                gp_PaintImage.IsEnabled = false;
                gp_PaintImage2.IsEnabled = false;
                gb_PaintProperties.IsEnabled = false;

                return;
            }

            lb_Paints.ItemsSource = CarColorLoader.PaintEntries;
            btn_SaveAllPaint.IsEnabled = true;

        }

        private void LoadImage2(string fileName)
        {
            System.Drawing.Image img = null;
            try
            {
                img = Bitmap.FromFile(fileName);
                if (img.Width != 64 || img.Height != 32)
                {
                    MessageBox.Show("Invalid image", "Image must be 64x32.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    img.Dispose();
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Invalid image", "Could not load image.", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            img_PaintImage2.Source = GetImageSourceFromBitmap((Bitmap)img);

            var paint = lb_Paints.SelectedItem as PaintEntry;
            paint.SetColorBuffer2((Bitmap)img);

            img.Dispose();
        }

        private void LoadImage(string fileName)
        {
            System.Drawing.Image img = null;
            try
            {
                img = Bitmap.FromFile(fileName);
                if (img.Width != 64 || img.Height != 32)
                {
                    MessageBox.Show("Invalid image", "Image must be 64x32.", MessageBoxButton.OK, MessageBoxImage.Warning);
                    img.Dispose();
                    return;
                }
            }
            catch
            {
                MessageBox.Show("Invalid image", "Could not load image.", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            img_PaintImage.Source = GetImageSourceFromBitmap((Bitmap)img);

            var paint = lb_Paints.SelectedItem as PaintEntry;
            paint.SetColorBuffer((Bitmap)img);

            img.Dispose();
        }
    }
}
