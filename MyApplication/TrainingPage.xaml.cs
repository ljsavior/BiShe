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
    using Posture;
    using Utils;
    using Training;
    using System.Timers;
    using Data;
    using Service;

    /// <summary>
    /// Interaction logic for TrainingPage.xaml
    /// </summary>
    public partial class TrainingPage : Page
    {
        private MyTraining1 training;

        private SkeletonDataComsumer consumer;

        private Timer timer = new Timer();

        private Service service = new Service();


        public TrainingPage()
        {
            InitializeComponent();

            this.Loaded += (s, e) => {
                this.timer.Elapsed += new ElapsedEventHandler(TimerCountDown);
                this.timer.Interval = 1000;

                //DataMessageQueue messageQueue = new ServerChannelDataMessageQueue();
                DataMessageQueue messageQueue = CommonDataMessageQueue.getInstance();

                this.consumer = new SkeletonDataComsumer(messageQueue, (vectors) => {
                    Posture pos = new Posture(PostureType.Both, vectors);
                    Dispatcher.Invoke(() => PostureDataReady(pos));
                });
            };

            this.Unloaded += (s, e) => {
                timer.Dispose();
            };
        }


        private void nextPosture(bool success)
        {
            training.next(success);

            //update UI
            TrainingProgress.Value = training.getProgess();
            StatusLabel.Content = training.SuccessCount + " / " + training.Index;

            if (!training.isFinish())
            {
                String picPath = training.getPic();
                targetImageElement.Source = new BitmapImage(new Uri(picPath));
            } else
            {
                TrainingFinish();
            }
        }


        private void resetInfoArea()
        {
            TimeLabel.Content = "";
            StatusLabel.Content = "";
            TrainingProgress.Value = 0;
        }

        private void Start_Training(object sender, RoutedEventArgs e)
        {
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

            consumer.start();
            timer.Start();
        }


        private void Stop_Training(object sender, RoutedEventArgs e)
        {
            this.training = null;
            this.targetImageElement.Source = null;
            resetInfoArea();

            timer.Stop();
            consumer.stop();
        }

        private void TimerCountDown(object source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() => {
                if (training.countDown() == 0)
                {
                    nextPosture(false);
                }

                TimeLabel.Content = training.CurrentPostureTime;
            }));
        }

        private void TrainingFinish()
        {
            LogUtil.log("Training finish.");

            this.targetImageElement.Source = null;

            timer.Stop();
            consumer.stop();

            service.uploadTrainingRecord(TrainingNameSelect.Text, training.TimeUsedList, training.ResultList);

        }

        public void PostureDataReady(Posture posture)
        {
            if (!training.isFinish())
            {
                if (PostureRecognition.matches(posture, training.getPosture()))
                {
                    LogUtil.log("匹配成功。");
                    nextPosture(true);
                }
            }
        }
    }
}
