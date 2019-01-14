using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// StartPlayRequest
/// 玩家开始游戏请求，继承自BaseRequest（请求基类）
/// </summary>
public class StartPlayRequest : BaseRequest
{
    #region 成员变量
    /// <summary>
    /// 玩家开始游戏标记，用于异步加载控制脚本
    /// </summary>
    private bool isStartPlaying = false;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// 重写唤醒方法
    /// </summary>
    public override void Awake()
    {
        actionCode = ActionCode.StartPlay; //玩家开始控制游戏的动作
        base.Awake(); //先于父类定义类型，然后才能添加字典
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 重写响应事件
    /// 回调方法不能同步调用，只能使用异步调用
    /// 响应玩家开始游戏事件
    /// </summary>
    /// <param name="data">服务器发送的响应数据</param>
    public override void OnResponse(string data)
    {
        isStartPlaying = true; //更改玩家开始游戏标记
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// Update
    /// 挂载到游戏物体上自动被调用
    /// </summary>
    private void Update()
    {
        //异步开始玩家控制游戏
        if (isStartPlaying)
        {
            facade.StartPlaying(); //调用facade中介者中的玩家开始游戏的方法
            isStartPlaying = false; //更新玩家开始游戏标记，等待下次标记
        }
    }
    #endregion
}
