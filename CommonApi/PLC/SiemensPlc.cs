using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HslCommunication.Profinet.Siemens;


namespace CommonApi.PLC
{
    public class SiemensPlc
    {

        public SiemensPlc(int plcs ,string ip,int port = 102, byte Rack = 0, byte Slot = 0)
        {
            s7Net?.ConnectClose(); 
            s7Net = new SiemensS7Net(GetPlCS(plcs))
            {
                IpAddress = ip,
                Port = port,
                Rack = Rack,
                Slot = Slot
            };

           
        }
       
        private  readonly SiemensS7Net s7Net;

        /// <summary>
        /// 读取西门子实例
        /// </summary>
        public SiemensS7Net PlcS7
        {
            get
            {
                if (!IsConnected)
                {
                    Connection();
                }
               return  s7Net;
            }
        }
        /// <summary>
        /// 是否连接成功
        /// </summary>
        public bool IsConnected { get; set; } = false;

        public bool Connection()
        {
            s7Net.ConnectClose();
            var result = s7Net.ConnectServer();
            if (!result.IsSuccess)
            {
                throw new Exception(result.Message);
            } 
             return   IsConnected = true; 
        }
        public void DisConnected()
        {
            var reslut = s7Net.ConnectClose();
            if (!reslut.IsSuccess)
            {
                throw new Exception(reslut.Message);
            }
            IsConnected = false;
        }

       
        private SiemensPLCS GetPlCS(int value)
        {
            switch (value)
            {
                case 1:
                    return SiemensPLCS.S1200;
                case 2:
                    return SiemensPLCS.S300;
                case 3:
                    return SiemensPLCS.S400;
                case 4:
                    return SiemensPLCS.S1500;
                case 5:
                    return SiemensPLCS.S200Smart;
                case 6:
                    return SiemensPLCS.S200;
                default:
                    return SiemensPLCS.S1200;
            }
        }
    }
}
