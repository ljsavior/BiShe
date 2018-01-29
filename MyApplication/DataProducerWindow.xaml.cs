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
    using MyPage;

    /// <summary>
    /// Interaction logic for DataProducerWindow.xaml
    /// </summary>
    public partial class DataProducerWindow : Window
    {
        public DataProducerWindow()
        {
            InitializeComponent();
            showPage(new DataProducerPage());
        }


        public void showPage(Page page)
        {
            this.frame.Navigate(page);
        }
    }
}
