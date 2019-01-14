using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using GameServer.Model;

namespace GameServer.DAO
{
    /// <summary>
    /// UserDAO
    /// 用于数据库user表和数据库的操作
    /// </summary>
    class UserDAO
    {
        #region 提供的方法
        /// <summary>
        /// 确认账号及是否一致
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="username">用户名称</param>
        /// <param name="password">用户密码</param>
        /// <returns>用户信息</returns>
        public User VerifyUser(MySqlConnection conn, string username,string password)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from user where username = @username and password = @password", conn); //sql语句
                //采用Parameters，防止用户名或密码被恶意添加sql语句
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);

                reader = cmd.ExecuteReader(); //执行sql语句
                if (reader.Read()) //有数据返回真，读取下一个，直到没有数据
                {
                    int id = reader.GetInt32("id"); //从返回值中读取出用户id和其他属性组成user模型进行返回
                    User user = new User(id, username, password);
                    return user;
                }
                else
                {
                    return null; //没有找到返回空
                }
            }catch(Exception e)
            {
                Console.WriteLine("在VerifyUser的时候出现异常："+e); //异常处理
            }
            finally
            {
                if (reader != null) reader.Close(); //关闭本次读取
            }
            return null;
        }

        /// <summary>
        /// 查询是否存在用户名
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="username">用户名称</param>
        /// <returns>真为存在</returns>
        public bool GetUserByUsername(MySqlConnection conn, string username)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from user where username = @username", conn); //sql语句
                //采用Parameters，防止用户名或密码被恶意添加sql语句                                                                                                            //采用Parameters，防止用户名或密码被恶意添加sql语句
                cmd.Parameters.AddWithValue("username", username);

                reader = cmd.ExecuteReader(); //执行语句
                if (reader.HasRows) 
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在GetUserByUsername的时候出现异常：" + e); //异常处理
            }
            finally
            {
                if (reader != null) reader.Close(); //关闭本次读取
            }
            return false; 
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="username">用户名称</param>
        /// <param name="password">用户密码</param>
        public void AddUser(MySqlConnection conn, string username, string password)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("insert into user set username = @username , password = @password", conn); //sql语句
                //采用Parameters，防止用户名或密码被恶意添加sql语句
                cmd.Parameters.AddWithValue("username", username);
                cmd.Parameters.AddWithValue("password", password);

                cmd.ExecuteNonQuery(); //执行语句
            }
            catch (Exception e)
            {
                Console.WriteLine("在AddUser的时候出现异常：" + e); //异常处理
            }
        }
        #endregion
    }
}
