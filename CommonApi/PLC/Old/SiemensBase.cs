using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonApi.PLC.Old
{

 
    public class SiemensBase  
    {
        private SocketClient sc = new SocketClient();
        private byte[] handShakeFirstInfo = new byte[] { 0x03, 0x00, 0x00, 0x16, 0x11, 0xE0, 0x00, 0x00, 0x00, 0x01, 0x00, 0xC1, 0x02, 0x01, 0x00, 0xC2, 0x02, 0x01, 0x02, 0xC0, 0x01, 0x09 };
        private byte[] handShakeSecondInfo = new byte[] { 0x03, 0x00, 0x00, 0x19, 0x02, 0xF0, 0x80, 0x32, 0x01, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x08, 0x00, 0x00, 0xF0, 0x00, 0x00, 0x01, 0x00, 0x01, 0x07, 0x80 };
        private ManualResetEvent connDone = new ManualResetEvent(false);
        private int sequenceNo = 1;
        private object readLock = new object();
        //private object writeLock = new object();
        private int canReadLength = 200;
        private object getMessageIDLock = new object();

        public int TcpPort { get; private set; }
        public string IpAddress { get; private set; }
        public int PlcSlot { get; private set; }
        public string Name { get; private set; }
        public bool IsConnected { get; private set; }

        public SiemensBase(string ip, int port, int slot, string name)
        {
            IpAddress = ip;
            TcpPort = port;
            PlcSlot = slot;
            Name = name;
            IsConnected = false; 
        }

        public bool Connect()
        {
            try
            {
                Ping ping = new Ping();
                PingReply reply = ping.Send(IpAddress, 500);
                if (reply.Status == IPStatus.Success)
                {
                    if (string.IsNullOrEmpty(IpAddress))
                    { return false; }
                    if (sc == null)
                        sc = new SocketClient();
                    if (sc.Connect(IpAddress, TcpPort))
                    {
                        if (HandShakeFirst())
                        {
                            if (HandShakeSecond())
                            {
                                IsConnected = true;
                                return true;
                            }
                        }
                    }
                    IsConnected = false;
                    return false;
                }
                else
                {
                    IsConnected = false;
                    //反馈原因网络不通
                    return false;
                }
            }
            catch (Exception ex)
            {
                IsConnected = false;
                return false;
            }
        } 
        public void DisConnect()
        {
            try
            {
                if (!IsConnected) return;
                if (sc != null)
                {
                    sc.Close();
                }
            }
            catch
            { }
            IsConnected = false;
        }





        /// <summary>
        /// 读取DB块
        /// </summary>
        /// <param name="dataBlockNum">DB块地址，DB1 ，其中的1 </param>
        /// <param name="startValue">起始位置 DB1.WO  0是起始位置  W122 122是起始位置</param>
        /// <param name="length">W 长度为 2 Dint 长度为 4</param>
        /// <param name="value">返回的结果集</param>
        /// <returns></returns>
        public bool Read(int dataBlockNum, int startValue, int length, ref byte[] value)
        {
            try
            {
                if (length > 0)
                {
                    int startValueTemp = startValue;
                    int startNum = 0;
                    while (length > canReadLength)
                    {
                        byte[] valueCanReadTemp = new byte[canReadLength];
                        if (ReadBytes(dataBlockNum, startValueTemp, canReadLength, ref valueCanReadTemp))
                        {
                            valueCanReadTemp.CopyTo(value, startNum);
                            length = length - canReadLength;
                            startValueTemp = startValueTemp + canReadLength;
                            startNum = startNum + canReadLength;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (length > 0)
                    {
                        byte[] valueCanReadOtherTemp = new byte[length];
                        if (ReadBytes(dataBlockNum, startValueTemp, length, ref valueCanReadOtherTemp))
                        {
                            valueCanReadOtherTemp.CopyTo(value, startNum);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

 

        /// <summary>
        /// 写入PLC数据
        /// </summary>
        /// <param name="dataBlockNum">数据块</param>
        /// <param name="startValue">开始地址</param>
        /// <param name="length">长度</param>
        /// <param name="value">读取返回值</param>
        /// <returns></returns>
        public bool Write(int dataBlockNum, int startValue, int length, byte[] value)
        {
            try
            {
                lock (readLock)
                {
                    int sequence = GetSequenceNo();
                    byte[] message = new byte[35 + value.Length];
                    message[0] = 0x03;
                    message[1] = 0x00;
                    int socketLength1 = (length + 35) / 256;
                    int socketLength2 = (length + 35) % 256;
                    message[2] = (byte)socketLength1;
                    message[3] = (byte)socketLength2;//2，3请求报文总长31
                    message[4] = 0x02;
                    message[5] = 0xF0;
                    message[6] = 0x80;
                    message[7] = 0x32;
                    message[8] = 0x01;
                    message[9] = 0x00;
                    message[10] = 0x00;
                    int sequence1 = sequence / 256;
                    int sequence2 = sequence % 256;
                    message[11] = (byte)sequence1;
                    message[12] = (byte)sequence2;//11，12序列号
                    message[13] = 0x00;
                    message[14] = 0x0E;
                    int writeLength1 = (length + 4) / 256;
                    int writeLength2 = (length + 4) % 256;
                    message[15] = (byte)writeLength1;
                    message[16] = (byte)writeLength2;
                    message[17] = 0x05;
                    message[18] = 0x01;
                    message[19] = 0x12;
                    message[20] = 0x0A;
                    message[21] = 0x10;
                    message[22] = 0x02;//01:bit； 02:byte
                    int writeLengthHigh = (length) / 256;
                    int writeLengthLow = (length) % 256;
                    message[23] = (byte)writeLengthHigh;
                    message[24] = (byte)writeLengthLow;//23,24写入长度
                    int dataBlockNum1 = dataBlockNum / 256;
                    int dataBlockNum2 = dataBlockNum % 256;
                    message[25] = (byte)dataBlockNum1;
                    message[26] = (byte)dataBlockNum2;//25,26 DB号
                    message[27] = 0x84;//类型为DB块 0x81-input ,0x82-output ,0x83-flag , 0x84-DB
                    int starth1 = startValue * 8 / 65536;//起始地址数*8
                    int starth2 = startValue * 8 % 65536;
                    int startl1 = starth2 / 256;
                    int startl2 = starth2 % 256;
                    message[28] = (byte)starth1;
                    message[29] = (byte)startl1;
                    message[30] = (byte)startl2;//28,29,30访问DB块的偏移量offset,相当于起始地址 
                    message[31] = 0x00;
                    message[32] = 0x04;//03:bit; 04:byte
                    int lengthHigh = (length * 8) / 256;
                    int lengthLow = (length * 8) % 256;
                    message[33] = (byte)lengthHigh;
                    message[34] = (byte)lengthLow;
                    for (int i = 0; i < value.Length; i++)
                    {
                        message[35 + i] = value[i];
                    }
                    if (sc.Send(message))
                    {
                        byte[] data = sc.Recv(null);
                        if (data.Length > 21)
                            return true;
                    }
                    IsConnected = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //DB31.w0
        internal object ReadInt32(string v)
        { 
            var blocknum = int.Parse(GetStringNum(v.Split('.')[0]));
            var start = int.Parse(GetStringNum(v.Split('.')[1]));
            byte[] vs = new byte[4];
            Read(blocknum, start, 4, ref vs);
            return  GetValue<int>.GetValueByBytes(PLCDbValueType.Dint, vs);
        }

        internal object ReadInt16(string v)
        {
            var blocknum = int.Parse(GetStringNum(v.Split('.')[0]));
            var start = int.Parse(GetStringNum(v.Split('.')[1]));
            byte[] vs = new byte[2];
            Read(blocknum, start, 2, ref vs);
            return GetValue<short>.GetValueByBytes(PLCDbValueType.W, vs);
        }

        internal void Write(string v1, short v2)
        {
            var blocknum = int.Parse(GetStringNum(v1.Split('.')[0]));
            var start = int.Parse(GetStringNum(v1.Split('.')[1]));

            Write(blocknum, start, 2, GetValue<short>.GetBytesByValue(PLCDbValueType.W, v2));
        }
        internal void Write(string v1, int v2)
        {
            var blocknum = int.Parse(GetStringNum(v1.Split('.')[0]));
            var start = int.Parse(GetStringNum(v1.Split('.')[1]));
            Write(blocknum, start, 4, GetValue<short>.GetBytesByValue(PLCDbValueType.Dint, v2));
        }
        /// <summary>
        /// 获取文本内的数字
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        internal string GetStringNum (string s)
        {
            return System.Text.RegularExpressions.Regex.Replace(s, @"[^0-9]+", "");
        }
        /// <summary>
        /// 读取PLC数据
        /// </summary>
        /// <param name="dataBlockNum">数据块</param>
        /// <param name="startValue">开始地址</param>
        /// <param name="length">长度</param>
        /// <param name="value">读取返回值</param>
        /// <returns></returns>
        private bool ReadBytes(int dataBlockNum, int startValue, int length, ref byte[] value)
        {
            try
            {
                lock (readLock)
                {
                    int sequence = GetSequenceNo();
                    byte[] message = new byte[31];
                    message[0] = 0x03;
                    message[1] = 0x00;
                    message[2] = 0x00;
                    message[3] = 0x1F;//2，3请求报文总长31
                    message[4] = 0x02;
                    message[5] = 0xF0;
                    message[6] = 0x80;
                    message[7] = 0x32;
                    message[8] = 0x01;
                    message[9] = 0x00;
                    message[10] = 0x00;
                    int sequence1 = sequence / 256;
                    int sequence2 = sequence % 256;
                    message[11] = (byte)sequence1;
                    message[12] = (byte)sequence2;//11，12序列号
                    message[13] = 0x00;
                    message[14] = 0x0E;
                    message[15] = 0x00;
                    message[16] = 0x00;
                    message[17] = 0x04;
                    message[18] = 0x01;
                    message[19] = 0x12;
                    message[20] = 0x0A;
                    message[21] = 0x10;
                    message[22] = 0x02;
                    int readLength1 = length / 256;
                    int readLength2 = length % 256;
                    message[23] = (byte)readLength1;
                    message[24] = (byte)readLength2;//23,23读取长度 读取byte数
                    int dataBlockNum1 = dataBlockNum / 256;
                    int dataBlockNum2 = dataBlockNum % 256;
                    message[25] = (byte)dataBlockNum1;
                    message[26] = (byte)dataBlockNum2;//25,26 DB号
                    message[27] = 0x84;//类型为DB块 0x81-input,0x82-output,0x83-flag,0x84-DB
                    int starth1 = startValue * 8 / 65536;//起始地址数*8
                    int starth2 = startValue * 8 % 65536;
                    int startl1 = starth2 / 256;
                    int startl2 = starth2 % 256;
                    message[28] = (byte)starth1;
                    message[29] = (byte)startl1;
                    message[30] = (byte)startl2;//28,29,30访问DB块的偏移量offset,相当于起始地址 
                    byte[] bytBuffer = new byte[25 + length];
                    if (sc.Send(message))
                    {
                        byte[] valueTemp = sc.Recv(null);
                        if (valueTemp.Length == bytBuffer.Length)
                        {
                            value = valueTemp.Skip(25).ToArray();
                            return true;
                        }
                    }
                    IsConnected = false;
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private int GetSequenceNo()
        {
            lock (getMessageIDLock)
            {
                int icount = sequenceNo + 1;
                if (icount > 60000 || icount < 1000)
                    icount = 1000;
                sequenceNo = icount;
                return icount;
            }
        }

        /// <summary>
        /// 第一次握手
        /// </summary>
        /// <returns></returns>
        private bool HandShakeFirst()
        {
            try
            {
                handShakeFirstInfo[18] = (byte)(PlcSlot);
                if (sc.Send(handShakeFirstInfo))
                {
                    byte[] data = sc.Recv(null);
                    if (data.Length == 22)
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        
        /// <summary>
        /// 第二次握手
        /// </summary>
        /// <returns></returns>
        private bool HandShakeSecond()
        {
            try
            {
                if (sc.Send(handShakeSecondInfo))
                {
                    byte[] data = sc.Recv(null);
                    if (data.Length == 27)
                        return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    internal enum PLCDbValueType
    {
        W,//short
        Dint//int
    }

    internal static class GetValue<T> where T : IConvertible
    {
        internal static T GetValueByBytes (PLCDbValueType type, byte[] vs)
        {
            try
            { 
                object result = 0;
                switch (type)
                {
                    case PLCDbValueType.W:
                        //高位* 256 + 低位    W int16 
                        result = short.Parse((vs[0] * 256 + vs[1]).ToString());
                        return (T)result;
                    case PLCDbValueType.Dint:
                        //第二位 * 65536 + 第三位*256 +第四位 DONT    
                        result = Convert.ToInt32(vs[1] * 65536 + vs[2] * 256 + vs[3]);
                        return (T)result;
                    default:
                        return (T)result;
                }
            }
            catch (Exception ex)
            { 
                throw ex;
            }
        }
        internal static byte[] GetBytesByValue(PLCDbValueType type, object tag)
        {
            try
            {

            switch (type)
            {
                case PLCDbValueType.W: 
                    return wbtye((short)tag); ;
                case PLCDbValueType.Dint: 
                    return dintbyte((int)tag);
                default:
                    return new byte[4];
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private static byte[] wbtye(short qwe)
        {
            byte[] vs = new byte[2]; 
            vs[0] = Convert.ToByte(Math.Floor(qwe / 256d));
            vs[1] = Convert.ToByte(qwe - (vs[0] * 256));
            return vs;
        }

       private static  byte[] dintbyte(int qwe)
        {
            byte[] vs = new byte[4];
            vs[0] = 0;
            vs[1] = Convert.ToByte(Math.Floor(qwe / 65536d)); 
            vs[2] = Convert.ToByte(Math.Floor((qwe - vs[1] * 65536) / 256d));
            vs[3] = Convert.ToByte(qwe - (vs[2] * 256 + vs[1] * 65536d));
            return vs;

        }
    }


  
}
