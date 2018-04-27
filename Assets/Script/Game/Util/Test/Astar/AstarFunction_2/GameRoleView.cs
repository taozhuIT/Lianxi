using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 测试寻路角色脚本
/// </summary>
public class GameRoleView : MonoBehaviour
{
    // 寻路根节点
    [SerializeField] private AstartView findPathRoot;
    // 3D相机(场景相机)
    [SerializeField] private Camera sceneCamera;
    // 角色
    [SerializeField] private Transform roleObj;
    // 移动协程
    private Coroutine moveCorouine = null;
    // 路径列表
    private List<AstartView.GridInfo> pathList = new List<AstartView.GridInfo>();

    // 测试
    // 攻击槽模型
    [SerializeField] private GameObject debugModel;
    // 攻击槽模型列表
    private List<GameObject> debugModelList = new List<GameObject>();

    /// <summary>
    /// 攻击槽数据
    /// </summary>
    private class slotInfo
    {
        // 槽位置
        public Vector3 attackPos = Vector3.zero;
        // 是否被占用
        public bool isOccupy = false;
        // 占用敌人信息

        /// <summary>
        /// 构造
        /// </summary>
        public slotInfo(Vector3 attackPos_)
        {
            attackPos = attackPos_;
        }
    }
    // 攻击槽列表
    private Dictionary<int, slotInfo> slotDict = new Dictionary<int, slotInfo>();

    /// <summary>
    /// 起始
    /// </summary>
    private void Start ()
    {

    }

    /// <summary>
    /// 起始
    /// </summary>
    private void Update ()
    {
        if (Input.GetMouseButtonUp(0))
            OnSelectTargetPos();
    }

    /// <summary>
    /// 测试工具函数-选择点
    /// </summary>
    private void OnSelectTargetPos()
    {
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit caseHit;

        if (Physics.Raycast(ray, out caseHit, Mathf.Infinity)) 
        {
            if (caseHit.collider != null && caseHit.collider.gameObject.layer == LayerMask.NameToLayer("Map"))
            {
                OnClickPlayerFindPath(caseHit.point);
                Debug.Log("选中目标点  " + caseHit.point);
            }
            else
            {
                Debug.LogError("没选择地形");
            }
        }
    }

    /// <summary>
    /// 测试工具函数-开始寻路
    /// </summary>
    private void OnClickPlayerFindPath(Vector3 targetPos_)
    {
        // 先暂停移动
        pathList.Clear();
        if (moveCorouine != null)
            StopCoroutine(moveCorouine);

        // 寻路
        findPathRoot.OnPlayerFindPath(roleObj.position, targetPos_, OnFindPathFinishHandel);
    }

    /// <summary>
    /// 寻路完成回调
    /// </summary>
    private void OnFindPathFinishHandel(List<AstartView.GridInfo> pathList_)
    {
        // 寻路完成开始移动
        pathList = pathList_;
        // 执行移动
        moveCorouine = StartCoroutine(OnSetModelMove());
    }

    /// <summary>
    /// 测试工具函数-设置移动
    /// </summary>
    private IEnumerator OnSetModelMove()
    {
        // 路径索引
        int index = 0;

        while (true)
        {
            AstartView.GridInfo info = pathList[index];

            if (Vector3.Distance(roleObj.position, info.gridPos) > 0.01f)
            {
                // 均速移动
                roleObj.transform.position = Vector3.MoveTowards(roleObj.transform.position, info.gridPos, Time.deltaTime * 2f);
                OnGetAttckSlots(6, 2);
            }
            else
            {
                if (index < (pathList.Count - 1))
                {
                    index++;
                    roleObj.transform.LookAt(pathList[index].gridPos);
                }
                else
                {
                    yield break;
                }
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    /// <summary>
    /// 更新角色攻击槽(攻击位)
    /// </summary>
    /// <param name="attackCount_">攻击槽数量</param>
    /// <param name="attackDist_">攻击距离</param>
    private void OnGetAttckSlots(int attackCount_, int attackDist_)
    {
        float degrees = 180 / attackCount_;

        for (int i = 0; i<attackCount_; ++i)
        {
            Vector3 rolePos = roleObj.transform.position;
            Vector3 attackDist = new Vector3(0f, 0f, attackDist_);
            
            // 得到攻击位
            Vector3 slotPos = rolePos + (Quaternion.Euler(new Vector3(0f, degrees * i, 0f)) * attackDist);

            slotInfo slotItem = null;
            if (slotDict.ContainsKey(i))
            {
                // 更新攻击槽信息
                slotItem = slotDict[i];
                slotItem.attackPos = slotPos;
            }
            else
            {
                // 创建新的攻击槽
                slotItem = new slotInfo(slotPos);
                slotDict[i] = slotItem;

                // 测试
                GameObject obj = Instantiate<GameObject>(debugModel);
                obj.name = "slot_" + i;
                obj.transform.SetParent(this.transform);
                obj.transform.localScale = new Vector3(1f, 1f, 1f);
                obj.SetActive(true);
                debugModelList.Add(obj);
            }
        }

        // 测试
        for (int i = 0; i < slotDict.Count; ++i)
        {
            slotInfo info = slotDict[i];
            GameObject obj = debugModelList[i];
            obj.transform.position = info.attackPos;
        }
    }
}
