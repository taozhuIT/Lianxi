using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// 活动配置项数据
/// </summary>
[Serializable]
public class AchieveItemVo : ConfVo
{
    /// <summary>
    /// 活动编号
    /// </summary>
    public int q_id;


    /// <summary>
    /// 活动名称
    /// </summary>
    public string q_name;


    /// <summary>
    /// 活动类型//ZQS:=1：特殊活动=2：每月签到=3：每日在线=4：每周在线=5：离线经验找回=6：首冲奖励=9：七日登录=10：功勋=13：每日充值=16：等级竞技=20：神翼竞技=21:坐骑竞技=208：充值=209：消费=210：每日签到=250：签到补签=261：每周绑元奖励=263:开服皇城争霸=264:开服BOSS复活时间减半=270：吃鸡活动=271:神秘商店=273：20元礼包==267:折扣商店
    /// </summary>
    public int q_type;


    /// <summary>
    /// 入口按钮显示用大类型//1、福利按钮=2、每日充值=3、首充=4、开服竞技=6、经验炼制=7、7天登陆=5、开服活动=8、消费夺宝=9、登录豪礼=10、圣诞=
    /// </summary>
    public int q_bigtype;


    /// <summary>
    /// 功能类型
    /// </summary>
    public int q_function_type;


    /// <summary>
    /// 客户端玩家参与条件
    /// </summary>
    public string q_client_condition;


    /// <summary>
    /// 活动开放提示
    /// </summary>
    public string q_open_notify;


    /// <summary>
    /// 活动结束提示
    /// </summary>
    public string q_finish_notify;


    /// <summary>
    /// 是否发送广播
    /// </summary>
    public int q_notify;


    /// <summary>
    /// 是否为推荐活动
    /// </summary>
    public int q_tuijian;


    /// <summary>
    /// 需要物品
    /// </summary>
    public string q_need_item;


    /// <summary>
    /// 奖励道具
    /// </summary>
    public string q_rewards;


    /// <summary>
    /// 界面展示奖励道具
    /// </summary>
    public string q_show_rewards;


    /// <summary>
    /// 公告栏权重
    /// </summary>
    public int q_notice_weight;


    /// <summary>
    /// 活动内容介绍
    /// </summary>
    public string q_info;


    /// <summary>
    /// 活动排序
    /// </summary>
    public int q_sort;


    /// <summary>
    /// 活动图标
    /// </summary>
    public string q_icon;


    /// <summary>
    /// 活动图标2
    /// </summary>
    public string q_icon2;


    /// <summary>
    /// 活动备用描述
    /// </summary>
    public string q_info_spare;


    /// <summary>
    /// 活动备用描述2
    /// </summary>
    public string q_info_spare2;


    /// <summary>
    /// 是否一直显示特效
    /// </summary>
    public int q_always_effect;


    /// <summary>
    /// 活动对应面板id
    /// </summary>
    public int q_panelId;


    /// <summary>
    /// 是否推送（0为不推送，大于1时，按照ID由小到大推送，同一个类型的活动，填写一样的ID）
    /// </summary>
    public int q_tuisong;


    /// <summary>
    /// 投资档次
    /// </summary>
    public int q_touzi_lv;


    /// <summary>
    /// 每日充值邮件奖励
    /// </summary>
    public string q_send_reward_email;
}
