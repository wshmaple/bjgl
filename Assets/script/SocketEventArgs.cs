#region Copyright
/*--------------------------------------------------------
 * Copyright (C) 2013 山东星科智能科技股份有限公司
 * 
 * 创建标识：任鹏 - 2013.11.1 8:32:26 
 * 功能概述：Socket 通讯事件参数
 *  
 * 修改标识：
 * 修改描述：
 *
//------------------------------------------------------*/
#endregion

using System;

namespace XK.Utils
{
    #region Socket通讯委托
    /// <summary>
    /// Socket 通讯事件模型委托
    /// </summary>
    public delegate void SocketEventHandler(object sender, SocketEventArgs e);
    #endregion
    /// <summary>
    /// Socket 通讯事件参数
    /// </summary>
    public class SocketEventArgs : EventArgs
    {
        string strMsg;
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Msg
        {
            get { return strMsg; }
        }
        /// <summary>
        /// 消息类型
        /// </summary>
        public enum EEventType { Connected, Disconnected, Sended, Recved, Closed }

        /// <summary>
        /// 消息类型
        /// </summary>
        public EEventType EventType;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_strMsg">消息内容</param>
        /// <param name="_msgType">消息类型</param>
        public SocketEventArgs(string _strMsg, EEventType _eventType)
        {
            strMsg = _strMsg;
            EventType = _eventType;
        }
    }
}