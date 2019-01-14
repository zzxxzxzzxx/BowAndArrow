using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    #region 枚举类型：请求类型
    /// <summary>
    /// 请求类型
    /// </summary>
    public enum RequestCode
    {
        /// <summary>
        /// 空
        /// </summary>
        None,

        /// <summary>
        /// 用户
        /// </summary>
        User,

        /// <summary>
        /// 房间
        /// </summary>
        Room,

        /// <summary>
        /// 游戏
        /// </summary>
        Game,
    }
    #endregion
}
