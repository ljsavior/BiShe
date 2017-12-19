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

namespace MyApplication
{
    using Microsoft.Kinect;

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private KinectSensor kinectSensor;

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) => DiscoverKinectSensor();
            this.Unloaded += (s, e) => this.kinectSensor = null;

        }

        public KinectSensor KinectSensor
        {
            get
            {
                return kinectSensor;
            }

            set
            {
                kinectSensor = value;
            }
        }

        private void InitializeKinectSensor(KinectSensor kinectSensor)
        {
            if(kinectSensor != null)
            {
                kinectSensor.ColorStream.Enable();
                kinectSensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(kinectSensor_ColorFrameReady);
                kinectSensor.Start();
            }
        }

        private void UninitializeKinectSensor(KinectSensor kinectSensor)
        {
            if(kinectSensor != null)
            {
                kinectSensor.Stop();
                kinectSensor.ColorFrameReady -= new EventHandler<ColorImageFrameReadyEventArgs>(kinectSensor_ColorFrameReady);
            }
        }

        private void kinectSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame frame = e.OpenColorImageFrame())
            {
                if(frame != null)
                {
                    byte[] pixelData = new byte[frame.PixelDataLength];
                    frame.CopyPixelDataTo(pixelData);
                    colorImageElement.Source = BitmapImage.Create(frame.Width, frame.Height, 96, 96, PixelFormats.Bgr32, null, pixelData, frame.Width * frame.BytesPerPixel);
                }
            }
        }

        private void DiscoverKinectSensor()
        {
            //KinectSensor.KinectSensors.StatusChanged += KinectSensors_StatusChanged;
            this.kinectSensor = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
            InitializeKinectSensor(this.kinectSensor);
        }

        private void KinectSensors_StatusChanged(object sender, StatusChangedEventArgs e)
        {
            switch (e.Status)
            {
                case KinectStatus.Connected:
                    if (this.kinectSensor == null)
                    {
                        this.kinectSensor = e.Sensor;
                    }
                    break;
                case KinectStatus.Disconnected:
                    if(this.kinectSensor == e.Sensor)
                    {
                        this.kinectSensor = null;
                        this.kinectSensor = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
                    }
                    if(this.kinectSensor == null)
                    {
                        MessageBox.Show("Kinect已拔出");
                    }
                    break;
                
            }
        }
    }
}
