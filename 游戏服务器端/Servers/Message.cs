using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace GameServer.Servers
{
    /// <summary>
    /// Message
    /// 数据类，用来处理传递的数据
    /// </summary>
    class Message
    {
        #region 成员变量
        /// <summary>
        /// 存放数据字节数组
        /// </summary>
        private byte[] data = new byte[1024];

        /// <summary>
        /// 已经存放的字节数量
        /// </summary>
        private int startIndex = 0;//我们存取了多少个字节的数据在数组里面

        //public void AddCount(int count)
        //{
        //    startIndex += count;
        //}

        /// <summary>
        /// 字节数组属性
        /// 共有get
        /// </summary>
        public byte[] Data
        {
            get { return data; }
        }

        /// <summary>
        /// 字节数的属性
        /// 共有get
        /// </summary>
        public int StartIndex
        {
            get { return startIndex; }
        }

        /// <summary>
        /// 剩余字节数的属性
        /// 共有get
        /// </summary>
        public int RemainSize
        {
            get { return data.Length - startIndex; }
        }
        #endregion

        #region 提供的方法
        /// <summary>
        /// 解析数据或者叫做读取数据
        /// </summary>
        public void ReadMessage(int newDataAmount, Action<RequestCode,ActionCode,string> processDataCallback )
        {
            startIndex += newDataAmount;
            while (true)
            {
                if (startIndex <= 4) return; //数据个数小于4，无法处理数据
                int count = BitConverter.ToInt32(data, 0); //从数据中取出前4字节转换成int
                if ((startIndex - 4) >= count) //若已经把整段数据读取出来，已经考虑到粘包，分包的问题
                {
                    //Console.WriteLine(startIndex);
                    //Console.WriteLine(count);
                    //string s = Encoding.UTF8.GetString(data, 4, count);
                    //Console.WriteLine("解析出来一条数据：" + s);
                    RequestCode requestCode = (RequestCode)BitConverter.ToInt32(data, 4); //转换RequestCode
                    ActionCode actionCode = (ActionCode)BitConverter.ToInt32(data, 8); //转换ActionCode
                    string s = Encoding.UTF8.GetString(data, 12, count-8); //转换剩余数据

                    processDataCallback(requestCode, actionCode, s); //将解析好的数据返回给回调方法
                    Array.Copy(data, count + 4, data, 0, startIndex - 4 - count); //将剩余数据前移
                    startIndex -= (count + 4); //更新数据数量
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 静态方法，给数据打包
        /// </summary>
        /// <param name="actionCode">要打包的ActionCode动作类型</param>
        /// <param name="data">要打包的数据</param>
        /// <returns></returns>
        public static byte[] PackData(ActionCode actionCode,string data)
        {
            byte[] requestCodeBytes = BitConverter.GetBytes((int)actionCode); //将ActionCode转换成字节
            byte[] dataBytes = Encoding.UTF8.GetBytes(data); //将数据转换成字节
            int dataAmount = requestCodeBytes.Length + dataBytes.Length; //计算总长度
            byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount); //将长度转换成字节
            byte[] newBytes =dataAmountBytes.Concat(requestCodeBytes).ToArray<byte>(); //组合数据
            return newBytes.Concat(dataBytes).ToArray<byte>(); //返回组合数据
        }
        #endregion
    }
}
