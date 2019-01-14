using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// MoveRequest
/// 移动请求，继承自BaseRequest（请求基类）
/// </summary>
public class MoveRequest : BaseRequest
{
    #region 成员变量
    /// <summary>
    /// 本地角色位置
    /// </summary>
    private Transform localPlayerTransform;

    /// <summary>
    /// 角色移动脚本
    /// </summary>
    private PlayerMove localPlayerMove;

    /// <summary>
    /// 异步同步频率
    /// </summary>
    private int syncRate = 30;

    /// <summary>
    /// 远程角色位置
    /// </summary>
    private Transform remotePlayerTransform;

    /// <summary>
    /// 远程角色动画
    /// </summary>
    private Animator remotePlayerAnim;

    /// <summary>
    /// 同步远程角色标记
    /// </summary>
    private bool isSyncRemotePlayer = false;

    /// <summary>
    /// 
    /// </summary>
    private Vector3 pos;

    /// <summary>
    /// 
    /// </summary>
    private Vector3 rotation;

    /// <summary>
    /// 
    /// </summary>
    private float forward;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// 重写唤醒方法
    /// </summary>
    public override void Awake()
    {
        requestCode = RequestCode.Game; //游戏的类型
        actionCode = ActionCode.Move; //移动的动作
        base.Awake(); //先于父类定义类型，然后才能添加字典
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 重写开始方法
    /// </summary>
    private void Start()
    {
        InvokeRepeating("SyncLocalPlayer", 1f, 1f / syncRate); //等待1s加载后，以syncRate频率同步远程角色信息
    }

    /// <summary>
    /// 重写每秒刷新方法
    /// </summary>
    private void FixedUpdate()
    {
        if (isSyncRemotePlayer) //同步标记
        {
            SyncRemotePlayer(); //进行一次同步
            isSyncRemotePlayer = false; //标记更新，等待下次同步
        }
    }

    /// <summary>
    /// 设置本地角色
    /// </summary>
    /// <param name="localPlayerTransform">本地角色位置</param>
    /// <param name="localPlayerMove">本地角色移动脚本</param>
    /// <returns>移动申请</returns>
    public MoveRequest SetLocalPlayer(Transform localPlayerTransform, PlayerMove localPlayerMove)
    {
        this.localPlayerTransform = localPlayerTransform;
        this.localPlayerMove = localPlayerMove;
        return this;
    }

    /// <summary>
    /// 设置远程角色
    /// </summary>
    /// <param name="remotePlayerTransform">远程角色位置</param>
    /// <returns>移动申请</returns>
    public MoveRequest SetRemotePlayer(Transform remotePlayerTransform)
    {
        this.remotePlayerTransform = remotePlayerTransform;
        this.remotePlayerAnim = remotePlayerTransform.GetComponent<Animator>();
        return this;
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 异步同步本地角色（向服务器申请同步）
    /// </summary>
    private void SyncLocalPlayer()
    {
        SendRequest(localPlayerTransform.position.x, localPlayerTransform.position.y, localPlayerTransform.position.z,
            localPlayerTransform.eulerAngles.x, localPlayerTransform.eulerAngles.y, localPlayerTransform.eulerAngles.z,
            localPlayerMove.forward);
    }

    /// <summary>
    /// 异步同步远程角色（同步到本地）
    /// </summary>
    private void SyncRemotePlayer()
    {
        remotePlayerTransform.position = pos;
        remotePlayerTransform.eulerAngles = rotation;
        remotePlayerAnim.SetFloat("Forward", forward);
    }

    /// <summary>
    /// 发送同步申请，调用父类的保护类型发送申请
    /// </summary>
    /// <param name="x">坐标x</param>
    /// <param name="y">坐标y</param>
    /// <param name="z">坐标z</param>
    /// <param name="rotationX">旋转状态x</param>
    /// <param name="rotationY">旋转状态y</param>
    /// <param name="rotationZ">旋转状态z</param>
    /// <param name="forward">混合树forward</param>
    private void SendRequest(float x,float y,float z,float rotationX,float rotationY,float rotationZ,float forward)
    {
        string data = string.Format("{0},{1},{2}|{3},{4},{5}|{6}", x, y, z, rotationX, rotationY, rotationZ, forward);
        base.SendRequest(data);
    }

    /// <summary>
    /// 重写响应事件
    /// 回调方法不能同步调用，只能使用异步调用
    /// 响应同步事件
    /// </summary>
    /// <param name="data">服务器发送的响应数据</param>
    public override void OnResponse(string data)
    {
        //分割数据
        string[] strs = data.Split('|');
        //使用工具类解析数据
        pos = UnityTools.ParseVector3(strs[0]); 
        rotation = UnityTools.ParseVector3(strs[1]);
        forward = float.Parse(strs[2]);
        isSyncRemotePlayer = true; //更改同步标记
    }
    #endregion
}
