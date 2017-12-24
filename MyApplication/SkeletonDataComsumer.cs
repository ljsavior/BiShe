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

        private IpcClientChannel channel;
        private DataObj dataObj;

        private static Thread consumeThread;

        public SkeletonDataComsumer(TrainingPage trainingPage)
        {
            this.trainingPage = trainingPage;

            initChannel();
        }

        private void initChannel()
        {
            try
            {
                channel = new IpcClientChannel();
                ChannelServices.RegisterChannel(channel, false);
                dataObj = (DataObj)Activator.GetObject(typeof(DataObj), "ipc://ServerChannel/DataObj");
            }
            catch (Exception e)
            {
                LogUtil.log("DataObj 获取异常");
            }
            if (dataObj == null)
            {
                LogUtil.log("DataObj 获取失败，为null");
            }

        }

        public void start()
        {
            if (dataObj != null)
            {
                consumeThread = new Thread(() => {
                    int count = dataObj.clear();
                    LogUtil.log("clear: " + count);
                    while (Thread.CurrentThread.IsAlive)
                    {
                        try
                        {
                            double[][] vectors = dataObj.receive();
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
            try
            {
                ChannelServices.UnregisterChannel(channel);
            } catch(Exception e)
            {
                LogUtil.log(e.Message);
            }
            consumeThread = null;
        }


    }
}
