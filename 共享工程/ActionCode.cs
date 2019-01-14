using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    #region 枚举类型：动作类型
    /// <summary>
    /// 动作类型
    /// </summary>
    public enum ActionCode
    {
        /// <summary>
        /// 空
        /// </summary>
        None,

        /// <summary>
        /// 登录
        /// </summary>
        Login,

        /// <summary>
        /// 注册
        /// </summary>
        Register,

        /// <summary>
        /// 刷新房间列表
        /// </summary>
        ListRoom,

        /// <summary>
        /// 创建房间
        /// </summary>
        CreateRoom,

        /// <summary>
        /// 加入房间
        /// </summary>
        JoinRoom,

        /// <summary>
        /// 更新房间
        /// </summary>
        UpdateRoom,

        /// <summary>
        /// 退出房间
        /// </summary>
        QuitRoom,

        /// <summary>
        /// 开始游戏
        /// </summary>
        StartGame,

        /// <summary>
        /// 开始倒计时
        /// </summary>
        ShowTimer,

        /// <summary>
        /// 玩家开始控制
        /// </summary>
        StartPlay,

        /// <summary>
        /// 移动
        /// </summary>
        Move,

        /// <summary>
        /// 射击
        /// </summary>
        Shoot,

        /// <summary>
        /// 攻击
        /// </summary>
        Attack,

        /// <summary>
        /// 游戏结束
        /// </summary>
        GameOver,

        /// <summary>
        /// 更新游戏结果
        /// </summary>
        UpdateResult,

        /// <summary>
        /// 退出游戏
        /// </summary>
        QuitBattle
    }
    #endregion
}
