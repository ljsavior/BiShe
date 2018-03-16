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
    using Data;
    using Training;
    using System.Timers;
    using Utils;
    using System.Threading;

    /// <summary>
    /// Interaction logic for ActionTrainingPage.xaml
    /// </summary>
    public partial class ActionTrainingPage : Page
    {
        private MyActionTraining training;

        private SkeletonDataComsumer consumer;

        private System.Timers.Timer timer = new System.Timers.Timer();

        private volatile ActionImgPlayer player;


        public ActionTrainingPage()
        {
            InitializeComponent();

            this.Loaded += (s, e) => {
                this.timer.Elapsed += new ElapsedEventHandler(TimerCountDown);
                this.timer.Interval = 1000;

                //DataMessageQueue messageQueue = new ServerChannelDataMessageQueue();
                DataMessageQueue messageQueue = CommonDataMessageQueue.getInstance();

                this.consumer = new SkeletonDataComsumer(messageQueue, (vectors) => {
                    Dispatcher.Invoke(() => VectorDataReady(vectors));
                });

                this.player = new ActionImgPlayer(img => {
                    Dispatcher.Invoke(() => this.targetImageElement.Source = img);
                });
            };

            this.Unloaded += (s, e) => {
                timer.Dispose();
            };
        }


        private void Start_Training(object sender, RoutedEventArgs e)
        {
            this.training = MyTraining1Factory.createActionTraining();

            training.next();
            if (!training.isFinish())
            {
                player.setAction(training.getActionData());
            }
            player.start();

            //update UI
            TrainingProgress.Value = training.getProgess();
            StatusLabel.Content = training.successCount + " / " + training.index;

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




        private void nextPosture(bool success)
        {
            training.next(success);

            //update UI
            TrainingProgress.Value = training.getProgess();
            StatusLabel.Content = training.successCount + " / " + training.index;

            if (!training.isFinish())
            {
                player.setAction(training.getActionData());
            }
            else
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


        private void TimerCountDown(object source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() => {
                if (training.countDown() == 0)
                {
                    nextPosture(false);
                }

                TimeLabel.Content = training.currentActionTime;
            }));
        }

        private void TrainingFinish()
        {
            LogUtil.log("Training finish.");

            this.targetImageElement.Source = null;

            timer.Stop();
            consumer.stop();
            player.stop();
        }

        public void VectorDataReady(double[][] vector)
        {
            if (!training.isFinish())
            {
                /*
                if (PostureRecognition.matches(posture, training.getPosture()))
                {
                    LogUtil.log("匹配成功。");
                    nextPosture(true);
                }
                */
            }
        }

    }

    class ActionImgPlayer
    {
        private Action<BitmapImage> action;
        private int idx = 0;
        private ActionData actionData;
        private Thread playThread;
        private volatile bool state = false;

        public ActionImgPlayer(Action<BitmapImage> action)
        {
            this.action = action;
        }


        public void start()
        {
            lock(this)
            {
                state = true;
                playThread = new Thread(() => {
                    while (state)
                    {
                        action(getImage());
                        Thread.Sleep(33);
                    }
                });
                playThread.IsBackground = true;
                playThread.Start();
            }
        }

        public void stop()
        {
            lock(this)
            {
                state = false;
                playThread = null;
                action(null);
            }
        }

        public BitmapImage getImage()
        {
            lock(this)
            {
                if (idx < 0)
                {
                    return null;
                } else
                {
                    BitmapImage img = actionData.imageList[idx];
                    if(++idx == actionData.imageList.Count)
                    {
                        idx = -60;
                    }
                    return img;
                }
            }
        }

        public void setAction(ActionData actionData)
        {
            lock(this)
            {
                this.actionData = actionData;
                idx = -60;
            }
        }



    }
}
