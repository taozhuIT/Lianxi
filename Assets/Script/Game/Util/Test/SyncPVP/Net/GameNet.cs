using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// 模拟帧同步网络
/// </summary>
public class GameNet : MonoBehaviour
{
    // 游戏逻辑帧更新时间
    private float logicUpdateTime = 0.05f;
    // 游戏本地帧更新累计时间
    private float accumUpdateTime = 0f;
    // 上一次位置
    private Vector3 upObjPos = Vector3.zero;

    // 角色物体
    [SerializeField] private GameObject roleObj;

    private void Start ()
    {
	    	
	}
	
	private void Update ()
    {
        // 当本地延迟后 追帧
        accumUpdateTime += Time.unscaledDeltaTime;
        if (accumUpdateTime > logicUpdateTime)
        {
            OnSendMessage();
            // 首先判断
            accumUpdateTime -= logicUpdateTime;
        }
    }

    private void OnSendMessage()
    {
        GameVoManage.roleSyncData.AddSyncFrame(roleObj.transform.position, roleObj.transform.rotation);
    }
}
