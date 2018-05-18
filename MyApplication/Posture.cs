using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace MyApplication.Posture
{
    using Microsoft.Kinect;
    using com.force.json;

    public class PostureRecognition
    {
        /// <summary>
        /// 余弦相似度阈值，当两向量的余弦相似度不小于此值时，两余弦匹配
        /// </summary>
        private const double THRESHOLD = 0.9;


        private static List<VectorType> leftArmVectorTypeList = new List<VectorType>(2);
        private static List<VectorType> rightArmVectorTypeList = new List<VectorType>(2);
        private static List<VectorType> bothArmVectorTypeList = new List<VectorType>(4);

        static PostureRecognition()
        {
            leftArmVectorTypeList.Add(VectorType.ShoulderElbowLeft);
            leftArmVectorTypeList.Add(VectorType.ElbowWristLeft);

            rightArmVectorTypeList.Add(VectorType.ShoulderElbowRight);
            rightArmVectorTypeList.Add(VectorType.ElbowWristRight);

            bothArmVectorTypeList.Add(VectorType.ShoulderElbowLeft);
            bothArmVectorTypeList.Add(VectorType.ElbowWristLeft);
            bothArmVectorTypeList.Add(VectorType.ShoulderElbowRight);
            bothArmVectorTypeList.Add(VectorType.ElbowWristRight);
        }


        /// <summary>
        ///  计算两个向量的余弦相似度
        /// </summary>
        /// <param name="v1">向量1</param>
        /// <param name="v2">向量2</param>
        /// <returns></returns>
        private static double computeCosineSimilarity(Vector v1, Vector v2)
        {
            return Utils.ActionRecognitionUtil.computeCosineSimilarity(v1.X, v1.Y, v1.Z, v2.X, v2.Y, v2.Z);
        }

        /// <summary>
        /// 获取制定姿势类型对应的向量类型list，采用静态变量，减少创建对象的次数
        /// </summary>
        /// <param name="postureType">姿势类型（左臂、右臂、双臂）</param>
        /// <returns></returns>
        public static List<VectorType> getVectorTypeList(PostureType postureType)
        {
            if (postureType == PostureType.LeftArm)
            {
                return leftArmVectorTypeList;
            }
            else if (postureType == PostureType.RightArm)
            {
                return rightArmVectorTypeList;
            }
            else if(postureType == PostureType.Both)
            {
                return bothArmVectorTypeList;
            }
            throw new Exception("postureType is illegal.");
        }

        /// <summary>
        /// 计算指定骨骼在指定姿势类型下，对应的姿势
        /// </summary>
        /// <param name="skeleton">骨骼数据</param>
        /// <param name="postureType">姿势类型（左臂、右臂、双臂）</param>
        /// <returns></returns>
        public static Posture computePosture(Skeleton skeleton, PostureType postureType)
        {
            Posture posture = new Posture(postureType);

            List<VectorType> vectorTypeList = getVectorTypeList(postureType);

            foreach (VectorType vectorType in vectorTypeList)
            {
                JointType jointType0, jointType1;
                switch(vectorType)
                {
                    case VectorType.ShoulderElbowLeft:
                        jointType0 = JointType.ShoulderLeft;
                        jointType1 = JointType.ElbowLeft;
                        break;
                    case VectorType.ElbowWristLeft:
                        jointType0 = JointType.ElbowLeft;
                        jointType1 = JointType.WristLeft;
                        break;
                    case VectorType.ShoulderElbowRight:
                        jointType0 = JointType.ShoulderRight;
                        jointType1 = JointType.ElbowRight;
                        break;
                    case VectorType.ElbowWristRight:
                        jointType0 = JointType.ElbowRight;
                        jointType1 = JointType.WristRight;
                        break;
                    default:
                        throw new Exception("vectorType is illegal.");
                }

                Joint joint0 = skeleton.Joints[jointType0];
                Joint joint1 = skeleton.Joints[jointType1];

                Vector vector = new Vector(joint1.Position.X - joint0.Position.X, joint1.Position.Y - joint0.Position.Y, joint1.Position.Z - joint0.Position.Z);
                posture.setVector(vectorType, vector);
            }

            return posture;
        }

        /// <summary>
        /// 判断指定骨骼数据是否与指定姿势匹配
        /// </summary>
        /// <param name="skeleton">骨骼数据</param>
        /// <param name="posture">姿势</param>
        /// <returns></returns>
        public static bool matches(Skeleton skeleton, Posture posture)
        {
            PostureType postureType = posture.Type;
            Posture pos = computePosture(skeleton, postureType);

            return matches(pos, posture);
        }

        public static bool matches(Posture pos1, Posture pos2)
        {
            if (pos1.Type != pos2.Type)
            {
                return false;
            }

            List<VectorType> vectorTypeList = getVectorTypeList(pos1.Type);

            foreach (VectorType index in vectorTypeList)
            {
                double cosineSimilarity = computeCosineSimilarity(pos1.getVector(index), pos2.getVector(index));
                if (cosineSimilarity < THRESHOLD)
                {
                    return false;
                }
            }

            return true;
        }
    }


    public class Posture
    {
        private static Dictionary<VectorType, String> vectorTypeMap = new Dictionary<VectorType, String>();

        private PostureType type;
        private Vector[] vectors = new Vector[4];
        private BitmapImage pic;

        static Posture()
        {
            vectorTypeMap.Add(VectorType.ShoulderElbowLeft, "ShoulderElbowLeft");
            vectorTypeMap.Add(VectorType.ElbowWristLeft, "ElbowWristLeft");
            vectorTypeMap.Add(VectorType.ShoulderElbowRight, "ShoulderElbowRight");
            vectorTypeMap.Add(VectorType.ElbowWristRight, "ElbowWristRight");
        }


        public Posture(PostureType type)
        {
            this.type = type;
        }

        public Posture(PostureType type, double[][] vectors)
        {
            this.type = type;
            List<VectorType> types = PostureRecognition.getVectorTypeList(type);
            for(int i = 0; i < types.Count; i++)
            {
                setVector(types[i], new Vector(vectors[i][0], vectors[i][1], vectors[i][2]));
            }
        }

        public Posture(PostureType type, double[][] vectors, String picPath) : this(type, vectors)
        {
            setPic(picPath);
        }


        public void setVector(VectorType vectorType, Vector vector)
        {
            int vectorIndex = (int)vectorType;
            vectors[vectorIndex] = vector;
        }

        public Vector getVector(VectorType vectorType)
        {
            int vectorIndex = (int)vectorType;
            return vectors[vectorIndex];
        }

        public void setPic(String picPath)
        {
            this.pic = new BitmapImage(new Uri(picPath));
        }

        public BitmapImage getPic()
        {
            return this.pic;
        }

        internal PostureType Type
        {
            get
            {
                return type;
            }
        }

        public String toJsonString()
        {
            JSONObject jsonObject = new JSONObject();
            List<VectorType> typeList = PostureRecognition.getVectorTypeList(this.type);
            foreach(VectorType t in typeList)
            {
                Vector v = getVector(t);
                if(v != null)
                {
                    JSONArray jsonArray = new JSONArray();
                    jsonArray.Put(v.X);
                    jsonArray.Put(v.Y);
                    jsonArray.Put(v.Z);
                    jsonObject.Put(vectorTypeMap[t], jsonArray);
                }
            }
            return jsonObject.ToString();
        }

    }


    public enum PostureType
    {
        Both = 0,
        LeftArm = 1,
        RightArm = 2
    }


    public enum VectorType
    {
        ShoulderElbowLeft = 0,
        ElbowWristLeft = 1,
        ShoulderElbowRight = 2,
        ElbowWristRight = 3
    }


    public class Vector
    {
        private double x;
        private double y;
        private double z;

        public Vector(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public double X
        {
            get
            {
                return x;
            }

            set
            {
                x = value;
            }
        }

        public double Y
        {
            get
            {
                return y;
            }

            set
            {
                y = value;
            }
        }

        public double Z
        {
            get
            {
                return z;
            }

            set
            {
                z = value;
            }
        }
    }

}
