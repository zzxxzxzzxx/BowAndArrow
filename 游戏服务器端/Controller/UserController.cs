using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;
using GameServer.DAO;
using GameServer.Model;

namespace GameServer.Controller
{
    /// <summary>
    /// UserController
    /// 用户控制器，继承自BaseController（请求基类）
    /// </summary>
    class UserController : BaseController
    {
        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public UserController()
        {
            requestCode = RequestCode.User; //申请类型为User
        }
        #endregion

        #region 成员变量
        /// <summary>
        /// UserDAO
        /// 处理数据库信息
        /// </summary>
        private UserDAO userDAO = new UserDAO();

        /// <summary>
        /// ResultDAO
        /// 处理数据库信息
        /// </summary>
        private ResultDAO resultDAO = new ResultDAO();
        #endregion

        #region 提供的方法
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="data">申请数据</param>
        /// <param name="client">客户端</param>
        /// <param name="server">服务端</param>
        /// <returns>响应数据</returns>
        public string Login(string data, Client client, Server server)
        {
            string[] strs = data.Split(','); //解析数据
            User user =  userDAO.VerifyUser(client.MySQLConn, strs[0], strs[1]); //确认是否存在
            if (user == null)
            {
                //Enum.GetName(typeof(ReturnCode), ReturnCode.Fail);
                return ((int)ReturnCode.Fail).ToString(); //返回响应数据
            }
            else
            {
                //获取战绩信息
                Result res = resultDAO.GetResultByUserid(client.MySQLConn, user.Id);
                //保存到服务器中的客户信息中
                client.SetUserData(user, res);
                return  string.Format("{0},{1},{2},{3}", ((int)ReturnCode.Success).ToString(), user.Username, res.TotalCount, res.WinCount); //返回响应数据
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="data">申请数据</param>
        /// <param name="client">客户端</param>
        /// <param name="server">服务端</param>
        /// <returns>响应数据</returns>
        public string Register(string data, Client client, Server server)
        {
            //解析数据
            string[] strs = data.Split(',');
            string username = strs[0];string password = strs[1];

            bool res = userDAO.GetUserByUsername(client.MySQLConn,username); //查找用户
            if (res)
            {
                return ((int)ReturnCode.Fail).ToString(); //返回响应数据
            }
            userDAO.AddUser(client.MySQLConn, username, password); //添加用户，注册成功
            return ((int)ReturnCode.Success).ToString(); //返回响应数据
        }
        #endregion
    }
}
