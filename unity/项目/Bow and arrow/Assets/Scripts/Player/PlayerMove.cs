using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// PlayerMove
/// 控制角色移动，继承自MonoBehaviour（可挂载到游戏物体上）
/// </summary>
public class PlayerMove : MonoBehaviour
{
    #region 成员变量

    public float jumpSpeed = 100.0F;
    public float gravity = 100.0F;
    /// <summary>
    /// 
    /// </summary>
    public float forward = 0;

    /// <summary>
    /// 角色的移动速度
    /// </summary>
    private float speed = 3;

    /// <summary>
    /// 角色的动画
    /// </summary>
    private Animator anim;

    /// </summary>
    ///网格导航代理
    /// </summary>
    public NavMeshAgent agent;

    public CharacterController characterController;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// Start
    /// MonoBehaviour中的初始化
    /// </summary>
    private void Start ()
    {
        anim = GetComponent<Animator>(); //获取角色动画
	}

    /// <summary>
    /// FixedUpdate
    /// MonoBehaviour中每秒更新
    /// </summary>
    private void FixedUpdate ()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Grounded") == false) return; //如果角色移动状态为Grounded，才可以进行移动
        float h = Input.GetAxis("Horizontal"); //获取横向变化
        float v = Input.GetAxis("Vertical"); //获取纵向变化

        //有变化才去更新
        if (Mathf.Abs(h) > 0 || Mathf.Abs(v) > 0)
        {
            Vector3 moveDirection = Vector3.zero;
            if (characterController.isGrounded)
            {
                moveDirection = new Vector3(h, 0, v);
                //moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= speed;
                if (Input.GetButton("Jump"))
                    moveDirection.y = jumpSpeed;
                transform.rotation = Quaternion.LookRotation(new Vector3(h, 0, v)); //跟新角色转向
            }
            moveDirection.y -= gravity * Time.deltaTime;
            characterController.Move(moveDirection * Time.deltaTime);
            //agent.SetDestination(transform.position + new Vector3(h, 0, v) * speed);
            //transform.Translate(new Vector3(h, 0, v) * speed * Time.deltaTime, Space.World); //世界坐标去更新位置变化


            //选取变化大的方向作为移动量
            float res = Mathf.Max(Mathf.Abs(h), Mathf.Abs(v)); 
            forward = res; 
            anim.SetFloat("Forward", res); //混合树Forward足够大才会去更新移动动画
        }
	}
    #endregion
}
