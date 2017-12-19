using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using Boo.Lang;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class LoadFromWWW : MonoBehaviour
{
    #region 变量
    WWW www;                     //请求
    WWW wwwPoint;                     //请求
    string filePath;             //保存的文件路径
    Texture2D texture2D;         //下载的图片
    public Image m_tSprite;  //场景中的一个Sprite
    Paper papers = new Paper();
    PaperManager paperManager = new PaperManager();

   // string strHostSerIp = "http://127.0.0.1:3010";

    string strHostSerIp = "http://192.168.7.74:3010";

    string strDeskNumber = "DeskNumber1"; // 该 桌子的标签 编号 在配置文件中

    // 操作点 列表
    List<Points> pointslist = new List<Points>();

    // 给服务器 发送消息
    public delegate void SendMsgEventHandler(string strMsg);
    public event SendMsgEventHandler SendMsgEvent;
    #endregion
    void Start ()
    {
	
	}
	// Update is called once per frame
	void Update ()
    {
    }
    // 调试用接口
    #region 测试函数
    
    public void ShowMsg()
    {
        ShowPaperMsg();
    }
    public void LoadTiMu1()
    {
        LoadPaperById(1);
    }
    public void LoadTiMu2()
    {
        LoadPaperById(2);
    }
    public void LoadTiMu3()
    {
        LoadPaperById(3);
    }
    public void CheckSteps1()
    {
        //LoadPaperById(3);
        CheckSteps("yanfeng", "yancong", "1");
    }
    public void CheckSteps2()
    {
        //LoadPaperById(3);
        CheckSteps("yanfeng", "yinfengji", "1");
    }
    public void CheckSteps3()
    {
        //LoadPaperById(3);
        CheckSteps("yanfeng", "tuoliushebei", "1");
    }
    public void CheckSteps4()
    {
        //LoadPaperById(3);
        CheckSteps("yanfeng", "chuchenshebei", "1");
    }
    public void CheckSteps5()
    {
        //LoadPaperById(3);
        CheckSteps("yanfeng", "yandao", "1");
    }
    public void CheckSteps6()
    {
        //LoadPaperById(3);
        CheckSteps("yanfeng", "tiaofengmen4", "1");
    }
    public void CheckSteps7()
    {
        //LoadPaperById(3);
        CheckSteps("yanfeng", "tiaofengmen3", "1");
    }
    public void CheckSteps8()
    {
        //LoadPaperById(3);
        CheckSteps("yanfeng", "tiaofengmen2", "1");
    }
    public void CheckSteps9()
    {
        //LoadPaperById(3);
        CheckSteps("yanfeng", "tiaofengmen1", "1");
    }
    public void CheckSteps10()
    {
        //LoadPaperById(3);
        CheckSteps("yanfeng", "fengdao", "1");
    }
    public void CheckSteps11()
    {
        //LoadPaperById(3);
        CheckSteps("yanfeng", "chuchenshebei", "1");
    }

    public void CheckSteps21()
    {
        //LoadPaperById(3);
        CheckSteps("cock", "t_fangshuixuansai", "1");
    }

    public void CheckSteps22()
    {
        //LoadPaperById(3);
        CheckSteps("cock", "t_shuixuansai", "0");
    }
    public void CheckSteps23()
    {
        //LoadPaperById(3);
        CheckSteps("cock", "t_shuixuansai", "1");
    }
    public void CheckSteps24()
    {
        //LoadPaperById(3);
        CheckSteps("cock", "t_qixuansai", "0");
    }
    public void CheckSteps25()
    {
        //LoadPaperById(3);
        CheckSteps("cock", "t_qixuansai", "1");
    }
    public void CheckSteps26()
    {
        //LoadPaperById(3);
        CheckSteps("cock", "t_fangshuixuansai", "0");
    }
 
    void ShowPaperMsg()
    {
        Debug.Log("—————————————ShowPaperMsg_________________");
        string strPaper = "nBigStepIndex: " + paperManager.nBigStepIndex + " nBigStepLevel :" + paperManager.nBigStepLevel +
            " nSmallStepIndex " + paperManager.nSmallStepIndex + " nSmallStepLevel " + paperManager.nSmallStepLevel + ""
            + "";
        Debug.Log(strPaper);
        foreach (var s in papers.data)
        {
            Debug.Log(s.t);// t1 \ t2 \t3
            Debug.Log(s.boilertype.ToString());// 锅炉类型 中文
            Debug.Log(s.opertype.ToString()); // 操作类型 认知、运行、事故
            Debug.Log(s.t1.hard); // 难度
            Debug.Log(s.t1.remark); // 备注
          //  Debug.Log()

            // 初始化
            foreach(var initStep in s.t1.init)
            {
             //   Debug.Log(initStep.type_operation);// 类型
              //  Debug.Log(initStep.key);// key
              //  Debug.Log(initStep.value);// value
            }
            // 步骤
            // 大步骤
            foreach (var bigstep in s.t1.step)
            {
               // Debug.Log(bigstep.blevel);
                // 小步骤
                foreach(var sStep in bigstep.step)
                {
                    //Debug.Log(sStep.level);
                   // Debug.Log(sStep.type_operation);
                   // Debug.Log(sStep.key);
                   // Debug.Log(sStep.value);
                    string strmsg = bigstep.blevel +"-"+ sStep.level + ":" + sStep.type_operation +" "+ sStep.key + sStep.value +" bDone "+ sStep.bDone;
                    Debug.Log(strmsg);
                    // 小步骤的 特效
                    foreach (var tx in sStep.texiao)
                    {
                       // Debug.Log(tx.type_operation);
                      //  Debug.Log(tx.key);
                      //  Debug.Log(tx.value);
                    }
                }
            }
        }
    }
    // 显示操作点
    public void ShowPoints()
    {

        foreach(var s in pointslist)
        {
           Debug.Log( s.ShowMsg());
        }
    }
    #endregion
    #region Unity接口
    public void TLoadAllPoint()
    {
        StartCoroutine(LoadAllPoint());
    }
    public void TLoadPaper()
    {
        StartCoroutine(LoadPaper());
    }
    public void TRandPaper()
    {
        StartCoroutine(RandPaper());
    }
    // 指定 加载 试卷 事故题目
    IEnumerator LoadPaper()
    {
        //开始下载图片
        www = new WWW(strHostSerIp + "/getpaper?paperid=66&&userid=20170522001");
        yield return www;


        //下载完成，保存图片到路径filePath
        //  texture2D = www.texture;
        //byte[] bytes = texture2D.EncodeToJPG();
        //File.WriteAllBytes(filePath, bytes);
        //File.WriteAllBytes

        //将图片赋给场景上的Sprite
        //  Sprite tempSp = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0));
        //  m_tSprite.sprite = tempSp;
        // www.text;
        Debug.Log("text");
        Debug.Log(www.text);
        // UnityEditor.EditorJsonUtility(www.text);
        string str = www.text;
    //    Paper rt = JsonUtility.FromJson<Paper>(str) as Paper;
     //   Debug.Log(rt.code);
        Debug.Log("方法二");
        // Root Object = JsonConvert.DeserializeObject<Root>(str);

        //  Debug.Log(Object.code);
        JObject jObj = JObject.Parse(str);

        if (jObj["code"].ToString() == "0")
        {
            Debug.Log("试卷加载成功!");

            papers.code = jObj["code"].ToString();
            papers.scoreid = jObj["scoreid"].ToString();
            papers.paperid = jObj["paperid"].ToString();
            papers.userid = jObj["userid"].ToString();
			
			papers.papertime = jObj["papertime"].ToString();
            papers.paperremark = jObj["paperremark"].ToString();
            papers.papercaption = jObj["papercaption"].ToString();
			
            foreach (var k in jObj["data"])
            {
                Debug.Log(k);
                //k["t"].ToString();
                DataItem data = new DataItem();
                data.t = k["t"].ToString();
                data.boilertype = k["boilertype"].ToString();
                data.opertype = k["opertype"].ToString();
                T1 t1 = new T1();
                // JToken jk = k["t1"];
                t1.caption = k["t1"]["caption"].ToString();
                t1.boilertype.key = k["t1"]["boilertype"]["key"].ToString();
                t1.boilertype.caption = k["t1"]["boilertype"]["caption"].ToString();
                t1.boileropertype.key = k["t1"]["boileropertype"]["key"].ToString();
                t1.boileropertype.caption = k["t1"]["boileropertype"]["caption"].ToString();
                t1.opertype.key = k["t1"]["opertype"]["key"].ToString();
                t1.opertype.caption = k["t1"]["opertype"]["caption"].ToString();
                t1.hard.key = k["t1"]["hard"]["key"].ToString();
                t1.hard.caption = k["t1"]["hard"]["caption"].ToString();
                t1.remark = k["t1"]["remark"].ToString();
                data.t1 = t1;

                data.nStepsCount = 0;
                data.nScore = 0;
                data.fStepScore = 0.0f;
                data.fUserScore = 0;
                // 设置 题目 在试卷中的 占的 分值
                if (t1.opertype.key == "type_renzhi") // 认知
                {
                    data.nScore = 20;
                }
                else if (t1.opertype.key == "type_yunxing") // 运行题目
                {
                    data.nScore = 30;
                }
                else if (t1.opertype.key == "type_shigu") // 事故题目
                {
                    data.nScore = 50;
                }
                //   Debug.Log(k["t1"]["boilertype"]["key"].ToString());
                papers.data.Add(data);
                foreach (var step in k["t1"]["step"])
                {
                    //    Debug.Log(step);
                    StepItemBig bigStep = new StepItemBig();
                    data.t1.step.Add(bigStep);
                    bigStep.blevel = int.Parse(step["blevel"].ToString());
                    foreach (var s in step["step"])
                    {
                        StepItem sStep = new StepItem();
                        bigStep.step.Add(sStep);
                        sStep.level = int.Parse(s["level"].ToString());
                        sStep.type_operation = s["type_operation"].ToString();
                        sStep.key = s["key"].ToString();
                        sStep.value = s["value"].ToString();
                        data.nStepsCount++;
                        string strValues = s.ToString();

                        //  Debug.Log(step["blevel"].ToString() + s["level"].ToString() +
                        //      s["type_operation"].ToString() + s["key"].ToString() + s["value"].ToString());
                        foreach (var tx in s["texiao"])
                        {
                            StepItemTeXiao texiao = new StepItemTeXiao();
                            sStep.texiao.Add(texiao);
                            texiao.key = tx["key"].ToString();
                            texiao.type_operation = tx["type_operation"].ToString();
                            texiao.value = tx["value"].ToString();
                        }
                    }
                }
                foreach (var inStep in k["t1"]["init"])
                {
                    InitItem itm = new InitItem();
                    itm.key = inStep["key"].ToString();
                    itm.type_operation = inStep["type_operation"].ToString();
                    itm.value = inStep["value"].ToString();
                    data.t1.init.Add(itm);
                }

                // 计算一下 每个步骤的 评价分值
                data.fStepScore = data.nScore / data.nStepsCount;

            }
            Debug.Log("papers.ToString()!");
            //  Debug.Log(papers.ToString());
        }
        else
        {
            Debug.Log("试卷加载失败!");
        }

        Debug.Log(papers.ToString());
        Debug.Log("加载完成");
        LoadPaperOK();
        ShowPaperMsg();

    }
    //随机 加载 试卷
    IEnumerator RandPaper()
    {
        //开始下载图片
        www = new WWW(strHostSerIp + "/randpaper?userid=20170522001");
        yield return www;


        //下载完成，保存图片到路径filePath
        //  texture2D = www.texture;
        //byte[] bytes = texture2D.EncodeToJPG();
        //File.WriteAllBytes(filePath, bytes);
        //File.WriteAllBytes

        //将图片赋给场景上的Sprite
        //  Sprite tempSp = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0, 0));
        //  m_tSprite.sprite = tempSp;
        // www.text;
        Debug.Log("text");
        Debug.Log(www.text);
        // UnityEditor.EditorJsonUtility(www.text);
        string str = www.text;
        Paper rt = JsonUtility.FromJson<Paper>(str) as Paper;
        Debug.Log(rt.code);
        Debug.Log("方法二");
        // Root Object = JsonConvert.DeserializeObject<Root>(str);

        //  Debug.Log(Object.code);
        JObject jObj = JObject.Parse(str);

        if (jObj["code"].ToString() == "0")
        {
            Debug.Log("试卷加载成功!");

            papers.code = jObj["code"].ToString();
            papers.scoreid = jObj["scoreid"].ToString();
            papers.paperid = jObj["paperid"].ToString();
            papers.userid = jObj["userid"].ToString();
			
            papers.papertime = jObj["papertime"].ToString();
            papers.paperremark = jObj["paperremark"].ToString();
            papers.papercaption = jObj["papercaption"].ToString();
            foreach (var k in jObj["data"])
            {
                Debug.Log(k);
                //k["t"].ToString();
                DataItem data = new DataItem();
                data.t = k["t"].ToString();
                data.boilertype = k["boilertype"].ToString();
                data.opertype = k["opertype"].ToString();
                T1 t1 = new T1();
                // JToken jk = k["t1"];
                t1.caption = k["t1"]["caption"].ToString();
                t1.boilertype.key = k["t1"]["boilertype"]["key"].ToString();
                t1.boilertype.caption = k["t1"]["boilertype"]["caption"].ToString();
                t1.boileropertype.key = k["t1"]["boileropertype"]["key"].ToString();
                t1.boileropertype.caption = k["t1"]["boileropertype"]["caption"].ToString();
                t1.opertype.key = k["t1"]["opertype"]["key"].ToString();
                t1.opertype.caption = k["t1"]["opertype"]["caption"].ToString();
                t1.hard.key = k["t1"]["hard"]["key"].ToString();
                t1.hard.caption = k["t1"]["hard"]["caption"].ToString();
                t1.remark = k["t1"]["remark"].ToString();
                data.t1 = t1;
                data.nStepsCount = 0;
                data.nScore = 0;
                data.fStepScore = 0.0f;
                data.fUserScore = 0;
                // 设置 题目 在试卷中的 占的 分值
                if (t1.opertype.key == "type_renzhi") // 认知
                {
                    data.nScore = 20;
                }
                else if (t1.opertype.key == "type_yunxing") // 运行题目
                {
                    data.nScore = 30;
                }
                else if (t1.opertype.key == "type_shigu") // 事故题目
                {
                    data.nScore = 50;
                }
                //   Debug.Log(k["t1"]["boilertype"]["key"].ToString());
                papers.data.Add(data);

                foreach (var step in k["t1"]["step"])
                {
                    //    Debug.Log(step);
                    StepItemBig bigStep = new StepItemBig();
                    data.t1.step.Add(bigStep);
                    bigStep.blevel = int.Parse(step["blevel"].ToString());
                    foreach (var s in step["step"])
                    {
                        StepItem sStep = new StepItem();
                        bigStep.step.Add(sStep);
                        sStep.level = int.Parse(s["level"].ToString());
                        sStep.type_operation = s["type_operation"].ToString();
                        sStep.key = s["key"].ToString();
                        sStep.value = s["value"].ToString();
                        data.nStepsCount++;
                        string strValues = s.ToString();
                        //  Debug.Log(step["blevel"].ToString() + s["level"].ToString() +
                        //      s["type_operation"].ToString() + s["key"].ToString() + s["value"].ToString());
                        foreach (var tx in s["texiao"])
                        {
                            StepItemTeXiao texiao = new StepItemTeXiao();
                            sStep.texiao.Add(texiao);
                            texiao.key = tx["key"].ToString();
                            texiao.type_operation = tx["type_operation"].ToString();
                            texiao.value = tx["value"].ToString();
                        }
                    }
                }
                foreach (var inStep in k["t1"]["init"])
                {
                    InitItem itm = new InitItem();
                    itm.key = inStep["key"].ToString();
                    itm.type_operation = inStep["type_operation"].ToString();
                    itm.value = inStep["value"].ToString();
                    data.t1.init.Add(itm);
                }
                // 计算一下 每个步骤的 评价分值
                data.fStepScore = data.nScore / data.nStepsCount;

            }
            Debug.Log("papers.ToString()!");
            //  Debug.Log(papers.ToString());
        }
        else
        {
            Debug.Log("试卷加载失败!");
        }

        Debug.Log(papers.ToString());
        Debug.Log("加载完成");
        LoadPaperOK();
        ShowPaperMsg();

    }
    // 加载 所有炉型的 操作点
    IEnumerator LoadAllPoint()
    {
        wwwPoint = new WWW(strHostSerIp + "/gettreelast");
        yield return wwwPoint;
        string str = wwwPoint.text;
        JObject jObj = JObject.Parse(str);

        if (jObj["code"].ToString() == "0")
        {
            Debug.Log("操作点加载成功!");
            Debug.Log(jObj["data"][0]["json"].ToString());
            // 2017年10月10日 17:05:34 明天写这个解析类 分析出 key value 的中文名称
            foreach (var s in jObj["data"][0]["json"]["items"])
            {

                //Debug.Log(s);
                // 锅炉类型
                // ps.boilertype.key = s["key"].ToString();
                //  ps.boilertype.caption = s["caption"].ToString();
                foreach (var m in s["items"])
                {
                    // 操作类 还是 认知类 如操作类
                    // m["key"].ToString();
                    // m["caption"].ToString();
                    //  ps.type.key = m["key"].ToString();
                    //  ps.type.caption = m["caption"].ToString();
                    foreach (var n in m["items"])
                    {
                        // 动画 特效 仪表 阀门 如阀门
                        //   ps.items.key =  n["key"].ToString();
                        //   ps.items.caption =  n["caption"].ToString();

                        foreach (var k in n["items"])
                        {
                            // 具体 操作部件 如 放空阀
                            //    ps.item.key =  k["key"].ToString();
                            //     ps.item.caption = k["caption"].ToString();
                            foreach (var p in k["items"])
                            {
                                // 具体 操作部件 动作及值 如 放空阀 打开 0 
                                Points ps = new Points();

                                ps.boilertype.key = s["key"].ToString();
                                ps.boilertype.caption = s["caption"].ToString();

                                ps.type.key = m["key"].ToString();
                                ps.type.caption = m["caption"].ToString();

                                ps.items.key = n["key"].ToString();
                                ps.items.caption = n["caption"].ToString();


                                ps.item.key = k["key"].ToString();
                                ps.item.caption = k["caption"].ToString();


                                ps.value.key = p["value"].ToString(); // 此处是 value 值 0、1、2等
                                ps.value.caption = p["caption"].ToString();

                                // 到此处 是不是就可以 放入到 列表里面了
                                pointslist.Add(ps);
                            }
                        }
                    }
                }
            }
        }

    }
    // 加载 题目n( 1,2,3)
    void LoadPaperById(int n)
    { 
        // 初始化
        InitTiMuById(n);
    }
    void InitTiMuById(int n)
    {
        Debug.Log("加载题目:"+n);
        paperManager.strIndex = "t" + n;
        paperManager.nIndex = n;
        paperManager.nBigStepIndex = 0;
        paperManager.nSmallStepIndex = 0;
        paperManager.nBigStepLevel = 1;
        paperManager.nSmallStepLevel = 1;
        paperManager.bFrist = true;
        paperManager.bJustOneBigOver = false;
        string strIndex = "t" + n;
        // 查找题目
        foreach (var s in papers.data)
        {
            if(s.t == strIndex)
            {
                Debug.Log("锅炉类型：" + s.boilertype + "操作类型:" + s.opertype + "难度等级:"+ s.t1.hard.caption+
                    "题目名称："+s.t1.caption);
                //Debug.Log(s.t1.hard.caption);
                // 初始化
                foreach (var initStep in s.t1.init)
                {
                    // 此处对 UI数据 进行初始化
                    Debug.Log(initStep.type_operation);// 类型
                    Debug.Log(initStep.key);// key
                    Debug.Log(initStep.value);// value
                }
            }
        }
        Debug.Log("加载题目: 完成！！");

    }
    void LoadPaperOK()
    {
        // 发送操作 记录 给服务器 或者 给数据库
        // string type_operation, string key, string value    
        JObject job = new JObject();
        job.Add("key", "paperok");
        JObject ju = new JObject();
        ju.Add("userid", papers.userid);
        ju.Add("paperid", papers.paperid);
        ju.Add("deskid", strDeskNumber);
        ju.Add("scoreid", papers.scoreid);
        job.Add("value", ju);  
        string strMsg = job.ToString();
        Debug.Log("LoadPaperOK SendMsgEvent");
        SendUserOperToServer(strMsg);
    }
    void UITeXiao(string type_operation, string key, string value)
    {
        // 此处 处理 特效
    }
    
    int CheckSteps(string type_operation, string key, string value)
    {
        bool bOk = XCheckSteps(type_operation, key, value);
        // 发送 操作记录
        SendOper(bOk, type_operation, key, value);


        // 判断 当前 题目 是否 结束了

        bool bOver = IsTiMuOver();
        if (bOver)
        {
            Debug.Log("本题目回答完毕！");
        }
        if (IsPaperOver())
        {
            Debug.Log("本试卷 回答完毕！");
        }

        return 0;
    }
    void SendUserOperToServer(string strMsg)
    {
        SocketWithVC.sock.SendMsg(strMsg);
    }
    void SendOper(bool bOk, string type_operation, string key, string value)
    {
        string strOk = "";
        float fUserGetScore = 0.0f;
        if (bOk)
        {
            // 作对了
            fUserGetScore = papers.data[paperManager.nIndex].fStepScore;
            strOk = "正确";
        }
        else
        {
            // 做错了
            strOk = "错误";
        }
        Debug.Log(strOk);
        string strOperCaption = GetChineseCaption(type_operation, key, value);
        string boilertype = "";
        string boileropertype = "";
        string opertype = "";
        boilertype = papers.data[paperManager.nIndex].t1.boilertype.key;
        boileropertype = papers.data[paperManager.nIndex].t1.boileropertype.key;
        opertype = papers.data[paperManager.nIndex].t1.opertype.key;
        JObject job = new JObject();
        job.Add("key", "useroper");
        JObject joper = new JObject();
        JObject ju = new JObject();
        ju.Add("userid", papers.userid);
        ju.Add("paperid", papers.paperid);
        ju.Add("deskid", strDeskNumber);
        ju.Add("scoreid", papers.scoreid);
        joper.Add("boileropertype", boileropertype);// 认知类、操作类
        joper.Add("opertype", opertype);// 认知 运行 事故
        joper.Add("boilertype", boilertype);
        joper.Add("type_operation", type_operation);
        joper.Add("key", key);
        joper.Add("value", value);
        joper.Add("bUserAnswerRight", strOk);
        joper.Add("strOperCaption", strOperCaption);
        joper.Add("userHasScore", papers.data[paperManager.nIndex].fUserScore); // 用户 已经 得到的分值
        joper.Add("fUserGetScore", fUserGetScore); // 用户 当前步骤 得到的分值
        job.Add("other", ju);
        job.Add("value", joper);
        string strMsg = job.ToString();
        Debug.Log(strMsg);
        // 调用 发送接口
        SendUserOperToServer(strMsg);
    }
    #endregion
    #region 内核 逻辑
    string GetChineseCaption(string type_operation, string key, string value)
    {
        string boilertype = "";
        string boileropertype = "";
        string opertype = "";
        boilertype = papers.data[paperManager.nIndex].t1.boilertype.key;
        boileropertype = papers.data[paperManager.nIndex].t1.boileropertype.key;
        opertype = papers.data[paperManager.nIndex].t1.opertype.key;
        string strOperCaption = "";
        foreach (var ss in pointslist)
        {
            if (ss.CheckPoint(boilertype, boileropertype, type_operation, key, value))
            {
                if (opertype != "type_renzhi") // 认知
                {
                    strOperCaption = ss.GetOperCaption();
                }
                else
                {
                    strOperCaption = ss.GetRenZhiCaption();
                }
                break;
            }
        }
        return strOperCaption;
    }
    bool JustCheck(string type_operation, string key, string value)
    {
        bool bOk = false;
        for (int n = 0; n < papers.data.Count; n++)
        {
            if (paperManager.strIndex == papers.data[n].t)
            {
                paperManager.nIndex = n;
                // 大步骤
                for (int m = 0; m < papers.data[n].t1.step.Count; m++)
                {
                    // Debug.Log(papers.data[n].t1.step[m].blevel);
                    // 判断 是不是 当前 要判断的
                    if (papers.data[n].t1.step[m].blevel == paperManager.nBigStepLevel &&
                        !papers.data[n].t1.step[m].bDone)
                    {
                        // 小步骤
                        for (int i = 0; i < papers.data[n].t1.step[m].step.Count; i++)
                        {
                            if (
                                papers.data[n].t1.step[m].step[i].level == paperManager.nSmallStepLevel &&
                                !papers.data[n].t1.step[m].step[i].bDone)
                            {
                                if (papers.data[n].t1.opertype.key == "type_renzhi") // 认知
                                {
                                    if (type_operation == papers.data[n].t1.step[m].step[i].type_operation &&
                                     key == papers.data[n].t1.step[m].step[i].key)
                                    {
                                        // 是该步骤的消息
                                        // 设置当前步骤 已经做了
                                        papers.data[n].t1.step[m].step[i].bDone = true;
                                        if (paperManager.bFrist)
                                        {
                                            paperManager.bFrist = false;
                                            paperManager.nBigStepIndex = m; // 锁定 当前 List的索引
                                        }
                                        if (paperManager.bJustOneBigOver)
                                        {
                                            paperManager.bJustOneBigOver = false;
                                            paperManager.nBigStepIndex = m; // 锁定 当前 List的索引
                                        }
                                        if (paperManager.bBigHasMore)
                                        {
                                            paperManager.bLockIndex = true;
                                        }
                                        Debug.Log("答对了一个步骤"+ type_operation+ key + value);

                                        bOk = true;
                                     //   bUserAnswerRight = 1;
                                        // 计算分值  
                                        papers.data[n].fUserScore = papers.data[n].fUserScore + papers.data[n].fStepScore;
                                        //   fUserThisStepScore = papers.data[n].fStepScore;
                                        // 小步骤的 特效
                                        foreach (var tx in papers.data[n].t1.step[m].step[i].texiao)
                                        {
                                            Debug.Log(tx.type_operation);
                                            Debug.Log(tx.key);
                                            Debug.Log(tx.value);
                                            UITeXiao(tx.type_operation, tx.key, tx.value);
                                        }
                                    }
                                }
                                else
                                {
                                    if (type_operation == papers.data[n].t1.step[m].step[i].type_operation &&
                                     key == papers.data[n].t1.step[m].step[i].key &&
                                     value == papers.data[n].t1.step[m].step[i].value)
                                    {
                                        // 是该步骤的消息
                                        // 设置当前步骤 已经做了
                                        papers.data[n].t1.step[m].step[i].bDone = true;
                                        if (paperManager.bFrist)
                                        {
                                            paperManager.bFrist = false;
                                            paperManager.nBigStepIndex = m; // 锁定 当前 List的索引
                                        }
                                        if (paperManager.bJustOneBigOver)
                                        {
                                            paperManager.bJustOneBigOver = false;
                                            paperManager.nBigStepIndex = m; // 锁定 当前 List的索引
                                        }
                                        if (paperManager.bBigHasMore)
                                        {
                                            paperManager.bLockIndex = true;
                                        }
                                        // paperManager.bFrist = false;
                                        //  paperManager.nBigStepIndex = m;
                                        Debug.Log("答对了一个步骤" + type_operation + key + value);
                                        //   bUserAnswerRight = 1;
                                        bOk = true;
                                        // 计算分值  
                                        papers.data[n].fUserScore = papers.data[n].fUserScore + papers.data[n].fStepScore;
                                     //  fUserThisStepScore = papers.data[n].fStepScore;
                                        // 小步骤的 特效
                                        foreach (var tx in papers.data[n].t1.step[m].step[i].texiao)
                                        {
                                            Debug.Log(tx.type_operation);
                                            Debug.Log(tx.key);
                                            Debug.Log(tx.value);
                                            UITeXiao(tx.type_operation, tx.key, tx.value);
                                        }
                                    }
                                }

                            }

                        }
                    }

                }
            }
        }
        return bOk;
    }
    bool JustCheckLockIndex(string type_operation, string key, string value)
    {
       bool  bOk = false;
        for (int n = 0; n < papers.data.Count; n++)
        {
            if (paperManager.strIndex == papers.data[n].t)
            {
                paperManager.nIndex = n;
                // 大步骤
                for (int m = 0; m < papers.data[n].t1.step.Count; m++)
                {
                    // Debug.Log(papers.data[n].t1.step[m].blevel);
                    // 判断 是不是 当前 要判断的
                    if (paperManager.nBigStepIndex == m &&
                        papers.data[n].t1.step[m].blevel == paperManager.nBigStepLevel &&
                        !papers.data[n].t1.step[m].bDone)
                    {
                        // 小步骤
                        for (int i = 0; i < papers.data[n].t1.step[m].step.Count; i++)
                        {
                            if (
                                papers.data[n].t1.step[m].step[i].level == paperManager.nSmallStepLevel &&
                                !papers.data[n].t1.step[m].step[i].bDone)
                            {
                                if (papers.data[n].t1.opertype.key == "type_renzhi") // 认知
                                {
                                    if (type_operation == papers.data[n].t1.step[m].step[i].type_operation &&
                                     key == papers.data[n].t1.step[m].step[i].key)
                                    {
                                        // 是该步骤的消息
                                        // 设置当前步骤 已经做了
                                        papers.data[n].t1.step[m].step[i].bDone = true;
                                        if (paperManager.bFrist)
                                        {
                                            paperManager.bFrist = false;
                                            paperManager.nBigStepIndex = m; // 锁定 当前 List的索引
                                        }
                                        if (paperManager.bJustOneBigOver)
                                        {
                                            paperManager.bJustOneBigOver = false;
                                            paperManager.nBigStepIndex = m; // 锁定 当前 List的索引
                                        }
                                        Debug.Log("答对了一个步骤" + type_operation + key + value);
                                        bOk = true;
                                        //   bUserAnswerRight = 1;
                                        // 计算分值  
                                        papers.data[n].fUserScore = papers.data[n].fUserScore + papers.data[n].fStepScore;
                                        //   fUserThisStepScore = papers.data[n].fStepScore;
                                        // 小步骤的 特效
                                        foreach (var tx in papers.data[n].t1.step[m].step[i].texiao)
                                        {
                                            Debug.Log(tx.type_operation);
                                            Debug.Log(tx.key);
                                            Debug.Log(tx.value);
                                            UITeXiao(tx.type_operation, tx.key, tx.value);
                                        }
                                    }
                                }
                                else
                                {
                                    if (type_operation == papers.data[n].t1.step[m].step[i].type_operation &&
                                     key == papers.data[n].t1.step[m].step[i].key &&
                                     value == papers.data[n].t1.step[m].step[i].value)
                                    {
                                        // 是该步骤的消息
                                        // 设置当前步骤 已经做了
                                        papers.data[n].t1.step[m].step[i].bDone = true;
                                        if (paperManager.bFrist)
                                        {
                                            paperManager.bFrist = false;
                                            paperManager.nBigStepIndex = m; // 锁定 当前 List的索引
                                        }
                                        if (paperManager.bJustOneBigOver)
                                        {
                                            paperManager.bJustOneBigOver = false;
                                            paperManager.nBigStepIndex = m; // 锁定 当前 List的索引
                                        }
                                        // paperManager.bFrist = false;
                                        //  paperManager.nBigStepIndex = m;
                                        Debug.Log("答对了一个步骤" + type_operation + key + value);
                                        //   bUserAnswerRight = 1;
                                        bOk = true;
                                        // 计算分值  
                                        papers.data[n].fUserScore = papers.data[n].fUserScore + papers.data[n].fStepScore;
                                        //  fUserThisStepScore = papers.data[n].fStepScore;
                                        // 小步骤的 特效
                                        foreach (var tx in papers.data[n].t1.step[m].step[i].texiao)
                                        {
                                            Debug.Log(tx.type_operation);
                                            Debug.Log(tx.key);
                                            Debug.Log(tx.value);
                                            UITeXiao(tx.type_operation, tx.key, tx.value);
                                        }
                                    }
                                }

                            }

                        }
                    }

                }
            }
        }
        return bOk;
    }
    bool XCheckSteps(string type_operation, string key, string value)
    {
        bool bOk = false;
        if(paperManager.bLockIndex)
        {
            bOk= JustCheckLockIndex(type_operation, key, value);
        }
        else
        {
            bOk= JustCheck(type_operation, key, value);
        }
        CheckBigStep();
        return bOk;
    }
    void CheckBigStep()
    {
        int aNoDone = 0;
        for (int k = 0; k < papers.data[paperManager.nIndex].t1.step[paperManager.nBigStepIndex].step.Count; k++)
        {
            if (!papers.data[paperManager.nIndex].t1.step[paperManager.nBigStepIndex].step[k].bDone)
            {
                aNoDone++;
            }
        }
 
        if(aNoDone==0)
        {
            //  大步骤 做完了
            papers.data[paperManager.nIndex].t1.step[paperManager.nBigStepIndex].bDone = true;
            paperManager.nSmallStepLevel = 1;
            paperManager.nSmallStepIndex = 0;
            paperManager.bJustOneBigOver = true;
            paperManager.bLockIndex = false;
            // 获取 最小的 大步骤 优先级
            ChecKBigLevel();
        }
        else
        {
            // 大步骤 没有 做完 只是 调整 小步骤的 最小优先级
            int nMinLeve = -1;
            for (int k = 0; k < papers.data[paperManager.nIndex].t1.step[paperManager.nBigStepIndex].step.Count; k++)
            {
                if (!papers.data[paperManager.nIndex].t1.step[paperManager.nBigStepIndex].step[k].bDone)
                {
                    int nLevel = papers.data[paperManager.nIndex].t1.step[paperManager.nBigStepIndex].step[k].level;
                    if (nMinLeve == -1)
                    {
                        nMinLeve = nLevel;
                    }
                    else
                    {
                        nMinLeve = nMinLeve < nLevel ? nMinLeve : nLevel;
                    }
                }
            }
            if(-1 != nMinLeve)
            {
                paperManager.nSmallStepLevel = nMinLeve;
            }
           // Debug.Log("调整后的 小优先级 为 ：" + paperManager.nSmallStepLevel);
        }

    }
    void ChecKBigLevel()
    {
        // 选择 命中
        // 从当前列表中选择出优先级的数值最小的大步骤
        // 先检查 当前 优先级 是否 存在 大于 1个
        int nLevel = -1;
        for (int m = 0; m < papers.data[paperManager.nIndex].t1.step.Count; m++)
        {
            if (!papers.data[paperManager.nIndex].t1.step[m].bDone)
            {
                if (nLevel == -1)
                {
                    nLevel = papers.data[paperManager.nIndex].t1.step[m].blevel;
                }
                else
                {
                    nLevel = nLevel < papers.data[paperManager.nIndex].t1.step[m].blevel ? nLevel : papers.data[paperManager.nIndex].t1.step[m].blevel;
                }
            }

        }
        // 找出当前 最小优先级的 大步骤的 个数
        if (nLevel != -1)
        {
            paperManager.nBigStepLevel = nLevel;
            int nBigLeveHasMore = 0;
            for (int m = 0; m < papers.data[paperManager.nIndex].t1.step.Count; m++)
            {
                if (!papers.data[paperManager.nIndex].t1.step[m].bDone &&
                    nLevel == papers.data[paperManager.nIndex].t1.step[m].blevel)
                {
                    nBigLeveHasMore++;
                }
            }
            if (nBigLeveHasMore > 1)
            {
                paperManager.bBigHasMore = true; // 此处 决定了 他的 index 是需要 选择的
            }
        }
        else
        {

        }
    }
    bool IsTiMuOver()
    {
        int aNoDone = 0;
        for (int k = 0; k < papers.data[paperManager.nIndex].t1.step.Count; k++)
        {
            if (!papers.data[paperManager.nIndex].t1.step[k].bDone)
            {
                aNoDone++;
            }
        }
        if (aNoDone == 0)
        {
            papers.data[paperManager.nIndex].bDone = true;
            return true;
        }
        return false;
    }
    bool IsPaperOver()
    {
        int nCount = 0;
         for(int k = 0; k < 3; k++)
        {
            if (papers.data[k].bDone)
            {
                nCount++;
            }
        }
        Debug.Log("已经答完的题目个数为:" + nCount);
         if(nCount == 3)
        {
            return true;
        }
        return false;
    }
    #endregion
    #region 试卷存储
    // 最终 要提交的 试卷
    public void SubmitPaper()
    {
        JObject job = new JObject();
        job.Add("key", "submit");
        JObject joper = new JObject();
        joper.Add("userid", papers.userid);
        joper.Add("paperid", papers.paperid);
        joper.Add("deskid", strDeskNumber);
        joper.Add("scoreid", papers.scoreid);
        job.Add("other", joper);
        float fTotalScore = 0.0f;
        int nI = 0;
        JObject jVaule = new JObject();
        foreach (var s in papers.data)
        {
            nI++;
            fTotalScore += s.fUserScore;
            JObject jPaper = new JObject();
            jPaper.Add("boilertype", s.boilertype);
            jPaper.Add("opertype", s.opertype);
            jPaper.Add("caption", s.t1.caption);
            jPaper.Add("hard", s.t1.hard.caption);
            jPaper.Add("fUserScore", s.fUserScore);

            // job.Add(jPaper);
            jVaule.Add(s.t, jPaper);
        }
		
        
		JObject jTmp = new JObject();
		jTmp.Add("paper", jVaule);
		jTmp.Add("fTotalScore", fTotalScore);
		
        job.Add("value", jTmp);
        Debug.Log(job.ToString());
        SendUserOperToServer(job.ToString());

        /*
         {
            "key": "submit",
            "other": {
            "userid": "20170522001",
            "paperid": "66",
            "deskid": "DeskNumber1",
            "scoreid": "736"
            },
            "value": {
            "t1": {
                "boilertype": "燃煤蒸汽锅炉",
                "opertype": "认知题目",
                "caption": "设备认知_烟风系统",
                "hard": "★★",
                "fUserScore": 8.0
            },
            "t2": {
                "boilertype": "燃煤蒸汽锅炉",
                "opertype": "运行题目",
                "caption": "锅炉运行_冲洗水位计",
                "hard": "★★",
                "fUserScore": 0.0
            },
            "t3": {
                "boilertype": "燃煤蒸汽锅炉",
                "opertype": "事故题目",
                "caption": "事故处理_缺水处理（严重缺水）",
                "hard": "★★",
                "fUserScore": 0.0
            },
            "fTotalScore": 8.0
            }
        }
         */
    }
    /// /////////////////////////////////////////////////////////
    /// 以下为 试卷控制类
    public class PaperManager
    {
        public string strIndex { get; set; }          // 试卷的题目 索引
        public int nIndex { get; set; }          // 试卷的题目 索引
        public int nBigStepIndex { get; set; }   // 大步骤索引
        public int nSmallStepIndex { get; set; } // 小步骤索引

        public int nBigStepLevel { get; set; }   // 大步骤优先级
        public int nSmallStepLevel { get; set; } // 小步骤优先级

        public bool bFrist { get; set; } // 是不是第一次 答题
        public bool bBigHasMore { get; set; } // 是不是多个同一个优先级的步骤
        public bool bJustOneBigOver { get; set; } // 一个大步骤  刚好 做完了
        public bool bLockIndex { get; set; } // 一个大步骤  刚好 做完了
    }
    #endregion
    #region 以下 是 解析 题目 存储类

    /////////////////////////////////////////////////
    //// 以下 是 解析 题目 存储类

    public class Boilertype
    {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 燃煤蒸汽锅炉
        /// </summary>
      
        public string caption { get; set; }
    }

    public class Boileropertype
    {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 认知类
        /// </summary>
        public string caption { get; set; }
    }

    public class Opertype
    {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 认知题目
        /// </summary>
        public string caption { get; set; }
    }

    public class Hard
    {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string caption { get; set; }
    }
   
    public class StepItemTeXiao
    {
        /// <summary>
        /// 
        /// </summary>
        public int level { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type_operation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string value { get; set; }
    }
    public class InitItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string type_operation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string value { get; set; }
    }

    public class StepItem
    {
        /// <summary>
        /// 
        /// </summary>
        public int level { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string type_operation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<StepItemTeXiao> texiao = new List<StepItemTeXiao>();
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string value { get; set; }

        public bool bDone = false;

        public string strUserSetvalue { get; set; }
    }
  
    public class StepItemBig
    {
        /// <summary>
        /// 
        /// </summary>
        public int blevel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<StepItem> step = new List<StepItem>();

        public bool bDone = false;
    }

    public class T1
    {
        /// <summary>
        /// 设备认知_汽水系统
        /// </summary>
        public string caption { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Boilertype boilertype = new Boilertype();
        /// <summary>
        /// 
        /// </summary>
        public Boileropertype boileropertype = new Boileropertype();
        /// <summary>
        /// 
        /// </summary>
        public Opertype opertype = new Opertype();
        /// <summary>
        /// 
        /// </summary>
        public Hard hard = new Hard();
        /// <summary>
        /// 设备认知_汽水系统
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<InitItem> init = new List<InitItem>();
        /// <summary>
        /// 
        /// </summary>
        public List<StepItemBig> step = new List<StepItemBig>();
        /// <summary>
        ///  每个 步骤的 分值 此处 暂时 取个评价值吧 后续 在修改成 可以配置的 在管理端
        /// </summary>
      //  public int nScore { get; set; } // 先注释 掉 后续 在完善
    }
  
    public class DataItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string t { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public T1 t1 { get; set; }
        /// <summary>
        /// 认知题目
        /// </summary>
        public string opertype { get; set; }
        /// <summary>
        /// 燃煤蒸汽锅炉
        /// </summary>
        public string boilertype { get; set; }
        /// <summary>
        ///  给个题目在试卷中的占比20 30 50 后续 在修改成 可以配置的 在管理端
        /// </summary>
        public int nScore { get; set; }
        /// <summary>
        ///  用户 在此题目中的 得分  只算加分 吧
        /// </summary>
        public float fUserScore { get; set; }
        /// <summary>
        /// 在此题目中的 步骤的 总个数 用于 计算 每个步骤的分值
        /// </summary>
        public int nStepsCount { get; set; }
        /// <summary>
        /// 在此题目中的  每个步骤的分值
        /// </summary>
        public float fStepScore { get; set; }

        public bool bDone = false;
    }
 
    public class Paper
    {
		public string papertime { get; set; }
        public string paperremark { get; set; }
        public string papercaption { get; set; }
		
        /// <summary>
        /// 
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string scoreid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string paperid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string userid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<DataItem> data = new List<DataItem>();
        /// <summary>
        ///  用户 在此试卷中的 得分  只算加分 吧
        /// </summary>
        public int nUserScore { get; set; }
		
  
    }
    public class PointItem
    {
        /// <summary>
        /// key值
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 中文名称
        /// </summary>
        public string caption { get; set; }

        public string  ShowMsg()
        {
            string str = "";

            str = " key " + key + " caption " + caption;
            return str;
        }
    }
    // 操作点的 列表
    public class Points
    {
        /// <summary>
        /// 锅炉类型
        /// </summary>
        public PointItem boilertype = new PointItem();
        /// <summary>
        /// 操作类型 操作类、认知类
        /// </summary>
        public PointItem type = new PointItem();
        /// <summary>
        /// 动画、特效、阀门、旋塞 等等
        /// </summary>
        public PointItem items = new PointItem();
        /// <summary>
        /// 放空阀等具体名称
        /// </summary>
        public PointItem item = new PointItem();
        /// <summary>
        /// 具体操作 打开 关闭
        /// </summary>
        public PointItem value = new PointItem();

        public string ShowMsg()
        {
            string str = "";

            str = " 锅炉类型 " + boilertype.ShowMsg() +
                  "操作类型:" + type.ShowMsg()+
                  "部件类型:" + items.ShowMsg()+
                  "部件名称:" + item.ShowMsg()+
                  "部件值:" + value.ShowMsg()
                ;
            return str;
        }
        public bool CheckPoint(string strboilertype,string strType,string strtype_operation, string strkey, string strvalue)
        {
            bool btr = false;
            if (strType == "type_renzhi") // 认知
            {
                if (strboilertype == boilertype.key &&
              type.key == strType &&
             items.key == strtype_operation &&
             item.key == strkey)
                {
                    btr = true;
                }
            }
            else
            {
                if (strboilertype == boilertype.key &&
              type.key == strType &&
             items.key == strtype_operation &&
             item.key == strkey && 
             value.key == strvalue)
                {
                    btr = true;
                }
            }
               
            return btr;
        }
        public string GetOperCaption()
        {
            string strCaption = "";
            strCaption = item.caption +"   " + value.caption;
            return strCaption;
        }
        public string GetRenZhiCaption()
        {
            string strCaption = "";
            strCaption = item.caption;
            return strCaption;
        }
    }
    #endregion
}
