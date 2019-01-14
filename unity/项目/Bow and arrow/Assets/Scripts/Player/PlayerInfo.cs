using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// PlayerInfo
/// 玩家角色信息，继承自MonoBehaviour（可挂载到游戏物体上）
/// </summary>
public class PlayerInfo : MonoBehaviour
{
    #region 成员变量
    /// <summary>
    /// 角色类型
    /// </summary>
    public RoleType roleType;

    /// <summary>
    /// 是否本地的标记
    /// </summary>
    public bool isLocal = false;
    #endregion
}
