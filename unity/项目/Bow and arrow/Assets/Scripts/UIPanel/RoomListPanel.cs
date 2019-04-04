using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Common;

/// <summary>
/// RoomListPanel
/// 房间列表面板，继承自BasePanel（面板基类）
/// </summary>
public class RoomListPanel : BasePanel
{
    #region 成员变量
    /// <summary>
    /// 自身信息面板
    /// </summary>
    private RectTransform battleRes;

    /// <summary>
    /// 房间列表面板
    /// </summary>
    private RectTransform roomList;

    /// <summary>
    /// 房间列表排列
    /// </summary>
    private VerticalLayoutGroup roomLayout;

    /// <summary>
    /// 房间预设体
    /// </summary>
    private GameObject roomItemPrefab;

    /// <summary>
    /// 刷新房间列表信息申请脚本
    /// </summary>
    private ListRoomRequest listRoomRequest;

    /// <summary>
    /// 创建房间申请脚本
    /// </summary>
    private CreateRoomRequest createRoomRequest;

    /// <summary>
    /// 刷新排名脚本
    /// </summary>
    private RankRequest rankRequest;

    /// <summary>
    /// 加入房间申请脚本
    /// </summary>
    private JoinRoomRequest joinRoomRequest;

    /// <summary>
    /// 房间list
    /// </summary>
    private List<UserData> udList = null;

    /// <summary>
    /// 房间用户信息，ud1
    /// </summary>
    private UserData ud1 = null;

    /// <summary>
    /// 房间用户信息，ud2
    /// </summary>
    private UserData ud2 = null;
    #endregion

    #region 游戏物体事件
    /// <summary>
    /// Start
    /// MonoBehaviour中的初始化
    /// </summary>
    private void Start()
    {
        battleRes = transform.Find("BattleRes").GetComponent<RectTransform>(); //获取自身信息面板
        roomList = transform.Find("RoomList").GetComponent<RectTransform>(); //获取房间列表面板
        roomLayout = transform.Find("RoomList/ScrollRect/Layout").GetComponent<VerticalLayoutGroup>(); //获取房间列表排版
        roomItemPrefab = Resources.Load("UIPanel/RoomItem") as GameObject; //获取房间预设体
        transform.Find("RoomList/CloseButton").GetComponent<Button>().onClick.AddListener(OnCloseClick); //监听关闭按钮
        transform.Find("RoomList/CreateRoomButton").GetComponent<Button>().onClick.AddListener(OnCreateRoomClick); //监听创建房间按钮
        transform.Find("RoomList/RefreshButton").GetComponent<Button>().onClick.AddListener(OnRefreshClick); //监听刷新按钮
        transform.Find("BattleRes/RankButton").GetComponent<Button>().onClick.AddListener(OnRankClick); //监听刷新按钮

        rankRequest = GetComponent<RankRequest>(); //获取刷新排名申请脚本
        listRoomRequest = GetComponent<ListRoomRequest>(); //获取刷新房间列表申请脚本
        createRoomRequest = GetComponent<CreateRoomRequest>(); //获取创建房间申请脚本
        joinRoomRequest = GetComponent<JoinRoomRequest>(); //获取加入房间申请脚本
        EnterAnim(); //进入动画
    }

    /// <summary>
    /// Update
    /// MonoBehaviour中的每帧更新
    /// </summary>
    private void Update()
    {
        //异步加载房间列表
        if (udList != null)
        {
            LoadRoomItem(udList); 
            udList = null;
        }
        //异步加入房间
        if (ud1 != null && ud2 != null)
        {
            BasePanel panel = uiMng.PushPanel(UIPanelType.Room); //加载游戏房间面板
            (panel as RoomPanel).SetAllPlayerResSync(ud1, ud2); //设置游戏房间信息
            ud1 = null; ud2 = null;
        }
    }

    /// <summary>
    /// OnEnter
    /// 重写BasePanel中的进入
    /// </summary>
    public override void OnEnter()
    {
        SetBattleRes(); //设置自身信息显示
        if (battleRes != null)
            EnterAnim(); //自身信息面板进入动画
        if(listRoomRequest==null)
            listRoomRequest = GetComponent<ListRoomRequest>(); //onenter比start早，可能会出现空指针
        listRoomRequest.SendRequest(); //发送刷新房间申请
    }

    /// <summary>
    /// OnExit
    /// 重写BasePanel中的退出
    /// </summary>
    public override void OnExit()
    {
        HideAnim(); //退出动画
    }

    /// <summary>
    /// OnPause
    /// 重写BasePanel中的暂停
    /// </summary>
    public override void OnPause()
    {
        HideAnim(); //退出动画
    }

    /// <summary>
    /// OnResume
    /// 重写BasePanel中的恢复
    /// </summary>
    public override void OnResume()
    {
        EnterAnim(); //进入动画
        listRoomRequest.SendRequest(); //发送刷新房间列表申请
    }
    #endregion

    #region 提供的方法
    /// <summary>
    /// 响应同步游戏结果事件
    /// </summary>
    /// <param name="totalCount">自身总场数</param>
    /// <param name="winCount">自身胜利场数</param>
    public void OnUpdateResultResponse(int totalCount, int winCount)
    {
        facade.UpdateResult(totalCount, winCount); //调用中介者提供的同步游戏结果方法
        SetBattleRes(); //设置自身信息面板
    }

    /// <summary>
    /// 异步加载各个房间
    /// </summary>
    /// <param name="udList">房间list</param>
    public void LoadRoomItemSync(List<UserData> udList)
    {
        this.udList = udList; //更新list，在update中去异步加载
    }

    /// <summary>
    /// 提供加入房间方法给每个RoomItem
    /// </summary>
    /// <param name="id">房间id</param>
    public void OnJoinClick(int id)
    {
        joinRoomRequest.SendRequest(id); //发出加入房间申请
    }

    /// <summary>
    /// 响应加入房间事件
    /// </summary>
    /// <param name="returnCode">服务器发送的响应动作</param>
    /// <param name="ud1">房间用户信息ud1</param>
    /// <param name="ud2">房间用户信息ud2</param>
    public void OnJoinResponse(ReturnCode returnCode, UserData ud1, UserData ud2)
    {
        switch (returnCode)
        {
            case ReturnCode.NotFound: 
                uiMng.ShowMessageSync("房间被销毁无法加入"); //异步显示消息提示
                break;
            case ReturnCode.Fail:
                uiMng.ShowMessageSync("房间已满，无法加入"); //异步显示消息提示
                break;
            case ReturnCode.Success:
                //异步添加用户信息，在update中加载进入房间
                this.ud1 = ud1; 
                this.ud2 = ud2;
                break;
        }
    }
    #endregion

    #region 私有方法
    /// <summary>
    /// 关闭按钮监听
    /// </summary>
    private void OnCloseClick()
    {
        PlayClickSound(); //发出点击的声音
        uiMng.PopPanel(); //弹出面板
    }

    /// <summary>
    /// 创建房间按钮监听
    /// </summary>
    private void OnCreateRoomClick()
    {
        BasePanel panel= uiMng.PushPanel(UIPanelType.Room); //加载房间面板
        createRoomRequest.SetPanel(panel); //设置创建的房间信息
        createRoomRequest.SendRequest(); //发出创建房间申请
    }

    /// <summary>
    /// 刷新按钮监听
    /// </summary>
    private void OnRefreshClick()
    {
        listRoomRequest.SendRequest(); //发出刷新房间列表申请
    }

    private void OnRankClick()
    {
        rankRequest.SendRequest();
        //GameFacade.Instance.PushPanel(UIPanelType.Rank);
    }

    /// <summary>
    /// 房间列表面板进入动画
    /// </summary>
    private void EnterAnim()
    {
        gameObject.SetActive(true); //显示房间列表面板

        battleRes.localPosition = new Vector3(-1000, 0); //自身信息面板设置初始位置
        battleRes.DOLocalMoveX(-290, 0.5f); //自身信息面板移动动画

        roomList.localPosition = new Vector3(1000, 0); //房间信息列表面板设置初始位置
        roomList.DOLocalMoveX(171, 0.5f); //房间信息列表面板移动动画
    }

    /// <summary>
    /// 房间列表面板退出动画
    /// </summary>
    private void HideAnim()
    {
        battleRes.DOLocalMoveX(-1000, 0.5f); //自身信息面板退出动画

        roomList.DOLocalMoveX(1000, 0.5f).OnComplete(() => gameObject.SetActive(false)); //房间信息列表面板动画，结束后关闭房间列表面板显示
    }

    /// <summary>
    /// 设置自身信息
    /// </summary>
    private void SetBattleRes()
    {
        UserData ud = facade.GetUserData(); //获取自身用户信息
        transform.Find("BattleRes/Username").GetComponent<Text>().text = ud.Username; //设置用户名称
        transform.Find("BattleRes/TotalCount").GetComponent<Text>().text = "总场数:"+ud.TotalCount.ToString(); //设置总场数信息
        transform.Find("BattleRes/WinCount").GetComponent<Text>().text = "胜利:"+ud.WinCount.ToString(); //设置胜利场数信息
    }

    /// <summary>
    /// 加载所有房间
    /// </summary>
    /// <param name="udList">房间list</param>
    private void LoadRoomItem( List<UserData> udList)
    {
        //销毁现有所有房间显示
        RoomItem[] riArray= roomLayout.GetComponentsInChildren<RoomItem>(); //获取所有房间信息
        foreach(RoomItem ri in riArray)
        {
            ri.DestroySelf();
        }

        //重新加载所有房间
        int count = udList.Count;
        for (int i = 0; i < count; i++)
        {
            GameObject roomItem = GameObject.Instantiate(roomItemPrefab); //实例化房间
            roomItem.transform.SetParent(roomLayout.transform); //设置房间位置
            UserData ud = udList[i]; //取出对应的房间信息
            roomItem.GetComponent<RoomItem>().SetRoomInfo(ud.Id, ud.Username, ud.TotalCount, ud.WinCount,this); //设置房间信息
        }
        int roomCount = GetComponentsInChildren<RoomItem>().Length;
        Vector2 size = roomLayout.GetComponent<RectTransform>().sizeDelta; //房间之间的间隔
        //计算roomlayout大小，使其正常显示
        roomLayout.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x,
            roomCount * (roomItemPrefab.GetComponent<RectTransform>().sizeDelta.y + roomLayout.spacing));
    }
    #endregion
 }
