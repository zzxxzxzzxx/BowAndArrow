using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// Arrow
/// 控制箭的脚本，继承自MonoBehaviour（可挂载到游戏物体上）
/// </summary>
public class Arrow : MonoBehaviour
{
    #region 成员变量
    /// <summary>
    /// 角色类型
    /// </summary>
    public RoleType roleType;

    /// <summary>
    /// 箭飞行的速度
    /// </summary>
    public int speed = 5;

    /// <summary>
    /// 爆炸特效游戏物体
    /// </summary>
    public GameObject explosionEffect;

    /// <summary>
    /// 是否本地标记
    /// </summary>
    public bool isLocal = false;

    /// <summary>
    /// 刚体组件
    /// </summary>
    private Rigidbody rgd;

    /// <summary>
    /// 能造成的伤害
    /// </summary>
    public int damage;

    /// <summary>
    /// 强化攻击力标记
    /// </summary>
    public bool addDamageFlag = false;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// Start
    /// MonoBehaviour中的初始化
    /// </summary>
    private void Start ()
    {
        rgd = GetComponent<Rigidbody>(); //获取刚体组件
        gameObject.GetComponent<Light>().enabled = addDamageFlag;
	}

    /// <summary>
    /// Update
    /// MonoBehaviour中的每帧更新
    /// </summary>
    private void Update ()
    {
        rgd.MovePosition(transform.position+ transform.forward * speed * Time.deltaTime); //
	}

    /// <summary>
    /// OnTriggerEnter
    /// 触发器进入触发
    /// </summary>
    /// <param name="other">碰撞的物体信息</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            GameFacade.Instance.PlayNormalSound(AudioManager.Sound_ShootPerson); //打到角色的声音

            //是否本地的箭打到远程的角色，造成伤害
            if (isLocal)
            {
                bool playerIsLocal = other.GetComponent<PlayerInfo>().isLocal; 
                if (isLocal != playerIsLocal)
                {
                    GameFacade.Instance.SendAttack(damage); //伤害申请
                }
            }
        }
        else
        {
            GameFacade.Instance.PlayNormalSound(AudioManager.Sound_Miss); //发出打到场地的声音
        }
        GameObject.Instantiate(explosionEffect, transform.position, transform.rotation); //产生爆炸粒子特效
        GameObject.Destroy(this.gameObject); //销毁自身游戏物体
    }
    #endregion
}
