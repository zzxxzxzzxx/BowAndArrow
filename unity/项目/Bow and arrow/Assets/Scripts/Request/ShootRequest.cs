using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// ShootRequest
/// 创建射击请求，继承自BaseRequest（请求基类）
/// </summary>
public class ShootRequest : BaseRequest
{
    #region 成员变量
    /// <summary>
    /// 角色管理器
    /// </summary>
    public PlayerManager playerMng;

    /// <summary>
    /// 射击标记，用于异步处理
    /// </summary>
    private bool isShoot = false;

    /// <summary>
    /// 角色类型
    /// </summary>
    private RoleType rt;

    /// <summary>
    /// 射击位置
    /// </summary>
    private Vector3 pos;

    /// <summary>
    /// 射击旋转状态
    /// </summary>
    private Vector3 rotation;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// 重写唤醒方法
    /// </summary>
    public override void Awake()
    {
        requestCode = RequestCode.Game; //游戏的类型
        actionCode = ActionCode.Shoot; //射击的动作
        base.Awake(); //先于父类定义类型，然后才能添加字典
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 发送射击申请，调用父类的保护类型发送申请
    /// </summary>
    /// <param name="rt">角色类型</param>
    /// <param name="pos">射击位置</param>
    /// <param name="rotation">射击旋转状态</param>
    public void SendRequest(RoleType rt,Vector3 pos,Vector3 rotation)
    {
        string data = string.Format("{0}|{1},{2},{3}|{4},{5},{6}", (int)rt, pos.x, pos.y, pos.z, rotation.x, rotation.y, rotation.z);  //组合数据
        base.SendRequest(data); //调用父类的保护类型发送请求
    }

    /// <summary>
    /// 重写响应事件
    /// 回调方法不能同步调用，只能使用异步调用
    /// 响应射击事件
    /// </summary>
    /// <param name="data">服务器发送的响应数据</param>
    public override void OnResponse(string data)
    {
        //分割数据
        string[] strs = data.Split('|');
        RoleType rt = (RoleType)int.Parse(strs[0]);
        //使用工具类解析数据
        Vector3 pos = UnityTools.ParseVector3(strs[1]);
        Vector3 rotation = UnityTools.ParseVector3(strs[2]);
        isShoot = true; //更新射击标记，准备异步响应
        //对异步信息的赋值
        this.rt = rt;
        this.pos = pos;
        this.rotation = rotation;
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// Update
    /// 挂载到游戏物体上自动被调用
    /// </summary>
    private void Update()
    {
        //异步射击处理
        if (isShoot) 
        {
            playerMng.RemoteShoot(rt, pos, rotation); //调用角色管理器异步射击处理的方法
            isShoot = false; //更新射击标记，等待下次标记
        }
    }
    #endregion
}
