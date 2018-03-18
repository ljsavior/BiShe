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

        private ActionMatcher actionMatcher = new ActionMatcher();


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
                actionMatcher.init(training.getActionData());
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
                actionMatcher.init(training.getActionData());
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
                if(actionMatcher.onData(vector))
                {
                    LogUtil.log("动作匹配成功。");
                    nextPosture(true);
                }
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
                BitmapImage res = null;
                if(idx < -30)
                {

                } else if(idx < 0)
                {
                    res = actionData.imageList[0];
                } else if(idx < actionData.imageList.Count)
                {
                    res = actionData.imageList[idx];
                } else if(idx < actionData.imageList.Count + 30)
                {
                    res = actionData.imageList[actionData.imageList.Count - 1];
                } else if(idx < actionData.imageList.Count + 60)
                {

                } else
                {
                    idx = -60;
                }
                idx++;

                return res;
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

    class ActionMatcher
    {
        private bool preStart = false;
        private bool start = false;

        private LinkedList<double[][]> vectorList = new LinkedList<double[][]>();

        private ActionData actionData;

        private Posture.Posture startPosture;

        private Posture.Posture prePosture;

        private int matchCount = 0;

        public void init(ActionData action)
        {
            actionData = action;
            preStart = false;
            start = false;
            vectorList.Clear();
            startPosture = new Posture.Posture(Posture.PostureType.Both, action.dataList[0]);
            matchCount = 0;
        }

        public bool onData(double[][] vectorData)
        {
            Posture.Posture currentPosture = new Posture.Posture(Posture.PostureType.Both, vectorData);
            if (!preStart)
            {
                if(Posture.PostureRecognition.matches(currentPosture, startPosture))
                {
                    vectorList.AddLast(vectorData);
                    preStart = true;
                    prePosture = currentPosture;

                    LogUtil.log("动作预开始");
                }
                return false;
            }

            if(!start && !Posture.PostureRecognition.matches(currentPosture, startPosture))
            {
                start = true;
                LogUtil.log("动作开始");
            }

            if(!start)
            {
                return false;
            }

            vectorList.AddLast(vectorData);

            if(Posture.PostureRecognition.matches(currentPosture, prePosture))
            {
                matchCount++;
            } else
            {
                matchCount = 0;
                prePosture = currentPosture;
            }


            if (matchCount > 15)
            {
                LogUtil.log("动作结束");

                int length = vectorList.Count - 15;
                List<double[][]> data = new List<double[][]>(length);

                int i = 0;
                foreach(double[][] v in vectorList)
                {
                    if(i++ == length)
                    {
                        break;
                    }
                    data.Add(v);
                }

                ActionData acData = ActionMatchingUtil.loadActionData(data);


                LogUtil.log(actionData.dataList.Count + "," + acData.dataList.Count);
                bool result = ActionMatchingUtil.match(actionData, acData);
                init(actionData);
                return result;
            }

            return false;
        }


    }
}
