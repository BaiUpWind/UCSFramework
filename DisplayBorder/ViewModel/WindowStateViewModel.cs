using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DisplayBorder.ViewModel
{
    public class WindowStateViewModel : ViewModelBase
    { 
        public WindowStateViewModel( )
        { 
        }
  
        public RelayCommand MaximizeCmd => new RelayCommand(() =>
        {
             
            var window = HandyControl.Tools.WindowHelper.GetActiveWindow();
            if (window != null)
            {
                if (window.WindowState == WindowState.Maximized)
                {
                    window.WindowState = WindowState.Normal; 
                }
                else
                {
                    window.WindowState = WindowState.Maximized; 
                }
            }
        });

        public RelayCommand MinimizeCmd => new RelayCommand(() =>
        {
           

            var window = HandyControl.Tools.WindowHelper.GetActiveWindow();
            if (window != null)
            {
                window.WindowState = WindowState.Minimized;
            }
        });

        public RelayCommand CloseCmd => new RelayCommand(() =>
        {
           
            var window = HandyControl.Tools.WindowHelper.GetActiveWindow();
            if (window != null)
            {
                window.Close();
            }
        });
    }
}
