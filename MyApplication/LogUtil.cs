using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MyApplication.Utils
{
    using System.IO;
    using System.Diagnostics;

    class LogUtil
    {
        private static System.Collections.Concurrent.BlockingCollection<String> messageQueue = new System.Collections.Concurrent.BlockingCollection<String>();

        private static Thread logThread;

        private static volatile bool state = true;

        static LogUtil()
        {
            logThread = new Thread(() => {
                using (StreamWriter sw = new StreamWriter(Constant.LOG_FILE_PATH, true))
                {
                    while (state)
                    {
                        try
                        {
                            String msg = messageQueue.Take();
                            sw.WriteLine(msg);
                            sw.Flush();
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            });
            logThread.IsBackground = true;
            logThread.Start();
        }



        public static void log(String message)
        {
            if (message != null)
            {
                StackFrame frame = new StackTrace().GetFrame(1);
                String type = frame.GetMethod().DeclaringType.ToString();
                String method = frame.GetMethod().Name.ToString();
                String time = DateTime.Now.ToString(Constant.LOG_TIME_FORMAT);

                String msg = String.Format("{0}: ({1}.{2}) - {3}", time, type, method, message);

                messageQueue.Add(msg);
            }
        }


    }
}
