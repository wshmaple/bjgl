#region Copyright
/*--------------------------------------------------------
 * Copyright (C) 2013 山东星科智能科技股份有限公司
 * 
 * 创建标识：任鹏 - 2013.10.30 16:15:34 
 * 功能概述：Socket TCP/IP异步通信Client端辅助类
 *  
 * 修改标识：
 * 修改描述：
 *
//------------------------------------------------------*/
#endregion

using System;
using System.Net.Sockets;
using System.Text;
#pragma warning disable
namespace XK.Utils
{

    public class TcpClientHelper
    {
        #region 私有变量
        /// <summary>
        /// Socket 客户端
        /// </summary>
        Socket sckClient;

        /// <summary>
        /// 超时
        /// </summary>
        const int SCK_NETRESET = 0x10052;

        /// <summary>
        /// 编码格式
        /// </summary>
        Encoding encodingCurrent;
        #endregion

        #region 公共变量
        /// <summary>
        /// 事件 - 连接、发送、接收、关闭
        /// </summary>
        public event SocketEventHandler OnSocket;
        #endregion

        /// <summary>
        /// 初始化SocketHelper
        /// </summary>
        /// <param name="_encoding">编码格式，例如：Encoding.UTF8</param>
        public TcpClientHelper(Encoding _encoding)
        {
            encodingCurrent = _encoding;
            sckClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// 链接 Server
        /// </summary>
        /// <param name="_strIP">IP地址</param>
        /// <param name="_nPort">端口号</param>
        public void Connect(string _strIP, int _nPort)
        {
            IAsyncResult iar = sckClient.BeginConnect(_strIP, _nPort, new AsyncCallback(ConnectCallBack), sckClient);
        }
        void ConnectCallBack(IAsyncResult _iar)
        {
            Socket sock = _iar.AsyncState as Socket;
            try
            {
                sock.EndConnect(_iar);
                OnSocket(this, new SocketEventArgs(null, SocketEventArgs.EEventType.Connected));
                //如果链接Server成功，则监测接收到的数据
                Recv();
            }
            catch
            {
                OnSocket(this, new SocketEventArgs(null, SocketEventArgs.EEventType.Disconnected));
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="_strData">消息内容</param>
        public void Send(string _strMsg)
        {
           
            if (sckClient.Connected)
            {
                byte[] btSend = encodingCurrent.GetBytes(_strMsg);
                byte[] utf8 = Encoding.UTF8.GetBytes(_strMsg);
                IAsyncResult iar = sckClient.BeginSend(utf8, 0, btSend.Length, SocketFlags.None, new AsyncCallback(SendCallBack), _strMsg);
            }
        }
        void SendCallBack(IAsyncResult _iar)
        {
            sckClient.EndSend(_iar);
            OnSocket(this, new SocketEventArgs(_iar.AsyncState.ToString(), SocketEventArgs.EEventType.Sended));
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        void Recv()
        {
            if (sckClient.Connected)
            {
                byte[] btRecv = new byte[sckClient.ReceiveBufferSize];
                IAsyncResult iar = sckClient.BeginReceive(btRecv, 0, sckClient.ReceiveBufferSize, SocketFlags.None, new AsyncCallback(RecvCallBack), btRecv);
            }
        }
        void RecvCallBack(IAsyncResult _iar)
        {
            if (sckClient.Connected)
            {
                byte[] btRecv = _iar.AsyncState as byte[];
                try
                {
                    int nCount = sckClient.EndReceive(_iar);
                    if (nCount == 0)//Server主动断开链接
                    {
                        //触发 Server 端断开事件
                        OnSocket(this, new SocketEventArgs(null, SocketEventArgs.EEventType.Disconnected));
                    }
                    if (nCount > 0)
                    {
                        //触发接收消息事件
                        OnSocket(this, new SocketEventArgs(encodingCurrent.GetString(btRecv, 0, nCount), SocketEventArgs.EEventType.Recved));
                        Recv();
                    }
                }
                catch (SocketException ex)//Server 超时处理
                {
                    if (ex.ErrorCode == SCK_NETRESET)
                    {
                        //触发 Server 端断开事件
                        OnSocket(this, new SocketEventArgs(null, SocketEventArgs.EEventType.Disconnected));
                        Close();
                    }
                }
            }
        }

        /// <summary>
        /// 关闭Socket客户端
        /// </summary>
        public void Close()
        {
            if (sckClient.Connected)
            {
                //禁用发送和接收
                sckClient.Shutdown(SocketShutdown.Receive);
                //断开链接
                sckClient.Disconnect(true);
                //释放 Socket 资源
                sckClient.Close();
                OnSocket(this, new SocketEventArgs(null, SocketEventArgs.EEventType.Closed));
            }
        }
    }
}