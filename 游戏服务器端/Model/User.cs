using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    /// <summary>
    /// User
    /// 用来存放数据库User表的信息
    /// </summary>
    class User
    {
        #region 构造方法
        /// <summary>
        /// 用户id+用户名称+用户密码的构造方法
        /// </summary>
        /// <param name="id">用户id</param>
        /// <param name="username">用户名称</param>
        /// <param name="password">用户密码</param>
        public User(int id,string username,string password)
        {
            this.Id = id;
            this.Username = username;
            this.Password = password;
        }
        #endregion

        #region 成员变量
        /// <summary>
        /// 用户Id属性
        /// 共有get
        /// 共有set
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户名称属性
        /// 共有get
        /// 共有set
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// 用户密码属性
        /// 共有get
        /// 共有set
        /// </summary>
        public string Password { get; set; }
        #endregion
    }
}
