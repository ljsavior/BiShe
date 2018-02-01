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
    /// <summary>
    /// Interaction logic for ActionTrainingWindow.xaml
    /// </summary>
    public partial class ActionTrainingWindow : Window
    {
        public ActionTrainingWindow()
        {
            InitializeComponent();

            this.Loaded += (s, e) => {
                
            };

            this.Unloaded += (s, e) => {
            };
        }
    }
}
