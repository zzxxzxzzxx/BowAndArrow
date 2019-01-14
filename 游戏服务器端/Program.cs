using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Servers;

namespace 游戏服务器端
{
    /// <summary>
    /// 服务端主类
    /// </summary>
    class Program
    {
        /// <summary>
        /// 主方法
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Server server = new Server("127.0.0.1", 6688); //建立服务端
            server.Start(); //开始监听
             
            Console.ReadKey(); //暂停控制台
        }
    }
}
