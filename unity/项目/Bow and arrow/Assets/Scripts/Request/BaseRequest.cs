using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// BaseRequest
/// 请求基类，继承自MonoBehaviour（可挂载到游戏物体上）
/// 所有Request都要继承这个类
/// 定义了Request的基本功能
/// </summary>
public class BaseRequest : MonoBehaviour
{
    #region 成员变量
    /// <summary>
    /// 请求类型，用于服务器判断请求的类型
    /// 初始默认为None，由子类修改
    /// </summary>
    protected RequestCode requestCode = RequestCode.None;

    /// <summary>
    /// 请求类型，用于服务器判断请求类型使用的方法，以及响应时使用的方法
    /// 初始默认为None，由子类修改
    /// </summary>
    protected ActionCode actionCode = ActionCode.None;

    /// <summary>
    /// GameFacade中介者
    /// </summary>
    protected GameFacade _facade;

    /// <summary>
    /// GameFacade中介者的属性
    /// 共有get
    /// </summary>
    protected GameFacade facade
    {
        get
        {
            if (_facade == null)
                _facade = GameFacade.Instance;
            return _facade;
        }
    }
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// Awake
    /// 虚方法唤醒，挂载到游戏物体上自动调用
    /// </summary>
    public virtual void Awake()
    {
        facade.AddRequest(actionCode, this); //创建申请时，自动在请求管理器添加对应的字典值
    }

    /// <summary>
    /// OnDestroy
    /// 虚方法销毁，挂载到游戏物体上自动调用
    /// </summary>
    public virtual void OnDestroy()
    {
        //自动销毁请求管理器中对应的字典值
        if (facade != null)
            facade.RemoveRequest(actionCode);
        Destroy(this);
    }
   #endregion

    #region 提供的方法
    /// <summary>
    /// 虚方法发送申请
    /// 用于外界调用发送申请，一般调用保护类型的重写发送申请方法
    /// 没有参数传递，子函数不需要发送具体数据时去重写，发送具体数据时具体创建
    /// </summary>
    public virtual void SendRequest() { }

    /// <summary>
    /// 虚方法响应事件
    /// </summary>
    /// <param name="data">由服务器发送回来的响应数据</param>
    public virtual void OnResponse(string data) { }
    #endregion

    #region 保护的方法，子类及自身可调用
    /// <summary>
    /// 由用户管理器发送申请
    /// </summary>
    /// <param name="data">发送申请的数据</param>
    protected void SendRequest(string data)
    {
        facade.SendRequest(requestCode, actionCode, data); //由中介者发送申请
    }
    #endregion
}
