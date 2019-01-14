using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 用户信息
/// DAO
/// </summary>
public class UserData
{
    #region 构造方法
    /// <summary>
    /// 重载构造方法
    /// </summary>
    /// <param name="userData">用户信息字符串</param>
    public UserData(string userData)
    {
        //分割字符串存储用户信息
        string[] strs = userData.Split(',');
        this.Id = int.Parse(strs[0]);
        this.Username = strs[1];
        this.TotalCount = int.Parse(strs[2]);
        this.WinCount = int.Parse(strs[3]);
    }

    /// <summary>
    /// 重载构造方法
    /// </summary>
    /// <param name="username">用户名称</param>
    /// <param name="totalCount">总场数</param>
    /// <param name="winCount">胜利场数</param>
    public UserData(string username, int totalCount, int winCount)
    {
        this.Username = username;
        this.TotalCount = totalCount;
        this.WinCount = winCount;
    }

    /// <summary>
    /// 重载构造方法
    /// </summary>
    /// <param name="id">用户id</param>
    /// <param name="username">用户名称</param>
    /// <param name="totalCount">总场数</param>
    /// <param name="winCount">胜利场数</param>
    public UserData(int id,string username, int totalCount, int winCount)
    {
        this.Id = id;
        this.Username = username;
        this.TotalCount = totalCount;
        this.WinCount = winCount;
    }
    #endregion

    #region 成员变量
    /// <summary>
    /// 用户id属性
    /// 共有get
    /// 私有set
    /// </summary>
    public int Id { get;private set; }

    /// <summary>
    /// 用户名称属性
    /// 共有get
    /// 私有set
    /// </summary>
    public string Username { get;private set; }

    /// <summary>
    /// 总场数属性
    /// 共有get
    /// 共有set
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 胜利场数属性
    /// 共有get
    /// 共有set
    /// </summary>
    public int WinCount { get; set; }
    #endregion
}
