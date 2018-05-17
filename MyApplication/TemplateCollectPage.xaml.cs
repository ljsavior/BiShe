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

namespace MyApplication.MyPage
{
    using Microsoft.Kinect;
    using System.Windows.Media;
    using System.Threading;
    using Posture;
    using Utils;
    using Data;

    /// <summary>
    /// TemplateCollectPage.xaml 的交互逻辑
    /// </summary>
    public partial class TemplateCollectPage : Page
    {
        private KinectSensor kinectSensor;
        private int status = 0;
        private List<BitmapSource> imgList;
        private List<double[][]> vectorsList;

        private Service.Service service = new Service.Service();

        public TemplateCollectPage()
        {
            InitializeComponent();

            init();
        }

        private void init()
        {
            this.startSlider.Maximum = 0;
            this.endSlider.Maximum = 0;


            this.Loaded += (s, e) => {
                DiscoverKinectSensor();
            };

            this.Unloaded += (s, e) =>
            {
                UninitializeKinectSensor(this.kinectSensor);
                this.kinectSensor = null;
            };
        }


        private void DiscoverKinectSensor()
        {
            this.kinectSensor = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);
            InitializeKinectSensor(this.kinectSensor);
        }

        private void InitializeKinectSensor(KinectSensor kinectSensor)
        {
            if (kinectSensor != null)
            {
                kinectSensor.ColorStream.Enable();
                kinectSensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(kinectSensor_ColorFrameReady);
                kinectSensor.Start();

                kinectSensor.SkeletonStream.Enable();
                kinectSensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinectSensor_SkeletonFrameReady);

                kinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
                //kinectSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;
            }
        }

        private void UninitializeKinectSensor(KinectSensor kinectSensor)
        {
            if (kinectSensor != null)
            {
                kinectSensor.Stop();
                kinectSensor.ColorFrameReady -= new EventHandler<ColorImageFrameReadyEventArgs>(kinectSensor_ColorFrameReady);

                kinectSensor.SkeletonFrameReady -= new EventHandler<SkeletonFrameReadyEventArgs>(kinectSensor_SkeletonFrameReady);
            }
        }

        private void kinectSensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            if (status != 1)
            {
                return;
            }


            using (ColorImageFrame frame = e.OpenColorImageFrame())
            {
                if (frame != null)
                {
                    byte[] pixelData = new byte[frame.PixelDataLength];
                    frame.CopyPixelDataTo(pixelData);
                    BitmapSource source = BitmapImage.Create(frame.Width, frame.Height, 96, 96, PixelFormats.Bgr32, null, pixelData, frame.Width * frame.BytesPerPixel);
                    if(vectorsList.Count > imgList.Count)
                    {
                        imgList.Add(source);
                        if(startImg.Source == null)
                        {
                            startImg.Source = source;
                        }
                        endImg.Source = source;
                    }
                }
            }
        }

        private void kinectSensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            if (status != 1)
            {
                return;
            }

            Skeleton[] skeletons = new Skeleton[0];

            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }

            if (skeletons.Length != 0)
            {
                foreach (Skeleton skel in skeletons)
                {
                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        double[][] vectors = ActionRecognitionUtil.computeVectors(skel);
                        vectorsList.Add(vectors);
                    }
                }
            }

        }




        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            imgList = new List<BitmapSource>();
            vectorsList = new List<double[][]>();
            status = 1;
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            status = 2;
            startSlider.Maximum = endSlider.Maximum = imgList.Count - 1;
            endSlider.Value = endSlider.Maximum;
        }

        private void startSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(startSlider.Value > endSlider.Value)
            {
                startSlider.Value = endSlider.Value;
            }

            int num = (int)startSlider.Value;
            if(imgList.Count > num)
            {
                startImg.Source = imgList[num];
            }
        }

        private void endSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (endSlider.Value < startSlider.Value)
            {
                endSlider.Value = startSlider.Value;
            }

            int num = (int)endSlider.Value;
            if (imgList.Count > num)
            {
                endImg.Source = imgList[num];
            }
        }

        private void updatePostureButton_Click(object sender, RoutedEventArgs e)
        {
            String name = nameText.Text.Trim();
            if(name.Length == 0)
            {
                MessageBox.Show("请输入姿势名称");
                return;
            }
            int idx = (int)startSlider.Value;

            bool res = service.uploadPosture(name, imgList[idx], vectorsList[idx], "");

            MessageBox.Show(res ? "姿势数据上传成功" : "姿势数据上传失败");
            nameText.Text = "";
        }

        private void updateActionButton_Click(object sender, RoutedEventArgs e)
        {
            String name = nameText.Text.Trim();
            if (name.Length == 0)
            {
                MessageBox.Show("请输入动作名称");
                return;
            }
            MessageBox.Show("动作数据上传成功");
            nameText.Text = "";
        }
    }
}
