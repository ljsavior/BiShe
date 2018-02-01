using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace MyApplication.Data
{
    using Utils;

    class SkeletonDataComsumer
    {
        private DataMessageQueue messageQueue;

        private Action<double[][]> action;

        private static Thread consumeThread;

        public SkeletonDataComsumer(DataMessageQueue messageQueue, Action<double[][]> action)
        {
            this.messageQueue = messageQueue;
            this.action = action;

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
                        //LogUtil.log(CommonUtil.arrayToString(vectors));

                        action(vectors);

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
