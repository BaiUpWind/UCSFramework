using CommonApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConfig.Core
{ 
    /// <summary>
    /// 指令基类
    /// </summary>
    public abstract  class CommandBase
    {
        /// <summary>
        /// 数据结果的标题
        /// </summary>
        [Control("Title", "结果标题", ControlType.TextBox)]
        public string Title { get; set; }

        [Control("SelectType", "结果显示图像", ControlType.ComboBox, EnumType: typeof(DataType))]
        public int SelectType { get; set; }

    
        /// <summary>
        /// 操作指令
        /// </summary>
        public abstract object CommandStr { get; set; }

        /// <summary>
        /// 指令的返回结果
        /// </summary>
        [JsonIgnore]
        public virtual ResultBase Result { get; set; }  
    } 
}
