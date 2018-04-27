using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色帧同步数据
/// </summary>
public class RoleSyncData : GameVoManage
{
    // 更新帧队列
    private Queue<RoleSyncItemVo> roleSyncQueue = new Queue<RoleSyncItemVo>();

    /// <summary>
    /// 添加同步帧
    /// 这里形参直接写两个三维向量，是为了模拟网络数据。实际中这里应该是解析出来的protobuf数据
    /// </summary>
    public void AddSyncFrame(Vector3 pos_, Quaternion dire_)
    {
        RoleSyncItemVo syncItem = new RoleSyncItemVo
        {
            objPos = pos_,
            objDire = dire_,
        };
        
        roleSyncQueue.Enqueue(syncItem);
    }

    /// <summary>
    /// 好的一次同步帧
    /// </summary>
    /// <returns></returns>
    public RoleSyncItemVo GetSyncFrame()
    {
        RoleSyncItemVo frameItem = null;
        if (roleSyncQueue.Count > 0)
            frameItem = roleSyncQueue.Dequeue();
        return frameItem;
    }

    /// <summary>
    /// 得到同步帧数据是否存在
    /// </summary>
    /// <returns></returns>
    public bool GetSyncMessage()
    {
        Debug.Log(roleSyncQueue.Count);
        return roleSyncQueue.Count > 0;
    }
}
