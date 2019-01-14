using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

/// <summary>
/// AttackRequest
/// 攻击请求，继承自BaseRequest（请求基类）
/// </summary>
public class AttackRequest : BaseRequest
{
    #region 游戏物体事件
    /// <summary>
    /// 重写唤醒方法
    /// </summary>
    public override void Awake()
    {
        requestCode = RequestCode.Game; //游戏中的类型
        actionCode = ActionCode.Attack; //攻击的动作
        base.Awake(); //先于父类定义类型，然后才能添加字典
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 发送攻击申请，调用保护类型的发送申请
    /// </summary>
    /// <param name="damage">伤害数值</param>
    public void SendRequest(int damage)
    {
        base.SendRequest(damage.ToString()); //调用父类的保护类型发送请求
    }
    #endregion
}

