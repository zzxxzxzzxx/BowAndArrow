using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Common;

/// <summary>
/// RoomPanel
/// 房间面板，继承自BasePanel（面板基类）
/// </summary>
public class RoomPanel : BasePanel
{
    #region 成员变量
    /// <summary>
    /// 本地角色用户名文本
    /// </summary>
    private Text localPlayerUsername;

    /// <summary>
    /// 本地角色总场数文本
    /// </summary>
    private Text localPlayerTotalCount;

    /// <summary>
    /// 本地角色胜利场数文本
    /// </summary>
    private Text localPlayerWinCount;

    /// <summary>
    /// 对手（远程）角色用户名文本
    /// </summary>
    private Text enemyPlayerUsername;

    /// <summary>
    /// 对手（远程）角色总场数文本
    /// </summary>
    private Text enemyPlayerTotalCount;

    /// <summary>
    /// 对手（远程）角色胜利场数文本
    /// </summary>
    private Text enemyPlayerWinCount;

    /// <summary>
    /// 蓝色面板
    /// </summary>
    private Transform bluePanel;

    /// <summary>
    /// 红色面板
    /// </summary>
    private Transform redPanel;

    /// <summary>
    /// 开始按钮
    /// </summary>
    private Transform startButton;

    /// <summary>
    /// 退出按钮
    /// </summary>
    private Transform exitButton;

    /// <summary>
    /// 用户信息ud
    /// </summary>
    private UserData ud = null;

    /// <summary>
    /// 用户信息ud1
    /// </summary>
    private UserData ud1 = null;

    /// <summary>
    /// 用户信息ud2
    /// </summary>
    private UserData ud2 = null;

    /// <summary>
    /// 退出房间申请脚本
    /// </summary>
    private QuitRoomRequest quitRoomRequest;

    /// <summary>
    /// 开始游戏申请脚本
    /// </summary>
    private StartGameRequest startGameRequest;

    /// <summary>
    /// 异步弹出面板标记
    /// </summary>
    private bool isPopPanel = false;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// Start
    /// MonoBehaviour中的初始化
    /// </summary>
    private void Start()
    {
        localPlayerUsername = transform.Find("BluePanel/Username").GetComponent<Text>(); //获取本地角色名称文本
        localPlayerTotalCount = transform.Find("BluePanel/TotalCount").GetComponent<Text>(); //获取本地角色总场数文本
        localPlayerWinCount = transform.Find("BluePanel/WinCount").GetComponent<Text>(); //获取本地角色胜利场数文本
        enemyPlayerUsername = transform.Find("RedPanel/Username").GetComponent<Text>(); //获取对手（远程）角色名称文本
        enemyPlayerTotalCount = transform.Find("RedPanel/TotalCount").GetComponent<Text>(); //获取对手（远程）角色总场数文本
        enemyPlayerWinCount = transform.Find("RedPanel/WinCount").GetComponent<Text>(); //获取对手（远程）角色胜利场数文本

        bluePanel = transform.Find("BluePanel"); //获取蓝色面板
        redPanel = transform.Find("RedPanel"); //获取红色面板
        startButton = transform.Find("StartButton"); //获取开始按钮
        exitButton = transform.Find("ExitButton"); //获取退出按钮

        transform.Find("StartButton").GetComponent<Button>().onClick.AddListener(OnStartClick); //开始按钮监听
        transform.Find("ExitButton").GetComponent<Button>().onClick.AddListener(OnExitClick); //退出按钮监听

        quitRoomRequest = GetComponent<QuitRoomRequest>(); //获取退出房间申请脚本
        startGameRequest = GetComponent<StartGameRequest>(); //获取开始游戏申请脚本

        EnterAnim(); //房间进入动画
    }

    /// <summary>
    /// Update
    /// MonoBehaviour中的每帧更新
    /// </summary>
    private void Update()
    {
        //异步创建房间设置房间信息
        if (ud != null)
        {
            SetLocalPlayerRes(ud.Username, ud.TotalCount.ToString(), ud.WinCount.ToString());
            ClearEnemyPlayerRes();
            ud = null;
        }

        //异步加入房间设置房间信息
        if (ud1 != null)
        {
            SetLocalPlayerRes(ud1.Username, ud1.TotalCount.ToString(), ud1.WinCount.ToString());
            if (ud2 != null)
                SetEnemyPlayerRes(ud2.Username, ud2.TotalCount.ToString(), ud2.WinCount.ToString());
            else
                ClearEnemyPlayerRes();
            ud1 = null; ud2 = null;
        }

        //异步弹出窗口
        if (isPopPanel)
        {
            uiMng.PopPanel(); 
            isPopPanel = false; //更新弹出窗口标记，等待下次标记
        }
    }

    /// <summary>
    /// OnEnter
    /// 重写BasePanel中的进入
    /// </summary>
    public override void OnEnter()
    {
        if(bluePanel!=null)
            EnterAnim(); //房间进入动画
    }

    /// <summary>
    /// OnExit
    /// 重写BasePanel中的退出
    /// </summary>
    public override void OnExit()
    {
        ExitAnim(); //房间退出动画
    }

    /// <summary>
    /// OnPause
    /// 重写BasePanel中的暂停
    /// </summary>
    public override void OnPause()
    {
        ExitAnim(); //房间退出动画
    }

    /// <summary>
    /// OnResume
    /// 重写BasePanel中的恢复
    /// </summary>
    public override void OnResume()
    {
        EnterAnim(); //房间进入动画
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 异步设置本地用户信息
    /// </summary>
    public void SetLocalPlayerResSync()
    {
        ud = facade.GetUserData(); //调用中介者提供的获取用户信息方法
    }

    /// <summary>
    /// 异步设置双方用户信息
    /// </summary>
    /// <param name="ud1">用户信息ud1</param>
    /// <param name="ud2">用户信息ud2</param>
    public void SetAllPlayerResSync(UserData ud1,UserData ud2)
    {
        this.ud1 = ud1;
        this.ud2 = ud2;
    }

    /// <summary>
    /// 设置本地用户信息
    /// </summary>
    /// <param name="username">本地用户名称</param>
    /// <param name="totalCount">本地用户总场数</param>
    /// <param name="winCount">本地用户胜利场数</param>
    public void SetLocalPlayerRes(string username, string totalCount, string winCount)
    {
        localPlayerUsername.text = username;
        localPlayerTotalCount.text = "总场数：" + totalCount;
        localPlayerWinCount.text = "胜利：" + winCount;
    }

    /// <summary>
    /// 初始化（清空）对手（远程）用户信息
    /// </summary>
    public void ClearEnemyPlayerRes()
    {
        enemyPlayerUsername.text = "";
        enemyPlayerTotalCount.text = "等待玩家加入....";
        enemyPlayerWinCount.text = "";
    }

    /// <summary>
    /// 响应退出房间时间
    /// </summary>
    public void OnExitResponse()
    {
        isPopPanel = true;
    }

    /// <summary>
    /// 响应游戏开始事件
    /// </summary>
    /// <param name="returnCode">服务器发送的响应动作</param>
    public void OnStartResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Fail)
        {
            uiMng.ShowMessageSync("您不是房主，无法开始游戏！！");
        }
        else
        {
            uiMng.PushPanelSync(UIPanelType.Game); //异步加载游戏面板
            facade.EnterPlayingSync(); //调用中介者的异步开始游戏方法
        }
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 设置对手（远程）用户信息
    /// </summary>
    /// <param name="username">用户名称</param>
    /// <param name="totalCount">总场数</param>
    /// <param name="winCount">胜利场数</param>
    private void SetEnemyPlayerRes(string username, string totalCount, string winCount)
    {
        enemyPlayerUsername.text = username;
        enemyPlayerTotalCount.text = "总场数：" + totalCount;
        enemyPlayerWinCount.text = "胜利：" + winCount;
    }

    /// <summary>
    /// 开始按钮监听
    /// </summary>
    private void OnStartClick()
    {
        startGameRequest.SendRequest(); //发送开始游戏申请
    }

    /// <summary>
    /// 退出按钮监听
    /// </summary>
    private void OnExitClick()
    {
        quitRoomRequest.SendRequest(); //发送退出房间申请
    }

    /// <summary>
    /// 房间进入动画
    /// </summary>
    private void EnterAnim()
    {
        gameObject.SetActive(true); //显示房间
        bluePanel.localPosition = new Vector3(-1000, 0, 0); //设置蓝色面板初始位置
        bluePanel.DOLocalMoveX(-174, 0.4f); //蓝色面板移动
        redPanel.localPosition = new Vector3(1000, 0, 0); //设置红色面板初始位置
        redPanel.DOLocalMoveX(174, 0.4f); //红色面板移动
        startButton.localScale = Vector3.zero; //设置开始按钮初始大小
        startButton.DOScale(1, 0.4f); //开始按钮逐渐变大
        exitButton.localScale = Vector3.zero; //设置退出按钮大小
        exitButton.DOScale(1, 0.4f); //退出按钮逐渐变大
    }

    /// <summary>
    /// 房间退出动画
    /// </summary>
    private void ExitAnim()
    {
        bluePanel.DOLocalMoveX(-1000, 0.4f); //蓝色面板移出
        redPanel.DOLocalMoveX(1000, 0.4f); //红色面板移出
        startButton.DOScale(0, 0.4f); //开始按钮逐渐变小为0
        exitButton.DOScale(0, 0.4f).OnComplete(() => gameObject.SetActive(false)); //退出按钮逐渐变小为0，结束后关闭房间面板显示
    }
    #endregion
}
