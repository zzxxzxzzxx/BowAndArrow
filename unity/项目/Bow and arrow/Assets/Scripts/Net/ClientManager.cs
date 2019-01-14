using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using Common;

/// <summary>
/// ClientManager
/// 管理服务器端的Socket连接，继承自BaseManager（管理器基类）
/// </summary>
public class ClientManager: BaseManager
{
    #region 构造方法
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="facade">facade中介者</param>
    public ClientManager(GameFacade facade) : base(facade) { }
    #endregion

    #region 成员变量
    /// <summary>
    /// 连接IP地址
    /// </summary>
    private const string IP = "127.0.0.1";

    /// <summary>
    /// 连接端口号
    /// </summary>
    private const int PORT = 6688;

    /// <summary>
    /// Socket连接
    /// </summary>
    private Socket clientSocket;

    /// <summary>
    /// 消息类
    /// </summary>
    private Message msg = new Message();
    #endregion

    #region 提供的方法
    /// <summary>
    /// 重写初始化方法
    /// </summary>
    public override void OnInit()
    {
        base.OnInit(); //调用父类的初始化方法

        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //创建Socket连接，使用ipv4，Socket流方式，Tcp协议
        try
        {
            clientSocket.Connect(IP, PORT); //尝试连接服务器
            Start(); //开始异步接收消息
        }
        catch (Exception e)
        {
            Debug.LogWarning("无法连接到服务器端，请检查您的网络！！" + e); //异常提醒
        }
    }

    /// <summary>
    /// 发送请求
    /// </summary>
    /// <param name="requestCode">对应服务器的类名</param>
    /// <param name="actionCode">对应类的方法名</param>
    /// <param name="data">请求数据</param>
    public void SendRequest(RequestCode requestCode, ActionCode actionCode, string data)
    {
        byte[] bytes = Message.PackData(requestCode, actionCode, data); //装包
        clientSocket.Send(bytes); //向服务器发送
    }

    /// <summary>
    /// 重写销毁方法
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy(); //调用父类的销毁方法
        try
        {
            clientSocket.Close(); //关闭Socket连接
        }catch(Exception e)
        {
            Debug.LogWarning("无法关闭跟服务器端的连接！！" + e); //异常提醒
        }
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 开始异步接收client信息
    /// </summary>
    private void Start()
    {
        clientSocket.BeginReceive(msg.Data, msg.StartIndex, msg.RemainSize, SocketFlags.None, ReceiveCallback, null);
    }

    /// <summary>
    /// 接收信息回调方法
    /// </summary>
    /// <param name="ar">接收的信息</param>
    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            if (clientSocket == null || clientSocket.Connected == false) return; //失去连接直接返回
            int count = clientSocket.EndReceive(ar); //获取接收信息数量

            msg.ReadMessage(count, OnProcessDataCallback); //消息解读

            Start(); //继续异步监听
        }
        catch (Exception e)
        {
            Debug.Log(e); //异常提醒
        }
    }

    /// <summary>
    /// 将解读出的消息返回给响应方法
    /// </summary>
    /// <param name="actionCode">客户端处理方式</param>
    /// <param name="data">响应数据</param>
    private void OnProcessDataCallback(ActionCode actionCode, string data)
    {
        facade.HandleReponse(actionCode, data);
    }
    #endregion
}