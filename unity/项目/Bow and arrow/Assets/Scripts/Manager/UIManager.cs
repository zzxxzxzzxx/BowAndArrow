using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// UIManager
/// UI管理器，继承自BaseManager（管理器基类）
/// </summary>
public class UIManager: BaseManager
{
    #region 构造方法
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="facade">facade中介者</param>
    public UIManager(GameFacade facade) : base(facade)
    {
        ParseUIPanelTypeJson();
    }
    #endregion

    #region Json类
    /// <summary>
    /// Json类
    /// </summary>
    [Serializable]
    class UIPanelTypeJson
    {
        public List<UIPanelInfo> infoList;
    }
    #endregion

    #region 成员变量
    /// <summary>
    /// 面板位置
    /// </summary>
    private Transform canvasTransform;

    /// <summary>
    /// 面板属性
    /// </summary>
    private Transform CanvasTransform
    {
        get
        {
            if (canvasTransform == null)
            {
                canvasTransform = GameObject.Find("Canvas").transform; //寻找面板的位置
            }
            return canvasTransform;
        }
    }

    private Dictionary<UIPanelType, string> panelPathDict;//存储所有面板Prefab的路径
    private Dictionary<UIPanelType, BasePanel> panelDict;//保存所有实例化面板的游戏物体身上的BasePanel组件
    private Stack<BasePanel> panelStack;
    private MessagePanel msgPanel;
    private UIPanelType panelTypeToPush = UIPanelType.None;
    #endregion

    #region 提供的方法
    /// <summary>
    /// 重写初始化方法
    /// </summary>
    public override void OnInit()
    {
        base.OnInit(); //调用父类的初始化方法
        PushPanel(UIPanelType.Message); //建立UI显示信息窗口
        PushPanel(UIPanelType.Start); //建立UI开始窗口（登录）
    }

    /// <summary>
    /// 重写每帧更新方法
    /// </summary>
    public override void Update()
    {
        //异步加载窗口
        if (panelTypeToPush != UIPanelType.None)
        {
            PushPanel(panelTypeToPush); //加载对应窗口
            panelTypeToPush = UIPanelType.None; //还原标记，等待下次加载
        }
    }

    /// <summary>
    /// 异步加载窗口
    /// </summary>
    /// <param name="panelType">UIPanelType，UI窗口</param>
    public void PushPanelSync(UIPanelType panelType)
    {
        panelTypeToPush = panelType; //更改加载窗口标记
    }

    /// <summary>
    /// 把某个页面入栈，  把某个页面显示在界面上
    /// </summary>
    public BasePanel PushPanel(UIPanelType panelType)
    {
        if (panelStack == null)
            panelStack = new Stack<BasePanel>();

        //判断一下栈里面是否有页面
        if (panelStack.Count > 0)
        {
            BasePanel topPanel = panelStack.Peek();
            topPanel.OnPause(); //加载新页面，将之前的页面置于暂停状态
        }

        BasePanel panel = GetPanel(panelType); //获取栈顶元素
        panel.OnEnter(); //执行加载页面的进入方法
        panelStack.Push(panel); //新页面入栈
        return panel; //返回入栈页面
    }
    /// <summary>
    /// 出栈 ，把页面从界面上移除
    /// </summary>
    public void PopPanel()
    {
        if (panelStack == null)
            panelStack = new Stack<BasePanel>();

        if (panelStack.Count <= 0) return;

        //关闭栈顶页面的显示
        BasePanel topPanel = panelStack.Pop(); //获取栈顶元素
        topPanel.OnExit(); //调用栈顶离开方法

        //使关闭页面后，最上方的页面恢复执行状态
        if (panelStack.Count <= 0) return; //栈内没有元素直接返回
        BasePanel topPanel2 = panelStack.Peek(); //获取栈顶元素
        topPanel2.OnResume(); //调用栈顶恢复方法

    }

    /// <summary>
    /// 设置消息窗口
    /// </summary>
    /// <param name="msgPanel">MessagePanel消息窗口</param>
    public void InjectMsgPanel(MessagePanel msgPanel)
    {
        this.msgPanel = msgPanel;
    }

    /// <summary>
    /// 消息展示
    /// </summary>
    /// <param name="msg">消息内容</param>
    public void ShowMessage(string msg)
    {
        //异常信息处理
        if (msgPanel == null)
        {
            Debug.Log("无法显示提示信息，MsgPanel为空");return;
        }
        msgPanel.ShowMessage(msg); //消息展示
    }

    /// <summary>
    /// 异步消息展示
    /// </summary>
    /// <param name="msg">消息内容</param>
    public void ShowMessageSync(string msg)
    {
        //异常信息处理
        if (msgPanel == null)
        {
            Debug.Log("无法显示提示信息，MsgPanel为空"); return;
        }
        msgPanel.ShowMessageSync(msg); //异步消息展示
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 根据面板类型 得到实例化的面板
    /// </summary>
    /// <returns>实例化的UI</returns>
    private BasePanel GetPanel(UIPanelType panelType)
    {
        if (panelDict == null)
        {
            panelDict = new Dictionary<UIPanelType, BasePanel>();
        }

        BasePanel panel = panelDict.TryGet(panelType);

        if (panel == null) //如果找不到，那么就找这个面板的prefab的路径，然后去根据prefab去实例化面板
        {
            //string path;
            //panelPathDict.TryGetValue(panelType, out path);
            string path = panelPathDict.TryGet(panelType);
            GameObject instPanel = GameObject.Instantiate(Resources.Load(path)) as GameObject;
            instPanel.transform.SetParent(CanvasTransform, false);
            instPanel.GetComponent<BasePanel>().UIMng = this; //设置UI管理器
            instPanel.GetComponent<BasePanel>().Facade = facade; //设置facade中介者
            panelDict.Add(panelType, instPanel.GetComponent<BasePanel>()); //在窗口字典中添加窗口
            return instPanel.GetComponent<BasePanel>();
        }
        else
        {
            return panel;
        }
    }

    /// <summary>
    ///
    /// </summary>
    private void ParseUIPanelTypeJson()
    {
        panelPathDict = new Dictionary<UIPanelType, string>(); //创建路径字典
        
        //???
        TextAsset ta = Resources.Load<TextAsset>("UIPanelType");

        UIPanelTypeJson jsonObject = JsonUtility.FromJson<UIPanelTypeJson>(ta.text);

        //在路径字典中添加Json中的所有UI组件路径
        foreach (UIPanelInfo info in jsonObject.infoList)
        {
            //Debug.Log(info.panelType);
            panelPathDict.Add(info.panelType, info.path);
        }
    }
    #endregion

    /// <summary>
    /// just for test
    /// </summary>
    //public void Test()
    //{
    //    string path ;
    //    panelPathDict.TryGetValue(UIPanelType.Knapsack,out path);
    //    Debug.Log(path);
    //}
}
