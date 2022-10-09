using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConfig
{
    /// <summary>
    /// 文件选择器
    /// <para>只对string类型生效</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class FileSelectorAttribute : Attribute
    {
        public FileSelectorAttribute(string fileType)
        {
            FileType = fileType;
        }

       public string FileType { get; private set; }
    }
}
