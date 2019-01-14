using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Common;

/// <summary>
/// RegisterPanel
/// 注册面板，继承自BasePanel（面板基类）
/// </summary>
public class RegisterPanel : BasePanel
{
    #region 成员变量
    /// <summary>
    /// 用户名输入框
    /// </summary>
    private InputField usernameIF;

    /// <summary>
    /// 密码输入框
    /// </summary>
    private InputField passwordIF;

    /// <summary>
    /// 重复密码输入框
    /// </summary>
    private InputField rePasswordIF;

    /// <summary>
    /// 注册申请脚本
    /// </summary>
    private RegisterRequest registerRequest;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// Start
    /// MonoBehaviour中的初始化
    /// </summary>
    private void Start()
    {
        registerRequest = GetComponent<RegisterRequest>(); //获取注册申请脚本

        usernameIF = transform.Find("UsernameLabel/UsernameInput").GetComponent<InputField>(); //获取用户输入框组件
        passwordIF = transform.Find("PasswordLabel/PasswordInput").GetComponent<InputField>(); //获取用户密码组件
        rePasswordIF = transform.Find("RePasswordLabel/RePasswordInput").GetComponent<InputField>(); //获取用户重复密码组件

        transform.Find("RegisterButton").GetComponent<Button>().onClick.AddListener(OnRegisterClick); //添加注册按钮监听
        transform.Find("CloseButton").GetComponent<Button>().onClick.AddListener(OnCloseClick); //添加关闭按钮监听
    }


    /// <summary>
    /// OnEnter
    /// 重写BasePanel中的进入
    /// </summary>
    public override void OnEnter()
    {
        gameObject.SetActive(true); //显示注册面板

        transform.localScale = Vector3.zero; //设置初始大小为0
        transform.DOScale(1, 0.2f); //逐渐放大
        transform.localPosition = new Vector3(1000, 0, 0); //设置初始位置
        transform.DOLocalMove(Vector3.zero, 0.2f); //面板进入动画
    }


    /// <summary>
    /// OnExit
    /// 重写BasePanel中的退出
    /// </summary>
    public override void OnExit()
    {
        base.OnExit();
        gameObject.SetActive(false); //关闭注册面板显示
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 响应注册事件
    /// </summary>
    /// <param name="returnCode">服务器发送的响应动作</param>
    public void OnRegisterResponse(ReturnCode returnCode)
    {
        if (returnCode == ReturnCode.Success)
        {
            uiMng.ShowMessageSync("注册成功");
        }
        else
        {
            uiMng.ShowMessageSync("用户名重复");
        }
    }

    #endregion

    #region 私有方法
    /// <summary>
    /// 注册按钮监听
    /// </summary>
    private void OnRegisterClick()
    {
        PlayClickSound(); //发出点击的声音

        //检测账号、密码的正确性
        string msg = "";
        if (string.IsNullOrEmpty(usernameIF.text))
        {
            msg += "用户名不能为空";
        }
        if (string.IsNullOrEmpty(passwordIF.text))
        {
            msg += "\n密码不能为空";
        }
        if ( passwordIF.text!=rePasswordIF.text )
        {
            msg += "\n密码不一致";
        }
        if (msg != "")
        {
            uiMng.ShowMessage(msg);return;
        }
        
        registerRequest.SendRequest(usernameIF.text, passwordIF.text); //进行注册 发送到服务器端
    }

    /// <summary>
    /// 关闭按钮监听
    /// </summary>
    private void OnCloseClick()
    {
        PlayClickSound(); //发出点击的声音

        transform.DOScale(0, 0.4f); //逐渐变小为0
        Tweener tweener = transform.DOLocalMove(new Vector3(1000, 0, 0), 0.4f); //面板退出动画
        tweener.OnComplete(() => uiMng.PopPanel()); //退出结束后，弹出窗口
    }
    #endregion
}
