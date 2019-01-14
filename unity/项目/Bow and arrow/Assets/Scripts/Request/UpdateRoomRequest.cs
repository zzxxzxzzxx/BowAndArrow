using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// UpdateRoomRequest
/// 更新游戏中信息请求，继承自BaseRequest（请求基类）
/// 用于服务器与客户端之间的同步
/// </summary>
public class UpdateRoomRequest : BaseRequest
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
        requestCode = RequestCode.Room; //房间的类型
        actionCode = ActionCode.UpdateRoom; //更新游戏房间的动作
        roomPanel = GetComponent<RoomPanel>(); //获取游戏房间面板
        base.Awake(); //先于父类定义类型，然后才能添加字典
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 重写响应事件
    /// 回调方法不能同步调用，只能使用异步调用
    /// 响应更新游戏房间事件
    /// </summary>
    /// <param name="data">服务器发送的响应数据</param>
    public override void OnResponse(string data)
    {
        UserData ud1 = null;
        UserData ud2 = null;
        //分割数据
        string[] udStrArray = data.Split('|');
        ud1 = new UserData(udStrArray[0]);
        if(udStrArray.Length>1)
            ud2 = new UserData(udStrArray[1]);
        roomPanel.SetAllPlayerResSync(ud1, ud2); //异步调用游戏房间面板的同步游戏房间方法
    }
    #endregion
}
