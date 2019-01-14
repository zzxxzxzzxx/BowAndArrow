using System;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// UpdateResultRequest
/// 更新游戏结果请求，继承自BaseRequest（请求基类）
/// 用于服务器与客户端之间的同步
/// </summary>
public class UpdateResultRequest : BaseRequest
{
    #region 成员变量
    /// <summary>
    /// 房间列表面板
    /// </summary>
    private RoomListPanel roomListPanel;

    /// <summary>
    /// 异步同步游戏结果标记
    /// </summary>
    private bool isUpdateResult = false;

    /// <summary>
    /// 更新后的游戏总场数
    /// </summary>
    private int totalCount;

    /// <summary>
    /// 更新后的胜利场数
    /// </summary>
    private int winCount;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// 重写唤醒方法
    /// </summary>
    public override void Awake()
    {
        actionCode = ActionCode.UpdateResult; //更新游戏结果的动作
        roomListPanel = GetComponent<RoomListPanel>(); //获取房间列表面板
        base.Awake(); //先于父类定义类型，然后才能添加字典
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 重写响应事件
    /// 回调方法不能同步调用，只能使用异步调用
    /// 响应更新游戏结果事件
    /// </summary>
    /// <param name="data">服务器发送的响应数据</param>
    public override void OnResponse(string data)
    {
        //分割数据
        string[] strs = data.Split(',');
        totalCount = int.Parse(strs[0]);
        winCount = int.Parse(strs[1]);
        isUpdateResult = true; //更新更改游戏结果标记，准备异步更新
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// Update
    /// 挂载到游戏物体上自动被调用
    /// </summary>
    private void Update()
    {
        //异步更新游戏结果
        if (isUpdateResult)
        {
            roomListPanel.OnUpdateResultResponse(totalCount, winCount); //调用游戏房间列表面板的响应更新游戏结果方法
            isUpdateResult = false; //更新更新游戏结果标记，等待下次标记
        }
    }
    #endregion
}
