using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
//using Boo.Lang;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class CLoadPaper : MonoBehaviour {
    public static CLoadPaper mCLoad;
    private WWW www; //请求
    Paper papers = new Paper ();
    PaperManager paperManager = new PaperManager ();
    [HideInInspector]
    public string paperLoadType = "get"; //题目加载类型 随机或者指定
    private string strHostSerIp = "http://127.0.0.1:3010";
  //  private string strHostSerIp = "http://192.168.7.74:3010";
    //   private XKINI xkini;
    /// <summary>
    /// 操作点列表 存储了操作点对应的所有的状态值
    /// </summary>
    private List<Points> pointslist = new List<Points> ();
    private List<string> pointKeyList = new List<string> ();
    [HideInInspector]
    public string paperID = "67";

    string wwwwStr = "";
    enum e_result
    {
        e_null,// 没有任何值
        e_error,//回答错误
        e_ok,// 回答正确
        e_error_jump,// 错误 并且跳过该步骤
        e_error_main// 错误 并且是关键项
    }

    class tagDoLua
    {
        public e_result eLuaRt;// lua执行结果
        public string strLuaInfo;// 执行结果 携带的 返回结果 文字描述
    }
    class tagDoStep
    {
        public tagDoLua tlua = new tagDoLua(); // 脚本执行结果
        public e_result eStepRt;//步骤
        public string strStepInfo; // 步骤结果 携带的文字描述
    }
    void Start () {
        mCLoad = this;
        //    xkini = new XKINI(System.IO.Directory.GetCurrentDirectory() + "/Configure/setting.ini");//查找config文件

        //  strHostSerIp = "";
        wwwwStr = strHostSerIp + "/getpaper?paperid=" + paperID + "&&userid=" + "655465456";
        //  TLoadAllPoint();

        IntoLogin ();
        //  TLoadPaper ();
        //  ShowPaperMsg ();
        //Invoke("TLoadPaper", 2);
    }

    void Update () {
        if (Input.GetKeyDown (KeyCode.A)) {
            CheckSteps ("yanfeng", "yinfengji", "1");
        }
        if (Input.GetKeyDown (KeyCode.S)) {
            ShowPaperMsg ();
        }
    }
    #region 测试用的 方法

    public void TLoadTiMu1 () {
        LoadQuestion (1);
    }
    public void TLoadTiMu2 () {
        LoadQuestion (2);

    }
    public void TLoadTiMu3 () {
        LoadQuestion (3);
    }
    public void TShowPapers () {
        ShowPaperMsg ();
    }
    public void T2Step1 () {
        // CheckSteps()(string type_operation, string key, string value)
#if _NoGMaganer
        Debug.Log("有测试吗？");
#else
        Debug.Log("没有测试吧？");
#endif

        CheckSteps("valve", "t_zhuzhengqifa", "0");
    }
    public void T2Step2 () {
        // CheckSteps()(string type_operation, string key, string value)
        CheckSteps ("valve", "t_zhuzhengqifa", "20");
    }

    ///////
    public void T2Step3 () {
        // CheckSteps()(string type_operation, string key, string value)
        CheckSteps ("valve", "t_gongqizongfa", "20");
    }

    public void T2Step4 () {
        // CheckSteps()(string type_operation, string key, string value)
        CheckSteps ("valve", "t_gongqizongfa", "60");
    }

    ///////
    public void T2Step5 () {
        // CheckSteps()(string type_operation, string key, string value)
        CheckSteps ("valve", "t_guotongchushuifa", "40");
    }

    public void T2Step6 () {
        // CheckSteps()(string type_operation, string key, string value)
        CheckSteps ("valve", "t_guotongjinshuifa", "60");
    }

    #endregion
    #region 加载试卷 操作点 初始化
    // 指定 加载 试卷 事故题目
    IEnumerator LoadPaper () {
        www = new WWW (wwwwStr);
        yield return www;

        Debug.Log (www.text);
        string str = www.text;

        JObject jObj = JObject.Parse (str);

        if (jObj["code"].ToString () == "0") {
            Debug.Log ("试卷加载成功!");

            papers.code = jObj["code"].ToString ();
            papers.scoreid = jObj["scoreid"].ToString ();
            papers.paperid = jObj["paperid"].ToString ();
            papers.userid = jObj["userid"].ToString ();

            papers.papertime = jObj["papertime"].ToString ();
            papers.paperremark = jObj["paperremark"].ToString ();
            papers.papercaption = jObj["papercaption"].ToString ();
            //  GMaganer.TotalTime = int.Parse(papers.papertime)*60;
#if _NoGMaganer
            Debug.Log("有测试吗？");
#else
           GMaganer.TotalTime = int.Parse(papers.papertime)*60;
#endif
            Debug.Log (papers.papertime);

            #region 解析试卷
            foreach (var k in jObj["data"]) {
                DataItem data = new DataItem ();
                data.t = k["t"].ToString (); //t1 t2 t3
                data.boilertype = k["boilertype"].ToString ();
                data.opertype = k["opertype"].ToString ();
                T1 t1 = new T1 ();
                t1.caption = k["t1"]["caption"].ToString ();
                //GMaganer.questionName.Add(t1.caption);//题目名称
                //GMaganer.boilerName = data.boilertype;//锅炉类型 中文描述
                t1.boilertype.key = k["t1"]["boilertype"]["key"].ToString ();
                //GMaganer.sceneName =  t1.boilertype.key;//锅炉类型英文 用于加载场景
#if _NoGMaganer
                Debug.Log("有测试吗？");
#else
           GMaganer.questionName.Add(t1.caption);//题目名称
           GMaganer.boilerName = data.boilertype;//锅炉类型 中文描述
           GMaganer.sceneName =  t1.boilertype.key;//锅炉类型英文 用于加载场景
#endif
                t1.boilertype.caption = k["t1"]["boilertype"]["caption"].ToString ();
                t1.boileropertype.key = k["t1"]["boileropertype"]["key"].ToString ();
                t1.boileropertype.caption = k["t1"]["boileropertype"]["caption"].ToString ();
                t1.opertype.key = k["t1"]["opertype"]["key"].ToString ();
                t1.opertype.caption = k["t1"]["opertype"]["caption"].ToString ();
                t1.hard.key = k["t1"]["hard"]["key"].ToString ();
                t1.hard.caption = k["t1"]["hard"]["caption"].ToString ();
                t1.remark = k["t1"]["remark"].ToString ();
                data.t1 = t1;

                data.nStepsCount = 0;
                data.nScore = 0;
                data.fStepScore = 0.0f;
                data.fUserScore = 0;
                // 设置 题目 在试卷中的 占的 分值
                if (t1.opertype.key == "type_renzhi") // 认知
                {
                    data.nScore = 20;
                } else if (t1.opertype.key == "type_yunxing") // 运行题目
                {
                    data.nScore = 20;
                } else if (t1.opertype.key == "type_shigu") // 事故题目
                {
                    data.nScore = 60;
                }
                papers.data.Add (data);
                int nScoreTmpWeight = 0; // 计算 当前 题目的 权重和 用来计算 没一步所占的分值

                foreach (var step in k["t1"]["step"]) {
                    StepItemBig bigStep = new StepItemBig ();
                    data.t1.step.Add (bigStep);
                    bigStep.blevel = int.Parse (step["blevel"].ToString ());
                    if (step["ckjump"] != null) {
                        string strCkJump = step["ckjump"].ToString ();
                        bigStep.bCkJump = bool.Parse (strCkJump);
                    } else {
                        bigStep.bCkJump = false;
                    }

                    foreach (var s in step["step"]) {
                        StepItem sStep = new StepItem ();
                        bigStep.step.Add (sStep);
                        sStep.level = int.Parse (s["level"].ToString ());
                        sStep.type_operation = s["type_operation"].ToString ();
                        sStep.key = s["key"].ToString ();
                        if (s["value"] != null)
                            sStep.value = s["value"].ToString ();
                        data.nStepsCount++;
                        string strValues = s.ToString ();

                        // add 2017年11月27日 14:38:09 wsh
                        // 
                        if (s["ckjump"] != null) {
                            string strCkJumpXiao = s["ckjump"].ToString ();
                            sStep.bCkJump = bool.Parse (strCkJumpXiao);
                        } else {
                            sStep.bCkJump = false;
                        }

                        if (s["nscoreLevel"] != null) {
                            sStep.nScoreWeight = int.Parse (s["nscoreLevel"].ToString ());
                        } else {
                            sStep.nScoreWeight = 1;
                        }

                        if (s["ckmain"] != null) {
                            sStep.bCkMainStep = bool.Parse (s["ckmain"].ToString ());
                        } else {
                            sStep.bCkMainStep = false;
                        }
                        if (sStep.nScoreWeight < 1) {
                            sStep.nScoreWeight = 1;
                        }

                        nScoreTmpWeight += sStep.nScoreWeight;
                        //  Debug.Log(step["blevel"].ToString() + s["level"].ToString() +
                        //      s["type_operation"].ToString() + s["key"].ToString() + s["value"].ToString());
                        foreach (var tx in s["texiao"]) {
                            StepItemTeXiao texiao = new StepItemTeXiao ();
                            sStep.texiao.Add (texiao);
                            texiao.key = tx["key"].ToString ();
                            texiao.type_operation = tx["type_operation"].ToString ();
                            texiao.value = tx["value"].ToString ();

                            if (tx["ntxtime"] != null) {
                                string strnTxTime = tx["ntxtime"].ToString ();
                                texiao.nTxTime = int.Parse (strnTxTime);
                            } else {
                                texiao.nTxTime = 0;
                            }
                        }
                        
                    }
                }

                foreach (var inStep in k["t1"]["init"]) {
                    InitItem itm = new InitItem ();
                    itm.key = inStep["key"].ToString ();
                    itm.type_operation = inStep["type_operation"].ToString ();
                    itm.value = inStep["value"].ToString ();
                    data.t1.init.Add (itm);
                }

                // 计算一下 每个步骤的 评价分值
                data.fStepScore = data.nScore / data.nStepsCount;

                // 计算一下 每个 步骤的 分值 用权重的方法

                data.t1.nScoreTotalWeight = nScoreTmpWeight;

                foreach (var s in data.t1.step) {
                    foreach (var st in s.step) {

                        st.fScoreStep = 1.0f * data.nScore * st.nScoreWeight / nScoreTmpWeight; // 权重
                    }
                }

            }
            #endregion
        } else {
            Debug.Log ("试卷加载失败!");
        }

        //   LoadPaperOK();
#if _NoGMaganer
        Debug.Log("有测试吗？");
#else
          LoadPaperOK();
#endif
        ShowPaperMsg();
#if _NoGMaganer
       
#else
         SocketWithVC.sock.LoginYes();
#endif
        //   SocketWithVC.sock.LoginYes();

    }
    /// <summary>
    /// 进入登陆场景
    /// </summary>
    void IntoLogin () {
        // DontDestroyOnLoad(this.gameObject);
        //  Application.LoadLevel("Login");
#if _NoGMaganer

#else
        DontDestroyOnLoad(this.gameObject);
        Application.LoadLevel("Login");
#endif
    }
    // 加载 所有炉型的 操作点
    IEnumerator LoadAllPoint () {
        www = new WWW (strHostSerIp + "/gettreelast");
        yield return www;
        string str = www.text;
        JObject jObj = JObject.Parse (str);

        if (jObj["code"].ToString () == "0") {
            Debug.Log ("操作点加载成功!");
            // Debug.Log(jObj["data"][0]["json"].ToString());
            // 2017年10月10日 17:05:34 明天写这个解析类 分析出 key value 的中文名称
            foreach (var s in jObj["data"][0]["json"]["items"]) {
                foreach (var m in s["items"]) {
                    // 操作类 还是 认知类 如操作类
                    // m["key"].ToString();
                    // m["caption"].ToString();
                    //  ps.type.key = m["key"].ToString();
                    //  ps.type.caption = m["caption"].ToString();
                    foreach (var n in m["items"]) {
                        // 动画 特效 仪表 阀门 如阀门
                        //   ps.items.key =  n["key"].ToString();
                        //   ps.items.caption =  n["caption"].ToString();

                        foreach (var k in n["items"]) {
                            // 具体 操作部件 如 放空阀
                            //    ps.item.key =  k["key"].ToString();
                            //     ps.item.caption = k["caption"].ToString();
                            foreach (var p in k["items"]) {
                                // 具体 操作部件 动作及值 如 放空阀 打开 0 
                                Points ps = new Points ();

                                ps.boilertype.key = s["key"].ToString ();
                                ps.boilertype.caption = s["caption"].ToString ();

                                ps.type.key = m["key"].ToString ();
                                ps.type.caption = m["caption"].ToString ();

                                ps.items.key = n["key"].ToString ();
                                ps.items.caption = n["caption"].ToString ();

                                ps.item.key = k["key"].ToString ();
                                ps.item.caption = k["caption"].ToString ();

                                ps.value.key = p["value"].ToString (); // 此处是 value 值 0、1、2等
                                ps.value.caption = p["caption"].ToString ();

                                // 到此处 是不是就可以 放入到 列表里面了
                                pointslist.Add (ps);

                                //如果列表中不存在该操作点 那么加入
#if _NoGMaganer

#else
                              if (!pointKeyList.Contains (ps.item.key)) {
                                    pointKeyList.Add (ps.item.key);
                                    switch (ps.items.key) {
                                        case "valve":
                                            //GMaganer.valvePointInfoDic.Add(ps.item.key, "0");
                                            //GMaganer.valveCaptionDic.Add(ps.item.key,ps.item.caption);
                                            break;
                                        case "cock":
                                            //GMaganer.cockPointInfoDic.Add(ps.item.key, "0");
                                            // GMaganer.cockCaptionDic.Add(ps.item.key,ps.item.caption);
                                            break;
                                        case "buffer":
                                            //  GMaganer.bafflePointInfoDic.Add(ps.item.key, "0");
                                            //  GMaganer.baffleCaptionDic.Add(ps.item.key,ps.item.caption);
                                            break;
                                        case "switch":
                                            // GMaganer.buttonPointInfoDic.Add(ps.item.key, "0");
                                            //  GMaganer.buttonCaptionDic.Add(ps.item.key,ps.item.caption);
                                            break;
                                        case "other1":
                                            //  GMaganer.knobPointInfoDic.Add(ps.item.key, "0");
                                            //  GMaganer.knobCaptionDic.Add(ps.item.key,ps.item.caption);
                                            break;
                                        case "meter":
                                            //  GMaganer.meterInfoDic.Add(ps.item.key, "0");
                                            //  GMaganer.meterCaptionDic.Add(ps.item.key,ps.item.caption);
                                            break;
                                        case "par":
                                            //  GMaganer.particleInfoDic.Add(ps.item.key, "0");
                                            break;
                                        default:
                                            break;
                                    }
                                }              
#endif


                            }
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// 加载所有的操作点
    /// </summary>
    public void TLoadAllPoint () {
        StartCoroutine (LoadAllPoint ());
    }
    /// <summary>
    /// 加载试卷
    /// </summary>
    public void TLoadPaper () {
        Debug.Log (wwwwStr); // t1 \ t2 \t3
        StartCoroutine (LoadPaper ());
    }
    public void fn_LoadPaper () {
        if (paperLoadType == "random") {
            wwwwStr = strHostSerIp + "/getpaper?userid=" + "6564564566"; //GMaganer.userInfo.ID;
        } else {
            paperID = "67";
            wwwwStr = strHostSerIp + "/getpaper?paperid=" + paperID + "&&userid=" + "655465456";
        }
        TLoadPaper ();
    }
    /// <summary>
    /// 根据选择的考题类型 初始化对应内容
    /// </summary>
    /// <param name="id"></param>
    public void LoadQuestion (int id) {
        InitTiMuById (id);
    }
    void ShowPaperMsg () {
        //Debug.Log("—————————————ShowPaperMsg_________________");
        //string strPaper = "nBigStepIndex: " + paperManager.nBigStepIndex + " nBigStepLevel :" + paperManager.nBigStepLevel +
        //    " nSmallStepIndex " + paperManager.nSmallStepIndex + " nSmallStepLevel " + paperManager.nSmallStepLevel + ""
        //    + "";
        //Debug.Log(strPaper);
        foreach (var s in papers.data) {
            Debug.Log (s.t); // t1 \ t2 \t3
            Debug.Log (s.boilertype.ToString ()); // 锅炉类型 中文
            Debug.Log (s.opertype.ToString ()); // 操作类型 认知、运行、事故
            Debug.Log (s.t1.hard); // 难度
            Debug.Log (s.t1.remark); // 备注 //题目的中文名字

            // 初始化
            foreach (var initStep in s.t1.init) {
                //   Debug.Log(initStep.type_operation);// 类型
                //  Debug.Log(initStep.key);// key
                //  Debug.Log(initStep.value);// value
            }
            // 步骤
            // 大步骤
            foreach (var bigstep in s.t1.step) {
                // Debug.Log(bigstep.blevel);
                // 小步骤
                foreach (var sStep in bigstep.step) {
                    //Debug.Log(sStep.level);
                    // Debug.Log(sStep.type_operation);
                    // Debug.Log(sStep.key);
                    // Debug.Log(sStep.value);
                    string strmsg = bigstep.blevel + "-" + sStep.level + ":" + sStep.type_operation + " " + sStep.key + sStep.value + " bDone " + sStep.bDone + "CkJump: " + sStep.bCkJump + " bCkMainStep: " + sStep.bCkMainStep + " 权重:" + sStep.nScoreWeight + "步骤分值:" + sStep.fScoreStep;
                    Debug.Log ("ffffff:  " + strmsg);
                    if (s.t == "t1") {
                        //Debug.Log("fsdjfhdfhsdfhload");
                        ///   GMaganer.cogOperationType = sStep.type_operation;
                        //   GMaganer.equipCogCaptionList.Add(GetCaption(sStep.type_operation, sStep.key));
                        //   GMaganer.equipCogKeyList.Add(sStep.key);
#if _NoGMaganer
#else
                        GMaganer.cogOperationType = sStep.type_operation;
                        GMaganer.equipCogCaptionList.Add(GetCaption(sStep.type_operation, sStep.key));
                        GMaganer.equipCogKeyList.Add(sStep.key);
#endif
                    }
                    // 小步骤的 特效
                    foreach (var tx in sStep.texiao) {
                        // Debug.Log(tx.type_operation);
                        //  Debug.Log(tx.key);
                        //  Debug.Log(tx.value);
                    }
                }
            }
        }
    }
    // 显示操作点
    public void ShowPoints () {
        foreach (var s in pointslist) {
            Debug.Log (s.ShowMsg ());
        }
    }
    /// <summary>
    /// 初始化题目
    /// </summary>
    /// <param name="n"></param>
    public void InitTiMuById (int n) {
        Debug.Log ("加载题目:" + n);
        paperManager.strIndex = "t" + n;
        paperManager.nIndex = n;
        paperManager.nIndexReCover = n - 1;
        paperManager.nBigStepIndex = 0;
        paperManager.nSmallStepIndex = 0;
        paperManager.nBigStepLevel = 1;
        paperManager.nSmallStepLevel = 1;
        paperManager.bFrist = true;
        string strIndex = "t" + n;
        // 查找题目
        foreach (var s in papers.data) {
            if (s.t == strIndex) {
                //Debug.Log("锅炉类型：" + s.boilertype + "操作类型:" + s.opertype + "难度等级:"+ s.t1.hard.caption+
                //    "题目名称："+s.t1.caption);
                // 初始化
#if _NoGMaganer
#else
              foreach (var initStep in s.t1.init) {
                #region  此处对 UI数据 进行初始化
                    switch (initStep.type_operation) {
                        case "valve":
                            //   InitValue(GMaganer.valvePointInfoDic, initStep.key, initStep.value);
                            break;
                        case "cock":
                            //   InitValue(GMaganer.cockPointInfoDic, initStep.key, initStep.value);
                            break;
                        case "buffer":
                            //   InitValue(GMaganer.bafflePointInfoDic, initStep.key, initStep.value);
                            break;
                        case "switch":
                            //   InitValue(GMaganer.buttonPointInfoDic, initStep.key, initStep.value);
                            break;
                        case "other1":
                            //   InitValue(GMaganer.knobPointInfoDic, initStep.key, initStep.value);
                            break;
                        case "meter":
                            Debug.Log ("initmeter:" + initStep.key);
                            //  InitValue(GMaganer.meterInfoDic, initStep.key, initStep.value);
                            break;
                        case "par":
                            //    InitValue(GMaganer.particleInfoDic, initStep.key, initStep.value);
                            break;
                        default:
                            break;
                    }
                #endregion
                }
#endif

            }
        }
        Debug.Log ("加载题目: 完成！！");
    }
    /// <summary>
    /// 初始化数据
    /// </summary>
    void InitValue (Dictionary<string, string> pDic, string key, string value) {
        pDic[key] = value;
    }
    /// <summary>
    /// 发送加载试卷成功消息给服务器
    /// </summary>
    void LoadPaperOK () {
        JObject job = new JObject ();
        job.Add ("key", "paperok");
        JObject ju = new JObject ();
        ju.Add ("userid", papers.userid);
        ju.Add ("paperid", papers.paperid);
        ju.Add ("deskid", "23");
        ju.Add ("scoreid", papers.scoreid);
        job.Add ("value", ju);
        string strMsg = job.ToString ();
        SendUserOperToServer (strMsg);
    }
#endregion
    /// <summary>
    /// 发送信息到服务器
    /// </summary>
    /// <param name="strMsg"></param>
    void SendUserOperToServer (string strMsg) {
#if _NoGMaganer
#else
          SocketWithVC.sock.SendMsg (strMsg);
#endif
    }
    void UITeXiao (string type_operation, string key, string value) {
        // 此处 处理 特效
        if(type_operation == "lua")
        {
            // 当前是脚本 需要按照 脚本操作了
            Debug.Log("key" + key);
            Debug.Log("value " + value);
        }
    }
  
    public int CheckSteps (string type_operation, string key, string value) {
        //  Debug.Log(type_operation + "    " + key);
        // ReCoverStep(type_operation, key, value);
        ReCoverStep2(type_operation, key, value);
        return 0;
        tagDoStep eRt = new tagDoStep();
        eRt = XCheckSteps (type_operation, key, value);
        // 发送 操作记录
        SendOper (eRt, type_operation, key, value);

        // 检查 是否 可跳过

        // 判断 当前 题目 是否 结束了

        bool bOver = IsTiMuOver ();
        if (bOver) {
            Debug.Log ("本题目回答完毕！");
        }
        if (IsPaperOver ()) {
            Debug.Log ("本试卷 回答完毕！");
        }
        return 0;
    }

    /// <summary>
    /// 对比设备认知结果
    /// </summary>
    /// <param name="key"></param>
    public void CheckCognitive (string key) {
        bool bOk = false;
        // if (GMaganer.equipCogKeyList[UICtrl.mUICtrl.curCogIndex] == key)
        // {
        //     bOk = true;
        //     papers.data[0].fUserScore = papers.data[0].fUserScore + papers.data[0].fStepScore;
        // }
        //   SendOper(bOk, GMaganer.cogOperationType, key, "1");
#if _NoGMaganer
#else
          if (GMaganer.equipCogKeyList[UICtrl.mUICtrl.curCogIndex] == key)
         {
             bOk = true;
             papers.data[0].fUserScore = papers.data[0].fUserScore + papers.data[0].fStepScore;
         }
           SendOper(bOk, GMaganer.cogOperationType, key, "1");
#endif
    }
    void SendOper (tagDoStep eRt, string type_operation, string key, string value) {
        string strOk = "";
        float fUserGetScore = 0.0f;
        if (eRt.eStepRt == e_result.e_ok) {
            // 作对了 
            strOk = "正确";
            fUserGetScore = papers.data[paperManager.nIndex].fStepScore;
        } else {
            // 做错了
            strOk = "错误";
        }
        string strErrorInfo = "";
        if (eRt.eStepRt == e_result.e_error)
        {
            strErrorInfo = "回答错误";
        }
        else if (eRt.eStepRt == e_result.e_error_jump)
        {
            strErrorInfo = "回答错误，自动跳过";
        }
        else if (eRt.eStepRt == e_result.e_error_main)
        {
            strErrorInfo = "关键项错误，本题目答题完毕";
        }
        else if(eRt.eStepRt == e_result.e_ok)
        {
            strErrorInfo = "回答正确";
        }
        Debug.Log (strOk);
        Debug.Log(strErrorInfo);
        string strOperCaption = GetChineseCaption (type_operation, key, value);
#if _NoGMaganer
#else
         if(GMaganer.curQueType != QuestionType.cognitive)
              UICtrl.mUICtrl.InstantiateRecordItem(strOperCaption);
        添加操作记录到列表
           RecordInfo record = new RecordInfo();
            record.dis = strOperCaption;
           record.result = strOk;

           if (GMaganer.curQueType == QuestionType.cognitive)
           {
               record.dis = GMaganer.equipCogCaptionList[UICtrl.mUICtrl.curCogIndex];
             GMaganer.cogStuResultList.Add(record);
           }
           else if (GMaganer.curQueType == QuestionType.operation)
               GMaganer.operationRecordList.Add(record);
           else if (GMaganer.curQueType == QuestionType.accident)
               GMaganer.accidentRecordList.Add(record);
#endif


        string boilertype = "";
        string boileropertype = "";
        string opertype = "";
        boilertype = papers.data[paperManager.nIndex].t1.boilertype.key;
        boileropertype = papers.data[paperManager.nIndex].t1.boileropertype.key;
        opertype = papers.data[paperManager.nIndex].t1.opertype.key;
        JObject job = new JObject ();
        job.Add ("key", "useroper");
        JObject joper = new JObject ();
        JObject ju = new JObject ();
        ju.Add ("userid", papers.userid);
        ju.Add ("paperid", papers.paperid);
        ju.Add ("deskid", "23");
        ju.Add ("scoreid", papers.scoreid);
        joper.Add ("boileropertype", boileropertype); // 认知类、操作类
        joper.Add ("opertype", opertype); // 认知 运行 事故
        joper.Add ("boilertype", boilertype);
        joper.Add ("type_operation", type_operation);
        joper.Add ("key", key);
        joper.Add ("value", value);
        joper.Add ("bUserAnswerRight", strOk);
        joper.Add ("strOperCaption", strOperCaption);
        joper.Add ("userHasScore", papers.data[paperManager.nIndex].fUserScore); // 用户 已经 得到的分值
        joper.Add ("fUserGetScore", fUserGetScore); // 用户 当前步骤 得到的分值
        joper.Add ("QuestionID", paperManager.nIndex);
        job.Add ("other", ju);
        job.Add ("value", joper);
        string strMsg = job.ToString ();
        //Debug.Log(strMsg);
        // 调用 发送接口
        SendUserOperToServer (strMsg);
    }
    /// <summary>
    /// 获取操作点的中文名字
    /// </summary>
    /// <param name="type_operation"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    string GetCaption (string type_operation, string key, string value = "1") {
        string strOperCaption = "";
        foreach (var ss in pointslist) {
            if (ss.CheckPoint (papers.data[0].t1.boilertype.key, "Type_Device", type_operation, key, value)) {
                strOperCaption = ss.GetRenZhiCaption ();
                break;
            }
        }
        return strOperCaption;
    }
    string GetChineseCaption (string type_operation, string key, string value) {
        string boilertype = "";
        string boileropertype = "";
        string opertype = "";

        boilertype = papers.data[paperManager.nIndex].t1.boilertype.key;
        boileropertype = papers.data[paperManager.nIndex].t1.boileropertype.key;
        opertype = papers.data[paperManager.nIndex].t1.opertype.key;
#if _NoGMaganer
#else
          if (GMaganer.curQueType == QuestionType.cognitive)
           {
               boileropertype="Type_Device";
               opertype = "type_renzhi";
           }
#endif
        //Debug.Log(boilertype + "   " + boileropertype + "  " + opertype+"    "+ type_operation + "   " + key);
        string strOperCaption = "";
        foreach (var ss in pointslist) {
            if (ss.CheckPoint (boilertype, boileropertype, type_operation, key, value)) {
                if (opertype != "type_renzhi") // 认知
                {
                    strOperCaption = ss.GetOperCaption ();
                } else {
                    Debug.Log ("jinrurenzhipanduan");
                    strOperCaption = ss.GetRenZhiCaption ();
                }
                break;
            }
        }
        return strOperCaption;
    }
    void SetNowStepDone(int nTimuIndex,int nBigStep,int nSmailStep)
    {
        int n = nTimuIndex;
        int m = nBigStep;
        int i = nSmailStep;
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
       // Debug.Log("答对了一个步骤" + type_operation + key + value);
    }
    // 设置当前步骤的特效
    void SetNowStepTexiao(int nTimuIndex, int nBigStep, int nSmailStep)
    {
        int n = nTimuIndex;
        int m = nBigStep;
        int i = nSmailStep;
        // 是该步骤的消息
        // 设置当前步骤 已经做了
        foreach (var tx in papers.data[n].t1.step[m].step[i].texiao)
        {
            Debug.Log(tx.type_operation);
            Debug.Log(tx.key);
            Debug.Log(tx.value);
            UITeXiao(tx.type_operation, tx.key, tx.value);
        }
    }
    private tagDoLua SetNowStepLua(int n, int m, int i)
    {
        tagDoLua eRt = new tagDoLua();
        eRt.eLuaRt = e_result.e_null;
        
        foreach (var tx in papers.data[n].t1.step[m].step[i].texiao)
        {
            Debug.Log(tx.type_operation);
            Debug.Log(tx.key);
            Debug.Log(tx.value);
            if(tx.type_operation == "lua")
            {
                // 执行 lua 脚本

                // 

                string strLuaScriptTest = "function DoMain()\n     var s = GetMeterValue(\"t_shuiweiji\");\n    if(s >50){\n     return true,\"水位计的水位>50\"\n  else \n    return false,\"水位计的水位<=50\"\nend";


                // 如果 错误  eRt.eLuaRt = e_result.e_error; //  (●ˇ∀ˇ●)

            }
        }

        return eRt;
    }
    tagDoStep JustCheckBigStep(int n ,int m , string type_operation, string key, string value)
    {
        tagDoStep eRt = new tagDoStep();
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
                    if (type_operation == papers.data[n].t1.step[m].step[i].type_operation &&
                            key == papers.data[n].t1.step[m].step[i].key)
                    {
                        if (papers.data[n].t1.opertype.key == "type_renzhi") // 认知
                        {
                            papers.data[n].t1.step[m].step[i].bDoneUser = true;// 设置 当前 用户是答对了
                            SetNowStepDone(n, m, i);
                    
                            papers.data[n].fUserScore = papers.data[n].fUserScore + papers.data[n].fStepScore;
                            SetNowStepTexiao(n, m, i);
                            Debug.Log("答对了一个 认知 判定" + type_operation + key + value);
                            eRt.eStepRt = e_result.e_ok;
                            return eRt;
                        }
                        else
                        {
                            // 我们需要根据value判定了
                            if (value == papers.data[n].t1.step[m].step[i].value)
                            {
                                // 值相等
                                Debug.Log("怎么 相等了");
                                
                                SetNowStepDone(n, m, i);
                                SetNowStepTexiao(n, m, i);

                                eRt.tlua= SetNowStepLua(n, m, i);
                               
                                eRt.eStepRt = e_result.e_ok;
                                if (eRt.tlua.eLuaRt != e_result.e_null)
                                {
                                    Debug.Log("步骤" + type_operation + key + value + "操作正确 但是 脚本 返回了 错误结果");
                                    eRt.eStepRt = eRt.tlua.eLuaRt;
                                  
                                    if (eRt.tlua.eLuaRt == e_result.e_error)
                                    {
                                        // 错误
                                    }else if (eRt.tlua.eLuaRt == e_result.e_error_jump)
                                    {
                                        // 错误 
                                    } else if (eRt.tlua.eLuaRt == e_result.e_error_jump)
                                    {
                                        // 错误
                                    }
                                    else if(eRt.tlua.eLuaRt == e_result.e_ok)
                                    {
                                        papers.data[n].t1.step[m].step[i].bDoneUser = true;// 设置 当前 用户是答对了
                                        Debug.Log("答对了一个步骤" + type_operation + key + value);
                                    }
                                }
                               
                               

                                // if (e_lua.e_false == eLuaRt.eLuaRt)
                                // {
                                //     // 错误了
                                //     // 这个 地方 还要修改 一下 其他的问题 比方说错误说明
                                //     // 如 水
                                //     // 泵关闭，水位下降到 正常水位。即：只有在水位下降到正常水位的时候，关闭水泵，才算是回答正确，否则不正确
                                //     // 返回带有 文字描述的 提示，如:水泵关闭，但是水位没有下降到正常水位，操作错误。
                                //     // 错误描述啊 错误描述 错误描述 错误描述 错误描述 错误描述
                                //     eRt = e_result.e_error; 
                                // }

                                return eRt;
                            }
                            else
                            {
                                // 不相等
                                Debug.Log("怎么 不相等了");
                                // 看看是不是 触发一次就跳过的那种
                                if (papers.data[n].t1.step[m].step[i].bCkJump)
                                {
                                    // 当前是 可以 跳过的
                                    SetNowStepDone(n, m, i);// 做错了 但是可以 跳过
                                    papers.data[n].t1.step[m].step[i].bDoneUser = false;// 设置 当前 用户是答错了
                                    Debug.Log("当前步骤用户进行了作答，但是作答错误，该步骤是可以跳过的，跳过该步骤，跳过了 一个步骤" + type_operation + key + value);
                                    eRt.eStepRt = e_result.e_error_jump;
                                    return eRt;
                                }
                                else
                                {
                                    // 不可跳过的 关键项
                                    if (papers.data[n].t1.step[m].step[i].bCkMainStep)
                                    {
                                        // 当前是关键项
                                        Debug.Log("当前步骤用户进行了作答，但是作答错误，该步骤是【关键项】，此题目答题完成，触发结束本题目的考试" + type_operation + key + value);
                                        eRt.eStepRt = e_result.e_error_main;
                                        return eRt;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (papers.data[n].t1.opertype.key == "type_renzhi") // 认知
                        {
                            // 当前 认知是 错误的 那么 当前 应该是 进行 下一个了

                        }
                    }

                }

            }
        }

        return eRt;
    }
    tagDoStep JustCheck2(string type_operation, string key, string value)
    {
        //  bool bOk = false;
        tagDoStep eRt = new tagDoStep();
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
                    if (paperManager.bLockIndex) // 锁定了
                    {
                        //  bOk = JustCheckLockIndex(type_operation, key, value);
                        if (paperManager.nBigStepIndex == m)
                        {
                            eRt = JustCheckBigStep(n, m, type_operation, key, value);
                            if (eRt.eStepRt == e_result.e_ok) return eRt;
                        }
                    }
                    else
                    {
                        //   bOk = JustCheck(type_operation, key, value);
                        // 未锁定的
                        eRt = JustCheckBigStep(n, m, type_operation, key, value);
                        if (eRt.eStepRt == e_result.e_ok) return eRt;
                    }
                    
                }
            }
        }
        return eRt;
    }
    void PrecheckBig(int n,int m,string type_operation, string key, string value)
    {
       
        if (papers.data[n].t1.step[m].blevel == paperManager.nBigStepLevel &&
                       !papers.data[n].t1.step[m].bDone)
        {
            int nXiaoStepNoDoneCount = 0;  // 未做的小步骤个数, 不可跳过的的 个数
            int nXiaoStepNoDoneCountLevelPlus1 = 0;  // 优先级+1，未做的小步骤个数, 不可跳过的的 个数
            // 小步骤的 个数
            for (int i = 0; i < papers.data[n].t1.step[m].step.Count; i++)
            {
                if (papers.data[n].t1.step[m].step[i].level == paperManager.nSmallStepLevel &&
                    !papers.data[n].t1.step[m].step[i].bDone && 
                    !papers.data[n].t1.step[m].step[i].bCkJump)
                {
                    nXiaoStepNoDoneCount++;
                }
                if (papers.data[n].t1.step[m].step[i].level == (paperManager.nSmallStepLevel+1) &&
                   !papers.data[n].t1.step[m].step[i].bDone &&
                   !papers.data[n].t1.step[m].step[i].bCkJump)
                {
                    nXiaoStepNoDoneCountLevelPlus1++;
                  //  papers.data[n].t1.step[m].step[i].type_operation;
                }
            }

            // 没有做的小步骤个数 > 1
            // 如果 不可跳过的 都做完了 那么

            // 小步骤的优先级+1 看看 是不是还有 不可跳过的

            // 寻找 不可跳过的 命中项  如果：没有找到不可跳过的命中项，且还存在不可跳过的步骤，那么本次判定结束。
            // 如果：没有找到不可跳过的命中项，且不存在不可跳过的步骤，那么 就进行 下一个优先级的判定 直到找到一个命中项或者 
            //       当前优先级存在了 不可跳过的步骤 还有没有命中，那么本次 查找结束。

            // 形成一个 迭代 循环 貌似 更加 好一些 或者是 用 链表保存 数据 并处理 后续的 命中项后对前面 可跳过的步骤 做跳过标识。
            if (nXiaoStepNoDoneCount == 0)
            {
                // 当前优先级 没有 不可以跳过的步骤 

                if(nXiaoStepNoDoneCountLevelPlus1 == 0)
                {
                    //当前优先级+1 也没有可以跳过的步骤

                    // 当前没有可以 跳过的步骤 继续 往下找

                }
                else
                {
                    // 判定 当前不可跳过的步骤 是否 命中了 当前的用户操作
                    if (true)
                    {
                        // 如果命中 那么 就认为 当前用户操作了 该步骤， 前面可以跳过的步骤 自动 跳过了，并且被跳过的步骤 不得分


                    }
                }
            }
        }
    }
    bool m_bLockStep = false; // 锁定步骤
    int m_nLockBigStepIndex = 0;// 锁定的 大步骤 index
    int m_nLockSmailStepIndex = 0; // 锁定 的 小步骤的 index
    void ReCoverStep(string type_operation, string key, string value)
    {
        tagDoStep tagRtDoStep = new tagDoStep();
        // 获取 大步骤的 没有做的最小 level 和索引值
        int nBigLevel = -1;
        //   int nBigIndex = 0;
        // 计算 当前的 level 一共 有多少 如果 >1 那么 就需要 进行 步骤 锁定判定
        int nBigCount = 0;
        int nLen = papers.data[paperManager.nIndexReCover].t1.step.Count;
        int []arrBigLevel = new int[nLen];
        int nnIndex = 0;

        int g_nSmailLevel = 0;
        int g_nBigsLevel = 0;
        bool g_isReSetBigLevel = true;
        bool g_isReSetSmailLevel = true;

        if (g_isReSetBigLevel)
        {
            foreach (var sb in papers.data[paperManager.nIndexReCover].t1.step)
            {
                if (!sb.bDone)
                {
                    if (nBigLevel == -1)
                    {
                        nBigLevel = sb.blevel;
                        nBigCount = 1;
                    }
                    else
                    {
                        if (nBigLevel == sb.blevel)
                        {
                            nBigCount++;
                        }
                        else
                        {
                            nBigLevel = nBigLevel > sb.blevel ? sb.blevel : nBigLevel;
                        }
                    }

                    arrBigLevel[nnIndex++] = sb.blevel;
                }
            }
            // 冒泡排序方法
            for (int i = 0; i < arrBigLevel.Length - 1; i++)
            {
                for (int x = 0; x < arrBigLevel.Length - 1 - i; x++)
                {
                    if (arrBigLevel[x] > arrBigLevel[x + 1])
                    {
                        // 数值交换
                        int a = arrBigLevel[x];
                        arrBigLevel[x] = arrBigLevel[x + 1];
                        arrBigLevel[x + 1] = a;
                    }
                }
            }

            if (nBigLevel == -1)
            {
                // 说明 没找到 题目答题完毕了
            }

            if (nBigCount > 1)
            {
                Debug.Log("注意有两个优先级 一样的步骤");
            }
        }

       
       // if (!m_bLockStep)
       // {
       //     // 步骤 没有 锁定
       //     Debug.Log("步骤 没有 锁定");
       // }
       // else
       // {
       //     // 步骤 锁定了
       //     Debug.Log(" 步骤 锁定了");
       //
       // }
        // 步骤 没有锁定的
        int nBigIndex = 0;
        int nTmpBig = -1;
        int nLockBigStepIndex = m_nLockBigStepIndex;
        bool nLockStep = m_bLockStep;

       
        
        foreach (var sb in papers.data[paperManager.nIndexReCover].t1.step)
        {
           // bool bCheckLock = false;
            if (!nLockStep)
            {
                // 步骤 没有 锁定
                Debug.Log("步骤 没有 锁定");
            }
            else
            {
                // 步骤 锁定了
                Debug.Log(" 步骤 锁定了");
               if(nBigIndex != nLockBigStepIndex)
                {
                    nBigIndex++;
                    continue;
                }

            }
            // 判断 当前 是否有 可跳过的小步骤

            int nSmailJumpCount = 0;
            int nSmailIndex = 0;
            // 找出 最小优先级的 小步骤
            int nSLevelTmp = -1;

            int nSMaxLevelTmp = -1;
         //   int nNextSLevelTmp = -1;
            int nSLevelCount = 0;

            int nNoDoneNoJumpCount = 0; // 当前 没有做的 步骤的个数
            int nLenSmail = sb.step.Count;
            
            int nNoDoneCount = 0;
            foreach (var ss in sb.step)
            {
                if (!ss.bDone)
                {
                    nNoDoneCount++;
                }
            }
            int[] arrSmailLevel = new int[nNoDoneCount];

            int nI = 0;
            foreach (var ss in sb.step)
            {
                if (!ss.bDone)
                {
                    arrSmailLevel[nI++] = ss.level;
                }
            }

            // 冒泡排序方法
            for (int i = 0; i < arrSmailLevel.Length - 1; i++)
            {
                for (int x = 0; x < arrSmailLevel.Length - 1 - i; x++)
                {
                    if (arrSmailLevel[x] > arrSmailLevel[x + 1])
                    {
                        // 数值交换
                        int a = arrSmailLevel[x];
                        arrSmailLevel[x] = arrSmailLevel[x + 1];
                        arrSmailLevel[x + 1] = a;
                    }
                }
            }
            if (g_isReSetSmailLevel)
            {
                
               
                foreach (var ss in sb.step)
                {
                    if (!ss.bDone)
                    {

                        if (!ss.bCkJump)
                        {
                            nNoDoneNoJumpCount++;
                        }
                        if (nSLevelTmp == -1)
                        {
                            nSLevelTmp = ss.level;
                            //  nNextSLevelTmp = 1;
                            nSLevelCount = 1;
                            nSMaxLevelTmp = 1;
                        }
                        else
                        {
                            if (nSLevelTmp == ss.level)
                            {
                                nSLevelCount++;
                            }
                            nSLevelTmp = nSLevelTmp < ss.level ? nSLevelTmp : ss.level;
                            nSMaxLevelTmp = nSMaxLevelTmp < ss.level ? ss.level : nSMaxLevelTmp;
                            // 预测 下一个 优先级
                        }
                      //  arrSmailLevel[nI++] = ss.level;
                    }
                }

              


                if (nNoDoneNoJumpCount > 0 && nSMaxLevelTmp > nSLevelTmp)
                {
                    Debug.Log("当前的步骤 还有 不可跳过且必须要做的步骤   nNoDoneNoJumpCount = " + nNoDoneNoJumpCount + "最小优先级:" + nSLevelTmp + "最大优先级" + nSMaxLevelTmp);
                }
            }
          

           

            if (sb.blevel == nBigLevel)
            {
                if(nSLevelCount > 1)
                {
                    Debug.Log("当前小步骤 居然有两个 最小的优先级呢？" + nSLevelTmp);
                }
                foreach (var ss in sb.step)
                {
                    if(ss.level == nSLevelTmp && !ss.bDone)
                    {
                        Debug.LogFormat("{0},{1},{2}",ss.type_operation, ss.key , ss.value);
                        if(ss.type_operation == type_operation && ss.key == key)
                        {
                            // 触发了 该步骤的 操作了
                            if(ss.value == value)
                            {
                                nTmpBig = nBigIndex;
                                m_nLockSmailStepIndex = nSmailIndex;
                                // 表示做对了
                                if (!m_bLockStep)
                                {
                                    m_bLockStep = true;
                                    m_nLockBigStepIndex = nBigIndex;
                                }
                                Debug.Log("用户 做对了 哈哈！");
                                Debug.Log("判断 特效 对不对 啦啦啦！");

                                papers.data[paperManager.nIndexReCover].t1.step[nBigIndex].step[nSmailIndex].bDoneUser = true;
                                papers.data[paperManager.nIndexReCover].t1.step[nBigIndex].step[nSmailIndex].bDone = true;

                                tagRtDoStep.eStepRt = e_result.e_ok;
                            }
                            else
                            {
                                // 除了认知 表示 都做对了
                                if (ss.bCkMainStep)
                                {
                                    Debug.Log("关键步骤 居然 还做错了 真是遗憾了");

                                    tagRtDoStep.eStepRt = e_result.e_error_main;
                                }
                                if (ss.bCkJump)
                                {
                                    Debug.Log("当前是一个 答错了 就直接跳过的步骤");
                                    tagRtDoStep.eStepRt = e_result.e_error_jump;

                                    papers.data[paperManager.nIndexReCover].t1.step[nBigIndex].step[nSmailIndex].bDoneUser = false;
                                    papers.data[paperManager.nIndexReCover].t1.step[nBigIndex].step[nSmailIndex].bDone = true;
                                }
                            }
                        }
                        else
                        {
                            // 动态 调整小步骤的 优先级
                            //
                           // nSLevelTmp = nSLevelTmp + 1;
                            Debug.Log("动态调整小步骤的优先级");

                            int nTmp = 0;
                            for(int i = 0;i< arrSmailLevel.Length; i++)
                            {
                              
                                if(nTmp > nSLevelTmp)
                                {
                                    nSLevelTmp = nTmp;
                                    break;
                                }
                            }
                            Debug.Log("动态调整小步骤的优先级,调整后的结果是:nSLevelTmp" + nSLevelTmp);
                            // 判定 当前 最小 优先级 是否 还有 不可跳过的 
                            int nNoDoneSTmpCount = 0;
                            foreach(var k in sb.step)
                            {
                                if(!k.bDone && !k.bCkJump)
                                {
                                    nNoDoneSTmpCount++;
                                }
                            }
                            if(nNoDoneSTmpCount == 0)
                            {
                                // 如果 不存在了 那么 就 调整 下一个优先级
                                g_isReSetSmailLevel = false;
                                nSLevelTmp = nSLevelTmp+1;

                            }
                        }
                    }
                    else
                    {
                        Debug.Log("在小步骤 哈哈 没找到 匹配的 我们要继续 查找 nSmailIndex= " + nSmailIndex);

                        // 调整 小步骤的 优先级


                        if (nSmailIndex == sb.step.Count-1)
                        {
                            Debug.Log("我们在该大步骤的所有小步骤中 都没有找到哦！");

                            //  这里的 这个 nBigLevel 要 动态的 去修改一下

                            if (nNoDoneNoJumpCount == 0 && sb.blevel == nBigLevel) // 大步骤哦
                            {
                                nLockStep = false;
                                int nTmp = nBigLevel;
                                // 我们 必须 进行 大步骤的 跳跃 来寻找了
                                for (int i = 0; i < arrBigLevel.Length; i++)
                                {
                                    if (arrBigLevel[i] > nTmp)
                                    {
                                        nTmp = arrBigLevel[2];
                                        break;
                                    }
                                }
                                if(nTmp == nBigLevel)
                                {
                                    Debug.Log("完全 没有必要找了 ");
                                }
                                else
                                {
                                    nBigLevel = nTmp;
                                }
                            }
                        }
                    }


                    nSmailIndex++;
                }
            }
            nBigIndex++;
        }

        // 判断 当前的 大步骤 做完了 没有哦

        if(nTmpBig > -1)
        {
            int nK = 0;
            int nKCount = 0;
            int nKUser = 0;
            foreach (var sb in papers.data[paperManager.nIndexReCover].t1.step[nTmpBig].step)
            {
                if (sb.bDone)
                {
                    nKCount++;
                }
                if (sb.bDoneUser)
                {
                    nKUser++;
                }
                nK++;
            }
            if(nK == nKCount)
            {
                Debug.Log("该大步骤都做完了");
                papers.data[paperManager.nIndexReCover].t1.step[nTmpBig].bDone = true;
                m_bLockStep = false;
                if (nKUser == nK)
                {
                    Debug.Log("该大步骤 用户 都做对了 哈哈！");
                }
                else
                {
                    Debug.Log("该大步骤 用户 做完了 但是 有部分步骤 没有做对");
                }
            }
            else
            {
                Debug.Log("该大步骤 没有做完");
            }
        }

        // 判断 当前 试卷 做完了 没有哦

        int nOkTotal = 0;
        foreach (var sb in papers.data[paperManager.nIndexReCover].t1.step)
        {
            if (sb.bDone)
            {
                nOkTotal++;
            }
        }
        if(nOkTotal == papers.data[paperManager.nIndexReCover].t1.step.Count)
        {
            Debug.Log("该试卷 做完了");
        }
        else
        {
            Debug.Log("该试卷 没有做完");
        }

        foreach (var sb in papers.data[paperManager.nIndexReCover].t1.step)
        {
            
            foreach(var ss in sb.step)
            {

            }
        }
    }

    bool _g_blockstep = false;
    int _g_nlockstep = 0;
    int _g_npreOkStep = 0; //上一个 命中的大步骤
    void ReCoverStep2(string type_operation, string key, string value)
    {
        // 一直循环 直到 找到 所需要的结果
        int sbIndex = 0;
        int skIndex = 0;
        bool bFind = false;
        foreach (var sb in papers.data[paperManager.nIndexReCover].t1.step)
        { 
            if (!sb.bDone)
            {
                skIndex = 0;
                foreach (var sk in sb.step)
                {
                    if (!sk.bDone)
                    {
                        if(sk.type_operation == type_operation && sk.key == key)
                        {
                            if(sk.value == value)
                            {
                              //  Debug.Log("回答正确了.." + type_operation + key + value + sk.level + sbIndex + skIndex);
                             //   Debug.Log("回答正确了..blevel" + sb.blevel+ "   level" + sk.level + "   sbIndex" + sbIndex+ "  skIndex" + skIndex);


                              
                                bFind = true;
                                break;
                            }
                            else
                            {
                                if (sk.bCkMainStep)
                                {
                                    // 关键项错误了
                                }
                                if (sk.bCkJump)
                                {
                                    // 可以跳过的
                                }
                            }
                        }
                    }
                    skIndex++;
                }
            }
            if (bFind)
            {
              
                break;
            }
            sbIndex++;
        }
     //   Debug.Log("当前深度:sbIndex  " + sbIndex+ "  skIndex  " + skIndex);

        // 判定 当前的 大步骤
        // 判定 当前 命中 之前 是否 还有 不可跳过的
        int nIndexBig = 0;
        int nIndexSmail = 0;
        int nNodoneButNoJumpCount = 0;
        int nNodoneCount = 0;
        if (bFind)
        {
          //  Debug.Log("找到 匹配的操作！");
            int nBigLevel = 0;
            int nSmailLevel = 0;

            int nCountBigLevel = 0;
            int nCountSmailLevel = 0;
            foreach (var sb in papers.data[paperManager.nIndexReCover].t1.step)
            {
                if (!sb.bDone && nIndexBig <= sbIndex)
                {
                    if (nBigLevel == 0)
                    {
                        nBigLevel = sb.blevel;
                        nCountBigLevel++;
                    }
                    else
                    {
                        nBigLevel = nBigLevel < sb.blevel ? nBigLevel:sb.blevel;
                        if(nBigLevel == sb.blevel)
                        {
                            nCountBigLevel++;
                        }
                    }
                    nIndexSmail = 0;
                    foreach (var sk in sb.step)
                    {
                        if (!sk.bDone && !sk.bCkJump && nIndexSmail <= skIndex)
                        {
                            nNodoneButNoJumpCount++;
                        }

                        if (!sk.bDone)
                        {
                            nNodoneCount++;
                        }
                        nIndexSmail++;
                    }
                }
                nIndexBig++;
            }

          
        //    Debug.Log("当前大步骤的最小优先级是:" + nBigLevel);
        //    Debug.Log("当前大步骤的最小优先级个数:" + nCountBigLevel);
            // 找出 当前 未做的 大步骤的 最小优先级

            /////////////////////////////////////////////////////////
           if (nCountBigLevel >1 && nBigLevel == papers.data[paperManager.nIndexReCover].t1.step[sbIndex].blevel)
           {
           //    Debug.Log("当前的 大步骤 也是可以的 但是 必须要将 该大步骤 全部做完哦！");
               _g_blockstep = true;
               _g_nlockstep = sbIndex;
          
          
               if (_g_nlockstep == sbIndex)
               {
                  
               }
           }
            ////////////////////////////////////////////////////////


            ///
            int ntmpLevelSmail = 0;
            int ntmpLevelSmailCount = 0;
            foreach (var sg in papers.data[paperManager.nIndexReCover].t1.step[sbIndex].step)
            {
                if (!sg.bDone)
                {
                    if(ntmpLevelSmail == 0)
                    {
                        ntmpLevelSmail = sg.level;
                        ntmpLevelSmailCount++;
                    }
                    else
                    {
                        ntmpLevelSmail = ntmpLevelSmail < sg.level ? ntmpLevelSmail : sg.level;
                        if(ntmpLevelSmail == sg.level)
                        {
                            ntmpLevelSmailCount++;
                        }
                    }
                }
            }

            if (ntmpLevelSmailCount > 1)
            {
                // 当前 优先级吧

            }

            if (_g_blockstep)// 锁定的
            {
                if(_g_nlockstep == sbIndex)
                {
                    //
                    if(ntmpLevelSmail == papers.data[paperManager.nIndexReCover].t1.step[sbIndex].step[skIndex].level)
                    {
                        Debug.Log("锁定大步骤 回答正确了.." + type_operation + key + value + sbIndex + skIndex);
                        // 看看 是不是 小步骤的 优先级
                        papers.data[paperManager.nIndexReCover].t1.step[sbIndex].step[skIndex].bDone = true;
                       

                        if (nCountBigLevel > 1 && nBigLevel == papers.data[paperManager.nIndexReCover].t1.step[sbIndex].blevel)
                        {
                            //    Debug.Log("当前的 大步骤 也是可以的 但是 必须要将 该大步骤 全部做完哦！");
                            _g_blockstep = true;
                            _g_nlockstep = sbIndex;


                            if (_g_nlockstep == sbIndex)
                            {

                            }
                        }

                    }
                    
                }
            }
            else
            {
                if (nNodoneButNoJumpCount <= 1)
                {
                   
                    if (ntmpLevelSmail == papers.data[paperManager.nIndexReCover].t1.step[sbIndex].step[skIndex].level)
                    {
                        Debug.Log("锁定大步骤 回答正确了.." + type_operation + key + value + sbIndex + skIndex);
                        // 看看 是不是 小步骤的 优先级
                        papers.data[paperManager.nIndexReCover].t1.step[sbIndex].step[skIndex].bDone = true;
                      //  _g_npreOkStep = sbIndex;

                        if (nCountBigLevel > 1 && nBigLevel == papers.data[paperManager.nIndexReCover].t1.step[sbIndex].blevel)
                        {
                            //    Debug.Log("当前的 大步骤 也是可以的 但是 必须要将 该大步骤 全部做完哦！");
                            _g_blockstep = true;
                            _g_nlockstep = sbIndex;


                            if (_g_nlockstep == sbIndex)
                            {

                            }
                        }
                    }
                }
            }
            /// 设置 之前 的 都是 bdone
            /// 

            /*
            if (nNodoneButNoJumpCount <= 1)
            {

                if (nCountBigLevel > 1 && nBigLevel == papers.data[paperManager.nIndexReCover].t1.step[sbIndex].blevel)
                {
                 //   Debug.Log("当前的 大步骤 也是可以的 但是 必须要将 该大步骤 全部做完哦！");
                    if (!_g_blockstep)
                    {
                        _g_blockstep = true;
                        _g_nlockstep = sbIndex;
                    }
                   
                }

                if (_g_blockstep)
                {
                    if(_g_nlockstep == sbIndex)
                    {
                     //   Debug.Log("锁定大步骤 回答正确了.." + type_operation + key + value + sbIndex + skIndex);
                     //   papers.data[paperManager.nIndexReCover].t1.step[sbIndex].step[skIndex].bDone = true;
                    }
                }
                else
                {
                   // Debug.Log("不锁定大步骤 回答正确了.." + type_operation + key + value + sbIndex + skIndex);
                  //  papers.data[paperManager.nIndexReCover].t1.step[sbIndex].step[skIndex].bDone = true;
                }
              
               
            }*/

            int nCountss = 0;
            // 判定 当前 大步骤 是否 全部 做完 
            foreach (var sg in papers.data[paperManager.nIndexReCover].t1.step[sbIndex].step)
            {
                if (!sg.bDone)
                {
                    nCountss++;
                }
            }
            // 做完了
            if (nCountss == 0)
            {
                papers.data[paperManager.nIndexReCover].t1.step[sbIndex].bDone = true;
             
                _g_blockstep = false;
                _g_nlockstep = 0;

                // 设置 比在这个 优先级低的 都 设置 做完了
                foreach (var sg in papers.data[paperManager.nIndexReCover].t1.step)
                {
                    if (!sg.bDone)
                    {
                         if(sg.blevel< nBigLevel)
                        {
                            sg.bDone = true;
                        }
                    }
                }
            }
        }
        else
        {
            Debug.Log("没有找到 匹配的操作！");
        }
      
     //   Debug.Log("不可跳过的步骤个数为:" + nNodoneButNoJumpCount);
     }
    bool PreCheck(string type_operation, string key, string value)
    {
        // 要不就写个 while 循环 吧

        bool bOk = false;
        //预测 下个步骤 里面的值
        //前提条件：1.大步骤里面的小步骤，当前优先级是不是 只有 一个步骤了。2.小步骤是不是可以跳过。3.跳过之后的 后续呢
        // 这个 地方的 主要问题 是 进行 超前的 下个步骤的预判
        // 并重新进行 步骤的对比

        // 命中 判定
        int aNoDone = 0;
        for (int k = 0; k < papers.data[paperManager.nIndex].t1.step[paperManager.nBigStepIndex].step.Count; k++)
        {
            if (!papers.data[paperManager.nIndex].t1.step[paperManager.nBigStepIndex].step[k].bDone)
            {
                aNoDone++;
            }
        }

        if (aNoDone == 0)
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
            if (-1 != nMinLeve)
            {
                paperManager.nSmallStepLevel = nMinLeve;
            }
            // Debug.Log("调整后的 小优先级 为 ：" + paperManager.nSmallStepLevel);
        }
        return bOk;
    }
    tagDoStep XCheckSteps (string type_operation, string key, string value) {
        Debug.Log("xchecksteps "  + type_operation+ key+ value);
        //  bool bOk = false;
        tagDoStep eRt =new tagDoStep();
        
        eRt = JustCheck2(type_operation, key, value);
        CheckBigStep ();
        
        if (eRt.eStepRt == e_result.e_error)
        {
            // 预判一下 下个步骤
            PreCheck(type_operation, key, value);
        }
        else if (eRt.eStepRt == e_result.e_error_jump)
        {
            // 预判一下 下个步骤
            PreCheck(type_operation, key, value);
        }
        else if (eRt.eStepRt == e_result.e_error_main)
        {
            // 预判一下 下个步骤
            PreCheck(type_operation, key, value);
        }
        return eRt;
    }
    void CheckBigStep () {
        int aNoDone = 0;
        for (int k = 0; k < papers.data[paperManager.nIndex].t1.step[paperManager.nBigStepIndex].step.Count; k++) {
            if (!papers.data[paperManager.nIndex].t1.step[paperManager.nBigStepIndex].step[k].bDone) {
                aNoDone++;
            }
        }

        if (aNoDone == 0) {
            //  大步骤 做完了
            papers.data[paperManager.nIndex].t1.step[paperManager.nBigStepIndex].bDone = true;
            paperManager.nSmallStepLevel = 1;
            paperManager.nSmallStepIndex = 0;
            paperManager.bJustOneBigOver = true;
            paperManager.bLockIndex = false;
            // 获取 最小的 大步骤 优先级
            ChecKBigLevel ();
        } else {
            // 大步骤 没有 做完 只是 调整 小步骤的 最小优先级
            int nMinLeve = -1;
            for (int k = 0; k < papers.data[paperManager.nIndex].t1.step[paperManager.nBigStepIndex].step.Count; k++) {
                if (!papers.data[paperManager.nIndex].t1.step[paperManager.nBigStepIndex].step[k].bDone) {
                    int nLevel = papers.data[paperManager.nIndex].t1.step[paperManager.nBigStepIndex].step[k].level;
                    if (nMinLeve == -1) {
                        nMinLeve = nLevel;
                    } else {
                        nMinLeve = nMinLeve < nLevel ? nMinLeve : nLevel;
                    }
                }
            }
            if (-1 != nMinLeve) {
                paperManager.nSmallStepLevel = nMinLeve;
            }
            // Debug.Log("调整后的 小优先级 为 ：" + paperManager.nSmallStepLevel);
        }

    }
    void ChecKBigLevel () {
        // 选择 命中
        // 从当前列表中选择出优先级的数值最小的大步骤
        // 先检查 当前 优先级 是否 存在 大于 1个
        int nLevel = -1;
        for (int m = 0; m < papers.data[paperManager.nIndex].t1.step.Count; m++) {
            if (!papers.data[paperManager.nIndex].t1.step[m].bDone) {
                if (nLevel == -1) {
                    nLevel = papers.data[paperManager.nIndex].t1.step[m].blevel;
                } else {
                    nLevel = nLevel < papers.data[paperManager.nIndex].t1.step[m].blevel ? nLevel : papers.data[paperManager.nIndex].t1.step[m].blevel;
                }
            }

        }
        // 找出当前 最小优先级的 大步骤的 个数
        if (nLevel != -1) {
            paperManager.nBigStepLevel = nLevel;
            int nBigLeveHasMore = 0;
            for (int m = 0; m < papers.data[paperManager.nIndex].t1.step.Count; m++) {
                if (!papers.data[paperManager.nIndex].t1.step[m].bDone &&
                    nLevel == papers.data[paperManager.nIndex].t1.step[m].blevel) {
                    nBigLeveHasMore++;
                }
            }
            if (nBigLeveHasMore > 1) {
                paperManager.bBigHasMore = true; // 此处 决定了 他的 index 是需要 选择的
            }
        } else {

        }
    }
    bool IsTiMuOver () {
        int aNoDone = 0;
        for (int k = 0; k < papers.data[paperManager.nIndex].t1.step.Count; k++) {
            if (!papers.data[paperManager.nIndex].t1.step[k].bDone) {
                aNoDone++;
            }
        }
        if (aNoDone == 0) {
            papers.data[paperManager.nIndex].bDone = true;
            return true;
        }
        return false;
    }
    bool IsPaperOver () {
        int nCount = 0;
        for (int k = 0; k < 3; k++) {
            if (papers.data[k].bDone) {
                nCount++;
            }
        }
        //Debug.Log("已经答完的题目个数为:" + nCount);
        if (nCount == 3) {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 提交试卷
    /// </summary>
    public void SubmitPaper () {
        JObject job = new JObject ();
        job.Add ("key", "submit");
        JObject joper = new JObject ();
        joper.Add ("userid", papers.userid);
        joper.Add ("paperid", papers.paperid);
        joper.Add ("deskid", "25");
        joper.Add ("scoreid", papers.scoreid);
        job.Add ("other", joper);
        float fTotalScore = 0.0f;
        int nI = 0;
        JObject jVaule = new JObject ();
        foreach (var s in papers.data) {
            nI++;
            fTotalScore += s.fUserScore;
            JObject jPaper = new JObject ();
            jPaper.Add ("boilertype", s.boilertype);
            jPaper.Add ("opertype", s.opertype);
            jPaper.Add ("caption", s.t1.caption);
            jPaper.Add ("hard", s.t1.hard.caption);
            jPaper.Add ("fUserScore", s.fUserScore);

            // job.Add(jPaper);
            jVaule.Add (s.t, jPaper);
        }
        JObject jTmp = new JObject ();
        jTmp.Add ("paper", jVaule);
        jTmp.Add ("fTotalScore", fTotalScore);

        job.Add ("value", jTmp);

        //   GMaganer.userInfo.score = fTotalScore;

        Debug.Log ("JobStr:   " + job.ToString ());
        SendUserOperToServer (job.ToString ());
        SocketWithVC.sock.Submit ();
        papers = new Paper ();

#region
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
#endregion

    }
    /// /////////////////////////////////////////////////////////
    /// 以下为 试卷控制类
    public class PaperManager {
        public string strIndex { get; set; } // 试卷的题目 索引
        public int nIndex { get; set; } // 试卷的题目 索引
        public int nBigStepIndex { get; set; } // 大步骤索引
        public int nSmallStepIndex { get; set; } // 小步骤索引

        public int nBigStepLevel { get; set; } // 大步骤优先级
        public int nSmallStepLevel { get; set; } // 小步骤优先级

        public bool bFrist { get; set; } // 是不是第一次 答题
        public bool bBigHasMore { get; set; } // 是不是多个同一个优先级的步骤
        public bool bJustOneBigOver { get; set; } // 一个大步骤  刚好 做完了
        public bool bLockIndex { get; set; } // 一个大步骤  刚好 做完了

        public int nIndexReCover { get; set; } // 试卷的题目 索引
    }

#region 以下 是 解析 题目 存储类

    /////////////////////////////////////////////////
    //// 以下 是 解析 题目 存储类

    public class Boilertype {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 燃煤蒸汽锅炉
        /// </summary>

        public string caption { get; set; }
    }

    public class Boileropertype {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 认知类
        /// </summary>
        public string caption { get; set; }
    }

    public class Opertype {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 认知题目
        /// </summary>
        public string caption { get; set; }
    }

    public class Hard {
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string caption { get; set; }
    }

    public class StepItemTeXiao {
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

        // add 2017年11月27日 14:15:37 wsh
        // 特效的持续时间
        public int nTxTime { get; set; } //0是立即特效，10是在10秒内到该仪表值
        // 这个特效 具体怎么 执行 根据 特效的分类不同 ， 是不是要进行不同的处理。
        // 
    }
    public class InitItem {
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

    public class StepItem {
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
        public List<StepItemTeXiao> texiao = new List<StepItemTeXiao> ();
        /// <summary>
        /// 
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string value { get; set; }

        public bool bDone = false; // 是否做完了该步骤

        public bool bDoneUser = false; // 用户回答是否正确

        public string strUserSetvalue { get; set; }

        // add 2017年11月27日 14:10:24 wsh
        // 分值的权重
        public int nScoreWeight { get; set; }

        // 分值
        public float fScoreStep = 0.0f; // 该步骤的实际分值

        // 步骤 
        public bool bCkJump = false; // 是否跳过：跳过true,不跳过false

        public bool bCkMainStep = false; // 是否是关键项 (一步错，结束）
    }

    public class StepItemBig {
        /// <summary>
        /// 
        /// </summary>
        public int blevel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<StepItem> step = new List<StepItem> ();

        public bool bDone = false;

        public bool bDoneUser = false; // 用户回答是否正确

        // add 2017年11月27日 14:09:11 wsh
        public bool bCkJump = false; // 是否跳过：跳过true,不跳过false
    }

    public class T1 {
        /// <summary>
        /// 设备认知_汽水系统
        /// </summary>
        public string caption { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Boilertype boilertype = new Boilertype ();
        /// <summary>
        /// 
        /// </summary>
        public Boileropertype boileropertype = new Boileropertype ();
        /// <summary>
        /// 
        /// </summary>
        public Opertype opertype = new Opertype ();
        /// <summary>
        /// 
        /// </summary>
        public Hard hard = new Hard ();
        /// <summary>
        /// 设备认知_汽水系统
        /// </summary>
        public string remark { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<InitItem> init = new List<InitItem> ();
        /// <summary>
        /// 
        /// </summary>
        public List<StepItemBig> step = new List<StepItemBig> ();
        /// <summary>
        ///  每个 步骤的 分值 此处 暂时 取个评价值吧 后续 在修改成 可以配置的 在管理端
        /// </summary>
        //  public int nScore { get; set; } // 先注释 掉 后续 在完善

        public int nScoreTotalWeight = 0; // 总权重
    }

    public class DataItem {
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
        /// 在此题目中的每个步骤的分值 //此处 弃用 不知可否 换到 小步骤里面 对应 分值
        /// </summary>
        public float fStepScore { get; set; }

        public bool bDone = false;
    }

    public class Paper {
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
        public List<DataItem> data = new List<DataItem> ();
        /// <summary>
        ///  用户 在此试卷中的 得分  只算加分 吧
        /// </summary>
        public int nUserScore { get; set; }
    }
    public class PointItem {
        /// <summary>
        /// key值
        /// </summary>
        public string key { get; set; }
        /// <summary>
        /// 中文名称
        /// </summary>
        public string caption { get; set; }

        public string ShowMsg () {
            string str = "";

            str = " key " + key + " caption " + caption;
            return str;
        }
    }
    // 操作点的 列表
    public class Points {
        /// <summary>
        /// 锅炉类型
        /// </summary>
        public PointItem boilertype = new PointItem ();
        /// <summary>
        /// 操作类型 操作类、认知类
        /// </summary>
        public PointItem type = new PointItem ();
        /// <summary>
        /// 动画、特效、阀门、旋塞 等等
        /// </summary>
        public PointItem items = new PointItem ();
        /// <summary>
        /// 放空阀等具体名称
        /// </summary>
        public PointItem item = new PointItem ();
        /// <summary>
        /// 具体操作 打开 关闭
        /// </summary>
        public PointItem value = new PointItem ();

        public string ShowMsg () {
            string str = "";

            str = " 锅炉类型 " + boilertype.ShowMsg () +
                "操作类型:" + type.ShowMsg () +
                "部件类型:" + items.ShowMsg () +
                "部件名称:" + item.ShowMsg () +
                "部件值:" + value.ShowMsg ();
            return str;
        }
        public bool CheckPoint (string strboilertype, string strType, string strtype_operation, string strkey, string strvalue) {
            bool btr = false;
            if (strType == "type_renzhi") // 认知
            {
                if (strboilertype == boilertype.key &&
                    type.key == strType &&
                    items.key == strtype_operation &&
                    item.key == strkey) {
                    btr = true;
                }
            } else {
                if (strboilertype == boilertype.key &&
                    type.key == strType &&
                    items.key == strtype_operation &&
                    item.key == strkey &&
                    value.key == strvalue) {
                    btr = true;
                }
            }

            return btr;
        }
        public string GetOperCaption () {
            string strCaption = "";
            strCaption = item.caption + "   " + value.caption;
            return strCaption;
        }
        public string GetRenZhiCaption () {
            string strCaption = "";
            strCaption = item.caption;
            return strCaption;
        }
    }
#endregion
}