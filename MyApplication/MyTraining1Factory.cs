using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Training
{
    using System.IO;
    using Posture;

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

    }
}
