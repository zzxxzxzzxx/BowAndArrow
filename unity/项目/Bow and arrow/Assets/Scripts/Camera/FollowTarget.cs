using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// FollowTarget
/// 摄像机跟随指定目标，继承自MonoBehaviour（可挂载到游戏物体上）
/// </summary>
public class FollowTarget : MonoBehaviour
{
    #region 成员变量
    //跟随目标的位置
    public Transform target;

    //摄像机初始位置
    private Vector3 offset = new Vector3(0, 11.98111f, -14.10971f);

    //摄像机移动速度（平滑程度）
    private float smoothing = 2;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// Update
    /// MonoBehaviour中的每帧更新
    /// </summary>
    private void Update ()
    {
        //计算摄像机跟随目标后的位置
        Vector3 targetPosition = target.position + offset;

        //摄像机根据插值平滑移动
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.deltaTime);

        //摄像机转向目标
        transform.LookAt(target);
	}
    #endregion
}
