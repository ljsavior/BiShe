using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;


namespace MyApplication.Data
{
    using Utils;
    using MyPage;
    using Posture;

    class SkeletonDataComsumer
    {
        private TrainingPage trainingPage;

        //private DataMessageQueue messageQueue = new ServerChannelDataMessageQueue();
        private DataMessageQueue messageQueue = CommonDataMessageQueue.getInstance();

        private static Thread consumeThread;

        public SkeletonDataComsumer(TrainingPage trainingPage)
        {
            this.trainingPage = trainingPage;

            initChannel();
        }

        private void initChannel()
        {
            messageQueue.init();

        }

        public void start()
        {
            consumeThread = new Thread(() => {
                int count = messageQueue.clear();
                LogUtil.log("clear: " + count);
                while (Thread.CurrentThread.IsAlive)
                {
                    try
                    {
                        double[][] vectors = messageQueue.poll();
                        //LogUtil.log(vectors.ToString());
                        Posture pos = new Posture(PostureType.Both, vectors);
                        trainingPage.Dispatcher.Invoke(new Action(() => trainingPage.PostureDataReady(pos)));
                    } catch(Exception e)
                    {
                        LogUtil.log(e.Message);
                    }
                }
            });
            consumeThread.IsBackground = true;
            consumeThread.Start();
        }

        public void stop()
        {
            try
            {
                consumeThread.Abort();
            } catch(Exception e)
            {
                LogUtil.log(e.Message);
            }
            messageQueue.release();
            consumeThread = null;
        }


    }
}
