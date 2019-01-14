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
    /// BaseController
    /// 控制基类
    /// 所有Controller都要继承这个类
    /// 定义了Controller的基本功能
    /// </summary>
    abstract class BaseController
    {
        #region 成员变量
        /// <summary>
        /// RequestCode
        /// 申请类型
        /// 用于区分申请的类型
        /// </summary>
        protected RequestCode requestCode = RequestCode.None;

        /// <summary>
        /// 申请类型的属性
        /// 共有get
        /// </summary>
        public RequestCode RequestCode {
            get
            {
                return requestCode;
            }
        }
        #endregion

        #region 提供的方法
        /// <summary>
        /// 虚方法
        /// </summary>
        /// <param name="data">传输数据</param>
        /// <param name="client">客户端</param>
        /// <param name="server">服务</param>
        /// <returns></returns>
        public virtual string DefaultHandle(string data,Client client,Server server)
        {
            return null;
        }
        #endregion
    }
}
