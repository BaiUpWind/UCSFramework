using System.Text;
using System.Net;
using System.Net.Sockets;
using System;

namespace CommonApi.TCP
{
    public class ClientTest
    {
        private readonly TcpClient mClient;
        private readonly IPEndPoint endPoint;
        private NetworkStream ns;

        public ClientTest(string sIP, int iPort)
        {
            mClient = new TcpClient();
            endPoint = new IPEndPoint(IPAddress.Parse(sIP), iPort);
        }

        public void ConnectServer()
        {
            try
            {
                mClient.Connect(endPoint);
                ns = mClient.GetStream();
                ns.WriteTimeout = 500;
                ns.ReadTimeout = 500;
            }
            catch (Exception  ex)
            {
                throw ex;
            }
        }
        public bool IsConnected =>  mClient.Connected;
         
        public void DisConnection()
        {
            try
            {
                mClient.Close();
                ns.Close();
                ns.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void SendString(string sSend)
        {
            int iBuff = mClient.Available;
            if (iBuff > 0)
            {
                byte[] bBuff = new byte[iBuff];
                ns.Read(bBuff, 0, bBuff.Length);
            }
            try
            {
                if (sSend != "")
                {
                    byte[] bSend = Encoding.Default.GetBytes(sSend);
                    ns.Write(bSend, 0, bSend.Length);

                }
            }
            catch  (Exception ex)
            {
                throw ex;
            }
        }

        public void SendByte(byte[] content)
        {
            int iBuff = mClient.Available;
            if (iBuff > 0)
            {
                ns.Flush();
                byte[] bBuff = new byte[iBuff];
                ns.Read(bBuff, 0, bBuff.Length);
            }
            try
            {
                if (content != null && content.Length != 0)
                {
                    ns.Flush();
                    ns.Write(content, 0, content.Length);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string RecString()
        {
            try
            {
                byte[] bRec = new byte[1024];
                int lengh = bRec.Length;
                int icount = ns.Read(bRec, 0, bRec.Length);
                return Encoding.Default.GetString(bRec);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] RecByte()
        {
            try
            {
                byte[] bRec = new byte[1024];
                int icount = ns.Read(bRec, 0, bRec.Length);
                return bRec;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
