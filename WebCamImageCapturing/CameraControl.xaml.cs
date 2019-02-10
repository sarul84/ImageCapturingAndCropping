using System;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WebEye.Controls.Wpf;

namespace Prakrishta.ImageCapturing
{
    /// <summary>
    /// Interaction logic for CameraControl.xaml
    /// </summary>
    public partial class CameraControl : Window
    {
        private bool isDragging = false;
        private Point anchorPoint = new Point();

        public CameraControl()
        {
            InitializeComponent();

            Crop.IsEnabled = false;
            InitializeComboBox();
        }

        private void InitializeComboBox()
        {
            ComboBox.ItemsSource = WebCameraControl.GetVideoCaptureDevices();


            if (ComboBox.Items.Count > 0)
            {
                ComboBox.SelectedItem = ComboBox.Items[0];
            }
        }


        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StartButton.IsEnabled = e.AddedItems.Count > 0;
        }


        private void OnStartButtonClick(object sender, RoutedEventArgs e)
        {
            var cameraId = (WebCameraId)ComboBox.SelectedItem;
            WebCameraControl.StartCapture(cameraId);
        }


        private void OnStopButtonClick(object sender, RoutedEventArgs e)
        {
            WebCameraControl.StopCapture();
        }


        private void OnImageButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                WebCameraControl.GetCurrentImage().Save(ms, ImageFormat.Bmp);
                byte[] buffer = ms.GetBuffer();
                MemoryStream bufferPasser = new MemoryStream(buffer);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = bufferPasser;
                bitmap.EndInit();
                CapturedImage.Source = bitmap;
                CapturedImage.Stretch = Stretch.Fill;
            }
            catch (Exception)
            {
                MessageBox.Show("There is an error while capturing image", "Image", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Gridimage1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (isDragging == false)
                {
                    anchorPoint.X = e.GetPosition(CropPanel).X;
                    anchorPoint.Y = e.GetPosition(CropPanel).Y;
                    isDragging = true;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("There is an error while selecting area for cropping", "Crop", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Gridimage1_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (isDragging)
                {
                    isDragging = false;
                    if (SelectionRectangle.Width > 0)
                    {
                        Crop.Visibility = System.Windows.Visibility.Visible;
                        Crop.IsEnabled = true;
                    }
                    if (SelectionRectangle.Visibility != Visibility.Visible)
                        SelectionRectangle.Visibility = Visibility.Visible;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("There is an error while selecting area for cropping", "Crop", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Gridimage1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (isDragging)
                {
                    double x = e.GetPosition(CropPanel).X;
                    double y = e.GetPosition(CropPanel).Y;
                    SelectionRectangle.SetValue(Canvas.LeftProperty, Math.Min(x, anchorPoint.X));
                    SelectionRectangle.SetValue(Canvas.TopProperty, Math.Min(y, anchorPoint.Y));
                    SelectionRectangle.Width = Math.Abs(x - anchorPoint.X);
                    SelectionRectangle.Height = Math.Abs(y - anchorPoint.Y);

                    if (SelectionRectangle.Visibility != Visibility.Visible)
                        SelectionRectangle.Visibility = Visibility.Visible;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("There is an error while selecting area for cropping", "Crop", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void Crop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CapturedImage.Source != null)
                {
                    Rect rect1 = new Rect(Canvas.GetLeft(SelectionRectangle), Canvas.GetTop(SelectionRectangle), SelectionRectangle.Width, SelectionRectangle.Height);

                    Int32Rect rcFrom = new Int32Rect
                    {
                        X = (int)((rect1.X) * (CapturedImage.Source.Width) / (CapturedImage.ActualWidth)),
                        Y = (int)((rect1.Y) * (CapturedImage.Source.Height) / (CapturedImage.ActualHeight)),
                        Width = (int)((rect1.Width) * (CapturedImage.Source.Width) / (CapturedImage.ActualWidth)),
                        Height = (int)((rect1.Height) * (CapturedImage.Source.Height) / (CapturedImage.ActualHeight))
                    };

                    BitmapSource bs = new CroppedBitmap(CapturedImage.Source as BitmapSource, rcFrom);
                    CapturedImage.Source = bs;
                    SelectionRectangle.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("There is an error while cropping", "Crop", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
