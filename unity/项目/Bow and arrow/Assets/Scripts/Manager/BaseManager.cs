using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BaseManager
/// 管理器基类
/// 所有Manager都要继承这个类
/// 定义了Manager的基本功能
/// </summary>
public class BaseManager{
    #region 成员变量
    /// <summary>
    /// 中介者facade
    /// </summary>
    protected GameFacade facade;

    /// <summary>
    /// 构造方法，获取facade
    /// </summary>
    /// <param name="facade">facade</param>
    public BaseManager(GameFacade facade)
    {
        this.facade = facade;
    }
    #endregion

    #region 定义虚方法
    //该脚本没有继承MonoBehaviour，方法不会自动被调用

    /// <summary>
    /// 虚方法初始化
    /// </summary>
    public virtual void OnInit() { }

    /// <summary>
    /// 虚方法每帧更新
    /// </summary>
    public virtual void Update() { }

    /// <summary>
    /// 虚方法销毁
    /// </summary>
    public virtual void OnDestroy() { }
    #endregion
}
