using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 帧同步数据模型
/// </summary>
public class RoleSyncItemVo : GameVoManage
{
    // 位置
    public Vector3 objPos = Vector3.zero;
    // 方向
    public Quaternion objDire = Quaternion.identity;
}
