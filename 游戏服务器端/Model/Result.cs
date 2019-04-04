using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    /// <summary>
    /// Result
    /// 用来存放数据库Result表的信息
    /// </summary>
    class Result
    {
        #region 构造方法
        /// <summary>
        /// id+用户id+总场数+胜利场数的构造方法
        /// </summary>
        /// <param name="id">主键id</param>
        /// <param name="userId">用户id</param>
        /// <param name="totalCount">总场数</param>
        /// <param name="winCount">胜利场数</param>
        public Result(int id, int userId,int totalCount,int winCount, int winRate)
        {
            this.Id = id;
            this.UserId = userId;
            this.TotalCount = totalCount;
            this.WinCount = winCount;
            this.WinRate = winRate;
        }
        #endregion

        #region 成员变量
        /// <summary>
        /// 主键Id属性
        /// 共有get
        /// 共有set
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户Id属性
        /// 共有get
        /// 共有set
        /// </summary>
        public int UserId { get; set; }

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

        /// <summary>
        /// 胜率属性
        /// 共有get
        /// 共有set        /// </summary>
        public int WinRate { get; set; }
        #endregion
    }
}
