﻿using Modle.DeviceCfg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonApi.Event;
using CommonApi;

namespace USC
{
    public partial class FmMain : Form
    {
        public FmMain()
        {
            InitializeComponent();
            eventManager = new EventManager();

            eventManager.Subscribe(OnClickEventArgs.EventId, OnClick);
        }

        private void OnClick(object sender, BaseEventArgs e)
        {
            if (e is OnClickEventArgs args)
            {
                MessageBox.Show(args.Info);
            }
        }

        EventManager eventManager;
        private void FmMain_Load(object sender, EventArgs e)
        {
            InitializationBase ib = new InitializationBase();
            ib.CreateSysFile();
            ib.ReadDbIni();
            ib.ReadDeviceCfg<SerialPortCfg>(Modle.DeviceConnectedType.Serial);
            ib.ReadDeviceCfg<PLCCfg>(Modle.DeviceConnectedType.PLC);
            ib.ReadDeviceCfg<TCPCfgBase>(Modle.DeviceConnectedType.TcpClient);
            
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            eventManager.Fire(this, OnClickEventArgs.Create("你好"));
        }
    }


    public class OnClickEventArgs : BaseEventArgs
    {
        public static int EventId => typeof(OnClickEventArgs).GetHashCode();
        public override int Id => EventId;

        public string Info { get; set; }

        public static OnClickEventArgs Create(string info)
        {
            OnClickEventArgs eventArgs = ReferencePool.Acquire<OnClickEventArgs>();
            eventArgs.Info = info;
            return eventArgs;
        }


        public override void Clear()
        {
            
        }
    }
}
