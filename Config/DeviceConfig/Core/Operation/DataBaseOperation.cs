﻿using ControlHelper.Attributes;
using DBHelper;
using System;
using System.Collections.Generic; 

namespace DeviceConfig.Core
{
    public delegate void OnStatusChanged(object sender, StatusModel e);

    [DependOn(typeof(DataBaseConnectCfg) ,typeof(SQLCmd))]
    public sealed class DataBaseOperation : OperationBase 
    {
      
        public DataBaseOperation( )  
        {
            if (ConnectConfig is DataBaseConnectCfg dbcc)
            {
                CreateType(dbcc);  
                return;
            }
           throw new ArgumentException("创建数据库操作实例失败!");
        }
        private DBUnitiyBase db; 

      
        /// <summary>
        /// 当状态发生变化时
        /// </summary>

        public event OnStatusChanged OnStatusChangedEvent;

        public override bool Connect()
        { 
            try
            {
                if(ConnectConfig is DataBaseConnectCfg dbcc)
                {
                  db.ConnStr =  dbcc.GetConnStr().Replace("\r\n", "");
                }
                db.CurrentConnection.Open();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                db?.CurrentConnection.Close();
            }
        }

        public override void Disconnected()
        {
            db.CurrentConnection.Close();
            //isChecked = false;
        } 
   
        protected override ResultBase Read(object cmd)
        {
            try
            {
                if (cmd is SQLCmd sqlcmd)
                {
                    if (ConnectConfig is DataBaseConnectCfg dbcc)
                    {
                        db.ConnStr = dbcc.GetConnStr().Replace("\r\n", "");
                    }
                    try
                    {
                        if (string.IsNullOrEmpty((sqlcmd.CommandStr?.ToString()))) return null;
                        ErrorCheck(sqlcmd);
                        var data = db.GetDataTable(sqlcmd.CommandStr.ToString().Replace("\r\n", ""), System.Data.CommandType.Text);
                        sqlcmd.Result.Data = data;

                        // 读取异常状态
                        //Check();
                        return sqlcmd.Result;
                    }
                    catch
                    {
                        return null;
                    }
                }
                return null;
            }
            catch (Exception ex)
            { 
                throw ex;
            }
           
        }

        
        private static void ErrorCheck(SQLCmd sqlcmd)
        {
            if (sqlcmd.CommandStr.ToString().ToLower().Contains("update"))
            {
                throw new Exception("不支持的语句 update");
            }
            if (sqlcmd.CommandStr.ToString().ToLower().Contains("delete"))
            {
                throw new Exception("不支持的语句 delete");
            }
            if (sqlcmd.CommandStr.ToString().ToLower().Contains("instert"))
            {
                throw new Exception("不支持的语句 delete");
            }
            if (sqlcmd.CommandStr.ToString().ToLower().Contains("alter"))
            {
                throw new Exception("不支持的语句 alter");
            }
            if (sqlcmd.CommandStr.ToString().ToLower().Contains("drop"))
            {
                throw new Exception("不支持的语句 drop");
            }
        }

        public override void SetConn(ConnectionConfigBase conn)
        {
            base.SetConn(conn);
             
            if (conn is DataBaseConnectCfg dbconn)
            {
                CreateType(dbconn); 
            }
        }

        private void CreateType(DataBaseConnectCfg type)
        {  
            if (type is DataBaseConnectCfg dataBase)
            {
                switch (dataBase.DbType)
                {
                    case  DBType.Oracle:
                        db = new OracleHelp(dataBase.GetConnStr());
                        break;
                    case  DBType.SqlServer:
                        db = new SQLServerHelp(dataBase.GetConnStr());
                        break;
                    case  DBType.MySql:
                        db = new MySqlHelp(dataBase.GetConnStr());
                        break;
                }
                ConnectConfig = dataBase;
                return; 
            }
            throw new ArgumentException("创建数据库操作实例失败!"); 
        }
        #region 状态检查弃用 2022 12 05 不要删除
        /// <summary>
        /// 状态检查
        /// 
        /// </summary>
        //[Control("StatusCheck", "状态检查", ControlType.Data, GenerictyType: typeof(StatusCheck), FieldName: nameof(StatusCheck))]
        //public StatusCheck StatusCheck { get; set; }  
        //private int currentStatus;
        //private void Check()
        //{
        //    if (StatusCheck == null) return;
        //    if (string.IsNullOrWhiteSpace(StatusCheck.AliveSql) ) return;
        //    if (db == null) return;
        //    currentStatus = StatusCheck.DefualtValue.Value;
        //    StatusCheck.AliveSql = StatusCheck.AliveSql.Replace("\r\n", "");
        //    //while (true)
        //    //{
        //        //if (!isChecked) continue; 
        //        try
        //        {

        //            var result = db.GetScalar(StatusCheck.AliveSql, System.Data.CommandType.Text);
        //            //当前的值
        //            int resultValue = int.Parse(result.ToString());

        //            if (currentStatus != resultValue)
        //            {
        //                //获取异常状态的图片
        //                var statusModel = StatusCheck.StatusModels.Find(a => a.Value == resultValue);
        //                if (statusModel != null)
        //                {
        //                    currentStatus = resultValue;
        //                    OnStatusChangedEvent?.Invoke(this, statusModel);
        //                }
        //                else
        //                {
        //                    //没有找到时 就使用默认值
        //                    OnStatusChangedEvent?.Invoke(this, StatusCheck.DefualtValue);
        //                    Console.WriteLine($"未找到对应的statusModel,resultValue'{resultValue}'");
        //                }
        //            }
        //            //if (StatusCheck.CheckInterval == 0) StatusCheck.CheckInterval = 1000;
        //            //Thread.Sleep(StatusCheck.CheckInterval);
        //        }
        //        catch (Exception ex)
        //        {
        //            //出现未知错误 3秒后再试
        //            Console.WriteLine($"状态检查时：出现未知错误'{ex.Message}'");
        //            //Thread.Sleep(3000);
        //        } 
        //    //}
        //}
        #endregion 
    }

    public sealed class StatusCheck
    { 
        /// <summary>
        /// 检测sql语句
        /// </summary>
        [NickName("检测语句","这里的返回结果是首行首列，注意编写规则!")]
        [Control("AliveSql", "检测语句", ControlType.TextBox)]
        public string AliveSql { get; set; }

        /// <summary>
        /// 正常值
        /// </summary>
        [NickName("默认值","当检测语句的结果与默认值不匹配时，则去状态中找到对应的状态展示")]
        [Control("DefualtValue","默认值", ControlType.Data, GenerictyType: typeof(StatusModel), FieldName: nameof(DefualtValue))]
        public StatusModel DefualtValue { get; set; }  

        /// <summary>
        /// 检测间隔
        /// </summary>
        //[NickName("检测间隔（毫秒）")]
        //public int CheckInterval { get; set; } = 1000;

        [NickName("状态组")]
        [Control("StatusModels", "状态组", ControlType.Collection, GenerictyType: typeof(StatusModel), FieldName: nameof(StatusModels))]
        public List<StatusModel> StatusModels { get; set; } 
    }

    public sealed class StatusModel
    {
        //[NickName("状态类型")]
        //public StatusType StatusType { get; set; }

        [NickName("对应值")]
        [Control("Value", "对应值", ControlType.TextBox)]
        public int Value { get; set; } = 1;

        [NickName("状态描述")]
        [Control("Description", "状态描述", ControlType.TextBox)]
        public string Description { get; set; }

        [NickName("状态图片", "双击选择文件路径")]
        [FileSelector("Jpge")]
        [Control("ImagePath", "图片配置路径", ControlType.TextBox)]
        [Control("ImagePath", null, ControlType.FilePathSelector, FieldName: nameof(ImagePath), FileType: "jpg")]
        public string ImagePath { get; set; }

       
    }
}
