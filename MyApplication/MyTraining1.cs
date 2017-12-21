using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApplication.Posture;

namespace MyApplication.Training
{
    using System.IO;

    class MyTraining1
    {
        private List<String> picPathList = new List<String>();
        private List<Posture.Posture> postureList = new List<Posture.Posture>();
        private int index = -1;

        private List<int> timeUsedList = new List<int>();

        public Boolean isFinish()
        {
            return index == postureList.Count;
        }

        public void next()
        {
            index++;
        }

        public String getPic()
        {
            return picPathList[index];
        }

        public Posture.Posture getPosture()
        {
            return postureList[index];
        }

        public void recordTime(int time)
        {
            timeUsedList.Add(time);
        }



        public List<int> TimeUsedList
        {
            get
            {
                return timeUsedList;
            }
        }

        public List<string> PicPathList
        {
            get
            {
                return picPathList;
            }
        }

        internal List<Posture.Posture> PostureList
        {
            get
            {
                return postureList;
            }
        }
    }

}
