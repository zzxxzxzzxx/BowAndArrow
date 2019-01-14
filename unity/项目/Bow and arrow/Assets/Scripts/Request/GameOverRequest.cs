using System;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// GameOverRequest
/// 游戏结束请求，继承自BaseRequest（请求基类）
/// </summary>
public class GameOverRequest : BaseRequest
{
    #region 成员变量
    /// <summary>
    /// 游戏面板
    /// </summary>
    private GamePanel gamePanel;

    /// <summary>
    /// 游戏结束标记
    /// </summary>
    private bool isGameOver = false;

    /// <summary>
    /// 状态类型，用于异步传递
    /// </summary>
    private ReturnCode returnCode;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// 重写唤醒方法
    /// </summary>
    public override void Awake()
    {
        requestCode = RequestCode.Game; //游戏中的类型
        actionCode = ActionCode.GameOver; //游戏结束的动作
        gamePanel = GetComponent<GamePanel>(); //获取游戏面板
        base.Awake(); //先于父类定义类型，然后才能添加字典
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 重写响应事件
    /// 回调方法不能同步调用，只能使用异步调用
    /// 响应游戏结束事件
    /// </summary>
    /// <param name="data">服务器发送的响应数据</param>
    public override void OnResponse(string data)
    {
        returnCode = (ReturnCode)int.Parse(data); //更新动作的类型
        isGameOver = true; //改变游戏结束标记
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// Update
    /// 挂载到游戏物体上自动被调用
    /// </summary>
    private void Update()
    {
        //异步结束游戏，根据游戏结束标记，去响应游戏结束
        if (isGameOver)
        {
            gamePanel.OnGameOverResponse(returnCode); //游戏面板响应游戏结束
            isGameOver = false; //更改游戏结束标记，等待下次标记
        }
    }
    #endregion
}
