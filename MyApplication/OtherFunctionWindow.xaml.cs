using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyApplication.MyWindow
{
    using Microsoft.Kinect;
    using System.IO;
    using System.Threading;
    using Utils;
    using Data;
    using com.force.json;


    /// <summary>
    /// OtherFunctionWindow.xaml 的交互逻辑
    /// </summary>
    public partial class OtherFunctionWindow : Window
    {
        public OtherFunctionWindow()
        {
            InitializeComponent();
        }

        private void DrawSkeleton(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择动作数据文件夹路径";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string foldPath = dialog.SelectedPath;
                String dataFilePath = foldPath + "/skeletonData.txt";
                String skeletonImageFoldPath = foldPath + "/skeletonFrame";

                if(!File.Exists(dataFilePath))
                {
                    MessageBox.Show("文件 " + dataFilePath + " 不存在.");
                    return;
                }
                if(!Directory.Exists(skeletonImageFoldPath))
                {
                    Directory.CreateDirectory(skeletonImageFoldPath);
                }

                Thread thread = new Thread(() => {
                    using (StreamReader sr = new StreamReader(dataFilePath))
                    {
                        String line;
                        while((line = sr.ReadLine()) != null)
                        {
                            MySkeleton skeleton = CommonUtil.jsonStrToMySkeleton(line);
                            
                        }
                    }
                    Dispatcher.Invoke(() => MessageBox.Show("图片绘制完成"));
                });
                thread.IsBackground = false;
                thread.Start();

            }

        }

        private void ActionRecognization(object sender, RoutedEventArgs e)
        {
            MyActionSegmentData action1 = new MyActionSegmentData("d:/MyApplication/ActionData/1517659759365/skeletonData.txt", 69, 107, 1);
            MyActionSegmentData action2 = new MyActionSegmentData("d:/MyApplication/ActionData/1517659759365/skeletonData.txt", 140, 181, 1);
            MyActionSegmentData action3 = new MyActionSegmentData("d:/MyApplication/ActionData/1517659759365/skeletonData.txt", 211, 278, 1);
            MyActionSegmentData action4 = new MyActionSegmentData("d:/MyApplication/ActionData/1517659759365/skeletonData.txt", 695, 734, 1);
            MyActionSegmentData action5 = new MyActionSegmentData("d:/MyApplication/ActionData/1517658799185/skeletonData.txt", 70, 115, 1);
            MyActionSegmentData action6 = new MyActionSegmentData("d:/MyApplication/ActionData/1517659759365/skeletonData.txt", 103, 132, 1);

            deal(action1, action2, action3, action4, action5, action6);
        }

        private void deal(params MyActionSegmentData[] actions)
        {
            try
            {
                if (actions.Length < 2)
                {
                    return;
                }
                for (int i = 1; i < actions.Length; i++)
                {
                    double[,] similarityMatrix = ActionRecognitionUtil.computeSimilarityMatrix(actions[0], actions[i]);
                    DTWResult dtwResult = ActionRecognitionUtil.DTW(similarityMatrix);
                    StringBuilder sb = new StringBuilder();
                    sb.Append("ActionRecognization\n");
                    sb.Append(actions[0]).Append('\n').Append(actions[i]).Append('\n');
                    sb.Append("result:\n");
                    sb.Append("Similarity:").Append(dtwResult.SumSimilarity).Append(", AvgSimilarity:").Append(dtwResult.AvgSimilarity).Append(", PathLength:").Append(dtwResult.PathLength).Append('\n');
                    sb.Append(dtwResult.PathStr);

                    LogUtil.log(sb.ToString());
                }
            } catch(Exception e)
            {
                LogUtil.log(e.Message);
            }
            
        }

        private void doTest(object sender, RoutedEventArgs e)
        {
            MyActionSegmentData action = new MyActionSegmentData("d:/MyApplication/ActionData/1517659759365/skeletonData.txt", 69, 107, 1);
            List<MySkeleton> skeletonList = action.SkeletonList;

            skeletonList = ActionRecognitionUtil.meanFilter(skeletonList);
            skeletonList = ActionRecognitionUtil.keySkeleton(skeletonList);


            StringBuilder sb = new StringBuilder();
            foreach(var skel in skeletonList)
            {
                foreach (var pair in skel.Joints)
                {
                    sb.Append(pair.Value.Position.X).Append(' ')
                        .Append(pair.Value.Position.Y).Append(' ')
                        .Append(pair.Value.Position.Z).Append(' ');
                }
                sb[sb.Length - 1] = '\n';
            }
            
            //LogUtil.log(sb.ToString());
            


        }
    }
}
