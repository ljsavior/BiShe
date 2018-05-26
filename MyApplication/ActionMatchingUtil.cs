using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace MyApplication.Utils
{
    using Data;
    using System.IO;
    using com.force.json;

    class ActionMatchingUtil
    {
        public static double[,] computeSimilarityMatrix(ActionData action1, ActionData action2)
        {
            List<double[][]> dataList1 = preDeal(action1.dataList);
            //List<double[][]> dataList2 = preDeal(action2.dataList);
            List<double[][]> dataList2 = action2.dataList;

            int row = dataList1.Count;
            int col = dataList2.Count;

            double[,] matrix = new double[row, col];

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    matrix[i, j] = computeSimilarity(dataList1[i], dataList2[j]);
                }
            }

            return matrix;
        }

        public static List<double[][]> preDeal(List<double[][]> dataList)
        {
            List<double[][]> res = new List<double[][]>(dataList.Count / 3 + 1);
            for(int i = 0; i < dataList.Count; i++)
            {
                if(i % 3 == 0)
                {
                    res.Add(dataList[i]);
                }
            }
            return res;
        }

        public static double computeSimilarity(double[][] p1, double[][] p2)
        {

            double similarity = 0;
            for (int i = 0; i < p1.Length; i++)
            {
                similarity += (1 - computeCosineSimilarity(p1[i][0], p1[i][1], p1[i][2], p2[i][0], p2[i][1], p2[i][2]));
            }

            return similarity / 4;
        }

        /// 计算两个向量的余弦相似度
        private static double computeCosineSimilarity(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            //分子
            double numerator = x1 * x2 + y1 * y2 + z1 * z2;
            //分母
            double denominator = Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1) * Math.Sqrt(x2 * x2 + y2 * y2 + z2 * z2);

            return numerator / denominator;
        }



        public static ActionData loadActionDataFromFile(string dirPath, int startNum, int endNum)
        {
            string dataFilePath = dirPath + "skeletonData.txt";
            int count = endNum - startNum + 1;
            ActionData actionData = new ActionData(count);

            if (!File.Exists(dataFilePath))
            {
                throw new Exception("文件[" + dataFilePath + "]不存在.");
            }
            using (StreamReader sr = new StreamReader(dataFilePath))
            {
                String line;
                for (int i = 1; i < startNum; i++)
                {
                    line = sr.ReadLine();
                    if (line == null)
                    {
                        throw new Exception("文件已读到结尾.row:" + i);
                    }
                }
                for (int i = 1; i <= count; i++)
                {
                    int lineNum = i + startNum - 1;
                    line = sr.ReadLine();
                    if (line == null)
                    {
                        throw new Exception("文件已读到结尾.row:" + lineNum);
                    }

                    Uri uri = new Uri(dirPath + "colorFrame/" + lineNum + ".jpg", UriKind.Absolute);
                    actionData.add(new BitmapImage(uri), computeVectors(line));
                }
            }

            return actionData;
        }

        public static ActionData loadActionData(List<double[][]> dataList)
        {
            return new ActionData(dataList);
        }

        private static double[][] computeVectors(String jsonStr)
        {
            HashSet<String> set = new HashSet<string>();
            set.Add("ShoulderLeft");
            set.Add("ElbowLeft");
            set.Add("WristLeft");
            set.Add("ShoulderRight");
            set.Add("ElbowRight");
            set.Add("WristRight");

            Dictionary<String, double[]> jointMap = new Dictionary<string, double[]>();

            JSONArray array = new JSONArray(jsonStr);
            for (int i = 0; i < array.Length(); i++)
            {
                JSONObject obj = array.GetJSONObject(i);
                String type = obj.GetString("JointType");
                if (set.Contains(type))
                {
                    JSONArray pos = obj.GetJSONArray("Position");
                    jointMap[type] = new double[] { double.Parse(pos.GetString(0)), double.Parse(pos.GetString(1)), double.Parse(pos.GetString(2)) };
                }
            }

            return new double[][] { computeVector(jointMap["ShoulderLeft"], jointMap["ElbowLeft"]),
                                    computeVector(jointMap["ElbowLeft"], jointMap["WristLeft"]),
                                    computeVector(jointMap["ShoulderRight"], jointMap["ElbowRight"]),
                                    computeVector(jointMap["ElbowRight"], jointMap["WristRight"]) };
        }

        private static double[] computeVector(double[] from, double[] to)
        {
            return new double[] { to[0] - from[0], to[1] - from[1], to[2] - from[2] };
        }

        public static bool match(ActionData action1, ActionData action2)
        {
            double[,] similarityMatrix = ActionMatchingUtil.computeSimilarityMatrix(action1, action2);
            DTWResult dtwResult = DTWUtil.DTW(similarityMatrix);


            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("result:\n");
            sb.Append("Similarity:").Append(dtwResult.SumSimilarity).Append(", AvgSimilarity:").Append(dtwResult.AvgSimilarity).Append(", PathLength:").Append(dtwResult.PathLength).Append('\n');
            sb.Append(dtwResult.PathStr);
            LogUtil.log(sb.ToString());


            return dtwResult.AvgSimilarity < 0.04;
        }
    }
}
