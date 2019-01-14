using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// LoginRequest
/// 登录请求，继承自BaseRequest（请求基类）
/// </summary>
public class LoginRequest : BaseRequest
{
    #region 成员变量
    /// <summary>
    /// 登录面板
    /// </summary>
    private LoginPanel loginPanel;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// 重写唤醒方法
    /// </summary>
    public override void Awake()
    {
        requestCode = RequestCode.User; //用户的类型
        actionCode = ActionCode.Login; //登录的动作
        loginPanel = GetComponent<LoginPanel>(); //获取登录面板
        base.Awake(); //先于父类定义类型，然后才能添加字典
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 发送登录申请，调用父类的保护类型发送申请
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    public void SendRequest(string username, string password)
    {
        string data = username + "," + password; //组合数据
        base.SendRequest(data); //调用父类的保护类型发送请求
    }

    /// <summary>
    /// 重写响应事件
    /// 回调方法不能同步调用，只能使用异步调用
    /// 响应登录事件
    /// </summary>
    /// <param name="data">服务器发送的响应数据</param>
    public override void OnResponse(string data)
    {
        //分割数据
        string[] strs = data.Split(',');
        ReturnCode returnCode = (ReturnCode)int.Parse(strs[0]);
        loginPanel.OnLoginResponse(returnCode); //异步调用登录面板响应事件

        //成功登录
        if (returnCode == ReturnCode.Success)
        {
            string username = strs[1];
            int totalCount = int.Parse(strs[2]);
            int winCount = int.Parse(strs[3]);
            UserData ud = new UserData(username, totalCount, winCount); //生成用户DAO
            facade.SetUserData(ud); //调用角色管理器的设置用户数据
        }
    }
    #endregion
}
