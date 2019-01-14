using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

/// <summary>
/// GameFacade
/// 游戏中介者管理，继承自MonoBehaviour（可挂载到游戏物体上）
/// 将facade类做成单例模式
/// 将各个插件用facade类统一管理起来
/// </summary>
public class GameFacade : MonoBehaviour
{
    #region 成员变量
    /// <summary>
    /// GameFacade类单例_instance
    /// </summary>
    private static GameFacade _instance;

    /// <summary>
    /// 建立GameFacade类单例
    /// </summary>
    public static GameFacade Instance
    {
        //共有get方法，若本身不存在则建立单例（获取游戏物体GameFacade中的GameFacade脚本），否则返回本身
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.Find("GameFacade").GetComponent<GameFacade>();
            }
            return _instance;
        }

        //私有set方法，让属性无法从外界赋值
        private set
        {
        }      
    }

    /// <summary>
    /// UI管理器
    /// </summary>
    private UIManager uiMng;

    /// <summary>
    /// 声音管理器
    /// </summary>
    private AudioManager audioMng;

    /// <summary>
    /// 角色管理器
    /// </summary>
    private PlayerManager playerMng;

    /// <summary>
    /// 相机管理器
    /// </summary>
    private CameraManager cameraMng;

    /// <summary>
    /// 请求管理器
    /// </summary>
    private RequestManager requestMng;

    /// <summary>
    /// 用户管理器
    /// </summary>
    private ClientManager clientMng;

    /// <summary>
    /// 开始游戏标记
    /// </summary>
    private bool isEnterPlaying = false;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// Awake
    /// MonoBehaviour中的唤醒
    /// </summary>
    //private void Awake()
    //{
    //    if (_instance != null)
    //    {
    //        Destroy(this.gameObject);return;
    //    }
    //    _instance = this;
    //}

    /// <summary>
    /// Start
    /// MonoBehaviour中的初始化
    /// </summary>
    private void Start ()
    {
        InitManager();
	}

    /// <summary>
    /// Update
    /// MonoBehaviour中的每帧更新
    /// </summary>
    private void Update()
    {
        UpdateManager(); //更新管理器每帧的处理

        if (isEnterPlaying) //判断游戏进行的标记
        {
            EnterPlaying(); //处理开始游戏标记
            
            isEnterPlaying = false; //处理完毕，等待下次标记
        }
	}

    /// <summary>
    /// OnDestroy
    /// MonoBehaviour中的销毁时运行
    /// </summary>
    private void OnDestroy()
    {
        DestroyManager(); //销毁所有管理器
    }
    #endregion

    #region 管理器以及游戏状态处理
    /// <summary>
    /// InitManager
    /// 初始化管理器
    /// </summary>
    private void InitManager()
    {
        //创建所有需要的管理器，把facade中介传递过去
        uiMng = new UIManager(this);
        audioMng = new AudioManager(this);
        playerMng = new PlayerManager(this);
        cameraMng = new CameraManager(this);
        requestMng = new RequestManager(this);
        clientMng = new ClientManager(this);

        //运行各自管理器的初始化
        uiMng.OnInit();
        audioMng.OnInit();
        playerMng.OnInit();
        cameraMng.OnInit();
        requestMng.OnInit();
        clientMng.OnInit();
    }

    /// <summary>
    /// UpdateManager
    /// 更新管理器中的Update
    /// </summary>
    private void UpdateManager()
    {
        uiMng.Update();
        audioMng.Update();
        playerMng.Update();
        cameraMng.Update();
        requestMng.Update();
        clientMng.Update();
    }

    /// <summary>
    /// 进入游戏处理
    /// </summary>
    private void EnterPlaying()
    {
        playerMng.SpawnRoles(); //加载角色
        cameraMng.FollowRole(); //摄像机状态从漫游改变到跟随角色
    }
  
    /// <summary>
    /// 异步进入游戏处理
    /// </summary>
    public void EnterPlayingSync()
    {
        isEnterPlaying = true; //更改开始游戏标记
    }

    /// <summary>
    /// 开始游戏处理
    /// </summary>
    public void StartPlaying()
    {
        playerMng.AddControlScript(); //给当前游戏角色添加控制脚本
        playerMng.CreateSyncRequest(); //？创建异步请求游戏物体
    }

   /// <summary>
    /// 游戏结束处理
    /// </summary>
    public void GameOver()
    {
        cameraMng.WalkthroughScene(); //显示游戏结果
        playerMng.GameOver(); //游戏结束时对角色的处理
    }

    /// <summary>
    /// DestroyManager
    /// 销毁所有管理器
    /// </summary>
    private void DestroyManager()
    {
        //调用各个管理器的Destroy
        uiMng.OnDestroy();
        audioMng.OnDestroy();
        playerMng.OnDestroy();
        cameraMng.OnDestroy();
        requestMng.OnDestroy();
        clientMng.OnDestroy();
    }
    #endregion

    #region 请求管理器的方法
    /// <summary>
    /// AddRequest
    /// 调用请求管理的AddRequest
    /// </summary>
    /// <param name="actionCode">传递对应服务器中的类名</param>
    /// <param name="request">传递对应服务器类中的方法名</param>
    public void AddRequest(ActionCode actionCode, BaseRequest request)
    {
        requestMng.AddRequest(actionCode, request);
    }

    /// <summary>
    /// 调用请求管理的RemoveRequest
    /// </summary>
    /// <param name="actionCode">传递对应服务器中的类名</param>
    public void RemoveRequest(ActionCode actionCode)
    {
        requestMng.RemoveRequest(actionCode);
    }

    /// <summary>
    /// 调用请求管理的HandleReponse
    /// 用于处理服务器发给客户端的信息
    /// </summary>
    /// <param name="actionCode">传递对应客户端中的类名</param>
    /// <param name="data">服务器返回的数据</param>
    public void HandleReponse(ActionCode actionCode, string data)
    {
        requestMng.HandleReponse(actionCode, data);
    }
    #endregion

    #region UI管理器的方法
    /// <summary>
    /// 调用UI管理器中的ShowMessage
    /// </summary>
    /// <param name="msg">显示的内容</param>
    public void ShowMessage(string msg)
    {
        uiMng.ShowMessage(msg);
    }
    #endregion

    #region 用户管理器的方法
    /// <summary>
    /// 调用用户管理器中的SendRequest
    /// </summary>
    /// <param name="requestCode">传递对应服务器中的类名</param>
    /// <param name="actionCode">传递对应服务器类中的方法名</param>
    /// <param name="data">客户端发出请求的数据</param>
    public void SendRequest(RequestCode requestCode, ActionCode actionCode, string data)
    {
        clientMng.SendRequest(requestCode, actionCode, data);
    }
    #endregion

    #region 声音管理器的方法
    /// <summary>
    /// 调用声音管理器中的PlayBgSound
    /// </summary>
    /// <param name="soundName">声音文件的名字</param>
    public void PlayBgSound(string soundName)
    {
        audioMng.PlayBgSound(soundName);
    }

    /// <summary>
    /// 调用声音管理器中的PlayNormalSound
    /// </summary>
    /// <param name="soundName">声音文件的名字</param>
    public void PlayNormalSound(string soundName)
    {
        audioMng.PlayNormalSound(soundName);
    }
    #endregion

    #region 角色管理器的方法
    /// <summary>
    /// 调用角色管理器中的UserData
    /// </summary>
    /// <param name="ud">UserData，用户数据类型DAO</param>
    public void SetUserData(UserData ud)
    {
        playerMng.UserData = ud;
    }

    /// <summary>
    /// 调用角色管理器中的GetUserData
    /// </summary>
    /// <returns>用户数据</returns>
    public UserData GetUserData()
    {
        return playerMng.UserData;
    }

    /// <summary>
    /// 调用角色管理器中的SetCurrentRoleType
    /// </summary>
    /// <param name="rt">RoleType，角色类型</param>
    public void SetCurrentRoleType(RoleType rt)
    {
        playerMng.SetCurrentRoleType(rt);
    }

    /// <summary>
    /// 调用角色管理器中的GetCurrentRoleGameObject
    /// </summary>
    /// <returns>当前角色的游戏物体</returns>
    public GameObject GetCurrentRoleGameObject()
    {
        return playerMng.GetCurrentRoleGameObject();
    }
   /// <summary>
    /// 调用角色管理器中的SendAttack
    /// </summary>
    /// <param name="damage">伤害数值</param>
    public void SendAttack(int damage)
    {
        playerMng.SendAttack(damage);
    }

    /// <summary>
    /// 调用角色管理器中的UpdateResult
    /// </summary>
    /// <param name="totalCount">总场数</param>
    /// <param name="winCount">胜利场数</param>
    public void UpdateResult(int totalCount, int winCount)
    {
        playerMng.UpdateResult(totalCount, winCount);
    }
    #endregion
}
