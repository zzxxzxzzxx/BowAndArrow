using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// RoomItem
/// 创建好的游戏房间面板，继承自BasePanel（面板基类）
/// </summary>
public class RoomItem : MonoBehaviour
{
    #region 成员变量
    /// <summary>
    /// 房主名文本
    /// </summary>
    public Text username;

    /// <summary>
    /// 房主总场数文本
    /// </summary>
    public Text totalCount;

    /// <summary>
    /// 房主胜利场数文本
    /// </summary>
    public Text winCount;

    /// <summary>
    /// 加入按钮
    /// </summary>
    public Button joinButton;

    /// <summary>
    /// 房间id，与房主id相同
    /// </summary>
    private int id;

    /// <summary>
    /// 房间列表面板
    /// </summary>
    private RoomListPanel panel;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// Start
    /// MonoBehaviour中的初始化
    /// </summary>
    private void Start ()
    {
        if (joinButton != null)
        {
            joinButton.onClick.AddListener(OnJoinClick); //加入按钮监听
        }
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 设置房间信息
    /// </summary>
    /// <param name="id">房间id或者房主id</param>
    /// <param name="username">房主名称</param>
    /// <param name="totalCount">房主总场数</param>
    /// <param name="winCount">房主胜利场数</param>
    /// <param name="panel">房间列表面板</param>
    public void SetRoomInfo(int id,string username, int totalCount, int winCount,RoomListPanel panel)
    {
        SetRoomInfo(id, username, totalCount.ToString(), winCount.ToString(), panel);
    }

    /// <summary>
    /// 重载设置房间信息方法
    /// </summary>
    /// <param name="id">房间id或者房主id</param>
    /// <param name="username">房主名称</param>
    /// <param name="totalCount">房主总场数</param>
    /// <param name="winCount">房主胜利场数</param>
    /// <param name="panel">房间列表面板</param>
    public void SetRoomInfo(int id,string username, string totalCount, string winCount, RoomListPanel panel)
    {
        this.id = id;
        this.username.text = username;
        this.totalCount.text = "总场数\n" + totalCount;
        this.winCount.text = "胜利\n" + winCount;
        this.panel = panel;
    }

    /// <summary>
    /// 销毁自身
    /// </summary>
    public void DestroySelf()
    {
        GameObject.Destroy(this.gameObject);
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 加入房间按钮监听
    /// </summary>
    private void OnJoinClick()
    {
        panel.OnJoinClick(id); //调用房间列表面板的发出加入房间申请方法
    }
    #endregion
}
