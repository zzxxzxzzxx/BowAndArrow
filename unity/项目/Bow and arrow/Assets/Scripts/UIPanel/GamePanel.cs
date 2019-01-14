using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Common;

/// <summary>
/// GamePanel
/// 游戏面板，继承自BasePanel（面板基类）
/// </summary>
public class GamePanel : BasePanel
{
    #region 成员变量
    /// <summary>
    /// 倒计时文本
    /// </summary>
    private Text timer;

    /// <summary>
    /// 倒计时变量
    /// </summary>
    private int time = -1;

    /// <summary>
    /// 胜利结果按钮
    /// </summary>
    private Button successBtn;

    /// <summary>
    /// 失败结果按钮
    /// </summary>
    private Button failBtn;

    /// <summary>
    /// 退出按钮
    /// </summary>
    private Button exitBtn;

    /// <summary>
    /// 退出游戏申请脚本
    /// </summary>
    private QuitBattleRequest quitBattleRequest;

    private GamePanel gamePanel;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// Start
    /// MonoBehaviour中的初始化
    /// </summary>
    private void Start()
    {
        timer = transform.Find("Timer").GetComponent<Text>(); //找到倒计时文本
        timer.gameObject.SetActive(false); //关闭倒计时的显示
        successBtn = transform.Find("SuccessButton"). GetComponent<Button>(); //找到胜利的按钮
        successBtn.onClick.AddListener(OnResultClick); //设置胜利按钮的监听
        successBtn.gameObject.SetActive(false); //关闭胜利游戏物体的显示
        failBtn = transform.Find("FailButton").GetComponent<Button>();  //找到失败的按钮
        failBtn.onClick.AddListener(OnResultClick); //设置失败按钮的监听
        failBtn.gameObject.SetActive(false); //关闭失败游戏物体的显示
        exitBtn = transform.Find("ExitButton").GetComponent<Button>(); //找到退出的按钮
        exitBtn.onClick.AddListener(OnExitClick); //设置退出按钮的监听
        exitBtn.gameObject.SetActive(false); //关闭退出游戏物体的显示

        quitBattleRequest = GetComponent<QuitBattleRequest>(); //找到退出申请组件
    }

    /// <summary>
    /// Update
    /// MonoBehaviour中的每帧更新
    /// </summary>
    private void Update()
    {
        //异步倒计时显示
        if (time > -1)
        {
            ShowTime(time); //显示倒计时
            time = -1; //更新倒计时标记，等待下次标记
        }
    }

    /// <summary>
    /// OnEnter
    /// 重写BasePanel中的进入
    /// </summary>
    public override void OnEnter()
    {
        //激活当前UI的游戏物体
        gameObject.SetActive(true);
    }

    /// <summary>
    /// OnExit
    /// 重写BasePanel中的退出
    /// </summary>
    public override void OnExit()
    {
        //将当前面板响应UI显示关闭
        successBtn.gameObject.SetActive(false);
        failBtn.gameObject.SetActive(false);
        exitBtn.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
   #endregion

    #region 提供的方法
    /// <summary>
    /// 异步显示倒计时
    /// </summary>
    /// <param name="time">倒计时时间</param>
    public void ShowTimeSync(int time)
    {
        this.time = time;
    }

    /// <summary>
    /// 显示退出按钮
    /// </summary>
    public void ShowexitBtn()
    {
        exitBtn.gameObject.SetActive(true);
    }

    /// <summary>
    /// 显示倒计时
    /// </summary>
    /// <param name="time"></param>
    public void ShowTime(int time)
    {
        if (time == 1)
        {
            //倒计时的时候不能显示退出按钮，TO DEBUG
            exitBtn.gameObject.SetActive(true); //显示退出按钮
        }
        timer.gameObject.SetActive(true); //显示倒计时游戏物体
        timer.text = time.ToString(); //显示倒计时时间
        timer.transform.localScale = Vector3.one; //设置倒计时大小
        //设置倒计时阿尔法值
        Color tempColor = timer.color; 
        tempColor.a = 1;
        timer.color = tempColor;
        timer.transform.DOScale(2, 0.3f).SetDelay(0.3f); //倒计时大小变大
        timer.DOFade(0, 0.3f).SetDelay(0.3f).OnComplete(() => timer.gameObject.SetActive(false)); //倒计时时间隐藏，之后关闭游戏物体

        facade.PlayNormalSound(AudioManager.Sound_Alert); //向中介者申请发出倒计时声音
    }

    /// <summary>
    /// 响应游戏结束事件
    /// </summary>
    /// <param name="returnCode">服务器发送的响应动作</param>
    public void OnGameOverResponse(ReturnCode returnCode)
    {
        Button tempBtn = null;
        //显示游戏结果
        switch (returnCode)
        {
            case ReturnCode.Success:
                tempBtn = successBtn;
                break;
            case ReturnCode.Fail:
                tempBtn = failBtn;
                break;
        }

        tempBtn.gameObject.SetActive(true); //激活游戏结果游戏物体
        tempBtn.transform.localScale = Vector3.zero; //设置游戏结果大小
        tempBtn.transform.DOScale(1, 0.5f); //游戏结果变大
    }

    /// <summary>
    /// 响应退出游戏方法
    /// </summary>
    public void OnExitResponse()
    {
        OnResultClick(); //调用结果显示
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 游戏结果转到房间列表响应
    /// </summary>
    private void OnResultClick()
    {
        //弹出窗口
        uiMng.PopPanel();
        uiMng.PopPanel();

        //游戏结束处理
        facade.GameOver();
    }
    private void OnExitClick()
    {
        quitBattleRequest.SendRequest(); //向服务器发送退出游戏申请，服务器响应后，会再次调用到OnResultClick
    }
    #endregion
}
