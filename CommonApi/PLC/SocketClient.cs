using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;

namespace CommonApi.PLC
{
    public class SocketClient
    {
        private readonly ManualResetEvent connDone = new ManualResetEvent(false);
        private int byteLength = 8192;
        private Socket cliSocket;
        private Thread comThread;
        private bool g_brun = false;
        private readonly object qLock = new object();
        private int timeOut = 3000;
        StringBuilder strRecvMessage = new StringBuilder();

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void Close()
        {
            g_brun = false;

            if (comThread != null)
            {
                if (comThread.ThreadState == ThreadState.Running)
                {
                    comThread.Abort();
                }
                Thread.Sleep(50);
            }

            if (cliSocket != null)
            {
                if (Connected())
                {
                    cliSocket.Shutdown(SocketShutdown.Both);
                }
                cliSocket.Close();
            }
            GC.Collect();
        }

        /// <summary>
        /// 返回连接状态
        /// </summary>
        /// <returns></returns>
        public bool Connected()
        {
            if (cliSocket != null)
                return cliSocket.Connected;
            return false;
        }
        
        /// <summary>
        /// socket连接
        /// </summary>
        /// <param name="hostIp">服务端IP</param>
        /// <param name="hostPort">服务端端口</param>
        /// <returns></returns>
        public bool Connect(string hostIp, int hostPort)//异步连接
        {
            IPEndPoint remoteEp = new IPEndPoint(IPAddress.Parse(hostIp), hostPort);
            try
            {
                connDone.Reset();
                cliSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                cliSocket.SendTimeout = timeOut;
                cliSocket.ReceiveTimeout = timeOut;
                cliSocket.BeginConnect(remoteEp, new AsyncCallback(ConnCallBack), cliSocket);
                connDone.WaitOne(500, false);

                if (!cliSocket.Connected)
                {
                    return false;
                }
                g_brun = true;
                return true;
            }

            catch (SocketException ex)
            {
                cliSocket.Close();
                return false;
            }

            catch (Exception ex)
            {
                cliSocket.Close();
                return false;
            }
        }

        private void ConnCallBack(IAsyncResult ar)
        {
            try
            {
                Socket s = (Socket)ar.AsyncState;
                s.EndConnect(ar);
            }

            catch (Exception ex)
            {
                return;
            }
            finally
            {
                connDone.Set();
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="bytData">数据内容</param>
        /// <returns></returns>
        public bool Send(byte[] bytData)
        {
            try
            {
                lock (qLock)
                {
                    if (cliSocket.Connected)
                    {
                        cliSocket.Send(bytData);
                        return true;
                    }
                    else
                    {
                        Close();
                        return false;
                    }
                }
            }
            catch (SocketException ex)
            {
                Close();
                return false;
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public byte[] Recv(Socket o)
        {
            byte[] bytBuffer = new byte[byteLength];
            while (g_brun)
            {
                try
                {
                    lock (qLock)
                    {
                        if (cliSocket != null)
                        {
                            if (cliSocket.Connected)
                            {
                                for (int i = 0; i < bytBuffer.Length; i++)
                                {
                                    bytBuffer[i] = 0;
                                }
                                int nLen = 0;
                                try
                                {
                                    //if (cliSocket.Available <= 0) continue;
                                    nLen = cliSocket.Receive(bytBuffer);
                                }
                                catch (SocketException ex)
                                {
                                    Close();
                                }
                                if (nLen > 0)
                                {
                                    return bytBuffer.Skip(0).Take(nLen).ToArray();
                                }
                                else
                                {
                                    Close();
                                    break;
                                }
                            }
                        }
                    }
                    return null;
                }
                catch (SocketException ex)
                {
                    Close();
                }
            }
            return null;
        }
    }
}
