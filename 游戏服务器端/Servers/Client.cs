using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Common;
using MySql.Data.MySqlClient;
using GameServer.Tool;
using GameServer.Model;
using GameServer.DAO;

namespace GameServer.Servers
{
    /// <summary>
    /// Client
    /// 用来存放已连接的客户端的数据
    /// </summary>
    class Client
    {
        #region 构造方法
        /// <summary>
        /// 空构造方法
        /// </summary>
        public Client() { }

        /// <summary>
        /// socke连接+服务器的构造方法
        /// </summary>
        /// <param name="clientSocket">socke连接</param>
        /// <param name="server">服务器</param>
        public Client(Socket clientSocket, Server server)
        {
            this.clientSocket = clientSocket;
            this.server = server;
            mysqlConn = ConnHelper.Connect();
        }
        #endregion 

        #region 成员变量
        /// <summary>
        /// 客户端的连接
        /// </summary>
        private Socket clientSocket;

        /// <summary>
        /// 服务器的连接
        /// </summary>
        private Server server;

        /// <summary>
        /// 数据类用来对数据的处理
        /// </summary>
        private Message msg = new Message();

        /// <summary>
        /// 数据库连接
        /// </summary>
        private MySqlConnection mysqlConn;

        /// <summary>
        /// 房间
        /// </summary>
        private Room room;

        /// <summary>
        /// 用户数据模型
        /// </summary>
        private User user;

        /// <summary>
        /// 结果数据模型
        /// </summary>
        private Result result;

        /// <summary>
        /// 结果DAO
        /// </summary>
        private ResultDAO resultDAO = new ResultDAO();

        /// <summary>
        /// 血量属性
        /// 共有get
        /// 共有set
        /// </summary>
        public int HP
        {
            get;set;
        }

        /// <summary>
        /// 数据库连接属性
        /// 共有get
        /// </summary>
        public MySqlConnection MySQLConn
        {
            get { return mysqlConn; }
        }

        /// <summary>
        /// 房间属性
        /// 共有get
        /// 共有set
        /// </summary>
        public Room Room
        {
            get { return room; }
            set { room = value; }
        }
        #endregion

        #region 提供的方法
        /// <summary>
        /// 返回是否为房主
        /// </summary>
        /// <returns>真代表是房主</returns>
        public bool IsHouseOwner()
        {
            return room.IsHouseOwner(this);
        }

        /// <summary>
        /// 造成伤害
        /// </summary>
        /// <param name="damage">随机产生伤害的数值</param>
        /// <returns>是否死亡，真代表已死亡</returns>
        public bool TakeDamage(int damage)
        {
            HP -= damage;
            HP = Math.Max(HP, 0);
            if (HP <= 0) return true;
            return false;
        }

        /// <summary>
        /// 返回客户端的角色是否死亡
        /// </summary>
        /// <returns>是否死亡，真代表已死亡</returns>
        public bool IsDie()
        {
            return HP <= 0;
        }

        /// <summary>
        /// 获取用户id
        /// </summary>
        /// <returns>返回用户id（int）</returns>
        public int GetUserId()
        {
            return user.Id;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <returns>用户名字，总场数，胜利场数组成的字符串</returns>
        public string GetUserData()
        {
            return user.Id + "," + user.Username + "," + result.TotalCount + "," + result.WinCount;
        }

        /// <summary>
        /// 设置用户数据
        /// </summary>
        /// <param name="user">用户表模型</param>
        /// <param name="result">用户战绩模型</param>
        public void SetUserData(User user,Result result)
        {
            this.user = user;
            this.result = result;
        }

        /// <summary>
        /// 开始从客户端接收数据
        /// </summary>
        public void Start()
        {
            if (clientSocket == null || clientSocket.Connected == false) return; //失去连接直接返回
            clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallback, null); //开始异步接收客户端的消息，回调方法为ReceiveCallback
        }

        /// <summary>
        /// 发送数据给客户端
        /// </summary>
        /// <param name="actionCode">动作类型</param>
        /// <param name="data">传输给客户端的数据</param>
        public void Send(ActionCode actionCode, string data)
        {
            try
            {
                byte[] bytes = Message.PackData(actionCode, data); //通过Message打包数据
                clientSocket.Send(bytes); //发送数据
            }
            catch (Exception e)
            { 
                Console.WriteLine("无法发送消息:" + e); //异常处理
            }
        }

        /// <summary>
        /// 更新游戏结果
        /// </summary>
        /// <param name="isVictory">真代表胜利</param>
        public void UpdateResult(bool isVictory)
        {
            UpdateResultToDB(isVictory); //向数据库更新结果
            UpdateResultToClient(); //向客户端更新结果
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 异步接收客户端发送数据的回调方法
        /// </summary>
        /// <param name="ar">接收数据</param>
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (clientSocket == null || clientSocket.Connected == false) return; //失去连接直接返回
                //结束监听判断
                int count = clientSocket.EndReceive(ar); 
                if (count == 0)
                {
                    Close(); //关闭连接
                }
                msg.ReadMessage(count,OnProcessMessage); //解析数据
                Start(); //继续异步接收客户端的数据
            }
            catch (Exception e)
            {
                Console.WriteLine(e); //异常处理
                Close(); //关闭连接
            }
        }
        private void OnProcessMessage(RequestCode requestCode,ActionCode actionCode,string data)
        {
            server.HandleRequest(requestCode, actionCode, data, this); //发送数据
        }

        /// <summary>
        /// 关闭与客户端的连接
        /// </summary>
        private void Close()
        {
            ConnHelper.CloseConnection(mysqlConn); //关闭与数据库的连接
            if (clientSocket != null) //断开连接
                clientSocket.Close();
            if (room != null) //移出房间
            {
                room.QuitRoom(this);
            }
            server.RemoveClient(this); //移出连接
        }

        /// <summary>
        /// 向数据库更新游戏结果
        /// </summary>
        /// <param name="isVictory">真代表胜利</param>
        private void UpdateResultToDB(bool isVictory)
        {
            result.TotalCount++;
            if (isVictory)
            {
                result.WinCount++;
            }
            resultDAO.UpdateOrAddResult(mysqlConn, result); //利用DAO更改数据库数据
        }

        /// <summary>
        /// 向客户端更新游戏结果
        /// </summary>
        private void UpdateResultToClient()
        {
            //发送动作状态，“游戏总场数，游戏胜利场数”的字符串
            Send(ActionCode.UpdateResult, string.Format("{0},{1}", result.TotalCount, result.WinCount));
        }
        #endregion
    }
}
