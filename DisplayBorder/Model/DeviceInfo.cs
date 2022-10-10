using DeviceConfig;
using DeviceConfig.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayBorder.Model
{
    /// <summary>
    /// 设备需要读取的信息所需要的配置 
    /// </summary>   
    public class DeviceInfo
    {
        private OperationBase operation;//对设备的操作 读取之类的 
        //private bool start;//是否开始获取信息 
        /// <summary>
        /// 对应的设备编号
        /// </summary> 
        [Hide]
        public int DeviceID { get; set; }
        /// <summary>
        /// 设备信息编号
        /// </summary>
        [Hide]
        public int DeviceInfoID { get; set; }
        /// <summary>
        /// 设备信息名称
        /// </summary>
        [NickName("设备信息名称")]
        [Control("DeviceInfoName", "设备信息名称", ControlType.TextBox)]
        public string DeviceInfoName { get; set; }

        /// <summary>
        /// 对设备的操作类型
        /// </summary>
        [JsonConverter(typeof(PolyConverter))]
        [Control("Operation", "创建操作类型", ControlType.Genericity, GenerictyType: typeof(OperationBase), FieldName: nameof(Operation))]
        [Control("编辑操作类型", null, ControlType.Data, GenerictyType: typeof(OperationBase), FieldName: nameof(Operation))]
        [NickName("读取类型")]
        public OperationBase Operation { get => operation; set => operation = value; }



        /// <summary>
        /// 刷新数据间隔  单位:毫秒 
        /// </summary>
        [Control("RefreshInterval", "数据刷新间隔(毫秒)", ControlType.TextBox)]
        [NickName("数据刷新间隔(毫秒)")]
        public int RefreshInterval { get; set; } = 2000;
        /// <summary>
        /// 停留时间  单位:毫秒 
        /// </summary>
        [Control("StayTime", "停留时间(毫秒)", ControlType.TextBox)]
        [NickName("停留时间(毫秒)")]
        public int StayTime { get; set; } = 10000;

        #region 操作的创建(弃用 20220830)

        /// <summary>
        /// 获取所有操作类型
        /// <para></para>
        /// <see cref=" Utility.Reflection.GetInheritors"/>
        /// </summary>
        /// <returns></returns>
        //public ClassData GetOperations() => Utility.Reflection.GetClassData<OperationBase>();

        /// <summary>
        /// 创建一个操作类型
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="Exception"></exception>
        //public void CreateOpertaion(string name)
        //{ 
        //    Operation = Utility.Reflection.CreateObjectShortName<OperationBase>(name);
        //    if (Operation == null)
        //    {
        //        throw new Exception($"{info} 创建失败！");
        //    } 
        //}

        #endregion



        [JsonIgnore]
        private string info => $"设备编号'{DeviceID}' 设备信息编号'{DeviceInfoID}' 设备信息名称'{DeviceInfoName}';";


    }
}
