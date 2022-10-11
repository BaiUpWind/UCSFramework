using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using CommonApi.Utilitys.Encryption;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;

namespace EncryptTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowInfo("启动"); 
            cmbEncryptType.ItemsSource = Enum.GetNames(typeof(EncryptType)); 
            cmbEncryptType.SelectionChanged += (s, e) =>
            {
                selectedType = (EncryptType)cmbEncryptType.SelectedIndex; 
                ShowInfo($"当前选择加密方式为'{selectedType}'");

                switch (selectedType)
                {
                    case EncryptType.SHA1: 
                    case EncryptType.MD5:
                        btnDecrypt.IsEnabled = false;
                        //btnDecrypt.ToolTip = "此加密方式为不可逆！";
                        break;
                    case EncryptType.DES: 
                    case EncryptType.Base64:
                        btnDecrypt.IsEnabled = true;
                        //btnDecrypt.ToolTip = "对密文进行解密";
                        break; 
                }
            };
            cmbEncryptType.SelectedIndex = 0;

            rtxtOgrin.Document.LineHeight = 1;
            rtxtResult.Document.LineHeight = 1;
        }
        private EncryptType selectedType;

        private string GetEncryptString(EncryptType type,string str,bool enORdE)
        {
            string result = string.Empty;
            switch (type)
            {
                case EncryptType.SHA1: 
                    return Cryptography.SHA1Encrypt(str); 
                case EncryptType.MD5: 
                    return Cryptography.MD5Encrypt(str);
                case EncryptType.DES: 
                    if (enORdE)
                    {
                        return Cryptography.DESEncrypt(str);
                    }
                    else
                    {
                        return Cryptography.DESDecrypt(str);
                    } 
                case EncryptType.Base64:
                    if (enORdE)
                    {
                        return Cryptography.Base64Encrypt(str);
                    }
                    else
                    {
                        return Cryptography.Base64Decrypt(str);
                    }
                default:
                    return string.Empty;
            }
        }

        private void ShowInfo(string info ,int type =0)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                ListBoxItem item = new ListBoxItem();
                item.Content =DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+":"+ info;
                if(type == 1)
                {
                    item.Background = new SolidColorBrush(Colors.Red);
                }
                else if(type == 2)
                {
                    item.Background = new SolidColorBrush(Colors.GreenYellow);
                }

                listInfo.Items.Insert(0, item); 
            }));
        }


        private void Btn_Mini(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Btn_Max(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void Btn_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Btn_EnOrDecrypt(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn)
            {
                bool enorde = btn.Content.ToString() == "加密";
                TextRange textRange = new TextRange(rtxtOgrin.Document.ContentStart, rtxtOgrin.Document.ContentEnd);
                var txt = textRange.Text;
                if (enorde)
                {
                    if (!(bool)cbIncludeLineFeed.IsChecked)
                    { 
                       txt = txt.Replace("\r\n", "").Replace("\n\r", "");
                    }
                    if (!(bool)cbIncludeEmpty.IsChecked)
                    {
                        txt = txt.Trim();
                        if (string.IsNullOrWhiteSpace(txt))
                        {
                            ShowInfo("在没有勾选包含空格的情况下依然进行加密，加密失败!", 1);
                            return;
                        }
                    } 
                }
                else
                {
                    if (string.IsNullOrEmpty(txt))
                    {
                        ShowInfo("尝试将空内容进行解密，解密失败！", 2);
                    }
                }
                //else
                //{
                //    txt = txt.Replace("\r\n","");
                //} 
               

                var result= GetEncryptString(selectedType, txt, enorde);
                rtxtResult.Document.Blocks.Clear();
                Paragraph paragraph = new Paragraph();
                paragraph.Inlines.Add(result);
                rtxtResult.Document.Blocks.Add(paragraph); 
            }
        }


       

        //清空
        private void Btn_Clear(object sender, RoutedEventArgs e)
        {
            rtxtOgrin.Document.Blocks.Clear();
            rtxtResult.Document.Blocks.Clear();
        }
    }
}
