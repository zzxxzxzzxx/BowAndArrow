using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// CameraManager
/// 摄像机管理器，继承自BaseManager（管理器基类）
/// </summary>
public class CameraManager : BaseManager
{
    #region 构造方法
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="facade">facade中介者</param>
    public CameraManager(GameFacade facade) : base(facade) { }
    #endregion

    #region 成员变量
    /// <summary>
    /// 主摄像机游戏物体
    /// </summary>
    private GameObject cameraGo;

    /// <summary>
    /// 摄像机动画，用于初始界面的漫游
    /// </summary>
    private Animator cameraAnim;

    /// <summary>
    /// 跟随目标脚本，用于摄像机跟随玩家
    /// </summary>
    private FollowTarget followTarget;

    /// <summary>
    /// 摄像机位置，用于摄像机漫游与跟随间的切换
    /// </summary>
    private Vector3 originalPosition;

    /// <summary>
    /// 摄像机旋转状态，用于摄像机漫游与跟随间的切换
    /// </summary>
    private Vector3 originalRotation;
    #endregion

    #region 提供的方法
    /// <summary>
    /// 重构初始化方法
    /// </summary>
    public override void OnInit()
    {
        cameraGo = Camera.main.gameObject; //获取主摄像机的游戏组件
        cameraAnim = cameraGo.GetComponent<Animator>(); //获取主摄像机的动画组件
        followTarget = cameraGo.GetComponent<FollowTarget>(); //获取主摄像机的跟随目标脚本
    }

    //public override void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        FollowTarget(null);
    //    }
    //    if (Input.GetMouseButtonDown(1))
    //    {
    //        WalkthroughScene(); 
    //    }
    //}

    /// <summary>
    /// 摄像机跟随角色
    /// </summary>
    public void FollowRole()
    {
        followTarget.target = facade.GetCurrentRoleGameObject().transform; //获得跟随目标的位置
        cameraAnim.enabled = false; //关闭摄像机动画，取消漫游状态
        originalPosition = cameraGo.transform.position; //记录当前摄像机的位置
        originalRotation = cameraGo.transform.eulerAngles; //记录当前摄像机的旋转状态

        Quaternion targetQuaternion = Quaternion.LookRotation(followTarget.target.position - originalPosition); //计算旋转的四元角
        cameraGo.transform.DORotateQuaternion(targetQuaternion, 1f).OnComplete(delegate() //摄像机旋转
        {
            followTarget.enabled = true; //旋转结束后，激活跟随脚本
        });
    }

    /// <summary>
    /// 摄像机漫游
    /// </summary>
    public void WalkthroughScene()
    {
        followTarget.enabled = false; //关闭跟随脚本
        cameraGo.transform.DOMove(originalPosition, 1f); //还原漫游状态（位置）
        cameraGo.transform.DORotate(originalRotation, 1f).OnComplete( delegate() //还原漫游状态（旋转状态）
        {
            cameraAnim.enabled = true; //还原漫游状态之后，激活摄像机动画，进行漫游
        });
    }
    #endregion
}
