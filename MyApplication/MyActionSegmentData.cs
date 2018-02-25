using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Data
{
    using System.IO;
    class MyActionSegmentData
    {
        public String DataFilePath { get; }

        public int StartNum { get; }

        public int EndNum { get; }

        public int Interval { get; }

        public List<MySkeleton> SkeletonList { get; }

        public MyActionSegmentData(String dataFilePath, int startNum, int endNum, int interval)
        {
            this.DataFilePath = dataFilePath;
            this.StartNum = startNum;
            this.EndNum = endNum;
            this.Interval = interval > 0 ? interval : 1;

            this.SkeletonList = new List<MySkeleton>();
            init();
        }

        private void init()
        {
            if(!File.Exists(DataFilePath))
            {
                throw new Exception("文件[" + DataFilePath + "]不存在.");
            }
            using (StreamReader sr = new StreamReader(DataFilePath))
            {
                String line;
                for(int i = 1; i < StartNum; i++)
                {
                    line = sr.ReadLine();
                    if(line == null)
                    {
                        throw new Exception("文件已读到结尾.row:" + i);
                    }
                }
                for(int i = 1, count = EndNum - StartNum + 1; i <= count; i++)
                {
                    line = sr.ReadLine();
                    if(line == null)
                    {
                        throw new Exception("文件已读到结尾.row:" + (i + StartNum - 1));
                    }
                    if((i - 1) % Interval == 0)
                    {
                        SkeletonList.Add(Utils.CommonUtil.jsonStrToMySkeleton(line));
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Format("MyActionSegmentData[Path={0}, StartNum={1}, EndNum={2}, Interval={3}]", DataFilePath, StartNum, EndNum, Interval);
        }
    }
}
