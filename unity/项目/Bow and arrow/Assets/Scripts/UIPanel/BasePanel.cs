using UnityEngine;
using System.Collections;

/// <summary>
/// BasePanel
/// 面板基类，继承自MonoBehaviour（可挂载到游戏物体上）
/// 所有Panel都要继承这个类
/// 定义了Panel的基本功能
/// </summary>
public class BasePanel : MonoBehaviour
{
    #region 成员变量
    /// <summary>
    /// UI管理器
    /// </summary>
    protected UIManager uiMng;

    /// <summary>
    /// GameFacade中介者
    /// </summary>
    protected GameFacade facade;

    /// <summary>
    /// UI管理器属性
    /// 共有set
    /// </summary>
    public UIManager UIMng
    {
        set { uiMng = value; }
    }

    /// <summary>
    /// facade中介者属性
    /// 共有set
    /// </summary>
    public GameFacade Facade
    {
        set { facade = value; }
    }
    #endregion

    #region 保护方法
    /// <summary>
    /// 玩家点击声音
    /// </summary>
    protected void PlayClickSound()
    {
        facade.PlayNormalSound(AudioManager.Sound_ButtonClick); //向中介者申请发出点击按钮声音
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 界面被显示出来
    /// </summary>
    public virtual void OnEnter() { }

    /// <summary>
    /// 界面暂停
    /// </summary>
    public virtual void OnPause() { }

    /// <summary>
    /// 界面继续
    /// </summary>
    public virtual void OnResume() { }

    /// <summary>
    /// 界面不显示,退出这个界面，界面被关闭
    /// </summary>
    public virtual void OnExit() { }
    #endregion
}
