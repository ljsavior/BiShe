using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApplication.Data
{
    using Microsoft.Kinect;
    class MySkeleton
    {
        public Dictionary<JointType, MyJoint> Joints { get; set; }

        public MySkeleton()
        {
            this.Joints = new Dictionary<JointType, MyJoint>();
        }
    }

    struct MyJoint
    {
        public JointType JointType { get; set; }

        public SkeletonPoint Position { get; set; }

        public JointTrackingState TrackingState { get; set; }


        public MyJoint(JointType jointType, SkeletonPoint position, JointTrackingState trackingState)
        {
            this.JointType = jointType;
            this.Position = position;
            this.TrackingState = trackingState;
        }


    }
}
