using UnityEngine;
using System.Collections;
using XK.Utils;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class MyJsonClass
{
    public string m_strDeskNumber; // 物理座位编号
    public string m_strLocalIp; // 本地IP
    public string m_strState; // 当前状态 刚运行、获取登录用户信息、用户登录成功、答题结束显示答题结果
    public string userType; // 当前运行的客户端类型 Unity
    public string MsgType; // 发送的数据 的数据 类型 message  login
    public string socketType; // Socket类型
    public string message; // 发送的数据 
    public string targetUserType; // 接受对象 zeromq
}
public class UserInfoByIDCard
{
    public string name { get; set; }
    public string sex { get; set; }
    public string people { get; set; }
    public string birthday { get; set; }
    public string address { get; set; }
    public string number { get; set; }
    public string signdate { get; set; }
    public string validtermOfStartEnd { get; set; }
}
public class SocketWithVC : MonoBehaviour
{
    public Text sendText;
    public Text recvText;
    #region 通讯变量
    private string IP = "192.168.7.74";
    private int PortNum = 60000;
    private TcpClientHelper sckClient;
    private string recivedStr = "";
    private List<string> recdStrList = new List<string>();
    private bool reciving = false;
    private bool sending = false;
    private bool validate = false;
    // private XKINI xkini;
    private string sockInfo = "";

    private UserInfoByIDCard m_userinfo = new UserInfoByIDCard();

    string strDeskNumber = "DeskNumber1"; // 该 桌子的标签 编号 在配置文件中

    CheckJson ck = new CheckJson();
    //LoadFromWWW www = new LoadFromWWW();
    enum eDeskState
    {
        eDNULL,// 空的 刚登录 还没用
        eConnect,// Unity 连接了      但是还没有 分配给考生  学生提交试卷 显示答题卡 完毕后切会改状态 等待继续分配
        eHasUser,// 分配了考生
        eUserOder,// 考生确认信息 即 考试开始了
        eSubmit,// 考生 提交试卷
        eCount
    };
    eDeskState m_eDeskState; // 当前 机器的状态
    public static SocketWithVC sock;
    #endregion
    void Start()
    {
        sock = this;
    }

    private void SendMsgEvent(string strMsg)
    {
        Debug.Log("SocketWithVC SendMsgEvent");
        SendMsg(strMsg);
    }

    public void Connect()
    {
        sckClient = new TcpClientHelper(Encoding.UTF8);
        sckClient.OnSocket += new SocketEventHandler(sckClient_SocketEvent);
        sckClient.Connect("127.0.0.1", 9900);
    }
    void sckClient_SocketEvent(object sender, SocketEventArgs e)
    {
        switch (e.EventType)
        {
            case SocketEventArgs.EEventType.Closed:
                print("closed");
                break;
            case SocketEventArgs.EEventType.Connected:
                JObject job = new JObject();
                job.Add("key", "connect");
                JObject ju = new JObject();
                ju.Add("deskid", strDeskNumber);
                ju.Add("usertype", "Unity");
                ju.Add("state", eDeskState.eConnect.ToString());
                ju.Add("test", "我只是想在字串里面加个中文试试");
                job.Add("value", ju);
                string json = job.ToString();
              //  SendMsg(json);
                m_eDeskState = eDeskState.eConnect;
                //连接服务器成功
                SendMsg("sdf");
               // www.SendMsgEvent += new LoadFromWWW.SendMsgEventHandler(SendMsgEvent);
                break;
            case SocketEventArgs.EEventType.Disconnected:

                print("disconnected");
                break;
            case SocketEventArgs.EEventType.Recved:
                recivedStr = e.Msg;
              //  Debug.Log(recivedStr);
                recivedStr = recivedStr.Trim(' ');        // 
                recivedStr = recivedStr.Replace("\n", "");//  
                recdStrList = new List<string>();
                Debug.Log("recd:" + recivedStr);
                int nLen = recivedStr.Length;
             //   Debug.Log("nLen" + nLen);
                if (nLen > 12)
                {
                   int nPos =  recivedStr.IndexOf("#$!#@XK");//返回 0 那就是以#$!#@XK开头的字串
                //    Debug.Log(nPos);
                    if (nPos ==0)
                    {
                        string strLength = recivedStr.Substring(7, 5);
                        int length = Convert.ToInt32(strLength);
                        // 数据总 长度 是否相等 不想等 可能就是 丢包了 字串直接做 丢掉处理
                        if (nLen == length)
                        {
                            // 说明是个 完整的 合法串
                            string strYes = recivedStr.Substring(12);
                           
                            recdStrList.Add(strYes);
                       //     Debug.Log("增加。。。");
                       //     Debug.Log(strYes);
                        }
                        else
                        {
                            Debug.Log("Error string !!!----nLen------begin-----length-------");
                            Debug.Log(recivedStr);
                            Debug.Log("Error string !!!---"+nLen +"-------end----"+length+"--------");
                        }

                    }
                }
               // while (recivedStr != "")
               // {
               //     for (int i = 0; i < recivedStr.Length; i++)
               //     {
               //         if (recivedStr[i] == 'K') // 遇到字串里有 K 那么 这个 就会 假死
               //         {
               //             int length = Convert.ToInt32(recivedStr.Substring(i + 1, 5));
               //             recdStrList.Add(recivedStr.Substring(0, length));
               //              Debug.Log(recdStrList[0]);
               //             recivedStr = recivedStr.Substring(length, recivedStr.Length - length);
               //             break;
               //         }
               //     }
               // }
               
                reciving = true;
                break;
            case SocketEventArgs.EEventType.Sended:
                break;
            default:
                print("disconnected");
                break;
        }
    }
   
    void Update()
    {

        if (reciving)
        {
            for (int i = 0; i < recdStrList.Count; i++)
            {
                if (recdStrList[i].Length > 2)
                {
                    if (ck.XIsJson(recdStrList[i]))//判断 是json
                    {
                        if (recdStrList[i].Substring(0, 1) == "{")
                        {
                            ParseStr(recdStrList[i]);
                        }
                        else
                        {
                            Debug.Log(recdStrList[i]);
                        }
                    }
                   
                }
                else
                {
                    Debug.Log("非法数据");
                }
                
            }
            recdStrList.Clear();
            reciving = false;
        }
    }
    void ParseStr(string str)
    {
        //判断是否 验证成功
        //if (str.Substring(0, 9) == "CV0009001")
        //{
        //    validate = true;
        //    sockInfo = "连接验证成功！";
        //}
        //if (validate)
        //{
        //    //解析对应功能
        //}
        //else
        //{
        //    sockInfo = "连接验证失败！";
        //}

        JObject jObj = JObject.Parse(str);
        Debug.Log("解析数据");
        Debug.Log(str);
        if (jObj["key"].ToString() == "heartcheck")
        {
          //  Debug.Log("心跳包.....");
            JObject jobs = new JObject();
            jobs.Add("key", "heartcheck");
            JObject jus = new JObject();
            jus.Add("deskid", strDeskNumber);
            jus.Add("usertype", "Unity");
            jus.Add("state", m_eDeskState.ToString());
            jobs.Add("value", jus);
            string jsons = jobs.ToString();
            SendMsg(jsons);
        }
        if (jObj["key"].ToString() == "userinfo")
        {
           //m_eDeskState = eDeskState.eHasUser;
            Debug.Log(jObj.ToString());
            /// 用户 登录
            m_userinfo.name = jObj["value"]["name"].ToString();
            m_userinfo.sex = jObj["value"]["sex"].ToString();
            m_userinfo.people = jObj["value"]["people"].ToString();
            m_userinfo.birthday = jObj["value"]["birthday"].ToString();
            m_userinfo.address = jObj["value"]["address"].ToString();
            m_userinfo.number = jObj["value"]["number"].ToString();
            m_userinfo.signdate = jObj["value"]["signdate"].ToString();
            m_userinfo.validtermOfStartEnd = jObj["value"]["validtermOfStartEnd"].ToString();
            Debug.Log("本机器 已经分配给了" + m_userinfo.number);
			
			string strPaperMethod = jObj["paper"]["method"].ToString();
			string strPaperMethodValue = jObj["paper"]["value"].ToString();
			if(strPaperMethod == "random"){
				// 全局随机试卷www = new WWW(strHostSerIp + "/randpaper?userid=20170522001");
			}
			else if(strPaperMethod == "get"){
				// 根据value 值随机试卷"/getpaper?paperid=strPaperMethodValue&&userid=20170522001");
				
			}
            //
            // 给服务器 发送 状态 更新 消息
           // JObject jobs = new JObject();
           // jobs.Add("key", "upsate"); // 更新 状态
           // JObject jus = new JObject();
           // jus.Add("deskid", strDeskNumber);
           // jus.Add("usertype", "Unity");
           // jus.Add("state", eDeskState.eHasUser.ToString());
           // jobs.Add("value", jus);
           //
           // string jsons = jobs.ToString();
           // SendMsg(jsons);

            m_eDeskState = eDeskState.eHasUser;

            // 此时 用户 可以 确认 身份证 等 信息

            // 用户 点击 确认登录 按钮



        }

    }
    // 更新 当前 用户 状态  如果 不是 在 m_eDeskState 发生 改变的时候 要给服务器更新一下
    public void UpState()
    {
        JObject jobs = new JObject();
        jobs.Add("key", "upsate"); // 更新 状态
        JObject jus = new JObject();
        jus.Add("deskid", strDeskNumber);
        jus.Add("usertype", "Unity");
        jus.Add("state", m_eDeskState.ToString());
        jobs.Add("value", jus);
        string jsons = jobs.ToString();
        SendMsg(jsons);
    }
    // 登录 确认
    public void LoginYes()
    {
        // 调用www 1.加载操作点 2.加载试卷 3.加载试卷中的题目
       // www.TLoadAllPoint();

     //   www.TLoadPaper();

     //   www.LoadTiMu1();
        // 状态 改为 考试
        m_eDeskState = eDeskState.eUserOder;

        // 发送 考试的 试卷ID    
    }
    // 提交
    public void Submit()
    {
        m_eDeskState = eDeskState.eSubmit;
        // 显示 结果 界面
    }

    // 结果界面 结束后 更新 状态
    public void OverRefresh()
    {
        m_eDeskState = eDeskState.eConnect;
    }

    void OnApplicationQuit()
    {
        if (sckClient != null)
            sckClient.Close();

    }
    public void SendMsg(string msg)
    {
        int nLen = System.Text.Encoding.UTF8.GetBytes(msg).Length + 12;
        msg = "#$!#@XK" + nLen.ToString().PadLeft(5, '0') + msg;
        sckClient.Send(msg);
    }
    public void TestSendMsg1()
    {
        MyJsonClass myJsonObj = new MyJsonClass();
        myJsonObj.m_strDeskNumber = "1";
        myJsonObj.m_strLocalIp = "127.0.0.1";
        myJsonObj.m_strState = "登录";
        myJsonObj.userType = "Unity";
        myJsonObj.MsgType = "message";
        myJsonObj.socketType = "tcp";
        myJsonObj.targetUserType = "targetUserType";
        myJsonObj.message = "我测试一下发送数据";
        string json = JsonUtility.ToJson(myJsonObj);
        SendMsg(json);
    }
    void OnGUI()
    {
        GUI.Label (new Rect (Screen.width / 2 - 200, Screen.height / 2 - 50, 400, 100), sockInfo);
    }
}
