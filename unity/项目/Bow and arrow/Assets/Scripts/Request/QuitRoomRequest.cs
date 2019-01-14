using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// QuitRoomRequest
/// 创建退出房间请求，继承自BaseRequest（请求基类）
/// </summary>
public class QuitRoomRequest : BaseRequest
{
    #region 成员变量
    /// <summary>
    /// 房间面板
    /// </summary>
    private RoomPanel roomPanel;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// 重写唤醒方法
    /// </summary>
    public override void Awake()
    {
        requestCode = RequestCode.Room; //房间的类型
        actionCode = ActionCode.QuitRoom; //退出房间的动作
        roomPanel = GetComponent<RoomPanel>(); //获取房间面板
        base.Awake(); //先于父类定义类型，然后才能添加字典
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 重写发送申请
    /// 发送创建房间申请
    /// </summary>
    public override void SendRequest()
    {
        base.SendRequest("r"); //不需要发送具体数据，但为了防止空数据报错，发送一个“r”
    }

    /// <summary>
    /// 重写响应事件
    /// 回调方法不能同步调用，只能使用异步调用
    /// 响应退出房间事件
    /// </summary>
    /// <param name="data">服务器发送的响应数据</param>
    public override void OnResponse(string data)
    {
        ReturnCode returnCode = (ReturnCode)int.Parse(data); //得到响应结果
        if (returnCode == ReturnCode.Success)
        {
            roomPanel.OnExitResponse(); //成功退出房间后，调用房间面板的响应退出房间的方法
        }
    }
    #endregion
}
