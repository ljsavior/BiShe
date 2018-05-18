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
    using System.Diagnostics;

    using System.Net.Sockets;
    using System.Net;
    using System.Threading;

    using Training;
    using Data;
    using Posture;
    using Utils;


    using System.IO;

    /// <summary>
    /// GameTrainingPage.xaml 的交互逻辑
    /// </summary>
    public partial class GameTrainingPage : Page
    {
        private MyPostureTraining training;

        private Socket serverSocket;
        private Socket socket;

        private SkeletonDataComsumer consumer;



        public GameTrainingPage()
        {
            InitializeComponent();

            this.Loaded += (s, e) => {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse("127.0.0.1");
                EndPoint endPoint = new IPEndPoint(ip, 6000);
                serverSocket.Bind(endPoint);
                serverSocket.Listen(1);

                //DataMessageQueue messageQueue = new ServerChannelDataMessageQueue();
                DataMessageQueue messageQueue = CommonDataMessageQueue.getInstance();

                this.consumer = new SkeletonDataComsumer(messageQueue, (vectors) => {
                    Posture pos = new Posture(PostureType.Both, vectors);
                    Dispatcher.Invoke(() => PostureDataReady(pos));
                });
            };

            this.Unloaded += (s, e) => {
                if(socket != null)
                {
                    socket.Close();
                }
                if(serverSocket != null)
                {
                    serverSocket.Close();
                }
                if(consumer != null)
                {
                    consumer.stop();
                }
            };
            
        }



        private void Start_Game(object sender, RoutedEventArgs e)
        {
            Process p = Process.Start(Constant.GAME_EXE_PATH);

            socket = serverSocket.Accept();

            this.training = MyTraining1Factory.create1();

            training.next();
            if (!training.isFinish())
            {
                targetImageElement.Source = training.getPosture().getPic();
            }
            consumer.start();

        }

        private void sendMsg(string msg)
        {
            if(socket != null)
            {
                var data = Encoding.UTF8.GetBytes(msg);
                socket.Send(data);
            }
        }

        private void PostureDataReady(Posture posture)
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

        private void nextPosture(bool success)
        {
            sendMsg("go");
            training.next(success);

            if (!training.isFinish())
            {
                targetImageElement.Source = training.getPosture().getPic();
            }
            else
            {
                TrainingFinish();
            }
        }

        private void TrainingFinish()
        {
            LogUtil.log("Training finish.");

            this.targetImageElement.Source = null;

            consumer.stop();

        }

    }
}
