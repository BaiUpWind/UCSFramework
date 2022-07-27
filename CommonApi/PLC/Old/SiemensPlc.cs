using System;
using System.Collections.Generic;
using System.Linq;
using HslCommunication.Profinet.Siemens;
using HslCommunication;

using System.Text.RegularExpressions;
using System.Collections;
 
 

namespace CommonApi.PLC.Old
{
 
    public class SiemensPlc    
    {
        /// <summary>
        /// 初始化
        /// 
        /// 
        /// 更改：2020年6月8日
        /// 从之前自动连接更换成带参数是否自动连接  默认自动连接（true）
        /// </summary>
        /// <param name="siemensPLCS"></param>
        /// <param name="listItem">DB块集合</param>
        /// <param name="ID">唯一ID</param>
        /// <param name="type"> 默认为自动连接(true), 手动连接（false）</param>
        public SiemensPlc(SiemensPLCS Sime, int ID, string Ip, bool type = true, byte Rack = 0, byte Slot = 0)
        {
            if (type )
            {
                
              
                SiemensTcpNet?.ConnectClose();
                SiemensTcpNet = new SiemensS7Net(Sime)
                {
                    IpAddress = Ip,
                    Rack = Rack,
                    Slot = Slot
                };
                HslCommunication. OperateResult operate = SiemensTcpNet.ConnectServer();//put get 
                if (!operate.IsSuccess)
                {
                    throw new Exception(operate.Message);
                }
                else
                {
                    IsConnectioned = true;
                    if (!SiemPlcServer.FindName(ID))
                    {
                        SiemensTcpNet.ConnectClose();//断开连接
                        throw new Exception("已创建的id" + id + "\r\n请勿重复创建！\r\n服务创建失败！");
                    }
                    SiemPlcServer.AddName(ID);
                    ServerId = ID;
                }
            }
            sime = Sime;
            id = ID;
            rack = Rack;
            slot = Slot;
            ip = Ip;

        }
       
        /// <summary>
        /// PLC的名称
        /// </summary>
        public string PLCName { get; set; }
        private SiemensS7Net SiemensTcpNet = null;
        /// <summary>
        /// 服务ID 唯一的
        /// </summary>
        private int ServerId { get; set; }
        /// <summary>
        /// 是否连接成功
        /// </summary>
        public bool IsConnectioned { get; set; } = false;


        /// <summary>
        /// DB地址集合, 一个PLC服务可以包含多个DB地址
        /// </summary>
        public List<SiemensDBRead> NListItem = new List<SiemensDBRead>();
        /// <summary>
        /// 键值对集合， 键 ：DB块名称  值 ：DB块的位置
        /// </summary>
        public Dictionary<string, SiemensDBRead> DirItem = new Dictionary<string, SiemensDBRead>();
        private SiemensPLCS sime;
        private string ip;
        private byte rack;
        private byte slot;
        private int id;

        public event Action<string, int> ShowMsg;

        /// <summary>
        /// 连接plc
        /// 在重连之前会进行一次非空的断开连接操作
        /// </summary>
        public bool Connection()
        {
            SiemensTcpNet?.ConnectClose();
            SiemPlcServer.RemoveName(id);
            SiemensTcpNet = new SiemensS7Net(sime)
            {
                IpAddress = ip,
                Rack = rack,
                Slot = slot
            };
          HslCommunication.  OperateResult operate = SiemensTcpNet.ConnectServer();//put get 
            if (!operate.IsSuccess)
            {
                throw new Exception(operate.Message);
            }
            else
            {
                IsConnectioned = true;
                if (!SiemPlcServer.FindName(id))
                {
                    SiemensTcpNet.ConnectClose();//断开连接
                    throw new Exception("已创建的id" + id + "\r\n请勿重复创建！\r\n服务创建失败！");
                }
                SiemPlcServer.AddName(id);
                ServerId = id;
                //foreach (var item in DirItem)
                //{
                //    item.Value.SiemensTcpNet = SiemensTcpNet;
                //}
                return true;
            }
        }


        public void DisConnection()
        {
            try
            {
                var reslut = SiemensTcpNet.ConnectClose();
                if (!reslut.IsSuccess)
                {
                    throw new Exception(reslut.Message);
                } 
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        ///添加一个DB地址,根据添加顺序自动生成索引
        /// </summary>
        /// <param name="list"></param>
        public void AddItemList(List<string> list)
        {
          
            SiemensDBRead dr = new SiemensDBRead(SiemensTcpNet, list);
            NListItem.Add(dr);
        }
         
        /// <summary>
        /// 添加一个db地址，根据键获取对应的db块的值 (推荐使用这个)key不能重复
        /// </summary>
        /// <param name="key">db块名称</param>
        /// <param name="value">db块的位置</param>
        public void AddItemList(string key, List<string> value)
        {
            try
            {
                 
                SiemensDBRead dr = new SiemensDBRead(SiemensTcpNet, value);
                DirItem.Add(key, dr);
            }
            catch (Exception ex)
            {

                throw ex;
            }
   
        }

        /// <summary>
        /// 获取DB块操作 (推荐使用这个)
        /// 传入的key等于GroupName PLCdb组的名称
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>这个PLC服务对应的DB地址操作</returns>
        public SiemensDBRead GetDBRead(string key)
        {
            try
            {
                return DirItem.Where(a => a.Key == key).First().Value;
            }
            catch (Exception ex)
            {

                throw new Exception("未找到对应的键 " + ex.Message);
            }

        }
        /// <summary>
        /// 获取DB块操作
        /// </summary>
        /// <param name="index">从0位开始,根据添加的ItemCollection 顺序</param>
        /// <returns>这个PLC服务对应的DB地址操作</returns>
        public SiemensDBRead GetDBRead(int index)
        {
            try
            {
                return NListItem[index];
            }
            catch (Exception ex)
            {

                throw new Exception("错误的索引:" + ex.Message);
            }

        }
        public void Dispose()
        {
            SiemPlcServer.RemoveName(ServerId);
            foreach (var item in NListItem)
            {
                item?.Close();
            }
            foreach (var item in DirItem)
            {
                item.Value?.Close();
            }
            HslCommunication. OperateResult operate = SiemensTcpNet.ConnectClose();
            if (!operate.IsSuccess)
            {
                throw new Exception("关闭失败！" + operate.Message);
            }
        }

    }
    public class SiemensDBRead  
    {
        public SiemensDBRead(SiemensS7Net netS7, List<string> list)
        {
            SiemensTcpNet = netS7;
            ListItem = list;
        }
        public SiemensS7Net SiemensTcpNet;
        List<string> ListItem;

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            SiemensTcpNet?.ConnectClose();
        }

        /// <summary>
        /// 返回DB块集合的个数
        /// </summary>
        /// <returns></returns>
        public int ListCount
        {
            get
            {
                if (ListItem.Any())
                {
                    return ListItem.Count;

                }
                else
                {
                    return -1;
                }
            }
        }
        /// <summary>
        /// 根据索引 读取DB块地址
        /// </summary>
        /// <param name="index">从0开始</param>
        /// <returns></returns>
        public object Read(int index)
        {
            try
            {
                object result;
                var item = ListItem[index];//m.m0.5
                var arr = item.Trim().Split('.');//db30.m0.5
                if (arr.Length > 1)
                {
                    string types = Regex.Replace(arr[1], "[0-9]", "", RegexOptions.IgnoreCase).Trim();//获取地址块的类型
                    switch (types.ToLower())
                    {
                        case "bool":
                            result = SiemensTcpNet?.ReadBool(GetNewItem(item)).Content;
                            break;
                        case "byte":
                            result = SiemensTcpNet?.ReadByte(GetNewItem(item)).Content;
                            break;
                        case "w":
                            result = SiemensTcpNet?.ReadInt16(GetNewItem(item)).Content;
                            break;
                        case "ushort"://ushort
                            result = SiemensTcpNet?.ReadUInt16(GetNewItem(item)).Content;
                            break;
                        case "dint":
                            result = SiemensTcpNet?.ReadInt32(GetNewItem(item)).Content;
                            break;
                        case "uint":
                            result = SiemensTcpNet?.ReadUInt32(GetNewItem(item)).Content;
                            break;
                        case "long":
                            result = SiemensTcpNet?.ReadInt64(GetNewItem(item)).Content;
                            break;
                        case "ulong":
                            result = SiemensTcpNet?.ReadUInt64(GetNewItem(item)).Content;
                            break;
                        case "real":
                            result = SiemensTcpNet?.ReadFloat(GetNewItem(item)).Content;
                            break;
                        case "double":
                            result = SiemensTcpNet?.ReadDouble(GetNewItem(item)).Content;
                            break;
                        case "string":
                            result = SiemensTcpNet?.ReadString(GetNewItem(item), 10).Content;
                            break;
                        case "m":
                            result = SiemensTcpNet?.ReadBool(GetNewItem(item)).Content;
                            break;
                        case "q":
                            result = SiemensTcpNet?.ReadBool(GetNewItem(item)).Content;
                            break;
                        default:
                            result = SiemensTcpNet?.ReadBool("M0.5").Content.ToString();
                            break;
                    }
                    return result;
                }
                return -1;
            }
            catch (Exception)
            {
                return -1;
            }
        } /// <summary>
          ///<summary>
          /// 写入DB块
          /// </summary>
          /// <param name="values">值</param>
          /// <param name="index">地址的位置</param>
        public void Write(object values, int index)
        {
            try
            {
                var item = ListItem[index];
                var arr = item.Trim().Split('.');
                if (arr.Length > 1)
                {
                    string types = Regex.Replace(arr[1], "[0-9]", "", RegexOptions.IgnoreCase).Trim();//获取地址块的类型

                    switch (types.ToLower())
                    {
                        case "bool":
                            SiemensTcpNet?.Write(GetNewItem(item), Convert.ToBoolean(values));
                            break;
                        case "byte":
                            SiemensTcpNet?.Write(GetNewItem(item), Convert.ToByte(values));
                            break;
                        case "w":
                            SiemensTcpNet?.Write(GetNewItem(item), short.Parse(values.ToString()));
                            break;
                        case "ushort"://ushort
                            SiemensTcpNet?.Write(GetNewItem(item), ushort.Parse(values.ToString()));
                            break;
                        case "dint":
                            SiemensTcpNet?.Write(GetNewItem(item), Convert.ToInt32(values));
                            break;
                        case "uint":
                            SiemensTcpNet?.Write(GetNewItem(item), uint.Parse(values.ToString()));
                            break;
                        case "long":
                            SiemensTcpNet?.Write(GetNewItem(item), long.Parse(values.ToString()));
                            break;
                        case "ulong":
                            SiemensTcpNet?.Write(GetNewItem(item), ulong.Parse(values.ToString()));
                            break;
                        case "real":
                            SiemensTcpNet?.Write(GetNewItem(item), float.Parse(values.ToString()));
                            break;
                        case "double":
                            SiemensTcpNet?.Write(GetNewItem(item), double.Parse(values.ToString()));
                            break;
                        case "string":
                            SiemensTcpNet?.Write(GetNewItem(item), values.ToString());
                            break;
                        case "m":
                            SiemensTcpNet?.Write(GetNewItem(item), Convert.ToBoolean(values));
                            break;
                        case "q":
                            SiemensTcpNet?.Write(GetNewItem(item), Convert.ToBoolean(values));
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }  
        public event Action<string, int> ShowMsg;

        /// <summary>
        /// 写入电控 DB的长度 支持异步写入
        /// </summary>
        /// <param name="values">对应整块DB块的值</param>
        public void Write(object[] values)
        {
            if (values.Length != ListItem.Count)
            {
                throw new Exception("写入值元素个数与定义DB块个数不一致!");
            }
             


                for (int i = 0; i < ListItem.Count; i++)
                {
                    if (i > ListItem.Count)
                    {
                        break;
                    }
                    var item = ListItem[i];
                    var arr = item.Trim().Split('.');
                    if (arr.Length > 1)
                    {
                        string types = Regex.Replace(arr[1], "[0-9]", "", RegexOptions.IgnoreCase).Trim();//获取地址块的类型 dint or w or real.....
                        checked
                        {
                            switch (types.ToLower())
                            {
                                case "bool":
                                    SiemensTcpNet?.Write(GetNewItem(item), Convert.ToBoolean(values[i]));
                                    break;
                                case "byte":
                                    SiemensTcpNet?.Write(GetNewItem(item), Convert.ToByte(values[i]));
                                    break;
                                case "w":
                                    SiemensTcpNet?.Write(GetNewItem(item), short.Parse(string.Format("{0:0}", values[i])));//string.Format( "0.##"", values[i])
                                    break;
                                case "ushort"://ushort
                                    SiemensTcpNet?.Write(GetNewItem(item), ushort.Parse(values[i].ToString()));
                                    break;
                                case "dint":
                                    SiemensTcpNet?.Write(GetNewItem(item), Convert.ToInt32(values[i]));
                                    break;
                                case "uint":
                                    SiemensTcpNet?.Write(GetNewItem(item), uint.Parse(values[i].ToString()));
                                    break;
                                case "long":
                                    SiemensTcpNet?.Write(GetNewItem(item), long.Parse(values[i].ToString()));
                                    break;
                                case "ulong":
                                    SiemensTcpNet?.Write(GetNewItem(item), ulong.Parse(values[i].ToString()));
                                    break;
                                case "real":
                                    SiemensTcpNet?.Write(GetNewItem(item), float.Parse(values[i].ToString()));
                                    break;
                                case "double":
                                    SiemensTcpNet?.Write(GetNewItem(item), double.Parse(values[i].ToString()));
                                    break;
                                case "string":
                                    SiemensTcpNet?.Write(GetNewItem(item), values[i].ToString());
                                    break;
                                case "m":
                                    SiemensTcpNet?.Write(GetNewItem(item), Convert.ToBoolean(values));
                                    break;
                                case "q":
                                    SiemensTcpNet?.Write(GetNewItem(item), Convert.ToBoolean(values));
                                    break;
                            }
                        }
                    }
                
            }
        }
        /// <summary>
        /// 拆解DB块内容 2019/12/26 增加bool量的读写方式 日期：2020年6月18日 增加M&Q量的读取方式
        /// </summary>
        /// <param name="oldString"></param>
        /// <returns></returns>
        string GetNewItem(string oldString)
        {
            string newStr = "";
            var a = oldString.Trim().Split('.');
            if (a.Length > 1)
            {
                newStr = Regex.Replace(a[1], "[a-z]", "", RegexOptions.IgnoreCase).Trim();
                if (a.Length > 2)
                {
                    newStr += "." + a[2];
                }
            }
            string types = Regex.Replace(a[0], "[0-9]", "", RegexOptions.IgnoreCase).Trim();//获取地址块的类型 dint or w or real.....
            if (types.ToLower() == "m")
            {
                newStr = (a[0] + newStr).ToUpper();//DB31.0 

            }
            else if (types.ToLower() == "q")
            {
                newStr = (a[0] + newStr).ToUpper();//DB31.0 
            }
            else
            {
                newStr = (a[0] + "." + newStr).ToUpper();//DB31.0

            }
            return newStr;
        }


    }
     class SiemPlcServer 
    {
        
        static ArrayList list = new ArrayList();

        public static void AddName(int id)
        {
            list.Add(id);
        }

        public static void RemoveName(int id)
        {
            list.Remove(id);
        }
        public static bool FindName(int id)
        {
            foreach (var item in list)
            {
                if( (int)item == id)
                {
                    return false;
                }
                //else
                //{
                //    return true;
                //}
            }
            return true;
        }
    }
 
}
