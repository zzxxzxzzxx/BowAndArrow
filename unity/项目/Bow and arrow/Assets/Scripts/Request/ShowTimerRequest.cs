using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// ShowTimerRequest
/// 显示倒计时请求，继承自BaseRequest（请求基类）
/// </summary>
public class ShowTimerRequest : BaseRequest
{
    #region 成员变量
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
        //requestCode = RequestCode.Game;
        actionCode = ActionCode.ShowTimer; //倒计时的动作
        gamePanel = GetComponent<GamePanel>(); //获取游戏面板
        base.Awake(); //先于父类定义类型，然后才能添加字典
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 重写响应事件
    /// 回调方法不能同步调用，只能使用异步调用
    /// 响应倒计时事件
    /// </summary>
    /// <param name="data">服务器发送的响应数据</param>
    public override void OnResponse(string data)
    {
        int time = int.Parse(data); //解析数据
        gamePanel.ShowTimeSync(time); //调用游戏面板的异步倒计时方法
    }
    #endregion
}
