using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model;
using MySql.Data.MySqlClient;

namespace GameServer.DAO
{
    /// <summary>
    /// ResultDAO
    /// 用于数据库result表和数据库的操作
    /// </summary>
    class ResultDAO
    {
        #region 提供的方法
        /// <summary>
        /// 通过用户id获取战绩
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="userId">用户id</param>
        /// <returns>result模型</returns>
        public Result GetResultByUserid( MySqlConnection conn,int userId)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from result where userid = @userid", conn); //sql语句
                //采用Parameters，防止用户名或密码被恶意添加sql语句
                cmd.Parameters.AddWithValue("userid", userId);

                reader = cmd.ExecuteReader(); //执行语句
                if (reader.Read())
                {
                    int id = reader.GetInt32("id"); //读取语句中的用户id信息
                    int totalCount = reader.GetInt32("totalcount"); //读取语句中的总场数信息
                    int winCount = reader.GetInt32("wincount"); //读取语句中的胜利场数信息
                    int winRate = reader.GetInt32("winrate");

                    Result res = new Result(id, userId, totalCount, winCount, winRate); //组合result模型
                    return res; 
                }
                else
                {
                    Result res = new Result(-1, userId, 0, 0, 0); //新建result
                    return res;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在GetResultByUserid的时候出现异常：" + e); //异常处理
            }
            finally
            {
                if (reader != null) reader.Close(); //关闭本次读取
            }
            return null;
        }

        public Result[] SortRankByRate(MySqlConnection conn)
        {
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = null;
                cmd = new MySqlCommand("select * from result order by winrate DESC, wincount DESC", conn);
                reader = cmd.ExecuteReader(); //执行语句
                int i = 1;
                Result[] res = new Result[101];
                while (reader.Read() && i <= 100)
                {
                    int id = reader.GetInt32("id"); //读取语句中的用户id信息
                    int userId = reader.GetInt32("userId");
                    int winRate = reader.GetInt32("winrate");
                    int totalCount = reader.GetInt32("totalcount"); //读取语句中的总场数信息
                    int winCount = reader.GetInt32("wincount"); //读取语句中的胜利场数信息
                    //Console.WriteLine("i = " + i +
                    //                  ", id = " + id +
                    //                  ", userId = " + userId +
                    //                  ", winRate = " + winRate +
                    //                  ", totalCount = " + totalCount +
                    //                  ", winCount = " + winCount);
                    res[i] = new Result(id, userId, totalCount, winCount, winRate);
                    i++;
                }
                return res;

            }
            catch (Exception e)
            {
                Console.WriteLine("在SortRankByRate的时候出现异常：" + e); //异常处理
            }
            finally
            {
                if (reader != null) reader.Close(); //关闭本次读取
            }
            return null;
        }

        /// <summary>
        /// 更新或者添加战绩
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="res">result模型</param>
        public void UpdateOrAddResult(MySqlConnection conn, Result res)
        {
            try
            {
                MySqlCommand cmd = null;
                
                if (res.Id <= -1) 
                {
                    //新建result
                    cmd = new MySqlCommand("insert into result set totalcount=@totalcount,wincount=@wincount,userid=@userid", conn);
                }
                else
                {
                    //更新result
                    cmd = new MySqlCommand("update result set totalcount=@totalcount,wincount=@wincount,winrate=@winrate where userid=@userid ", conn);
                }
                //采用Parameters，防止用户名或密码被恶意添加sql语句
                cmd.Parameters.AddWithValue("totalcount", res.TotalCount);
                cmd.Parameters.AddWithValue("wincount", res.WinCount);
                cmd.Parameters.AddWithValue("userid", res.UserId);
                cmd.Parameters.AddWithValue("winrate", res.WinRate);

                cmd.ExecuteNonQuery(); //执行语句
                if (res.Id <= -1)
                {
                    Result tempRes = GetResultByUserid(conn, res.UserId); //重新获取数据库自动生成result id
                    res.Id = tempRes.Id;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("在UpdateOrAddResult的时候出现异常：" + e); //异常处理
            }
        }
        #endregion
    }
}
