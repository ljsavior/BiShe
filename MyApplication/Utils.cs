using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Utils
{
    using System.Windows.Media;
    using System.IO;
    //using System.Reflection;
    using System.Diagnostics;

    class ImageUtil
    {
        public static System.Drawing.Image ImageWpfToGDI(ImageSource image)
        {
            MemoryStream ms = new MemoryStream();
            var encoder = new System.Windows.Media.Imaging.BmpBitmapEncoder();
            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(image as System.Windows.Media.Imaging.BitmapSource));
            encoder.Save(ms);
            ms.Flush();
            return System.Drawing.Image.FromStream(ms);
        }
    }


    class LogUtil
    {
        private static System.Collections.Concurrent.BlockingCollection<String> messageQueue = new System.Collections.Concurrent.BlockingCollection<String>();

        private static System.Threading.Thread logThread;

        static LogUtil()
        {
            logThread = new System.Threading.Thread(() => {
                using (StreamWriter sw = new StreamWriter(Constant.LOG_FILE_PATH, true))
                {
                    while (logThread.IsAlive)
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
