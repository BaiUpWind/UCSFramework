using DisplayConveyer.Config;
using DisplayConveyer.DA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using DisplayConveyer.Model;
using System.Threading;

namespace DisplayConveyer.Logic
{
    public class BeltLogic
    {

        private readonly BeltConfig config;
        private readonly Thread runThread;
        private DA_BeltConfig da;

        public UC_Storages WholeBelts { get; private set; }

        /// <summary>
        /// 这个工艺的所有物流区域
        /// </summary>
        public Dictionary<int, UC_Storages> DicBelts { get; private set; } = new Dictionary<int, UC_Storages>();
        public bool IsRunning { get; private set; }
        public BeltLogic(BeltConfig config)
        {
            this.config = config;
            WholeBelts = new UC_Storages(true);
            WholeBelts.Title = config.BeltName;
            WholeBelts.Margin = new Thickness(5);
            runThread = new Thread(Run);
            runThread.Start();
            runThread.IsBackground = true;
            IsRunning = false;
            ReSet();
        }

        public void ReSet()
        {
            DicBelts.Clear();
            da = new DA_BeltConfig(config.DBConfig.GetConnectionStr());
            double top = 15d, left = 15d;
            int id = 1;
            foreach (var editor in config.Editors)
            {
                string errInfo;
                var result = da.GetBeltEditors(editor.TableName,out  errInfo);
                var belt = new UC_Storages(result, editor.XOffSet, editor.YOffSet); 
                if (!string.IsNullOrWhiteSpace(errInfo))
                {
                    belt.ErrorInfo = errInfo;
                } 
                belt.Title = editor.Title;
                belt.ReadTableName = editor.ReadTableName;  
                belt.RenderTransform = new MatrixTransform(1d, 0, 0, 1d, left, 15d); 
                left += (belt.Width + 15d) ;
                top = belt.Height > top ? belt.Height : top;
                WholeBelts.store.Children.Add(belt);
                DicBelts.Add(id, belt);
                id++;
            }
            WholeBelts.Width = left;
            WholeBelts.store.Height = top + 35d;
            WholeBelts.Height  = WholeBelts.store.Height + Math.Ceiling(WholeBelts.txtTitle.FontSize * WholeBelts.txtTitle.FontFamily.LineSpacing);

        }

        public void Start()
        {
            IsRunning = true;
        }

        public void Stop()
        {
            IsRunning = false;
        }

        public void RegisterEvent(Action<UC_Storages> onZoomIn, Action<UC_Storages> onZoomOut )
        { 
       
            WholeBelts.OnZoomIn += onZoomIn;
            WholeBelts.OnZoomOut += onZoomOut; 
            foreach (var item in DicBelts.Values)
            {
                item.OnZoomIn += onZoomIn;
                item.OnZoomOut += onZoomOut;
            }
        }

        public void UnRegisterEvent(Action<UC_Storages> onZoomIn, Action<UC_Storages> onZoomOut)
        {
            WholeBelts.OnZoomIn -= onZoomIn;
            WholeBelts.OnZoomOut -= onZoomOut;
            foreach (var item in DicBelts.Values)
            {
                item.OnZoomIn -= onZoomIn;
                item.OnZoomOut -= onZoomOut;
            }
        }
        private void Run()
        {
            while (true)
            {
                if (!IsRunning)
                {
                    Thread.Sleep(200);
                    continue;
                }

                foreach (var ucs in DicBelts.Values)
                {
                    var readTableName = ucs.ReadTableName;
                    if (string.IsNullOrWhiteSpace(readTableName))
                    {
                        continue;
                    }
                    string errInfo;
                    var reads = da.GetBeltReadPlcs(readTableName,out errInfo);
                    if (!string.IsNullOrWhiteSpace(errInfo))
                    {
                        ucs.ErrorInfo = errInfo;
                        Stop();
                    }
                    else
                    { 
                        foreach (var read in reads)
                        {
                            ucs.SetWorkPosColor(read.Work_id, read.Plc_status);
                        }
                    }
                    Thread.Sleep(100);
                }


                Thread.Sleep(200);
            }
        }

    }
}
