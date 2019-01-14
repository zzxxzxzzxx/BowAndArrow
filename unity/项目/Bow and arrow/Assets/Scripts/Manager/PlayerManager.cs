using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// PlayerManager
/// 角色管理器，继承自BaseManager（管理器基类）
/// </summary>
public class PlayerManager : BaseManager
{
    #region 构造方法
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="facade">facade中介者</param>
    public PlayerManager(GameFacade facade) : base(facade) { }
    #endregion

    #region 成员变量
    /// <summary>
    /// 用户数据
    /// </summary>
    private UserData userData;

    /// <summary>
    /// 用户数据属性
    /// </summary>
    public UserData UserData
    {
        set { userData = value; }
        get { return userData; }
    }

    /// <summary>
    /// 角色类型字典
    /// </summary>
    private Dictionary<RoleType, RoleData> roleDataDict = new Dictionary<RoleType, RoleData>();

    /// <summary>
    /// 本地角色类型
    /// </summary>
    private RoleType currentRoleType;

    /// <summary>
    /// 创建角色的位置
    /// </summary>
    private Transform rolePositions;

    /// <summary>
    /// 本地角色游戏物体
    /// </summary>
    private GameObject currentRoleGameObject;

    /// <summary>
    /// 服务器同步申请游戏物体
    /// </summary>
    private GameObject playerSyncRequest;

    /// <summary>
    /// 远程角色游戏物体
    /// </summary>
    private GameObject remoteRoleGameObject;

    /// <summary>
    /// 设计申请
    /// </summary>
    private ShootRequest shootRequest;

    /// <summary>
    /// 攻击申请
    /// </summary>
    private AttackRequest attackRequest;
    #endregion

    #region 提供的方法
    /// <summary>
    /// 重构初始化方法
    /// </summary>
    public override void OnInit()
    {
        rolePositions = GameObject.Find("RolePositions").transform; //获取创建角色的位置
        InitRoleDataDict(); //初始化角色字典
    }

    /// <summary>
    /// 设置当前角色的类型
    /// </summary>
    /// <param name="rt">RoleType角色类型</param>
    public void SetCurrentRoleType(RoleType rt)
    {
        currentRoleType = rt;
    }

    /// <summary>
    /// 创建所有角色
    /// </summary>
    public void SpawnRoles()
    {
        foreach(RoleData rd in roleDataDict.Values) //遍历角色字典
        {
            GameObject go= GameObject.Instantiate(rd.RolePrefab, rd.SpawnPosition, Quaternion.identity); //实例化角色
            go.tag = "Player"; //更改游戏物体标签为Player

            //区分本地角色和远程角色
            if (rd.RoleType == currentRoleType)
            {
                currentRoleGameObject = go; //获取本地角色
                currentRoleGameObject.GetComponent<PlayerInfo>().isLocal = true; //更改角色信息本地信息
            }
            else
            {
                remoteRoleGameObject = go; //获取远程角色
            }
        }
    }
 
    /// <summary>
    /// 给本地角色添加脚本
    /// </summary>
    public void AddControlScript()
    {
        currentRoleGameObject.AddComponent<PlayerMove>(); //给本地角色添加角色移动脚本
        PlayerAttack playerAttack = currentRoleGameObject.AddComponent<PlayerAttack>(); //给本地角色添加角色攻击脚本
        RoleType rt = currentRoleGameObject.GetComponent<PlayerInfo>().roleType; //获取角色类型
        RoleData rd = GetRoleData(rt); //取得角色类型信息
        playerAttack.arrowPrefab = rd.ArrowPrefab; //给角色攻击脚本添加箭预设体
        playerAttack.SetPlayerMng(this); //设置角色攻击脚本的角色管理器
    }

    /// <summary>
    /// 添加服务器同步申请游戏物体
    /// </summary>
    public void CreateSyncRequest()
    {
        playerSyncRequest=new GameObject("PlayerSyncRequest"); //创建游戏物体PlayerSyncRequest
        playerSyncRequest.AddComponent<MoveRequest>() //添加脚本移动申请
            .SetLocalPlayer(currentRoleGameObject.transform, currentRoleGameObject.GetComponent<PlayerMove>()) //设置本地角色
            .SetRemotePlayer(remoteRoleGameObject.transform); //设置远程角色
        shootRequest=playerSyncRequest.AddComponent<ShootRequest>(); //添加脚本射击申请
        shootRequest.playerMng = this; //设置射击申请脚本的角色管理器
        attackRequest = playerSyncRequest.AddComponent<AttackRequest>(); //添加攻击申请脚本
    }

    /// <summary>
    /// 获取本地角色游戏物体
    /// </summary>
    /// <returns></returns>
    public GameObject GetCurrentRoleGameObject()
    {
        return currentRoleGameObject;
    }

    /// <summary>
    /// 射击处理
    /// </summary>
    /// <param name="arrowPrefab">箭预设体</param>
    /// <param name="pos">创建的位置</param>
    /// <param name="rotation">当时的四元角</param>
    public void Shoot(GameObject arrowPrefab,Vector3 pos,Quaternion rotation)
    {
        facade.PlayNormalSound(AudioManager.Sound_Timer); //播放射击声音
        GameObject.Instantiate(arrowPrefab, pos, rotation) //实例化箭
            .GetComponent<Arrow>().isLocal = true; //设置箭的脚本属性为本地
        shootRequest.SendRequest(arrowPrefab.GetComponent<Arrow>().roleType, pos, rotation.eulerAngles); //发出射击申请
    }

    /// <summary>
    /// 远程客户端射击处理
    /// </summary>
    /// <param name="rt">箭预设体</param>
    /// <param name="pos">创建的位置</param>
    /// <param name="rotation">当时的四元角</param>
    public void RemoteShoot(RoleType rt, Vector3 pos, Vector3 rotation)
    {
        GameObject arrowPrefab = GetRoleData(rt).ArrowPrefab; //获取箭的预设体
        Transform transform = GameObject.Instantiate(arrowPrefab). //实例化箭
            GetComponent<Transform>();
        //修改箭的位置和四元角
        transform.position = pos; 
        transform.eulerAngles = rotation;
    }

    /// <summary>
    /// 攻击申请
    /// </summary>
    /// <param name="damage">伤害数值</param>
    public void SendAttack(int damage)
    {
        attackRequest.SendRequest(damage); //向服务器发出攻击申请
    }

    /// <summary>
    /// 更新战果信息
    /// </summary>
    /// <param name="totalCount">总场数</param>
    /// <param name="winCount">胜利场数</param>
    public void UpdateResult(int totalCount, int winCount)
    {
        userData.TotalCount = totalCount; //更新userData总场数
        userData.WinCount = winCount; //更新userData胜利场数
    }

    /// <summary>
    /// 游戏结束处理
    /// </summary>
    public void GameOver()
    {
        //private GameObject currentRoleGameObject;
        //private GameObject playerSyncRequest;
        //private GameObject remoteRoleGameObject;

        //private ShootRequest shootRequest;
        //private AttackRequest attackRequest;
        GameObject.Destroy(currentRoleGameObject); //销毁本地角色游戏物体
        GameObject.Destroy(playerSyncRequest); //销毁服务器同步申请游戏物体
        GameObject.Destroy(remoteRoleGameObject); //销毁远程角色游戏物体
        shootRequest = null; //断开设计申请引用
        attackRequest = null; //断开攻击申请引用
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 初始化游戏角色类型字典
    /// </summary>
    private void InitRoleDataDict()
    {
        roleDataDict.Add(RoleType.Blue, new RoleData(RoleType.Blue, "Hunter_BLUE", "Arrow_BLUE", "Explosion_BLUE", rolePositions.Find("Position1")));
        roleDataDict.Add(RoleType.Red, new RoleData(RoleType.Red, "Hunter_RED", "Arrow_RED", "Explosion_RED", rolePositions.Find("Position2")));
    }

    /// <summary>
    /// 根据游戏物体类型获取类型信息
    /// </summary>
    /// <param name="rt">RoleType游戏类型</param>
    /// <returns></returns>
    private RoleData GetRoleData(RoleType rt)
    {
        RoleData rd = null;
        roleDataDict.TryGetValue(rt, out rd);
        return rd;
    }
    #endregion
}
