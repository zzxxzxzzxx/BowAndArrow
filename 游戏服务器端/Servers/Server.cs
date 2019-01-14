using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using GameServer.Controller;
using Common;

namespace GameServer.Servers
{
    /// <summary>
    /// Client
    /// 用来存放已连接的客户端的数据
    /// </summary>
    class Server
    {
        #region 构造方法
        /// <summary>
        /// 空构造方法
        /// </summary>
        public Server() { }

        /// <summary>
        /// ip地址+端口的构造方法
        /// </summary>
        /// <param name="ipStr">ip地址</param>
        /// <param name="port">端口号</param>
        public Server(string ipStr, int port)
        {
            controllerManager = new ControllerManager(this);
            SetIpAndPort(ipStr, port); //设置ip以及端口号
        }
        #endregion

        #region 成员变量
        /// <summary>
        /// 端口
        /// </summary>
        private IPEndPoint ipEndPoint;

        /// <summary>
        /// 服务器socket
        /// </summary>
        private Socket serverSocket;

        /// <summary>
        /// 客户端list
        /// </summary>
        private List<Client> clientList = new List<Client>();

        /// <summary>
        /// 房间list
        /// </summary>
        private List<Room> roomList = new List<Room>();

        /// <summary>
        /// 控制器管理器
        /// </summary>
        private ControllerManager controllerManager;
        #endregion

        #region 提供的方法
        /// <summary>
        /// 设置ip以及端口
        /// </summary>
        /// <param name="ipStr">ip地址</param>
        /// <param name="port">端口号</param>
        public void SetIpAndPort(string ipStr, int port)
        {
            ipEndPoint = new IPEndPoint(IPAddress.Parse(ipStr), port);
        }

        /// <summary>
        /// 开始异步连接
        /// </summary>
        public void Start()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //创建一个socket连接，ipv4地址，数据流方式，tcp协议
            serverSocket.Bind(ipEndPoint); //连接ip端口
            serverSocket.Listen(0); //开始监听
            serverSocket.BeginAccept(AcceptCallBack, null); //异步连接，回调方法为AcceptCallBack
        }

        /// <summary>
        /// 异步接收消息的回调方法
        /// </summary>
        /// <param name="ar">连接信息</param>
        private void AcceptCallBack(IAsyncResult ar  )
        {
            Socket clientSocket = serverSocket.EndAccept(ar); //取得一个尝试连接的客户端
            Client client = new Client(clientSocket,this); //新建客户端
            client.Start(); //开始监听客户端
            clientList.Add(client); //客添加到户端list
            serverSocket.BeginAccept(AcceptCallBack, null); //继续异步监听
        }

        /// <summary>
        /// 移除一个客户端
        /// </summary>
        /// <param name="client">客户端</param>
        public void RemoveClient(Client client)
        {
            lock (clientList) //锁住！防止其他操作干扰
            {
                clientList.Remove(client); //移除客户端
            }
        }

        /// <summary>
        /// 发送回应
        /// </summary>
        /// <param name="client">客户端</param>
        /// <param name="actionCode">动作类型</param>
        /// <param name="data">响应给客户端的数据</param>
        public void SendResponse(Client client,ActionCode actionCode,string data)
        {
            client.Send(actionCode, data); //发送数据
        }

        /// <summary>
        /// 接收申请
        /// </summary>
        /// <param name="requestCode">申请的类型</param>
        /// <param name="actionCode">类型的动作状态</param>
        /// <param name="data">客户端发出的申请数据</param>
        /// <param name="client">客户端</param>
        public void HandleRequest(RequestCode requestCode, ActionCode actionCode, string data, Client client)
        {
            controllerManager.HandleRequest(requestCode, actionCode, data, client);
        }

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="client">客户端</param>
        public void CreateRoom(Client client)
        {
            Room room = new Room(this);
            room.AddClient(client); //将客户端添加到房间内
            roomList.Add(room); //添加到房间list
        }

        /// <summary>
        /// 移除房间
        /// </summary>
        /// <param name="room">房间</param>
        public void RemoveRoom(Room room)
        {
            if (roomList != null && room != null) //房间还存在，房间list也还存在
            {
                roomList.Remove(room); //移除房间
            }
        }

        /// <summary>
        /// 获取房间list
        /// </summary>
        /// <returns></returns>
        public List<Room> GetRoomList()
        {
            return roomList;
        }

        /// <summary>
        /// 根据id获取房间
        /// </summary>
        /// <param name="id">房间id</param>
        /// <returns>对应的房间</returns>
        public Room GetRoomById(int id)
        {
            //遍历搜索房间
            foreach(Room room in roomList)
            {
                if (room.GetId() == id) return room;
            }
            return null;
        }
        #endregion
    }
}
