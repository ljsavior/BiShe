using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Data
{
    using System.Collections.Concurrent;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Ipc;
    using System.Threading;

    using Utils;

    interface DataMessageQueue
    {
        double[][] poll();

        void offer(double[][] data);

        int clear();

        void init();

        void release();
    }

    class ServerChannelDataMessageQueue : DataMessageQueue
    {
        private IpcClientChannel channel;

        private DataObj dataObj;

        private volatile bool inited = false;

        public void init()
        {
            try
            {
                channel = new IpcClientChannel();
                ChannelServices.RegisterChannel(channel, false);
                dataObj = (DataObj)Activator.GetObject(typeof(DataObj), "ipc://ServerChannel/DataObj");
                inited = true;
            }
            catch (Exception e)
            {
                LogUtil.log("DataObj 获取异常. " + e.Message);
            }
            if (dataObj == null)
            {
                LogUtil.log("DataObj 获取失败，为null");
            }
        }

        public void release()
        {
            try
            {
                ChannelServices.UnregisterChannel(channel);
            }
            catch (Exception e)
            {
                LogUtil.log(e.Message);
            }
            inited = false;
        }

        public int clear()
        {
            checkInited();
            return dataObj.clear();
        }

        public void offer(double[][] data)
        {
            checkInited();
            dataObj.send(data);
        }

        public double[][] poll()
        {
            checkInited();
            return dataObj.receive();
        }

        private void checkInited()
        {
            if (!inited)
            {
                throw new Exception("ServerChannelDataMessageQueue 尚未初始化");
            }
        }
    }

    class CommonDataMessageQueue : DataMessageQueue
    {
        private static BlockingCollection<double[][]> messageQueue = new BlockingCollection<double[][]>();
        private volatile int interval = 1;
        private volatile int count = 0;

        private static CommonDataMessageQueue instance = new CommonDataMessageQueue();


        private CommonDataMessageQueue()
        {
        }

        public static CommonDataMessageQueue getInstance()
        {
            return getInstance(-1);
        }

        public static CommonDataMessageQueue getInstance(int interval)
        {
            if(interval > 0)
            {
                instance.interval = interval;
            }
            instance.clear();
            return instance;
        }


        public void init()
        {
        }

        public void release()
        {
            instance.clear();
            count = 0;
        }

        public int clear()
        {
            lock(this)
            {
                int count = messageQueue.Count;
                for (int i = 0; i < count; i++)
                {
                    messageQueue.Take();
                }
                return count;
            }
        }

        public void offer(double[][] data)
        {
            if(interval == 1 || IncrementAndGet() % interval == 0)
            {
                messageQueue.Add(data);
            }
        }

        public double[][] poll()
        {
            return messageQueue.Take();
        }

        private int IncrementAndGet()
        {
            return Interlocked.Increment(ref count);
        }
    }
}
