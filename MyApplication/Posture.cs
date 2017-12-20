﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Posture
{
    using Microsoft.Kinect;

    class PostureRecognition
    {
        /// <summary>
        /// 余弦相似度阈值，当两向量的余弦相似度不小于此值时，两余弦匹配
        /// </summary>
        private const double THRESHOLD = 0.8;

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
            //分子
            double numerator = v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
            //分母
            double denominator = Math.Sqrt(v1.X * v1.X + v1.Y * v1.Y + v1.Z * v1.Z) * Math.Sqrt(v2.X * v2.X + v2.Y * v2.Y + v2.Z * v2.Z);

            return numerator / denominator;
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

            List<VectorType> vectorTypeList = getVectorTypeList(postureType);

            foreach (VectorType index in vectorTypeList)
            {
                double cosineSimilarity = computeCosineSimilarity(pos.getVector(index), posture.getVector(index));
                if(cosineSimilarity < THRESHOLD)
                {
                    return false;
                }
            }

            return true;
        }
    }


    class Posture
    {
        private PostureType type;
        private Vector[] vectors = new Vector[4];


        public Posture(PostureType type)
        {
            this.type = type;
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

        internal PostureType Type
        {
            get
            {
                return type;
            }
        }

    }


    enum PostureType
    {
        LeftArm,
        RightArm,
        Both
    }


    enum VectorType
    {
        ShoulderElbowLeft = 0,
        ElbowWristLeft = 1,
        ShoulderElbowRight = 2,
        ElbowWristRight = 3
    }


    class Vector
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
