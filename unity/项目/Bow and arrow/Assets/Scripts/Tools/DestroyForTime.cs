using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DestroyForTime
/// 延时销毁，继承自MonoBehaviour（可挂载到游戏物体上）
/// 控制物体延时销毁
/// </summary>
public class DestroyForTime : MonoBehaviour
{
    #region 成员变量
    /// <summary>
    /// 延时销毁的时间
    /// </summary>
    public float time = 1;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// Start
    /// MonoBehaviour中的初始化
    /// </summary>
    private void Start ()
    {
        Destroy(this.gameObject, time); //time时间后，销毁本身
	}
    #endregion
}
