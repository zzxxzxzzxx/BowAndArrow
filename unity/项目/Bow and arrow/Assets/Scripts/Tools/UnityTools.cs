using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

/// <summary>
/// UnityTools
/// 静态工具，帮助处理数据
/// </summary>
public static class UnityTools
{
    #region 静态方法
    /// <summary>
    /// 将对应模式的字符串转换成Vector3类型
    /// </summary>
    /// <param name="str">Vector3所需要的字符串，格式为“{0},{1},{2}”</param>
    /// <returns></returns>
    public static Vector3 ParseVector3(string str)
    {
        string[] strs = str.Split(',');
        float x = float.Parse(strs[0]);
        float y = float.Parse(strs[1]);
        float z = float.Parse(strs[2]);
        return new Vector3(x, y, z);
    }
    #endregion
}

