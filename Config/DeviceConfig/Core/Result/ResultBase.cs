﻿using ControlHelper.Attributes;
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
        [NickName("结果标题")]
        public virtual string Title { get; set; } = "统计信息";

        /// <summary>
        /// 结果显示选择类型
        /// <para>对应类型<see cref="DataType"/></para>
        /// </summary>
        [Control("SelectType", "结果显示图像", ControlType.ComboBox, EnumType: typeof(DataType))]
        [NickName("结果显示图像")]
        public virtual DataType SelectType { get; set; } = 0;
        /// <summary>
        /// 数据,原始数据
        /// </summary>
        [JsonIgnore]
        [Hide]
        public virtual object Data { get; set; } = null; 
    }
}
