using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Training
{
    using System.IO;
    using Posture;
    using Data;
    using Utils;

    class MyTraining1Factory
    {
        public static MyPostureTraining create1()
        {
            MyPostureTraining training = new MyPostureTraining();

            using (StreamReader sr = new StreamReader(Constant.BASE_PATH + "/data/data1/data.txt"))
            {
                for (int i = 1; i <= 9; i++)
                {
                    training.PicPathList.Add(Constant.BASE_PATH + "/data/data1/" + i + "_color.jpg");

                    Posture pos = new Posture(PostureType.Both);
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



        public static MyActionTraining createActionTraining()
        {
            ActionData action1 = ActionMatchingUtil.loadActionDataFromFile(Constant.BASE_PATH + "/ActionData/1517659759365/", 69, 107);
            ActionData action2 = ActionMatchingUtil.loadActionDataFromFile(Constant.BASE_PATH + "/ActionData/1517658799185/", 70, 115); 


            MyActionTraining training = new MyActionTraining();
            training.addActions(action1, action2);

            return training;
        }
    }
}
