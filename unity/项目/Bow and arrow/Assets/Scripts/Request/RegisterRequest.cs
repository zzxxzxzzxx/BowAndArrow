using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// RegisterRequest
/// 创建注册请求，继承自BaseRequest（请求基类）
/// </summary>
public class RegisterRequest : BaseRequest
{
    #region 成员变量
    /// <summary>
    /// 注册面板
    /// </summary>
    private RegisterPanel registerPanel;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// 重写唤醒方法
    /// </summary>
    public override void Awake()
    {
        requestCode = RequestCode.User; //用户的事件
        actionCode = ActionCode.Register; //注册的动作
        registerPanel = GetComponent<RegisterPanel>(); //获取注册面板
        base.Awake(); //先于父类定义类型，然后才能添加字典
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 发送注册申请，调用父类的保护类型发送申请
    /// </summary>
    /// <param name="username">用户名</param>
    /// <param name="password">密码</param>
    public void SendRequest(string username, string password)
    {
        string data = username + "," + password; //组合数据
        base.SendRequest(data);  //调用父类的保护类型发送请求
    }

    /// <summary>
    /// 重写响应事件
    /// 回调方法不能同步调用，只能使用异步调用
    /// 响应注册事件
    /// </summary>
    /// <param name="data">服务器发送的响应数据</param>
    public override void OnResponse(string data)
    {
        ReturnCode returnCode = (ReturnCode)int.Parse(data); //获取响应结果
        registerPanel.OnRegisterResponse(returnCode); //调用注册面板上的响应注册的方法
    }
    #endregion
}
