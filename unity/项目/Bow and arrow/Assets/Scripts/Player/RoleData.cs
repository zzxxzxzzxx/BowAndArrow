using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// 角色属性信息
/// 包括角色类型，角色预设体，箭预设体，角色的创建位置，爆炸粒子特效
/// </summary>
public class RoleData
{
    #region 构造方法
    /// <summary>
    /// 根据给予的角色类型，角色预设体路径，箭预设体路径，爆炸粒子特效路径，角色的创建位置创建角色属性信息
    /// </summary>
    /// <param name="roleType">角色类型</param>
    /// <param name="rolePath">角色预设体路径</param>
    /// <param name="arrowPath">箭预设体路径</param>
    /// <param name="explosionPath">爆炸粒子特效路径</param>
    /// <param name="spawnPos">角色的创建位置</param>
    public RoleData(RoleType roleType, string rolePath, string arrowPath, string explosionPath, Transform spawnPos)
    {
        this.RoleType = roleType;
        this.RolePrefab = Resources.Load(PREFIX_PREFAB + rolePath) as GameObject;
        this.ArrowPrefab = Resources.Load(PREFIX_PREFAB + arrowPath) as GameObject;
        this.ExplostionEffect = Resources.Load(PREFIX_PREFAB + explosionPath) as GameObject;
        ArrowPrefab.GetComponent<Arrow>().explosionEffect = ExplostionEffect;
        this.SpawnPosition = spawnPos.position;
    }
    #endregion

    #region 成员变量
    /// <summary>
    /// 加载路径
    /// </summary>
    private const string PREFIX_PREFAB = "Prefabs/";

    /// <summary>
    /// 角色类型属性
    /// 共有get
    /// 私有set
    /// </summary>
    public RoleType RoleType { get; private set; }

    /// <summary>
    /// 角色预设体属性
    /// 共有get
    /// 私有set
    /// </summary>
    public GameObject RolePrefab { get; private set; }

    /// <summary>
    /// 箭预设体
    /// 共有get
    /// 私有set
    /// </summary>
    public GameObject ArrowPrefab { get; private set; }

    /// <summary>
    /// 角色创建的位置
    /// 共有get
    /// 私有set
    /// </summary>
    public Vector3 SpawnPosition { get; private set; }

    /// <summary>
    /// 爆炸粒子特效
    /// 共有get
    /// 私有set
    /// </summary>
    public GameObject ExplostionEffect { get; private set; }
    #endregion
}
