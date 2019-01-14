using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// MessagePanel
/// 消息提示面板，继承自BasePanel（面板基类）
/// </summary>
public class MessagePanel : BasePanel
{
    #region 成员变量
    /// <summary>
    /// 消息提示文本
    /// </summary>
    private Text text;

    /// <summary>
    /// 显示时间
    /// </summary>
    private float showTime = 1;

    /// <summary>
    /// 显示信息字符串
    /// </summary>
    private string message = null;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// Update
    /// MonoBehaviour中的每帧更新
    /// </summary>
    private void Update()
    {
        //异步显示消息
        if (message != null) 
        {
            ShowMessage(message); //将消息显示到面板上
            message = null; //更新消息提示内容，等待下次显示
        }
    }

    /// <summary>
    /// OnEnter
    /// 重写BasePanel中的进入
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        text = GetComponent<Text>();
        text.enabled = false;
        uiMng.InjectMsgPanel(this); //向UI管理器添加本身
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 异步显示消息提示
    /// </summary>
    /// <param name="msg">消息提示字符串</param>
    public void ShowMessageSync(string msg)
    {
        message = msg;
    }

    /// <summary>
    /// 显示消息提示
    /// </summary>
    /// <param name="msg">消息提示字符串<</param>
    public void ShowMessage(string msg)
    {
        text.CrossFadeAlpha(1, 0.2f, false); //设置文本阿尔法值
        text.text = msg; //设置文本内容
        text.enabled = true; //显示文本
        Invoke("Hide", showTime); //延时隐藏文本
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 延时隐藏文本
    /// </summary>
    private void Hide()
    {
        text.CrossFadeAlpha(0, 1,false); //将文本的阿尔法值设置为0
    }
    #endregion
}
