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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyApplication
{
    using System.Windows;
    using MyWindow;

    using System.Diagnostics;

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private TrainingWindow trainingWindow;


        public MainWindow()
        {
            InitializeComponent();
        }


        private void open_TrainingWindow(object sender, RoutedEventArgs e)
        {
            trainingWindow = new TrainingWindow();
            trainingWindow.Show();
        }

        private void start_Simulation(object sender, RoutedEventArgs e)
        {
            Process p = Process.Start(Constant.SIMLUATION_EXE_PATH);
        }

        private void open_LoginWindow(object sender, RoutedEventArgs e)
        {
            new LoginWindow().Show();
        }

        private void open_PostureCollectWindow(object sender, RoutedEventArgs e)
        {
            new PostureCollectWindow().Show();
        }

        private void open_DataCollectWindow(object sender, RoutedEventArgs e)
        {
            Process p = Process.Start(Constant.DATA_COLLECT_EXE_PATH);
        }

        private void open_DataProducerWindow(object sender, RoutedEventArgs e)
        {
            new DataProducerWindow().Show();
        }



        private void do_Test(object sender, RoutedEventArgs e)
        {
            Service.Service service = new Service.Service();
            String[] names = service.getTrainingNames();
            foreach (String name in names)
            {
                Utils.LogUtil.log(name);
            }


        }
        
    }


}
