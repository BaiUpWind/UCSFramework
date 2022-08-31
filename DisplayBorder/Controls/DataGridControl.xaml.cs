using HandyControl.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

namespace DisplayBorder.Controls
{
    /// <summary>
    /// DataGridControl.xaml 的交互逻辑
    /// </summary>
    public partial class DataGridControl : UserControl//, ISingleOpen, IControlData
    {
        public DataGridControl()
        {
            InitializeComponent(); 
        }

        public bool CanDispose => true;

        public object TypeData { get  ; set  ; }

        public event Action<object> OnSet;
        public event Action OnClose;

        public void Dispose()
        {
            dg1.ItemsSource = null;
        }

        public object GetData() => TypeData;

        public void SetData(object data)
        {
            TypeData = data;
            OnSet?.Invoke(data);
            if (data != null && data is DataTable dt)
            {
                DataView dv = new DataView( dt);
                dg1.ItemsSource = dv;
            }
            else if(data is IEnumerable list)
            {
                dg1.ItemsSource = list;
            }
        
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            OnClose?.Invoke();
        }
    }
}
