using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace GameServer.Tool
{
    /// <summary>
    /// ConnHelper
    /// 用管理和数据库的连接
    /// </summary>
    class ConnHelper
    {
        #region 成员变量
        /// <summary>
        /// 静态地址
        /// 数据库连接信息
        /// </summary>
        public const string CONNECTIONSTRING = "datasource=127.0.0.1;port=3306;database=game01;user=root;pwd=123456;";
        #endregion

        #region 提供的方法
        /// <summary>
        /// MySqlConnection
        /// 静态方法，创建和数据库连接
        /// </summary>
        /// <returns></returns>
        public static MySqlConnection Connect()
        {
            MySqlConnection conn = new MySqlConnection(CONNECTIONSTRING); //连接数据库
            try
            {
                conn.Open(); //打开数据库
                return conn; //返回连接
            }
            catch(Exception e)
            {
                Console.WriteLine("链接数据库的时候实现异常：" + e); //异常处理
                return null;
            }
            
        }

        /// <summary>
        /// CloseConnection
        /// 静态方法，关闭和数据库的连接
        /// </summary>
        /// <param name="conn">数据库连接</param>
        public static void CloseConnection(MySqlConnection conn)
        {
            if(conn!=null)
                conn.Close(); //断开连接
            else
            {
                Console.WriteLine("MySqlConnection不能为空"); //异常处理
            }
        }
        #endregion
    }
}
