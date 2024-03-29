﻿using Config.DeviceConfig.Models;
using ControlHelper.Interfaces;
using DeviceConfig.Core;
using DisplayConveyer.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace DisplayConveyer.Logic
{
    public class ReadStatusLogic : IMsgShow
    {
        public event Action<string, int> ShowMsg;

        private readonly List<AreaData> Areas;
        private readonly Thread[] threads;
        //调试模式
        Random demoRd = new Random();
        List<AreaData> demoAreas = GlobalPara.ConveyerConfig.Areas;
        Dictionary<uint,List<StatusData>> demoDicStatusDatas = new Dictionary<uint, List<StatusData>>();

        public ReadStatusLogic(List<AreaData> areas)
        {
            Areas = areas;
            if (Areas != null && Areas.Any())
            {
                threads = new Thread[Areas.Where(a => a.Devices != null && a.Devices.Count() > 0).Count()];
                for (int i = 0; i < Areas.Count; i++)
                {
                    var area = Areas[i];
                    threads[i] = new Thread(() => Read(area));
                    threads[i].IsBackground = true;
                    threads[i].Start();
                    if (i >= 2)
                    {
                        break;
                    }
                }
            } 
            foreach (var item in demoAreas)
            {
                demoDicStatusDatas.Add(item.ID, item.Devices.Select(a => new StatusData
                {
                    WorkId = a.WorkId,
                    LoadState = 1,
                    MachineState = 1
                }).ToList()); 
            }

        }
        public void Stop()
        {
            if (threads != null && threads.Length > 0)
            {
                for (int i = 0; i < threads.Length; i++)
                {
                    threads[i]?.Abort();
                }
            }
        }
        private void Read(AreaData area)
        { 
          if(!GlobalPara.ConveyerConfig.DemoMode)  OperationConnect(area);
            Random random = new Random();
            while (true)
            {
                if (GlobalPara.ConveyerConfig.DemoMode)
                {
                    DemoMode(area);
                    Thread.Sleep(random.Next(1000,5000));
                    continue;
                }

                List<object> results;
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
                        //解析西门子PLC读取数据
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
                //限制最低的间隔为3秒
                if (area.ReadInterval <= 3000)
                {
                    area.ReadInterval = 3000;
                }
                Thread.Sleep(area.ReadInterval);
            }
        }

        private void OperationConnect(AreaData area)
        {
            while (true)
            {
                if (area.Operation.Connect())
                {
                    foreach (var item in area.Devices)
                    {
                        item.StatusChanged?.Invoke(new StatusData() { MachineState = 101 });
                    }
                    break;
                }
                else
                {
                    //todo:通知连接失败
                }
                Thread.Sleep(3000);
            }

        }
        private void TryShowStatus(List<DeviceData> devices, StatusData state)
        {
            if (state == null)
            {
                //todo 将这个为空的信息传出去 出错了
                return;
            }
            var device = devices.Find(a => a.WorkId == state.WorkId);
            if (device == null)
            {
                InternalShowMsg($"显示状态时未找到ID为'{state.WorkId}'的设备;状态为'{state}'", 2);
                return;
            }
            device.StatusChanged?.Invoke(state);
        }

        private void DemoMode(AreaData area)
        {  
            if(demoDicStatusDatas.TryGetValue(area.ID,out var data))
            {
                foreach (var state in data)
                {
                    state.LoadState = demoRd.Next(1, 100) >= 95 ? 1 : 0;
                    state.MachineState = GetMachieState(demoRd.Next(1, 100));
                    TryShowStatus(area.Devices, state);
                }
            }   
        }
        private int GetMachieState(int probability)
        {
            if (probability > 100) return 0;
            else if (probability > 100) return 55;
            else if (probability > 90) return 101;
            else if (probability > 30) return 100; 
            else return 0; 
        }


        private void InternalShowMsg(string msg, int level =1) => ShowMsg?.Invoke(msg,level);
    }
}
