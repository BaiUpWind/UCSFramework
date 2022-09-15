using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  DeviceConfig.Core 
{
    public abstract class ResultBase
    {
        /// <summary>
        /// 数据结果的标题
        /// </summary>
        [Control("Title", "结果标题", ControlType.TextBox)]
        public string Title { get; set; } = "统计信息";

        /// <summary>
        /// 结果显示选择类型
        /// <para>对应类型<see cref="DataType"/></para>
        /// </summary>
        [Control("SelectType", "结果显示图像", ControlType.ComboBox, EnumType: typeof(DataType))]
        public int SelectType { get; set; } = 0;
        /// <summary>
        /// 数据,原始数据
        /// </summary>
       [JsonIgnore]
        public object Data { get; set; } = null;
        /// <summary>
        /// 解析完成后的数据
        /// </summary>
        [JsonIgnore]
        public object FinalData { get; set; } = null; 
    }
}
