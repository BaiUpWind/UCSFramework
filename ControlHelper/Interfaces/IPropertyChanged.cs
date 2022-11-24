using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlHelper.Interfaces
{
    /// <summary>
    /// 当属性发生变化时
    /// </summary>
    public interface IPropertyChanged
    {
        /// <summary>
        /// 当值发生变化时
        /// <para>string 发生变化属性的名称</para>
        /// <para>object 对象值</para>
        /// </summary>
        event Action<string, object> OnPropertyChanged;
    }
}
