using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using GameServer.Servers;

namespace GameServer.Controller
{
    /// <summary>
    /// GameController
    /// 游戏控制器，继承自BaseController（请求基类）
    /// </summary>
    class GameController : BaseController
    {
        #region 构造方法
        /// <summary>
        /// 游戏控制器构造方法
        /// </summary>
        public GameController()
        {
            requestCode = RequestCode.Game; //类型为Game
        }
        #endregion

        #region 提供的方法
        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="data">申请数据</param>
        /// <param name="client">客户端</param>
        /// <param name="server">服务器</param>
        /// <returns>响应数据</returns>
        public string StartGame(string data, Client client, Server server)
        {
            //只有房主才能开始游戏
            if (client.IsHouseOwner()) 
            {
                Room room =  client.Room;
                room.BroadcastMessage(client, ActionCode.StartGame, ((int)ReturnCode.Success).ToString()); //广播开始游戏
                room.StartTimer(); //开始倒计时
                return ((int)ReturnCode.Success).ToString(); //返回响应数据
            }
            else
            {
                return ((int)ReturnCode.Fail).ToString(); //返回响应数据
            }
        }

        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="data">申请数据</param>
        /// <param name="client">客户端</param>
        /// <param name="server">服务端</param>
        /// <returns>响应数据</returns>
        public string Move(string data, Client client, Server server)
        {
            Room room = client.Room; //获取客户端所在房间
            if (room != null)
                room.BroadcastMessage(client, ActionCode.Move, data); //房间存在，广播移动信息
            return null; //返回响应信息
        }

        /// <summary>
        /// 射击
        /// </summary>
        /// <param name="data">申请数据</param>
        /// <param name="client">客户端</param>
        /// <param name="server">服务端</param>
        /// <returns>响应数据</returns>
        public string Shoot(string data, Client client, Server server)
        {
            Room room = client.Room; //获取客户端所在房间
            if (room != null)
                room.BroadcastMessage(client, ActionCode.Shoot, data); //房间存在，广播射击信息
            return null; //返回响应信息
        }

        /// <summary>
        /// 攻击
        /// </summary>
        /// <param name="data">申请数据</param>
        /// <param name="client">客户端</param>
        /// <param name="server">服务端</param>
        /// <returns>响应数据</returns>
        public string Attack(string data, Client client, Server server)
        {
            int damage = int.Parse(data); //解析数据
            Room room = client.Room; //获取客户端所在房间
            if (room == null) return null;
            room.TakeDamage(damage, client); //房间存在，调用房间造成伤害方法
            return null; //返回响应信息
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        /// <param name="data">申请数据</param>
        /// <param name="client">客户端</param>
        /// <param name="server">服务端</param>
        /// <returns>响应数据</returns>
        public string QuitBattle(string data, Client client, Server server)
        {
            Room room = client.Room; //获取客户端所在房间

            if (room != null)
            {
                room.BroadcastMessage(null, ActionCode.QuitBattle, "r"); //房间存在，广播退出房间
                room.Close(); //关闭房间
            }
            return null; //返回响应信息
        }
        #endregion
    }
}
