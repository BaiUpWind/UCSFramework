using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
using System.Threading;
using System.Collections;

namespace WchCommon.PLC
{

    public class SiemensPlc : IDisposable
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="siemensPLCS"></param>
        /// <param name="listItem">DB块集合</param>
        /// <param name="ID">唯一ID</param>
        public SiemensPlc(int ID, string ip, int port=102, byte slot = 0)
        {
            SiemensTcpNet = new SiemensBase(ip, port, slot, "");
           var operate = SiemensTcpNet.Connect();//put get 
            if (!operate )
            {
                throw new Exception("连接PLC服务器失败");
            }
            else
            {
                IsConnectioned = true;
                if (!SiemPlcServer.FindName(ID))
                {
                    SiemensTcpNet.DisConnect();//断开连接
                    throw new Exception("已创建的id" + ID + "\r\n请勿重复创建！\r\n服务创建失败！");
                }
                SiemPlcServer.AddName(ID);
                ServerId = ID;
            }
        }
        private SiemensBase SiemensTcpNet = null;
        /// <summary>
        /// 服务ID 唯一的
        /// </summary>
        private int ServerId { get; }
        /// <summary>
        /// 是否连接成功
        /// </summary>
        public bool IsConnectioned { get; } = false;
        /// <summary>
        /// DB地址集合, 一个PLC服务可以包含多个DB地址
        /// </summary>
        public List<DBRead> NListItem = new List<DBRead>();


        /// <summary>
        ///添加一个DB地址,根据添加顺序自动生成索引
        /// </summary>
        /// <param name="list"></param>
        public void AddItemList(List<string> list)
        {
            DBRead dr = new DBRead(SiemensTcpNet, list);
            NListItem.Add(dr);
        }

        /// <summary>
        /// 获取DB块操作
        /// </summary>
        /// <param name="index">从0位开始,根据添加的ItemCollection 顺序</param>
        /// <returns>这个PLC服务对应的DB地址操作</returns>
        public DBRead GetDBRead(int index)
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
              SiemensTcpNet.DisConnect();
            //if (!operate.IsSuccess)
            //{
            //    throw new Exception("关闭失败！" + operate.Message);
            //}
        }

    }
    public class DBRead
    {
        public DBRead(SiemensBase netS7, List<string> list)
        {
            SiemensTcpNet = netS7;
            ListItem = list;
        }
        SiemensBase SiemensTcpNet;
        List<string> ListItem;

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            SiemensTcpNet?.DisConnect();
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
                object result =0;
                var item = ListItem[index];//m.m0.5                DB1.w0
                var arr = item.Trim().Split('.');//db30.m0.5     DB1  W0
                if (arr.Length > 1)
                {
                    string types = Regex.Replace(arr[1], "[0-9]", "", RegexOptions.IgnoreCase).Trim();//获取地址块的类型 w
                    switch (types.ToLower())
                    { 
                        case "w":
                            result = SiemensTcpNet.ReadInt16(GetNewItem(item));
                            break;
                       
                        case "dint":
                            result = SiemensTcpNet.ReadInt32(GetNewItem(item));
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
                        case "w":
                            SiemensTcpNet.Write(GetNewItem(item), short.Parse(values.ToString()));
                            break; 
                        case "dint":
                            SiemensTcpNet.Write(GetNewItem(item), Convert.ToInt32(values));
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 写入电控
        /// </summary>
        /// <param name="values">对应整块DB块的值</param>
        public void Write(object[] values)
        { 
            if(values.Length != ListItem.Count)
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
                            case "w":
                                SiemensTcpNet.Write(GetNewItem(item), short.Parse(string.Format("{0:0}", values[i])));//string.Format( "0.##"", values[i])
                                break; 
                            case "dint":
                                SiemensTcpNet.Write(GetNewItem(item), Convert.ToInt32(values[i]));
                                break; 
                        }
                    }
                }
            }  
        }
        /// <summary>
        /// 拆解DB块内容 2019/12/26 增加bool量的读写方式
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
            newStr = (a[0] + "." + newStr).ToUpper();//DB31.0
            newStr = oldString;
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
