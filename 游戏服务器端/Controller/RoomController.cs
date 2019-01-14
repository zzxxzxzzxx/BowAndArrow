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
    /// RoomController
    /// 房间控制器，继承自BaseController（请求基类）
    /// </summary>
    class RoomController : BaseController
    {
        #region 构造方法
        /// <summary>
        /// 构造方法
        /// </summary>
        public RoomController()
        {
            requestCode = RequestCode.Room; //申请的类型为Room
        }
        #endregion

        #region 提供的方法
        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="data">申请数据</param>
        /// <param name="client">客户端</param>
        /// <param name="server">服务端</param>
        /// <returns>响应数据</returns>
        public string CreateRoom(string data, Client client, Server server)
        {
            server.CreateRoom(client); //服务端创建房间
            return ((int)ReturnCode.Success).ToString()+","+ ((int)RoleType.Blue).ToString(); //返回响应数据
        }

        /// <summary>
        /// 获取房间列表
        /// </summary>
        /// <param name="data">申请数据</param>
        /// <param name="client">客户端</param>
        /// <param name="server">服务端</param>
        /// <returns>响应数据</returns>
        public string ListRoom(string data, Client client, Server server)
        {
            StringBuilder sb = new StringBuilder();
            //遍历房间list
            foreach(Room room in server.GetRoomList())
            {
                if (room.IsWaitingJoin())
                {
                    sb.Append(room.GetHouseOwnerData()+"|");
                }
            }
            if (sb.Length == 0)
            {
                //没有房间
                sb.Append("0");
            }
            else
            {
                sb.Remove(sb.Length - 1, 1); //把最后一个“|”去掉
            }
            return sb.ToString(); //返回响应数据
        }

        /// <summary>
        /// 加入房间
        /// </summary>
        /// <param name="data">申请数据</param>
        /// <param name="client">客户端</param>
        /// <param name="server">服务端</param>
        /// <returns>响应数据</returns>
        public string JoinRoom(string data, Client client, Server server)
        {
            int id = int.Parse(data); //解析出房间id
            Room room = server.GetRoomById(id); //获取房间
            if(room == null)
            {
                return ((int)ReturnCode.NotFound).ToString(); //获取房间失败
            }
            else if (room.IsWaitingJoin() == false)
            {
                return ((int)ReturnCode.Fail).ToString(); //房间人数已满
            }
            else
            {
                room.AddClient(client); //房间添加客户端
                string roomData = room.GetRoomData();//"returncode,roletype-id,username,tc,wc|id,username,tc,wc"
                room.BroadcastMessage(client, ActionCode.UpdateRoom, roomData); //广播加入房间信息
                return ((int)ReturnCode.Success).ToString() + "," + ((int)RoleType.Red).ToString()+ "-" + roomData; //返回响应信息
            }
        }

        /// <summary>
        /// 退出房间
        /// </summary>
        /// <param name="data">申请数据</param>
        /// <param name="client">客户端</param>
        /// <param name="server">服务端</param>
        /// <returns>响应数据</returns>
        public string QuitRoom(string data, Client client, Server server)
        {
            bool isHouseOwner = client.IsHouseOwner(); //是否为房主
            Room room = client.Room;
            if (isHouseOwner)
            {
                //房主退出，房间关闭
                room.BroadcastMessage(client, ActionCode.QuitRoom, ((int)ReturnCode.Success).ToString());
                room.Close();
                return ((int)ReturnCode.Success).ToString(); //返回响应信息
            }
            else
            {
                client.Room.RemoveClient(client); //客户端退出房间
                room.BroadcastMessage(client, ActionCode.UpdateRoom, room.GetRoomData()); //广播退出房间
                return ((int)ReturnCode.Success).ToString(); //返回响应信息
            }
        }
        #endregion
    }
}
