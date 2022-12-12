using Config.DeviceConfig.Models;
using ControlHelper.Interfaces;
using DeviceConfig.Core;
using DisplayConveyer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DisplayConveyer.Logic
{
    public class ReadStatusLogic: IMsgShow
    {

        List<AreaData> Areas = new List<AreaData>();

        public event Action<string, int> ShowMsg;

        public void Read()
        {
            foreach (var area in Areas)
            {
                List<object> results = null;
                try
                {
                      results = area.Operation.GetResults();
                }
                catch (Exception ex)
                {
                    InternalShowMsg($"在'{area.Name}'区域,读取设备信息时发生错误,'{ex.Message}'", 2);
                    continue;
                }
                List<StatusData> states = null;
                foreach (var result in results)
                {
                    if (result is SQLResult sqlRes && sqlRes.Data is DataTable dt)
                    {
                        //解析数据库
                        states = dt.ToEntityList<StatusData>().ToList();  
                    }
                    else if (result is PLCDBResult plcRes && plcRes.Data is List<StatusData> dl)
                    {
                        //解析西门子PLC
                        states = dl; 
                    }

                    //通知UI显示
                    if (states != null && states.Any())
                    {
                        foreach (var state in states)
                        {
                            TryShowStatus(area.Devices, state);
                        }
                    }
                }
            }
        }

        private void TryShowStatus(List<DeviceData> devices, StatusData state )
        {
            if (state == null)
            {
                //todo 将这个为空的信息传出去 出错了
                return;
            }
            var device = devices.Find(a => a.ID == state.WorkId);
            if (device == null)
            {
                InternalShowMsg($"显示状态时未找到ID为'{state.WorkId}'的设备;状态为'{state}'", 2);
                return;
            }
            device.StatusChanged?.Invoke(state);
        }

        private void InternalShowMsg(string msg, int level =1) => ShowMsg?.Invoke(msg,level);
    }
}
