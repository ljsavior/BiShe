using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Utils
{
    using Microsoft.Kinect;
    using com.force.json;
    using Data;
    using System.Drawing;

    class CommonUtil
    {
        public static Dictionary<JointType, String> jointTypeNameMap = new Dictionary<JointType, string>();
        public static Dictionary<JointTrackingState, String> jointTrackingStateNameMap = new Dictionary<JointTrackingState, string>();

        public static Dictionary<String, JointType> jointTypeNameReverseMap = new Dictionary<string, JointType>();
        public static Dictionary<String, JointTrackingState> jointTrackingStateNameReverseMap = new Dictionary<string, JointTrackingState>();

        static CommonUtil()
        {
            jointTypeNameMap.Add(JointType.HipCenter, "HipCenter");
            jointTypeNameMap.Add(JointType.Spine, "Spine");
            jointTypeNameMap.Add(JointType.ShoulderCenter, "ShoulderCenter");
            jointTypeNameMap.Add(JointType.Head, "Head");
            jointTypeNameMap.Add(JointType.ShoulderLeft, "ShoulderLeft");
            jointTypeNameMap.Add(JointType.ElbowLeft, "ElbowLeft");
            jointTypeNameMap.Add(JointType.WristLeft, "WristLeft");
            jointTypeNameMap.Add(JointType.HandLeft, "HandLeft");
            jointTypeNameMap.Add(JointType.ShoulderRight, "ShoulderRight");
            jointTypeNameMap.Add(JointType.ElbowRight, "ElbowRight");
            jointTypeNameMap.Add(JointType.WristRight, "WristRight");
            jointTypeNameMap.Add(JointType.HandRight, "HandRight");
            jointTypeNameMap.Add(JointType.HipLeft, "HipLeft");
            jointTypeNameMap.Add(JointType.KneeLeft, "KneeLeft");
            jointTypeNameMap.Add(JointType.AnkleLeft, "AnkleLeft");
            jointTypeNameMap.Add(JointType.FootLeft, "FootLeft");
            jointTypeNameMap.Add(JointType.HipRight, "HipRight");
            jointTypeNameMap.Add(JointType.KneeRight, "KneeRight");
            jointTypeNameMap.Add(JointType.AnkleRight, "AnkleRight");
            jointTypeNameMap.Add(JointType.FootRight, "FootRight");


            jointTrackingStateNameMap.Add(JointTrackingState.NotTracked, "NotTracked");
            jointTrackingStateNameMap.Add(JointTrackingState.Inferred, "Inferred");
            jointTrackingStateNameMap.Add(JointTrackingState.Tracked, "Tracked");


            jointTypeNameReverseMap.Add("HipCenter", JointType.HipCenter);
            jointTypeNameReverseMap.Add("Spine", JointType.Spine);
            jointTypeNameReverseMap.Add("ShoulderCenter", JointType.ShoulderCenter);
            jointTypeNameReverseMap.Add("Head", JointType.Head);
            jointTypeNameReverseMap.Add("ShoulderLeft", JointType.ShoulderLeft);
            jointTypeNameReverseMap.Add("ElbowLeft", JointType.ElbowLeft);
            jointTypeNameReverseMap.Add("WristLeft", JointType.WristLeft);
            jointTypeNameReverseMap.Add("HandLeft", JointType.HandLeft);
            jointTypeNameReverseMap.Add("ShoulderRight", JointType.ShoulderRight);
            jointTypeNameReverseMap.Add("ElbowRight", JointType.ElbowRight);
            jointTypeNameReverseMap.Add("WristRight", JointType.WristRight);
            jointTypeNameReverseMap.Add("HandRight", JointType.HandRight);
            jointTypeNameReverseMap.Add("HipLeft", JointType.HipLeft);
            jointTypeNameReverseMap.Add("KneeLeft", JointType.KneeLeft);
            jointTypeNameReverseMap.Add("AnkleLeft", JointType.AnkleLeft);
            jointTypeNameReverseMap.Add("FootLeft", JointType.FootLeft);
            jointTypeNameReverseMap.Add("HipRight", JointType.HipRight);
            jointTypeNameReverseMap.Add("KneeRight", JointType.KneeRight);
            jointTypeNameReverseMap.Add("AnkleRight", JointType.AnkleRight);
            jointTypeNameReverseMap.Add("FootRight", JointType.FootRight);

            jointTrackingStateNameReverseMap.Add("NotTracked", JointTrackingState.NotTracked);
            jointTrackingStateNameReverseMap.Add("Inferred", JointTrackingState.Inferred);
            jointTrackingStateNameReverseMap.Add("Tracked", JointTrackingState.Tracked);
        }


        public static String arrayToString(double[] array)
        {
            return String.Format("[{0}]", string.Join(",", array));
        }

        public static String arrayToString(double[][] array)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (double[] a in array)
            {
                sb.Append(arrayToString(a)).Append(",");
            }
            if(sb.Length > 1)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            sb.Append("]");
            return sb.ToString();
        }

        public static int[] stringToIntArray1(String str)
        {
            if (str == null || !str.StartsWith("[") || !str.EndsWith("]"))
            {
                throw new Exception("str格式错误.str=" + str);
            }
            str = str.Substring(1, str.Length - 2);
            String[] strs = str.Split(',');
            int[] res = new int[strs.Length];
            for (int i = 0; i < strs.Length; i++)
            {
                res[i] = int.Parse(strs[i]);
            }
            return res;
        }

        public static double[] stringToDoubleArray1(String str)
        {
            if (str == null || !str.StartsWith("[") || !str.EndsWith("]"))
            {
                throw new Exception("str格式错误.str=" + str);
            }
            str = str.Substring(1, str.Length - 2);
            String[] strs = str.Split(',');
            double[] res = new double[strs.Length];
            for(int i = 0; i < strs.Length; i++)
            {
                res[i] = double.Parse(strs[i]);
            }
            return res;
        }

        public static double[][] stringToDoubleArray2(String str)
        {

            List<String> strList = splitArray2Str(str);
            double[][] res = new double[strList.Count][];
            for(int i = 0; i < strList.Count; i++)
            {
                res[i] = stringToDoubleArray1(strList[i]);
            }
            return res;
        }

        private static List<String> splitArray2Str(String str)
        {
            if (str == null || !str.StartsWith("[[") || !str.EndsWith("]]"))
            {
                throw new Exception("str格式错误.str=" + str);
            }
            str = str.Substring(1, str.Length - 2);

            List<String> strs = new List<string>();
            int begin = 0, end = 1, len = str.Length;
            while(end <= len)
            {
                if((end == len || str[end] == ',') && str[end - 1] == ']')
                {
                    strs.Add(str.Substring(begin, end - begin));
                    begin = ++end;
                }
                end++;
            }
            return strs;
        }


        public static long currentTimeMillis()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds);
        }

        public static String skeletonDataToString(Skeleton skeleton)
        {
            JSONArray jsonArray = new JSONArray();

            foreach(Joint joint in skeleton.Joints)
            {
                JSONObject obj = jointToJSONObject(joint);
                jsonArray.Put(obj);
            }

            return jsonArray.ToString();
        }


        private static JSONObject jointToJSONObject(Joint joint)
        {
            JSONObject obj = new JSONObject();

            JSONArray array = new JSONArray();
            array.Put(joint.Position.X);
            array.Put(joint.Position.Y);
            array.Put(joint.Position.Z);

            obj.Put("JointType", jointTypeNameMap[joint.JointType]);
            obj.Put("TrackingState", jointTrackingStateNameMap[joint.TrackingState]);
            obj.Put("Position", array);

            return obj;
        }

        public static MySkeleton jsonStrToMySkeleton(String jsonStr)
        {
            MySkeleton skel = new MySkeleton();

            JSONArray array = new JSONArray(jsonStr);
            for(int i = 0; i < array.Length(); i++)
            {
                JSONObject jointObj = array.GetJSONObject(i);
                JSONArray positionArray = jointObj.GetJSONArray("Position");

                JointType jointType = jointTypeNameReverseMap[jointObj.GetString("JointType")];
                JointTrackingState trackingState = jointTrackingStateNameReverseMap[jointObj.GetString("TrackingState")];
                SkeletonPoint position = new SkeletonPoint();
                position.X = float.Parse(positionArray.GetString(0));
                position.Y = float.Parse(positionArray.GetString(1));
                position.Z = float.Parse(positionArray.GetString(2));

                skel.Joints[jointType] = new MyJoint(jointType, position, trackingState);
            }

            return skel;
        }

    }
}
