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

namespace MyApplication.MyPage
{
    using MyWindow;

    /// <summary>
    /// LoginPage.xaml 的交互逻辑
    /// </summary>
    public partial class LoginPage : Page
    {
        private LoginWindow loginWindow;

        public LoginPage(LoginWindow loginWindow)
        {
            InitializeComponent();
            this.loginWindow = loginWindow;
        }

        private void do_Login(object sender, RoutedEventArgs e)
        {
            String username = Text_Username.Text;
            String password = Password_Password.Password;

            if(username.Length == 0)
            {
                MessageBox.Show("请输入用户名");
                return;
            }

            bool loginStatus = Service.LoginService.getInstance().login(username, password);
            MessageBox.Show(loginStatus ? "登录成功" : "登录失败!用户名或密码错误.");
            if(loginStatus)
            {
                loginWindow.Close();
            }
        }
    }
}
