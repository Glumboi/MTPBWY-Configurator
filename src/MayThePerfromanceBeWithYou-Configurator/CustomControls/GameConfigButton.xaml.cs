using MayThePerfromanceBeWithYou_Configurator.Pages;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Wpf.Ui.Common;

namespace MayThePerfromanceBeWithYou_Configurator.CustomControls
{
    /// <summary>
    /// Interaction logic for GameConfigButton.xaml
    /// </summary>
    public partial class GameConfigButton : Wpf.Ui.Controls.Button
    {
        public string GameName
        {
            get => (string)GetValue(GameNameProperty);
            set => SetValue(GameNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for GameName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GameNameProperty =
            DependencyProperty.Register("GameName", typeof(string), typeof(GameConfigButton), new PropertyMetadata(null));

        public ImageSource GameImageSource
        {
            get => (ImageSource)GetValue(GameImageSourceProperty);
            set => SetValue(GameImageSourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for GameImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GameImageSourceProperty =
            DependencyProperty.Register("GameImageSource", typeof(ImageSource), typeof(GameConfigButton), new PropertyMetadata(null));

        public int GlobalMargin
        {
            get => (int)GetValue(GlobalMarginProperty);
            set => SetValue(GlobalMarginProperty, value);
        }

        // Using a DependencyProperty as the backing store for GlobalMargin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GlobalMarginProperty =
            DependencyProperty.Register("GlobalMargin", typeof(int), typeof(GameConfigButton), new PropertyMetadata(5));

        public int DesiredImageSize
        {
            get { return (int)GetValue(DesiredImageSizeProperty); }
            set { SetValue(DesiredImageSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DesiredImageSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DesiredImageSizeProperty =
            DependencyProperty.Register("DesiredImageSize", typeof(int), typeof(GameConfigButton), new PropertyMetadata(130));

        public static readonly DependencyProperty OpenConfigCommandProperty =
            DependencyProperty.Register("OpenConfigCommand", typeof(ICommand), typeof(GameConfigButton));

        public ICommand OpenConfigCommand
        {
            get { return (ICommand)GetValue(OpenConfigCommandProperty); }
            set { SetValue(OpenConfigCommandProperty, value); }
        }

        public void CreateOpenConfigCommand()
        {
            OpenConfigCommand = new RelayCommand(OpenConfig);
        }

        public void OpenConfig()
        {
            //_pluginIndex
            var convertedWindow = _callerWindow as MainWindow;
            convertedWindow.mainPage.ViewModel.SelectedPlugin = _pluginIndex;
            convertedWindow.NavigateToPage(convertedWindow.mainPage);
        }

        public int DesiredButtonSize
        {
            get => (int)GetValue(DesiredButtonSizeProperty);
            set => SetValue(DesiredButtonSizeProperty, value);
        }

        // Using a DependencyProperty as the backing store for DesiredButtonSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DesiredButtonSizeProperty =
            DependencyProperty.Register("ImageSize", typeof(int), typeof(GameConfigButton), new PropertyMetadata(155));

        public static readonly DependencyProperty DownloadShaderCommandProperty =
            DependencyProperty.Register("DownloadShaderCommand", typeof(ICommand), typeof(GameConfigButton));

        private int _pluginIndex;

        private Window _callerWindow;

        private static ImageSource ConvertToImageSource(System.Drawing.Image image)
        {
            Bitmap bitmap = new Bitmap(image);

            IntPtr hBitmap = bitmap.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions()
                );
            }
            finally
            {
                // Release the HBitmap created by GetHbitmap
                NativeMethods.DeleteObject(hBitmap);
            }
        }

        private static class NativeMethods
        {
            [System.Runtime.InteropServices.DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
        }

        public GameConfigButton(
            int desiredImageSize,
            int desiredButtonSize,
            string gameCover,
            int myPluginIndex,
            Window callerWindow)
        {
            InitializeComponent();
            Style = (Style)Application.Current.Resources[typeof(Wpf.Ui.Controls.Button)];
            DesiredImageSize = desiredImageSize;
            DesiredButtonSize = desiredButtonSize;
            var bmp = ImageEXT.LoadImageFromUrl(gameCover, desiredImageSize, desiredImageSize);
            GameImageSource = ImageEXT.ConvertToImageSource(bmp);
            _pluginIndex = myPluginIndex;
            _callerWindow = callerWindow;
            CreateOpenConfigCommand();
        }
    }

    internal static class ImageEXT
    {
        public static ImageSource ConvertToImageSource(System.Drawing.Image img)
        {
            using (var ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }

        public static Bitmap LoadImageFromUrl(string url, int maxWidth, int maxHeight)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();

            using (System.Drawing.Image originalImage = System.Drawing.Image.FromStream(responseStream))
            {
                int newWidth = originalImage.Width;
                int newHeight = originalImage.Height;

                // Scale the image down if it is larger than the maximum size
                if (originalImage.Width > maxWidth)
                {
                    newWidth = maxWidth;
                    newHeight = (int)(originalImage.Height * ((float)maxWidth / originalImage.Width));
                }
                if (newHeight > maxHeight)
                {
                    newHeight = maxHeight;
                    newWidth = (int)(originalImage.Width * ((float)maxHeight / originalImage.Height));
                }

                // Create a new Bitmap object with the scaled size
                Bitmap newBitmap = new Bitmap(newWidth, newHeight);
                using (Graphics graphics = Graphics.FromImage(newBitmap))
                {
                    // Draw the original image onto the new Bitmap object with the scaled size
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
                }

                return newBitmap;
            }
        }
    }
}