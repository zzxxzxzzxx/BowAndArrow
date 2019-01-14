using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// StartPanel
/// 开始面板，继承自BasePanel（面板基类）
/// </summary>
public class StartPanel : BasePanel
{
    #region 成员变量
    /// <summary>
    /// 登录按钮
    /// </summary>
    private Button loginButton;

    /// <summary>
    /// 登录按钮动画 
    /// </summary>
    private Animator btnAnimator;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// Start
    /// MonoBehaviour中的初始化
    /// </summary>
    private void Start()
    {
        loginButton = transform.Find("LoginButton").GetComponent<Button>(); //获取登录按钮
        btnAnimator = loginButton.GetComponent<Animator>(); //获取登录动画
        loginButton.onClick.AddListener(OnLoginClick); //登录按钮监听
    }

    /// <summary>
    /// OnEnter
    /// 重写BasePanel中的进入
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter();
        gameObject.SetActive(true);
    }

    /// <summary>
    /// OnPause
    /// 重写BasePanel中的暂停
    /// </summary>
    public override void OnPause()
    {
        base.OnPause();
        btnAnimator.enabled = false; //关闭登录动画
        loginButton.transform.DOScale(0, 0.3f).OnComplete(() => loginButton.gameObject.SetActive(false)); //登录按钮逐渐变小为0，之后关闭登录按钮的显示
    }

    /// <summary>
    /// OnResume
    /// 重写BasePanel中的恢复
    /// </summary>
    public override void OnResume()
    {
        base.OnResume();
        loginButton.gameObject.SetActive(true); //激活登录按钮
        loginButton.transform.DOScale(1, 0.3f).OnComplete(() => btnAnimator.enabled = true); //登录按钮逐渐放大，之后激活登录动画
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 监听登录按钮
    /// </summary>
    private void OnLoginClick()
    {
        PlayClickSound(); //发出点击的声音
        uiMng.PushPanel(UIPanelType.Login); //加载登录面板
    }
    #endregion
}
