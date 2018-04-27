using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 障碍物
/// </summary>
public class GridBarrierView : MonoBehaviour 
{
    // 障碍物占的格子列表
    private List<GameObject> barrierGridList = new List<GameObject>();
    public List<GameObject> barrierGridLIST
    {
        get{ return barrierGridList;}
    }

    /// <summary>
    /// 碰撞开始
    /// </summary>
    public void OnTriggerEnter(Collider other)
    {
        barrierGridList.Add(other.gameObject);
    }
}
