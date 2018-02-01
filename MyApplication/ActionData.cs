using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;


namespace MyApplication.Data
{
    using System.Drawing;
    using System.IO;
    using Microsoft.Kinect;

    using Utils;

    class ActionData
    {
        private List<BitmapSource> colorFrameList = new List<BitmapSource>();
        private List<BitmapSource> skeletonFrameList = new List<BitmapSource>();
        private List<Skeleton> skeletonDataList = new List<Skeleton>();

        private int index = -1;

        public void reset()
        {
            index = -1;
        }

        public bool next()
        {
            return ++index < skeletonDataList.Count;
        }

        public BitmapSource getColorFrame()
        {
            return colorFrameList[index];
        }

        public BitmapSource getSkeletonFrame()
        {
            return skeletonFrameList[index];
        }

        public Skeleton getSkeletonData()
        {
            return skeletonDataList[index];
        }

        public double[][] getVectorData()
        {
            return CommonUtil.computeVectors(skeletonDataList[index]);
        }

        public void saveToFile(String dirPath)
        {
            if(skeletonDataList.Count == 0)
            {
                LogUtil.log("saveToFile：ActionData中无数据.");
                return;
            }

            dirPath = (dirPath.EndsWith("/") ? dirPath : dirPath + "/") + CommonUtil.currentTimeMillis().ToString();

            if(Directory.Exists(dirPath))
            {
                throw new Exception("ActionData saveData 文件夹[" + dirPath + "]已存在.");
            }
            Directory.CreateDirectory(dirPath);


            int n = 1;
            String colorFramePath = dirPath + "/colorFrame/";
            Directory.CreateDirectory(colorFramePath);
            foreach (BitmapSource frame in colorFrameList)
            {
                ImageUtil.saveBitMapSourceToFile(frame, colorFramePath + n++ + ".jpg");
            }

            n = 1;
            String skeletonFramePath = dirPath + "/skeletonFrame/";
            Directory.CreateDirectory(skeletonFramePath);
            foreach (BitmapSource frame in skeletonFrameList)
            {
                ImageUtil.saveBitMapSourceToFile(frame, skeletonFramePath + n++ + ".jpg");
            }

            String skeletonDataPath = dirPath + "/skeletonData.txt";
            using (StreamWriter sw = new StreamWriter(skeletonDataPath, true))
            {
                foreach (Skeleton skel in skeletonDataList)
                {
                    /*
                    double[][] data = CommonUtil.computeVectors(skel);
                    sw.WriteLine(CommonUtil.arrayToString(data));
                    */
                    sw.WriteLine(CommonUtil.skeletonDataToString(skel));

                }
                sw.Flush();
            }
        }

        public void add(BitmapSource colorFrame, BitmapSource skeletonFrame, Skeleton skeletonData)
        {
            colorFrameList.Add(colorFrame);
            //skeletonFrameList.Add(skeletonFrame);
            this.skeletonDataList.Add(skeletonData);
        }
    }
}
