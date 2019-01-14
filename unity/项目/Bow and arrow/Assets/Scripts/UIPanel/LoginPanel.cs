using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Common;

/// <summary>
/// LoginPanel
/// 登录面板，继承自BasePanel（面板基类）
/// </summary>
public class LoginPanel : BasePanel
{
    #region 成员变量
    /// <summary>
    /// 关闭按钮
    /// </summary>
    private Button closeButton;

    /// <summary>
    /// 用户名输入框
    /// </summary>
    private InputField usernameIF;

    /// <summary>
    /// 密码输入框
    /// </summary>
    private InputField passwordIF;

    /// <summary>
    /// 登录申请脚本
    /// </summary>
    private LoginRequest loginRequest;
    //private Button loginButton;
    //private Button registerButton;
    #endregion

    #region 游戏物体事件
     /// <summary>
    /// Start
    /// MonoBehaviour中的初始化
    /// </summary>
    private void Start()
    {
        loginRequest = GetComponent<LoginRequest>(); //获取登录请求脚本
        usernameIF = transform.Find("UsernameLabel/UsernameInput").GetComponent<InputField>(); //获取用户名输入框组件
        passwordIF = transform.Find("PasswordLabel/PasswordInput").GetComponent<InputField>(); //获取密码输入框组件

        closeButton = transform.Find("CloseButton").GetComponent<Button>(); //获取关闭按钮

        closeButton.onClick.AddListener(OnCloseClick); //给关闭按钮添加监听
        transform.Find("LoginButton").GetComponent<Button>().onClick.AddListener(OnLoginClick); //给登录按钮添加监听
        transform.Find("RegisterButton").GetComponent<Button>().onClick.AddListener(OnRegisterClick); //给注册按钮添加监听
    }

    /// <summary>
    /// OnEnter
    /// 重写BasePanel中的进入
    /// </summary>
    public override void OnEnter()
    {
        base.OnEnter(); 
        EnterAnimation(); //进入动画
    }

    /// <summary>
    /// OnPause
    /// 重写BasePanel中的暂停
    /// </summary>
    public override void OnPause()
    {
        HideAnimation(); //退出动画
    }

    /// <summary>
    /// OnResume
    /// 重写BasePanel中的恢复
    /// </summary>
    public override void OnResume()
    {
        EnterAnimation(); //进入动画
    }

    /// <summary>
    /// OnExit
    /// 重写BasePanel中的退出
    /// </summary>
    public override void OnExit()
    {
        HideAnimation(); //退出动画
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 响应登录事件
    /// </summary>
    /// <param name="returnCode">服务器发送的响应动作</param>
    public void OnLoginResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Success)
        {
            uiMng.PushPanelSync(UIPanelType.RoomList); //成功返回房间列表
        }
        else
        {
            uiMng.ShowMessageSync("用户名或密码错误，无法登录，请重新输入!!"); //异常处理
        }
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 关闭按钮监听
    /// </summary>
    private void OnCloseClick()
    {
        PlayClickSound(); //发出点击声音
        uiMng.PopPanel(); //弹出窗口
    }

    /// <summary>
    /// 登录按钮监听
    /// </summary>
    private void OnLoginClick()
    {
        PlayClickSound(); //发出点击声音

        //判断账号正确性
        string msg = "";
        if(string.IsNullOrEmpty(usernameIF.text))
        {
            msg += "用户名不能为空 ";
        }
        if (string.IsNullOrEmpty(passwordIF.text))
        {
            msg += "密码不能为空 ";
        }
        if (msg != "")
        {
            uiMng.ShowMessage(msg);return;
        }

        loginRequest.SendRequest(usernameIF.text, passwordIF.text); //向服务器确认账号是否存在
    }

    /// <summary>
    /// 注册按钮监听
    /// </summary>
   private void OnRegisterClick()
    {
        PlayClickSound(); //发出点击声音
        uiMng.PushPanel(UIPanelType.Register); //加载注册面板
    }

    /// <summary>
    /// 进入动画显示
    /// </summary>
    private void EnterAnimation()
    {
        gameObject.SetActive(true); //显示登录面板
        transform.localScale = Vector3.zero; //大小为0
        transform.DOScale(1, 0.2f); //逐渐放大
        transform.localPosition = new Vector3(1000, 0, 0); //初始位置设置
        transform.DOLocalMove(Vector3.zero, 0.2f); //面板进入动画
    }

    /// <summary>
    /// 退出动画显示
    /// </summary>
    private void HideAnimation()
    {
        transform.DOScale(0, 0.3f); //大小变为0
        transform.DOLocalMoveX(1000, 0.3f).OnComplete(() => gameObject.SetActive(false)); //面板移出动画，结束后关闭登录面板显示
    }
    #endregion
}
