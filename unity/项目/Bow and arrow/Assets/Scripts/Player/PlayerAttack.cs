using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerAttack
/// 控制角色攻击，继承自MonoBehaviour（可挂载到游戏物体上）
/// </summary>
public class PlayerAttack : MonoBehaviour
{
    #region 成员变量
    /// <summary>
    /// 箭预设体
    /// </summary>
    public GameObject arrowPrefab;
    
    /// <summary>
    /// 动画组件
    /// </summary>
    private Animator anim;

    /// <summary>
    /// 左手位置
    /// </summary>
    private Transform leftHandTrans;
    
    /// <summary>
    /// 射击方向
    /// </summary>
    private Vector3 shootDir;

    /// <summary>
    /// 角色管理器
    /// </summary>
    private PlayerManager playerMng;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// Start
    /// MonoBehaviour中的初始化
    /// </summary>
    void Start ()
    {
        anim = GetComponent<Animator>(); //获取动画组件
        leftHandTrans = transform.Find("Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 Neck/Bip001 L Clavicle/Bip001 L UpperArm/Bip001 L Forearm/Bip001 L Hand"); //获取左手位置
	}

    /// <summary>
    /// Update
    /// MonoBehaviour中的每帧更新
    /// </summary>
    void Update ()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded")) //动画状态是否为Grounded
        {
            if (Input.GetMouseButtonDown(0)) //监听左键按下
            {
                //获取点击位置
                Ray ray= Camera.main.ScreenPointToRay(Input.mousePosition); 
                RaycastHit hit;
                bool isCollider = Physics.Raycast(ray, out hit);
                if (isCollider)
                {
                    Vector3 targetPoint = hit.point;
                    targetPoint.y = transform.position.y;
                    shootDir = targetPoint - transform.position;
                    transform.rotation = Quaternion.LookRotation(shootDir); //转向点击位置
                    anim.SetTrigger("Attack"); //发起攻击动画
                    Invoke("Shoot", 0.1f); //延迟0.1s调用射击方法
                }
            }
        }
	}
    #endregion

    #region 提供的方法
    /// <summary>
    /// 设置角色管理器
    /// </summary>
    /// <param name="playerMng"></param>
    public void SetPlayerMng(PlayerManager playerMng)
    {
        this.playerMng = playerMng;
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 实例化箭的射击
    /// </summary>
    private void Shoot()
    {
        //将射击方法放置到角色管理器上，统一管理
        playerMng.Shoot(arrowPrefab, leftHandTrans.position, Quaternion.LookRotation(shootDir));
    }
    #endregion
}
