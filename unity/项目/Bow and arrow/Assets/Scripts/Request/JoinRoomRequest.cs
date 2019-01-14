using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// JoinRoomRequest
/// 加入房间请求，继承自BaseRequest（请求基类）
/// </summary>
public class JoinRoomRequest : BaseRequest
{
    #region 成员变量
    /// <summary>
    /// 游戏房间列表面板
    /// </summary>
    private RoomListPanel roomListPanel;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// 重写唤醒方法
    /// </summary>
    public override void Awake()
    {
        requestCode = RequestCode.Room; //游戏房间的类型
        actionCode = ActionCode.JoinRoom; //加入房间的动作
        roomListPanel = GetComponent<RoomListPanel>(); //获取游戏房间列表面板
        base.Awake(); //先于父类定义类型，然后才能添加字典
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 发送加入房间申请
    /// </summary>
    /// <param name="id">房间id</param>
    public void SendRequest(int id)
    {
        base.SendRequest(id.ToString()); //调用父类保护方法
    }

    /// <summary>
    /// 重写响应时间
    /// 回调方法不能同步调用，只能使用异步调用
    /// 响应加入房间事件
    /// </summary>
    /// <param name="data">服务器发送的响应数据</param>
    public override void OnResponse(string data)
    {
        //分割数据
        string[] strs = data.Split('-');
        string[] strs2 = strs[0].Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs2[0]);
        UserData ud1 = null;
        UserData ud2 = null;

        //加入房间成功后，解析房间成员信息
        if (returnCode == ReturnCode.Success) 
        {
            string[] udStrArray = strs[1].Split('|');
            ud1 = new UserData(udStrArray[0]);
            ud2 = new UserData(udStrArray[1]);

            RoleType roleType = (RoleType)int.Parse(strs2[1]);
            facade.SetCurrentRoleType(roleType); //设置当前的角色类型
        }

        roomListPanel.OnJoinResponse(returnCode, ud1, ud2); //异步更新，更新面板的加入房间响应
    }
    #endregion
}
