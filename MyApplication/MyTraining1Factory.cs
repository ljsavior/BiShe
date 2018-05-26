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
    using com.force.json;

    class MyTraining1Factory
    {
        public static MyPostureTraining create1()
        {
            MyPostureTraining training = new MyPostureTraining();

            using (StreamReader sr = new StreamReader(Constant.BASE_PATH + "/data/data1/data.txt"))
            {
                for (int i = 1; i <= 9; i++)
                {

                    Posture pos = new Posture(PostureType.Both);
                    List<VectorType> vectorTypeList = PostureRecognition.getVectorTypeList(PostureType.Both);
                    pos.setPic(Constant.BASE_PATH + "/data/data1/" + i + "_color.jpg");

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

        public static MyPostureTraining createPostureTraining(String trainingName)
        {
            Service.Service service = new Service.Service();
            PostureLoader postureLoader = new PostureLoader();

            MyPostureTraining training = new MyPostureTraining();
            JSONObject trainingData = service.queryTraining(trainingName);

            if(trainingData == null)
            {
                LogUtil.log("查询不到名称为[" + trainingName + "]的姿势训练.");
                return training;
            }

            String postures = trainingData.GetString("postures");
            int[] postureIds = Utils.CommonUtil.stringToIntArray1(postures);

            for(int i = 0; i < postureIds.Length; i++)
            {
                training.PostureList.Add(postureLoader.Load(postureIds[i]));
            }

            return training;
        }



        public static MyActionTraining createActionTraining()
        {
            ActionData action1 = ActionMatchingUtil.loadActionDataFromFile(Constant.BASE_PATH + "/ActionData/1517659759365/", 69, 107);
            ActionData action2 = ActionMatchingUtil.loadActionDataFromFile(Constant.BASE_PATH + "/ActionData/1517658799185/", 70, 115);
            ActionData action3 = ActionMatchingUtil.loadActionDataFromFile(Constant.BASE_PATH + "/ActionData/1517659205489/", 190, 270);
            ActionData action4 = ActionMatchingUtil.loadActionDataFromFile(Constant.BASE_PATH + "/ActionData/1517658315751/", 240, 280);


            MyActionTraining training = new MyActionTraining();
            training.addActions(action1, action2, action3, action4);

            return training;
        }
    }
}
