using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Threading;

namespace GameServer.Servers
{
    #region 枚举类型：房间的状态
    /// <summary>
    /// 枚举类型
    /// 房间的状态
    /// </summary>
    enum RoomState
    {
        /// <summary>
        /// 等待加入
        /// </summary>
        WaitingJoin,

        /// <summary>
        /// 等待游戏（人数已满）
        /// </summary>
        WaitingBattle,

        /// <summary>
        /// 游戏中
        /// </summary>
        Battle,

        /// <summary>
        /// 结束
        /// </summary>
        End
    }
    #endregion

    /// <summary>
    /// Room
    /// 用来存放一局游戏的数据
    /// </summary>
    class Room
    {
        #region 成员变量
        /// <summary>
        /// 静态最大血量
        /// </summary>
        private const int MAX_HP = 200;

        /// <summary>
        /// 房间内的客户端list
        /// </summary>
        private List<Client> clientRoom = new List<Client>();

        /// <summary>
        /// 房间状态
        /// </summary>
        private RoomState state = RoomState.WaitingJoin;

        /// <summary>
        /// 服务端的连接
        /// </summary>
        private Server server;

        /// <summary>
        /// 房间属性
        /// </summary>
        /// <param name="server">服务端</param>
        public Room(Server server)
        {
            this.server = server;
        }
        #endregion

        #region 提供的方法
        /// <summary>
        /// 返回是否为房主
        /// </summary>
        /// <param name="client">客户端</param>
        /// <returns>真代表是房主</returns>
        public bool IsHouseOwner(Client client)
        {
            return client == clientRoom[0];
        }

        /// <summary>
        /// 房间状态是否为等待加入
        /// </summary>
        /// <returns>真代表等待加入</returns>
        public bool IsWaitingJoin()
        {
            return state == RoomState.WaitingJoin;
        }

        /// <summary>
        /// 获取房间id
        /// </summary>
        /// <returns>房间id</returns>
        public int GetId()
        {
            if (clientRoom.Count > 0)
            {
                return clientRoom[0].GetUserId();
            }
            return -1;
        }

        /// <summary>
        /// 获取房主数据
        /// </summary>
        /// <returns>房主用户信息</returns>
        public string GetHouseOwnerData()
        {
            return clientRoom[0].GetUserData();
        }

        /// <summary>
        /// 获取房间数据
        /// </summary>
        /// <returns>房间所有用户信息</returns>
        public String GetRoomData()
        {
            //遍历获取所有用户信息
            StringBuilder sb = new StringBuilder();
            foreach (Client client in clientRoom)
            {
                sb.Append(client.GetUserData() + "|");
            }
            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 添加客户端
        /// </summary>
        /// <param name="client">客户端</param>
        public void AddClient(Client client)
        {
            client.HP = MAX_HP; //定义血量
            clientRoom.Add(client); //添加一个用户
            client.Room = this; //设置房间
            if (clientRoom.Count>= 2)
            {
                state = RoomState.WaitingBattle; //转换房间状态
            }
        }

        //开始倒计时
        public void StartTimer()
        {
            new Thread(RunTimer).Start(); //开线程处理倒计时
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="excludeClient">不去广播的客户端</param>
        /// <param name="actionCode">广播信息的动作类型</param>
        /// <param name="data">广播的数据</param>
        public void BroadcastMessage(Client excludeClient,ActionCode actionCode,string data)
        {
            //遍历房间内所有客户端进行广播
            foreach(Client client in clientRoom)
            {
                if (client != excludeClient)
                {
                    server.SendResponse(client, actionCode, data);
                }
            }
        }

       /// <summary>
        /// 造成伤害处理
        /// </summary>
        /// <param name="damage">伤害数值</param>
        /// <param name="excludeClient">发起攻击的客户端</param>
        public void TakeDamage(int damage, Client excludeClient)
        {
            bool isDie = false; //死亡标记
            //广播伤害
            foreach (Client client in clientRoom)
            {
                if (client != excludeClient)
                {
                    if (client.TakeDamage(damage)) //若死亡了，进行标记
                    {
                        isDie = true;
                    }
                }
            }
            if (isDie == false) return;
            //如果其中一个角色死亡，要结束游戏
            foreach (Client client in clientRoom)
            {
                if (client.IsDie()) 
                {
                    //死亡（战败）的用户返回结果
                    client.UpdateResult(false);
                    client.Send(ActionCode.GameOver, ((int)ReturnCode.Fail).ToString());
                }
                else
                {
                    //存活（胜利）的用户返回结果
                    client.UpdateResult(true);
                    client.Send(ActionCode.GameOver, ((int)ReturnCode.Success).ToString());
                }
            }
            //游戏结束，关闭房间
            Close();
        }

        /// <summary>
        /// 退出房间
        /// </summary>
        /// <param name="client">客户端</param>
        public void QuitRoom(Client client)
        {
            if (client == clientRoom[0])
            {
                Close(); //房主退出房间，直接关闭房间
            }
            else
                clientRoom.Remove(client); //非房主退出房间，将客户端移出房间
        }

        /// <summary>
        /// 移除客户端
        /// </summary>
        /// <param name="client">客户端</param>
        public void RemoveClient(Client client)
        {
            client.Room = null; //断开房间连接
            clientRoom.Remove(client); //移出客户端

            //转换房间状态
            if (clientRoom.Count >= 2)
            {
                state = RoomState.WaitingBattle;
            }
            else
            {
                state = RoomState.WaitingJoin;
            }
        }

        /// <summary>
        /// 关闭房间
        /// </summary>
        public void Close()
        {
            //遍历房间所有客户端，断开连接
            foreach (Client client in clientRoom)
            {
                client.Room = null;
            }
            //移出房间
            server.RemoveRoom(this);
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 倒计时方法
        /// </summary>
        private void RunTimer()
        {
            Thread.Sleep(1000); //线程等待1s
            for (int i = 3; i > 0; i--)
            {
                BroadcastMessage(null, ActionCode.ShowTimer, i.ToString()); //广播倒计时时间
                Thread.Sleep(1000); //线程等待1s
            }
            BroadcastMessage(null, ActionCode.StartPlay, "r"); //倒计时完毕，开始广播开始游戏
        }
        #endregion
    }
}
