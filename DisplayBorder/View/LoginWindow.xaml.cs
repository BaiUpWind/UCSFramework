using CommonApi.Utilitys.Encryption;
using HandyControl.Controls;
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
using MessageBox = HandyControl.Controls.MessageBox;
using Window = System.Windows.Window;

namespace DisplayBorder.View
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            SetDefault(); 
        }
        public LoginWindow(EncryptType encryptType, string encryptStr)
        {
            InitializeComponent();
            EncryptString = encryptStr;
            EncryptType = encryptType;
            SetDefault();
        }
        private  string EncryptString;
        private readonly EncryptType EncryptType = EncryptType.SHA1;
        private void SetDefault()
        {
            if (string.IsNullOrWhiteSpace(EncryptString))
            {
                //默认密码都是123456的加密后的 密文
                switch (EncryptType)
                {
                    case EncryptType.SHA1:
                        EncryptString = "7c4a8d09ca3762af61e59520943dc26494f8941b";
                        break;
                    case EncryptType.MD5:
                        EncryptString = "E1ADC3949BA59ABBE56E057F2F883E";
                        break;
                    case EncryptType.DES:
                        EncryptString = "lpzlRMWity8=";
                        break;
                    case EncryptType.Base64:
                        EncryptString = "MTIzNDU2";
                        break;
                    default:
                        EncryptString = "7c4a8d09ca3762af61e59520943dc26494f8941b";
                        break;
                }
            }
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
       
        private void PinBox_Completed(object sender, RoutedEventArgs e)
        {
            if(sender is PinBox pb)
            {
         
                if (!Cryptography.ValiDate(EncryptType, EncryptString, pb.Password))
                { 
                    WindowHelper.WindowShake();
                }
                else
                {
                    DialogResult = true; 
                }
                pb.Password = string.Empty;

            }
        }

      
    }
}
