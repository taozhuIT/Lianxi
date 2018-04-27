using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 帧同步模拟显示层
/// </summary>
public class RoleObjView : MonoBehaviour
{
    // 角色物体
    [SerializeField] private GameObject roleObj;
    // 角色物体
    [SerializeField] private GameObject roleObj2;

    // 游戏逻辑帧更新时间
    private float logicUpdateTime = 0.05f;
    // 游戏本地帧更新累计时间
    private float accumUpdateTime = 0f;

    // 移动目标点
    private Vector3 objTarget = Vector3.zero;
    private Vector3 currv = Vector3.zero;

    private float sTime = 0f;

    /// <summary>
    /// 起始
    /// </summary>
	private void Start ()
    {
        sTime = Time.realtimeSinceStartup;
    }
	
    /// <summary>
    /// 更新
    /// </summary>
	private void Update ()
    {
        UpateFrame();
        OnRandomMove();
    }

    /// <summary>
    /// 帧同步更新
    /// </summary>
    private void UpateFrame()
    {
        //Debug.Log(Time.realtimeSinceStartup - sTime);
        // 模拟一个10秒本地延迟
        if((Time.realtimeSinceStartup - sTime) > 20 && (Time.realtimeSinceStartup - sTime) < 30)
        {
            logicUpdateTime = 0.08f;
        }
        else
        {
            logicUpdateTime = 0.05f;
        }


        // 当本地延迟后 追帧
        accumUpdateTime += Time.unscaledDeltaTime;
        if (accumUpdateTime > logicUpdateTime)
        {
            if (GameVoManage.roleSyncData.GetSyncMessage())
                OnRreshRole();
            else
                Debug.Log("没有同步数据");

            // 首先判断
            accumUpdateTime -= logicUpdateTime;
        }
    }

    /// <summary>
    /// 刷新角色
    /// </summary>
    private void OnRreshRole()
    {
        RoleSyncItemVo syncItem = GameVoManage.roleSyncData.GetSyncFrame();
        if(syncItem != null)
        {
            roleObj2.transform.position = syncItem.objPos;
            roleObj2.transform.rotation = syncItem.objDire;
            //Vector3 eulerAngles = Quaternion.FromToRotation(roleObj.transform.forward, syncItem.objDire).eulerAngles;
            //roleObj.transform.eulerAngles = eulerAngles;
        }
    }

    /// <summary>
    /// 角色随机移动
    /// </summary>
    private void OnRandomMove()
    {
        if (objTarget != Vector3.zero)
        {
            if (Vector3.Distance(roleObj.transform.position, objTarget) > 0.01f)
            {
                //roleObj.transform.Translate(Vector3.forward * Time.deltaTime);
                // 插值移动(减速度移动)
                //roleObj.transform.position = Vector3.SmoothDamp(roleObj.transform.position, objTarget, ref currv, 1f);
                // 均速移动
                roleObj.transform.position = Vector3.MoveTowards(roleObj.transform.position, objTarget, Time.deltaTime * 5f);
            }
            else
            {
                Invoke("OnGetTarget", 2f);
            }
        }
        else
        {
            OnGetTarget();
        }
    }

    /// <summary>
    /// 随机寻找目标点
    /// </summary>
    private void OnGetTarget()
    {
        // 随机在一定范围内找一个目标点
        int seed = (int)Time.realtimeSinceStartup * 1000;
        System.Random rand = new System.Random(seed);
        float randX = rand.Next(1, 10);
        float randY = rand.Next(1, 10);
        Vector3 randPos = new Vector3(randX, 0, randY);
        if (Vector3.Distance(randPos, objTarget) > 5)
        {
            objTarget = randPos;
            roleObj.transform.LookAt(objTarget);
        }  
    }
}
