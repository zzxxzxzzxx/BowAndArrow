using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// ListRoomRequest
/// 刷新房间列表请求，继承自BaseRequest（请求基类）
/// </summary>
public class ListRoomRequest : BaseRequest
{
    #region 成员变量
    /// <summary>
    /// 房间列表面板
    /// </summary>
    private RoomListPanel roomListPanel;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// 重写唤醒方法
    /// </summary>
    public override void Awake()
    {
        requestCode = RequestCode.Room; //房间的类型
        actionCode = ActionCode.ListRoom; //刷新房间列表的动作
        roomListPanel = GetComponent<RoomListPanel>(); //获取房间列表面板
        base.Awake(); //先于父类定义类型，然后才能添加字典
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 重写发送申请
    /// 发送刷新房间列表申请
    /// </summary>
    public override void SendRequest()
    {
        base.SendRequest("r"); //不需要发送具体数据，但为了防止空数据报错，发送一个“r”
    }

    /// <summary>
    /// 重写响应事件   
    /// 回调方法不能同步调用，只能使用异步调用
    /// 响应刷新房间列表事件
    /// </summary>
    /// <param name="data">服务器发送的响应数据</param>
    public override void OnResponse(string data)
    {
        List<UserData> udList = new List<UserData>(); //存放房间的list
        if (data != "0") //房间不为空
        {
            //分割数据
            string[] udArray = data.Split('|');
            foreach (string ud in udArray)
            {
                string[] strs = ud.Split(',');
                udList.Add(new UserData(int.Parse(strs[0]), strs[1], int.Parse(strs[2]), int.Parse(strs[3])));
            }
        }
        
        roomListPanel.LoadRoomItemSync(udList); //异步更新，根据解析好的房间信息更新房间列表面板
    }
    #endregion
}
