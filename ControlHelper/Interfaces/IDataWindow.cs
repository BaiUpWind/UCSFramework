using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlHelper.Interfaces
{
    /// <summary>
    /// 窗体数据
    /// 创建对应的控件,和创建对应实例
    /// </summary>
    public interface IDataWindow
    {

        void CreateType(Type targetType, object target);

        object GetType(params object[] paras);

        event Action<object> OnEnter;
        event Action<object> OnCancel;

    }
}
