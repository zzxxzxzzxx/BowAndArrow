using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// RequestManager
/// 请求管理器，继承自BaseManager（管理器基类）
/// </summary>
public class RequestManager : BaseManager
{
    #region 构造方法
    /// <summary>
    /// 构造方法
    /// </summary>
    /// <param name="facade">facade中介者</param>
    public RequestManager(GameFacade facade) : base(facade) { }
    #endregion

    #region 成员变量
    /// <summary>
    /// 申请字典
    /// </summary>
    private Dictionary<ActionCode, BaseRequest> requestDict = new Dictionary<ActionCode, BaseRequest>();
    #endregion

    #region 提供的方法
    /// <summary>
    /// 在字典添加申请
    /// </summary>
    /// <param name="actionCode"></param>
    /// <param name="request"></param>
    public void AddRequest(ActionCode actionCode,BaseRequest request)
    {
        requestDict.Add(actionCode, request);
    }

    /// <summary>
    /// 从字典移除申请
    /// </summary>
    /// <param name="actionCode"></param>
    public void RemoveRequest(ActionCode actionCode)
    {
        requestDict.Remove(actionCode);
    }

    /// <summary>
    /// 处理服务器返回的信息
    /// </summary>
    /// <param name="actionCode">处理方式</param>
    /// <param name="data">数据信息</param>
    public void HandleReponse(ActionCode actionCode, string data)
    {
        BaseRequest request = requestDict.TryGet<ActionCode, BaseRequest>(actionCode); //尝试得到处理方式对应的请求类
        
        //处理异常信息
        if (request == null)
        {
            Debug.LogWarning("无法得到ActionCode[" + actionCode + "]对应的Request类");return;
        }

        request.OnResponse(data); //处理从服务器发来的响应信息
    }
    #endregion
}
