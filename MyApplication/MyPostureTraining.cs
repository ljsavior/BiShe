using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyApplication.Posture;

namespace MyApplication.Training
{
    using System.IO;

    class MyPostureTraining
    {
        private const int EachPositionTime = 30;

        private List<String> picPathList = new List<String>();
        private List<Posture.Posture> postureList = new List<Posture.Posture>();
        private int index = -1;
        private int successCount = 0;

        private int currentPostureTime;


        private List<int> timeUsedList = new List<int>();
        private List<bool> resultList = new List<bool>();


        public Boolean isFinish()
        {
            return index == postureList.Count;
        }

        public void next()
        {
            index++;
            currentPostureTime = EachPositionTime;
        }

        public void next(bool success)
        {
            if (!success && currentPostureTime != 0)
            {
                return;
            }

            if(success)
            {
                successCount++;
            }

            this.resultList.Add(success);
            this.timeUsedList.Add(EachPositionTime - currentPostureTime);

            index++;
            currentPostureTime = EachPositionTime;
        }

        public String getPic()
        {
            return picPathList[index];
        }

        public Posture.Posture getPosture()
        {
            return postureList[index];
        }


        public List<int> TimeUsedList
        {
            get
            {
                return timeUsedList;
            }
        }

        public List<bool> ResultList
        {
            get
            {
                return resultList;
            }
        }

        public List<string> PicPathList
        {
            get
            {
                return picPathList;
            }
        }

        public int Index
        {
            get
            {
                return index;
            }
        }

        public int CurrentPostureTime
        {
            get
            {
                return currentPostureTime;
            }
        }

        public int SuccessCount
        {
            get
            {
                return successCount;
            }
        }

        internal List<Posture.Posture> PostureList
        {
            get
            {
                return postureList;
            }
        }

        public int countDown()
        {
            return --currentPostureTime;
        }

        public int getProgess()
        {
            return 100 * index / postureList.Count;
        }

    }

}
