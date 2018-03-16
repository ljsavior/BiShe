using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Utils
{
    using Data;
    using Microsoft.Kinect;

    class ActionRecognitionUtil
    {
        /// 计算两个向量的余弦相似度
        public static double computeCosineSimilarity(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            //分子
            double numerator = x1 * x2 + y1 * y2 + z1 * z2;
            //分母
            double denominator = Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1) * Math.Sqrt(x2 * x2 + y2 * y2 + z2 * z2);

            return numerator / denominator;
        }

        public static double[][] computeVectors(Skeleton skeleton)
        {
            Joint joint0 = skeleton.Joints[JointType.ShoulderLeft];
            Joint joint1 = skeleton.Joints[JointType.ElbowLeft];
            double[] vectorShoulderElbowLeft = computeVector(joint0, joint1);

            joint0 = skeleton.Joints[JointType.ElbowLeft];
            joint1 = skeleton.Joints[JointType.WristLeft];
            double[] vectorElbowWristLeft = computeVector(joint0, joint1);

            joint0 = skeleton.Joints[JointType.ShoulderRight];
            joint1 = skeleton.Joints[JointType.ElbowRight];
            double[] vectorShoulderElbowRight = computeVector(joint0, joint1);

            joint0 = skeleton.Joints[JointType.ElbowRight];
            joint1 = skeleton.Joints[JointType.WristRight];
            double[] vectorElbowWristRight = computeVector(joint0, joint1);


            return new double[][] { vectorShoulderElbowLeft, vectorElbowWristLeft, vectorShoulderElbowRight, vectorElbowWristRight };
        }

        private static double[] computeVector(Joint joint0, Joint joint1)
        {
            return new double[] { joint1.Position.X - joint0.Position.X, joint1.Position.Y - joint0.Position.Y, joint1.Position.Z - joint0.Position.Z };
        }

        public static double[][] computeVectors(MySkeleton skeleton)
        {
            MyJoint joint0 = skeleton.Joints[JointType.ShoulderLeft];
            MyJoint joint1 = skeleton.Joints[JointType.ElbowLeft];
            double[] vectorShoulderElbowLeft = computeVector(joint0, joint1);

            joint0 = skeleton.Joints[JointType.ElbowLeft];
            joint1 = skeleton.Joints[JointType.WristLeft];
            double[] vectorElbowWristLeft = computeVector(joint0, joint1);

            joint0 = skeleton.Joints[JointType.ShoulderRight];
            joint1 = skeleton.Joints[JointType.ElbowRight];
            double[] vectorShoulderElbowRight = computeVector(joint0, joint1);

            joint0 = skeleton.Joints[JointType.ElbowRight];
            joint1 = skeleton.Joints[JointType.WristRight];
            double[] vectorElbowWristRight = computeVector(joint0, joint1);


            return new double[][] { vectorShoulderElbowLeft, vectorElbowWristLeft, vectorShoulderElbowRight, vectorElbowWristRight };
        }

        private static double[] computeVector(MyJoint joint0, MyJoint joint1)
        {
            return new double[] { joint1.Position.X - joint0.Position.X, joint1.Position.Y - joint0.Position.Y, joint1.Position.Z - joint0.Position.Z };
        }


        public static double computeSimilarity(MySkeleton skel1, MySkeleton skel2)
        {
            double[][] vector1 = computeVectors(skel1);
            double[][] vector2 = computeVectors(skel2);

            double similarity = 0;
            for(int i = 0; i < vector1.Length; i++)
            {
                similarity += (1 - computeCosineSimilarity(vector1[i][0], vector1[i][1], vector1[i][2], vector2[i][0], vector2[i][1], vector2[i][2]));
            }

            return similarity / 4;
        }

        private static List<MySkeleton> preDeal(List<MySkeleton> list)
        {
            //list = meanFilter(list);
            //list = keySkeleton(list);

            return list;
        }

        public static List<MySkeleton> meanFilter(List<MySkeleton> list)
        {
            int N = 5;

            int d = (N - 1) / 2;
            List<MySkeleton> result = new List<MySkeleton>();
            
            for(int i = 0; i < list.Count; i++)
            {
                int start = Math.Max(0, i - d), end = Math.Min(list.Count - 1, i + d), count = end - start + 1;
                Dictionary<JointType, float[]> data = new Dictionary<JointType, float[]>();
                for(int j = start; j <= end; j++)
                {
                    foreach(KeyValuePair<JointType, MyJoint> pair in list[j].Joints)
                    {
                        if(!data.ContainsKey(pair.Key))
                        {
                            data[pair.Key] = new float[] {pair.Value.Position.X, pair.Value.Position.Y, pair.Value.Position.Z};
                        } else
                        {
                            data[pair.Key][0] += pair.Value.Position.X;
                            data[pair.Key][1] += pair.Value.Position.Y;
                            data[pair.Key][2] += pair.Value.Position.Z;
                        }
                    }
                }

                MySkeleton skel = new MySkeleton();
                foreach(KeyValuePair<JointType, float[]> pair in data)
                {
                    SkeletonPoint point = new SkeletonPoint();
                    point.X = data[pair.Key][0] / count;
                    point.Y = data[pair.Key][1] / count;
                    point.Z = data[pair.Key][2] / count;
                    skel.Joints[pair.Key] = new MyJoint(list[i].Joints[pair.Key].JointType, point, list[i].Joints[pair.Key].TrackingState);
                }
                result.Add(skel);
            }

            return result;
        }

        public static List<MySkeleton> keySkeleton(List<MySkeleton> list)
        {
            double threshold = 0.98;

            List<MySkeleton> result = new List<MySkeleton>();
            int preIndex = 0;
            double[][] vPre = computeVectors(list[preIndex]);
            result.Add(list[preIndex]);

            StringBuilder sb = new StringBuilder();
            sb.Append(1);
            for (int i = 1; i < list.Count; i++)
            {
                double[][] v = computeVectors(list[i]);
                for(int j = 0; j < vPre.Length; j++)
                {
                    double similarity = computeCosineSimilarity(vPre[j][0], vPre[j][1], vPre[j][2], v[j][0], v[j][1], v[j][2]);
                    if(similarity < threshold)
                    {
                        preIndex = i;
                        vPre = v;
                        result.Add(list[i]);
                        sb.Append(",").Append(i);
                        break;
                    }
                }
            }

            LogUtil.log("key skeleton:" + sb.ToString());
            return result;
        }

        public static double[,] computeSimilarityMatrix(MyActionSegmentData action1, MyActionSegmentData action2)
        {
            List<MySkeleton> skelList1 = preDeal(action1.SkeletonList);
            List<MySkeleton> skelList2 = preDeal(action2.SkeletonList);

            int row = skelList1.Count;
            int col = skelList2.Count;

            double[,] matrix = new double[row, col];

            for(int i = 0; i < row; i++)
            {
                for(int j = 0; j < col; j++)
                {
                    matrix[i, j] = computeSimilarity(skelList1[i], skelList2[j]);
                }
            }

            return matrix;
        }

        
    }
}
