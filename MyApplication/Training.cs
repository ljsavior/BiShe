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

    class MyTraining1Factory
    {
        public static MyTraining1 create1()
        {
            MyTraining1 training = new MyTraining1();

            using (StreamReader sr = new StreamReader("d:/MyApplication/data/data.txt"))
            {
                for (int i = 1; i <= 7; i++)
                {
                    training.PicPathList.Add("d:/MyApplication/data/" + i + "_color.jpg");

                    Posture.Posture pos = new Posture.Posture(PostureType.Both);
                    List<VectorType> vectorTypeList = PostureRecognition.getVectorTypeList(PostureType.Both);

                    foreach (VectorType type in vectorTypeList)
                    {
                        string[] lineData = sr.ReadLine().Split(',');
                        pos.setVector(type, new Vector(double.Parse(lineData[0]), double.Parse(lineData[1]), double.Parse(lineData[2])));
                    }
                    sr.ReadLine();
                    training.PostureList.Add(pos);
                }
            }

            return training;
        }
        
    }
}
