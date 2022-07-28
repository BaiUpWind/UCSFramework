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
using CommonApi.Event;
using CommonApi;
using Modle;
using Modle.Config;

namespace USC
{
    public partial class FmMain : Form
    {
        public FmMain()
        {
            InitializeComponent();
            eventManager = new EventManager();

            eventManager.Subscribe(OnClickEventArgs.EventId, OnClick);
            eventManager.Subscribe(OnPlcKindSelectChangedArgs.EventId, OnPlcKindChanged);

            cbChosseType.DataSource = Enum.GetNames(typeof(DeviceConnectedType));

            LoadGroupCfg();
        }

        private void OnPlcKindChanged(object sender, BaseEventArgs e)
        {
            if(e is OnPlcKindSelectChangedArgs args)
            {
                if (args.Container != null && args.Container is Control control)
                {
                    control.Controls.Clear();
                }
            }
        }

        private void OnClick(object sender, BaseEventArgs e)
        {
            if (e is OnClickEventArgs args)
            {
                MessageBox.Show(args.Info);
            }
        }

        EventManager eventManager;
        GroupCfg currentGroupCfg = new GroupCfg();
        List<GroupCfg> listGroups = new List<GroupCfg>();
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
        test t = new test();
        private void button1_Click(object sender, EventArgs e)
        {
    
            var result = t.GetCfgNames<IDeviceConfig,PLCConfigAttribute>(); //t.GetDeviceConfigs<SiemensCfgAttribute>(); 

            StringBuilder sb = new StringBuilder();
            foreach (var item in result)
            {
                sb.Append(item + ",");
            }
            MessageBox.Show(sb.ToString());
        }
      
        private void button2_Click(object sender, EventArgs e)
        {
          var data =   t.CreateInstance<DataBaseCfg, IDeviceConfig, DeviceConnectedTypeAttribute>("DataBase");
            if(data != null)
            {
                data.DbName = "我是数据库";
                data.DbIp = "192.168.0.1";
            MessageBox.Show( data.DbName +"" + data.DbIp);
            }
        }

        private void cbChosseType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DeviceConnectedType connType = (DeviceConnectedType)cbChosseType.SelectedIndex;
            gbConnType.Text = connType.ToString();
            if (connType == DeviceConnectedType.None)
            {
              
                return;
            }
            gbConnType.Controls.Clear();
            switch (connType)
            {
                case DeviceConnectedType.Serial:
                    var serialPort = t.CreateInstance<SerialPortCfg, IDeviceConfig, DeviceConnectedTypeAttribute>("SerialPort");
                    CreateControlHelp.Create(gbConnType, serialPort);

                    break;
                case DeviceConnectedType.PLC:

                    //ComboBox plcselect = new ComboBox();
                    //plcselect.DataSource = Enum.GetNames(typeof(PLCKind));
                    //plcselect.SelectedIndexChanged += Plcselect_SelectedIndexChanged;
                    var plccfg = t.CreateInstance<PLCCfg, IDeviceConfig, DeviceConnectedTypeAttribute>("PLC");
                    CreateControlHelp.Create(gbConnType, plccfg);


                    break;
                case DeviceConnectedType.TcpClient:
                    var tcp = t.CreateInstance<TCPCfgBase, IDeviceConfig, DeviceConnectedTypeAttribute>("TcpClient");
                    CreateControlHelp.Create(gbConnType, tcp);

                    break;
                case DeviceConnectedType.DataBase:
                  var cfg =   t.CreateInstance<DataBaseCfg, IDeviceConfig, DeviceConnectedTypeAttribute>("DataBase");
                    CreateControlHelp.Create(gbConnType, cfg); 
                    break;
            }
          
        }

        private void Plcselect_SelectedIndexChanged(object sender, EventArgs e)
        {
           if(sender is ComboBox combo)
            {
                PLCKind pLCKind = (PLCKind)combo.SelectedIndex;

                if (pLCKind == PLCKind.None) return;

                switch (pLCKind)
                {
                    case PLCKind.None:
                        break;
                    case PLCKind.Siemens:

                        
                        break; 
                }
            }
        }

        private void btnAddGroup_Click(object sender, EventArgs e)
        {
            Button btn = new Button();
            GroupCfg gc = new GroupCfg();
            gc.DeviceConfigs = currentGroupCfg.DeviceConfigs;
            gc.GroupNo = int.Parse( CreateControlHelp.GetCrontrl<TextBox>(pGroup,nameof(gc.GroupNo)).Text );
            gc.GroupName = CreateControlHelp.GetCrontrl<TextBox>(pGroup, nameof(gc.GroupName)).Text;
            btn.Name = gc.GroupName;
            btn.Dock = DockStyle.Top;
            btn.Text = gc.GroupName;
            btn.Click += Btn_Click;
            listGroups.Add(gc);
            gbGroups.Controls.Add(btn);
        }

        private void Btn_Click(object sender, EventArgs e)
        {
             if(sender is Button btn )
            {
                var group = listGroups.Find(a => a.GroupName == btn.Text);
                if(group != null)
                {
                    pGroupInfo.Controls.Clear();

                    CreateControlHelp.Create(pGroupInfo, group);
                }
            }
        }

        private void LoadGroupCfg()
        {
            CreateControlHelp.Create(pGroup, currentGroupCfg);
        }
    }

    public class OnPlcKindSelectChangedArgs : BaseEventArgs
    {
        public static int EventId => typeof(OnPlcKindSelectChangedArgs).GetHashCode();
        public override int Id => EventId;

        /// <summary>
        /// 容器
        /// </summary>
        public object Container { get; private set; }
        public int Index { get; private set; }

        /// <summary>
        /// 创建事件参数
        /// </summary>
        /// <param name="container">容器控件</param>
        /// <param name="index">当前变换的索引</param>
        /// <returns></returns>
        public static OnPlcKindSelectChangedArgs Create(object container, int index)
        {
            var args = ReferencePool.Acquire<OnPlcKindSelectChangedArgs>();
            args.Index = index;
            args.Container = container;
            return args;
        }


        public override void Clear()
        {

        }
    }

    //测试事件系统
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


    public static class CreateControlHelp
    {

        public static void Create<T1, T2>(T1 fatherControl, T2 target) where T1 : Control, new() where T2 : class
        {
            Point orgin = new Point
            {
                X = 5,
                Y = 15
            };
            fatherControl.Controls.Clear();
            Type objType = typeof(T2);
            foreach (var propInfo in objType.GetProperties())
            {
                //只对公共的进行处理
                if (!propInfo.DeclaringType.IsPublic) return;
                //获取所有特性
                object[] objAttrs = propInfo.GetCustomAttributes(typeof(ControlAttribute), true);
                if (objAttrs.Length > 0)
                {
                    foreach (var item in objAttrs)
                    {
                        //找到有控件标签的特性
                        if (item is ControlAttribute attr)
                        {
                            switch (attr.ControlType)
                            {
                                case ControlType.Label:
                                    Label lbl = new Label();
                                    lbl.Name = attr.Name;
                                    lbl.Location = orgin;
                                    lbl.Text = $"{lbl.Name}：{objType.GetProperty(propInfo.Name)?.GetValue(target, null)}";
                                    orgin.Y += lbl.Height;
                                    break;
                                case ControlType.TextBox:
                                    TextBox txt = new TextBox();
                                    Label lbl2 = new Label();
                                    lbl2.Name = Guid.NewGuid().ToString();
                                    lbl2.Location = orgin;
                                    lbl2.Text = propInfo.Name + "：";
                                    txt.Name = attr.Name;
                                    txt.Location = new Point(orgin.X + lbl2.Width, orgin.Y);
                                    txt.Text = objType.GetProperty(propInfo.Name).GetValue(target, null)?.ToString();
                                    orgin.Y += txt.Height + 2;
                                    fatherControl.Controls.Add(txt);
                                    fatherControl.Controls.Add(lbl2);
                                    break;
                                case ControlType.ComboBox:
                                    Label lbl3 = new Label();
                                    lbl3.Name = Guid.NewGuid().ToString();
                                    lbl3.Location = orgin;
                                    lbl3.Text = propInfo.Name + "：";
                                    fatherControl.Controls.Add(lbl3);
                                    ComboBox box = new ComboBox();
                                    box.Name = attr.Name;
                                    box.Location = new Point(orgin.X + lbl3.Width, orgin.Y);
                                    box.DropDownStyle = ComboBoxStyle.DropDownList;
                                    foreach (var cbItem in attr.Items)
                                    {
                                        box.Items.Add(cbItem);
                                    }

                                    fatherControl.Controls.Add(box); 
                                    orgin.Y += box.Height + 2;
                                    break;
                                case ControlType.ComboBoxEnum:
                                    Label lbl4 = new Label();
                                    lbl4.Name = Guid.NewGuid().ToString();
                                    lbl4.Location = orgin;
                                    lbl4.Text = propInfo.Name + "：";

                                    ComboBox box1 = new ComboBox();
                                    box1.Name = attr.Name;
                                    box1.Location = new Point(orgin.X + lbl4.Width, orgin.Y);
                                    box1.DataSource = Enum.GetNames(attr.EnumType);
                                    fatherControl.Controls.Add(lbl4);
                                    fatherControl.Controls.Add(box1);
                                    orgin.Y += box1.Height + 2;
                                    break;
                            }
                        }
                    }
                }

            }
        }

        public static T GetCrontrl<T >(Control container,string name,int index = 0, bool searchChilden =false) where T : Control
        {
            if(container!= null && container.Controls.Count != 0)
            {
               var result =  container.Controls.Find(name, searchChilden)[index];
                return (T)result ;
            }
            throw new Exception("容器为空，或者控件元素为0");
        }
    }
}
