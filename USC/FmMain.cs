using Modle.DeviceCfg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USC
{
    public partial class FmMain : Form
    {
        public FmMain()
        {
            InitializeComponent();
        }

        private void FmMain_Load(object sender, EventArgs e)
        {
            InitializationBase ib = new InitializationBase();
            ib.CreateSysFile();
            ib.ReadDbIni();
            ib.ReadDeviceCfg<SerialPortCfg>(Modle.DeviceType.Serial);
            ib.ReadDeviceCfg<PLCCfg>(Modle.DeviceType.PLC);
            ib.ReadDeviceCfg<TCPCfgBase>(Modle.DeviceType.TCP);
            
        }
    }
}
