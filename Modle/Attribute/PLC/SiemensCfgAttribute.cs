using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modle 
{
    /// <summary>
    /// 西门子配置类属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class SiemensCfgAttribute : Attribute
    {
        //readonly int siemensSelected;
        //public int SiemensSelected => siemensSelected;

        /// <summary>
        /// 配置西门子
        /// </summary>
        /// <param name="siemensSelected"></param>
        //public SiemensCfgAttribute(int siemensSelected     )
        //{
        //    this.siemensSelected = siemensSelected;
        //} 
    }
}
