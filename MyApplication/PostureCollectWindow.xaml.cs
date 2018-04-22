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
    /// PostureCollectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PostureCollectWindow : Window
    {
        public PostureCollectWindow()
        {
            InitializeComponent();

            //showPage(new PostureCollectPage());
            showPage(new TemplateCollectPage());
        }

        public void showPage(Page page)
        {
            this.frame.Navigate(page);
        }
    }
}
