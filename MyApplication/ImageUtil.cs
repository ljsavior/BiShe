using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MyApplication.Utils
{
    using System.Windows.Media;
    using System.IO;

    class ImageUtil
    {
        public static void saveBitMapSourceToFile(BitmapSource source, String path)
        {
            saveBitmapFrameToFile(BitmapFrame.Create(source), path);
        }


        public static void saveBitmapFrameToFile(BitmapFrame frame, String path)
        {
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(frame);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                encoder.Save(stream);
            }
        }

    }
    
}
