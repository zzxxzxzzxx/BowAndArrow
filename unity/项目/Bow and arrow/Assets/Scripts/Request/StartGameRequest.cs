using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// StartGameRequest
/// 开始游戏请求，继承自BaseRequest（请求基类）
/// </summary>
public class StartGameRequest : BaseRequest
{
    #region 成员变量
    /// <summary>
    /// 游戏房间面板
    /// </summary>
    private RoomPanel roomPanel;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// 重写唤醒方法
    /// </summary>
    public override void Awake()
    {
        requestCode = RequestCode.Game; //游戏的事件
        actionCode = ActionCode.StartGame; //开始游戏的动作
        roomPanel = GetComponent<RoomPanel>(); //获取游戏房间面板
        base.Awake(); //先于父类定义类型，然后才能添加字典
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 重写发送申请
    /// 发送开始游戏申请
    /// </summary>
    public override void SendRequest()
    {
        base.SendRequest("r"); //不需要发送具体数据，但为了防止空数据报错，发送一个“r”
    }

    /// <summary>
    /// 重写响应事件
    /// 回调方法不能同步调用，只能使用异步调用
    /// 响应开始游戏事件
    /// </summary>
    /// <param name="data">服务器发送的响应数据</param>
    public override void OnResponse(string data)
    {
        ReturnCode returnCode = (ReturnCode)int.Parse(data); //解析数据
        roomPanel.OnStartResponse(returnCode); //调用游戏房间面板的响应开始游戏的方法
    }
    #endregion
}
