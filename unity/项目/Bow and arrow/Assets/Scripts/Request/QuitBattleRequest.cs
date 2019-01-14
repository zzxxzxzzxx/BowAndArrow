using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// QuitBattleRequest
/// 在游戏房间退出游戏请求，继承自BaseRequest（请求基类）
/// </summary>
public class QuitBattleRequest : BaseRequest
{
    #region 成员变量
    /// <summary>
    /// 异步退出标记
    /// </summary>
    private bool isQuitBattle = false;

    /// <summary>
    /// 游戏面板
    /// </summary>
    private GamePanel gamePanel;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// 重写唤醒方法
    /// </summary>
    public override void Awake()
    {
        requestCode = RequestCode.Game; //游戏的类型
        actionCode = ActionCode.QuitBattle; //退出游戏的动作
        gamePanel = GetComponent<GamePanel>(); //获取游戏面板
        base.Awake(); //先于父类定义类型，然后才能添加字典
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 重写发送申请
    /// 发送退出游戏申请
    /// </summary>
    public override void SendRequest()
    {
        base.SendRequest("r"); ////不需要发送具体数据，但为了防止空数据报错，发送一个“r”
    }

    /// <summary>
    /// 重写响应事件
    /// 回调方法不能同步调用，只能使用异步调用
    /// 响应退出游戏事件
    /// </summary>
    /// <param name="data">服务器发送的响应数据</param>
    public override void OnResponse(string data)
    {
        isQuitBattle = true; //改变异步退出标记，准备异步退出游戏
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// Update
    /// 挂载到游戏物体上自动被调用
    /// </summary>
    private void Update()
    {
        //异步退出游戏
        if (isQuitBattle)
        {
            gamePanel.OnExitResponse(); //调用游戏面板响应退出事件
            isQuitBattle = false; //更改异步退出标记，等待下次标记
        }
    }
    #endregion
}
