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
    using Posture;
    using Utils;
    using Training;
    using System.Timers;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for TrainingPage.xaml
    /// </summary>
    public partial class TrainingPage : Page
    {
        private KinectSensor kinectSensor;

        private long index = 0;

        private MyTraining1 training;

        private Timer timer = new Timer();


        public TrainingPage()
        {
            InitializeComponent();

            this.Loaded += (s, e) => {
                //DiscoverKinectSensor();
                //nextPosture();
                this.timer.Elapsed += new ElapsedEventHandler(TimerCountDown);
                this.timer.Interval = 1000;
            };

            this.Unloaded += (s, e) => {
                UninitializeKinectSensor(kinectSensor);
                kinectSensor = null;
                timer.Dispose();
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
            using (ColorImageFrame frame = e.OpenColorImageFrame())
            {
                if (frame != null)
                {
                    byte[] pixelData = new byte[frame.PixelDataLength];
                    frame.CopyPixelDataTo(pixelData);
                    colorImageElement.Source = BitmapImage.Create(frame.Width, frame.Height, 96, 96, PixelFormats.Bgr32, null, pixelData, frame.Width * frame.BytesPerPixel);
                }
            }
        }

        private void kinectSensor_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
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
                        if (++index % 30 == 0)
                        {
                            if (!training.isFinish())
                            {
                                if (PostureRecognition.matches(skel, training.getPosture()))
                                {
                                    LogUtil.log("匹配成功。");
                                    nextPosture(true);
                                }
                            }
                        }
                    }
                }
            }

        }

        private void nextPosture(bool success)
        {
            training.next(success);
            if (!training.isFinish())
            {
                String picPath = training.getPic();
                targetImageElement.Source = new BitmapImage(new Uri(picPath));
            }

            //update UI
            TrainingProgress.Value = training.getProgess();
            StatusLabel.Content = training.SuccessCount + " / " + training.Index;
        }


        private void resetInfoArea()
        {
            TimeLabel.Content = "";
            StatusLabel.Content = "";
            TrainingProgress.Value = 0;
        }

        private void Start_Training(object sender, RoutedEventArgs e)
        {
            DiscoverKinectSensor();
            this.training = MyTraining1Factory.create1();

            training.next();
            if (!training.isFinish())
            {
                String picPath = training.getPic();
                targetImageElement.Source = new BitmapImage(new Uri(picPath));
            }

            //update UI
            TrainingProgress.Value = training.getProgess();
            StatusLabel.Content = training.SuccessCount + " / " + training.Index;

            timer.Start();
        }


        private void Stop_Training(object sender, RoutedEventArgs e)
        {
            UninitializeKinectSensor(kinectSensor);
            this.kinectSensor = null;
            this.training = null;
            this.colorImageElement.Source = null;
            this.targetImageElement.Source = null;
            resetInfoArea();

            timer.Stop();
        }

        private void TimerCountDown(object source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() => {
                if (training.countDown() == 0)
                {
                    nextPosture(false);
                    if(training.isFinish())
                    {
                        TrainingFinish();
                    }
                }

                TimeLabel.Content = training.CurrentPostureTime;
            }));
        }

        private void TrainingFinish()
        {
            LogUtil.log("Training finish.");

            timer.Stop();
        }
    }
}
