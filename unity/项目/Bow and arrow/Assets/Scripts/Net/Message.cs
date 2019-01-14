using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Common;
using System.Text;
using System.Linq;

/// <summary>
/// Message
/// 消息类
/// 用来存放网络传输的字节
/// </summary>
public class Message
{
    #region 成员变量
    /// <summary>
    /// 存放传输字节数组
    /// </summary>
    private byte[] data = new byte[1024];

    /// <summary>
    /// 存取的字节数据
    /// </summary>
    private int startIndex = 0;

    //public void AddCount(int count)
    //{
    //    startIndex += count;
    //}

    /// <summary>
    /// 传输字节数组属性
    /// </summary>
    public byte[] Data
    {
        get { return data; }
    }

    /// <summary>
    /// 存取字节数属性
    /// </summary>
    public int StartIndex
    {
        get { return startIndex; }
    }

    /// <summary>
    /// 剩余字节数属性
    /// </summary>
    public int RemainSize
    {
        get { return data.Length - startIndex; }
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 解析数据或者叫做读取数据
    /// 处理粘包和分包问题
    /// </summary>
    public void ReadMessage(int newDataAmount, Action<ActionCode, string> processDataCallback)
    {
        startIndex += newDataAmount; //总量加上新接收数量
        while (true)
        {
            if (startIndex <= 4) return; //前4字节代表新信息个数，小于4字节直接返回
            int count = BitConverter.ToInt32(data, 0); //从数据中0号位置二进制转换出四字节
            if ((startIndex - 4) >= count) //该条数据是否全部接收
            {
                //Console.WriteLine(startIndex);
                //Console.WriteLine(count);
                //string s = Encoding.UTF8.GetString(data, 4, count);
                //Console.WriteLine("解析出来一条数据：" + s);
                ActionCode actionCode = (ActionCode)BitConverter.ToInt32(data, 4); //取出actionCode
                string s = Encoding.UTF8.GetString(data, 8, count - 4); //将剩余数据从二进制转换出来
                processDataCallback(actionCode, s); //回调响应解析出来的数据
                Array.Copy(data, count + 4, data, 0, startIndex - 4 - count); //剩余数据前移
                startIndex -= (count + 4); //更新总量
            }
            else
            {
                break;
            }
        }
    }
    //public static byte[] PackData(ActionCode actionCode, string data)
    //{
    //    byte[] requestCodeBytes = BitConverter.GetBytes((int)actionCode);
    //    byte[] dataBytes = Encoding.UTF8.GetBytes(data);
    //    int dataAmount = requestCodeBytes.Length + dataBytes.Length;
    //    byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);
    //    byte[] newBytes = dataAmountBytes.Concat(requestCodeBytes).ToArray<byte>();//Concat(dataBytes);
    //    return newBytes.Concat(dataBytes).ToArray<byte>();
    //}

    /// <summary>
    /// 静态方法，装包
    /// </summary>
    /// <param name="requestData">服务器对应类</param>
    /// <param name="actionCode">服务器类对应方法</param>
    /// <param name="data">传输数据</param>
    /// <returns></returns>
    public static byte[] PackData(RequestCode requestData,ActionCode actionCode, string data)
    {
        //转换二进制
        byte[] requestCodeBytes = BitConverter.GetBytes((int)requestData);
        byte[] actionCodeBytes = BitConverter.GetBytes((int)actionCode); 
        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
        
        int dataAmount = requestCodeBytes.Length + dataBytes.Length + actionCodeBytes.Length; //计算数据长度

        //组合数据，即装包
        byte[] dataAmountBytes = BitConverter.GetBytes(dataAmount);
        //byte[] newBytes = dataAmountBytes.Concat(requestCodeBytes).ToArray<byte>();//Concat(dataBytes);
        //return newBytes.Concat(dataBytes).ToArray<byte>();
        return dataAmountBytes.Concat(requestCodeBytes).ToArray<byte>()
            .Concat(actionCodeBytes).ToArray<byte>()
            .Concat(dataBytes).ToArray<byte>();
    }
    #endregion
}
