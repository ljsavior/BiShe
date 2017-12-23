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


        private void open_Training_Window(object sender, RoutedEventArgs e)
        {
            trainingWindow = new TrainingWindow();
            trainingWindow.Show();
        }

        private void start_Simulation(object sender, RoutedEventArgs e)
        {
            Process p = Process.Start(Constant.SIMLUATION_EXE_PATH);
        }
    }


}
