using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.Reflection;
using GameServer.Servers;

namespace GameServer.Controller
{
    /// <summary>
    /// ControllerManager
    /// 控制管理器
    /// 用于管理所有的控制器
    /// </summary>
    class ControllerManager
    {
        #region 成员变量
        private Dictionary<RequestCode, BaseController> controllerDict = new Dictionary<RequestCode, BaseController>();
        private Server server;

        public ControllerManager(Server server) {
            this.server = server;
            InitController();
        }
        #endregion

        #region 提供的方法
        /// <summary>
        /// HandleRequest
        /// 客户端所有申请的处理
        /// </summary>
        /// <param name="requestCode">申请的类型</param>
        /// <param name="actionCode">类型的行为动作</param>
        /// <param name="data">从客户端传输过来的数据</param>
        /// <param name="client">客户端</param>
        public void HandleRequest(RequestCode requestCode, ActionCode actionCode, string data, Client client)
        {
            //根据RequestCode取得相应的控制器
            BaseController controller;
            bool isGet = controllerDict.TryGetValue(requestCode, out controller);
            if (isGet == false)
            {
                Console.WriteLine("无法得到[" + requestCode + "]所对应的Controller,无法处理请求"); return;
            }

            //执行控制器相应方法返回给客户端
            string methodName = Enum.GetName(typeof(ActionCode), actionCode); //取得actionCode的string名
            MethodInfo mi = controller.GetType().GetMethod(methodName); //根据actionCode转换成的方法名取得对应控制器的方法
            if (mi == null)
            {
                Console.WriteLine("[警告]在Controller[" + controller.GetType() + "]中没有对应的处理方法:[" + methodName + "]"); return;
            }
            object[] parameters = new object[] { data, client, server }; //将所需传递的数据存储在parameters中
            object o = mi.Invoke(controller, parameters); //调用对应方法
            if (o == null || string.IsNullOrEmpty(o as string)) //得到其返回值不能为空
            {
                return;
            }
            server.SendResponse(client, actionCode, o as string); //将返回值传递回客户端
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 初始化控制器
        /// </summary>
        private void InitController()
        {
            //初始化控制器字典
            DefaultController defaultController = new DefaultController();
            controllerDict.Add(defaultController.RequestCode, defaultController);
            controllerDict.Add(RequestCode.User, new UserController());
            controllerDict.Add(RequestCode.Room, new RoomController());
            controllerDict.Add(RequestCode.Game, new GameController());
        }
        #endregion
    }
}
