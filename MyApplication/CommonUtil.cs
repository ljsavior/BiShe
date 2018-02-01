using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Utils
{
    using Microsoft.Kinect;
    using com.force.json;

    class CommonUtil
    {
        public static Dictionary<JointType, String> jointTypeNameMap = new Dictionary<JointType, string>();
        public static Dictionary<JointTrackingState, String> jointTrackingStateNameMap = new Dictionary<JointTrackingState, string>();

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

    }
}
