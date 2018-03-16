using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace MyApplication.Data
{
    class ActionData
    {
        public List<BitmapImage> imageList { get; }
        public List<double[][]> dataList { get; }

        public int count { get; }

        public ActionData(int count)
        {
            imageList = new List<BitmapImage>(count);
            dataList = new List<double[][]>(count);
            this.count = count;
        }

        public ActionData(List<double[][]> dataList)
        {
            this.dataList = dataList;
            this.count = dataList.Count;
        }

        public void add(BitmapImage img, double[][] data)
        {
            imageList.Add(img);
            dataList.Add(data);
        }

        public void add(double[][] data)
        {
            dataList.Add(data);
        }

    }
}
