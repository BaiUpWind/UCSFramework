using ControlHelper.Interfaces;
using DeviceConfig.Core;
using DisplayConveyer.Model;
using System;
using System.Collections.Generic;
using System.Data;

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
                
                foreach (var result in results)
                {
                    if (result is SQLResult sqlRes && sqlRes.Data is DataTable dt)
                    {  
                        //解析数据库
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            var id = dt.Rows[i]["work_id"]?.ToString();
                            if (!int.TryParse(dt.Rows[i]["status"]?.ToString(), out int state))
                            {
                                state = -1;
                            }
                            TryShowStatus(area.Devices, id, state);
                        }
                    }
                    else if (result is PLCDBResult plcRes && plcRes.Data is List<DBData> dl)
                    {
                        //解析西门子PLC
                        foreach (var dbData in dl)
                        {
                            TryShowStatus(area.Devices, dbData.WorkId, dbData.Status);
                        }
                    }
                }
            }
        }

        private void TryShowStatus(List<DeviceData> devices, string id, int state)
        {
            var device = devices.Find(a => a.ID == id);
            if (device == null)
            {
                InternalShowMsg($"显示状态时未找到ID为'{id}'的设备;状态为'{state}'", 2);
                return;
            }
            device.StatusChanged?.Invoke(state);
        }

        private void InternalShowMsg(string msg, int level =1) => ShowMsg?.Invoke(msg,level);
    }
}
