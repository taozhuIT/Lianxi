using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 寻找攻击位 (测试)
/// </summary>
public class SelectAttackSlot : MonoBehaviour
{
    // 攻击目标
    [SerializeField] private GameObject targetObj;

    // 攻击者生成点
    [SerializeField] private GameObject attackRolePos;
    // 攻击者模板
    [SerializeField] private GameObject attackRoleTemp;
    // 攻击者Y数量
    [SerializeField] private int attackRoleY;
    // 攻击者X数量
    [SerializeField] private int attackRoleX;

    /// <summary>
    /// 攻击槽信息
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

    // 攻击槽列表 key = 攻击距离  value = 攻击槽列表
    private Dictionary<float, List<slotInfo>> slotDict = new Dictionary<float, List<slotInfo>>();
    // 攻击者列表 key = 攻击距离  value = 攻击者数量
    private Dictionary<float, float> attackInfoNumDict = new Dictionary<float, float>();


    // --------------------------测试--------------------------
    /// <summary>
    /// 攻击者信息
    /// </summary>
    private class attackRoleInfo
    {
        // 节点
        public Transform attackRole = null;
        // 目标
        public Vector3 targetPos = Vector3.zero;
        // 攻击距离
        public float attackDist = 0;

        /// <summary>
        /// 构造
        /// </summary>
        public attackRoleInfo(Transform attackRole_, float attackDist_)
        {
            attackDist = attackDist_;
            attackRole = attackRole_;
        }
    }

    // 攻击者列表
    private List<attackRoleInfo> attackList = new List<attackRoleInfo>();
    // --------------------------测试--------------------------

    /// <summary>
    /// 初始
    /// </summary>
    private void Start ()
    {
        // --------------------------测试--------------------------
        OnCreateAttackRole();
        OnSelectAttackPos();
        // --------------------------测试--------------------------
    }

    /// <summary>
    /// 更新
    /// </summary>
    private void Update ()
    {
		
	}


    /// <summary>
    /// 得到指定距离可用攻击位
    /// </summary>
    /// <param name="attackDist_">攻击距离</param>
    private Vector3 OnGetAttackSlot(attackRoleInfo attackInfo_)
    {
        // 填充攻击者
        if (attackInfoNumDict.ContainsKey(attackInfo_.attackDist))
            attackInfoNumDict[attackInfo_.attackDist] += 1;
        else
            attackInfoNumDict.Add(attackInfo_.attackDist, 1);

        // 找到有没有当前攻击距离的攻击槽信息
        List<slotInfo> nowSlotList = OnGetAttckSlots(attackInfo_.attackDist);
        //if (slotDict.ContainsKey(attackInfo_.attackDist))
        //    nowSlotList = slotDict[attackInfo_.attackDist];  // 有就直接得到
        //else
        //    nowSlotList = OnGetAttckSlots(attackInfo_.attackDist); // 没有就获取

        // 得到攻击槽位
        slotInfo slotInfo = null;
        if (nowSlotList.Count > 0)
        {
            for (int j = 0; j < nowSlotList.Count; ++j)
            {
                slotInfo info = nowSlotList[j];
                if (!info.isOccupy)
                {
                    if (slotInfo == null)
                        slotInfo = info;
                    else
                    {
                        // 目标槽位距离
                        float targetDis = Vector3.Distance(attackInfo_.attackRole.position, slotInfo.attackPos);
                        // 当前槽位距离
                        float currDis = Vector3.Distance(attackInfo_.attackRole.position, info.attackPos);

                        // 找到和自己距离最短的攻击位
                        if (currDis < targetDis)
                            slotInfo = info;
                    }
                }
            }
        }

        // 设置占用
        if (slotInfo != null)
            slotInfo.isOccupy = true;

        return slotInfo != null ? slotInfo.attackPos : Vector3.zero;
    }

    // <summary>
    /// 得到指定攻击距离的全部槽(攻击位)
    /// </summary>
    private List<slotInfo> OnGetAttckSlots(float attackDist_)
    {
        // 首先判断(攻击者数量)是否大于(攻击距离*默认数量)
        // 大于则生成攻击者数量的攻击位数量，小于则根据攻击距离来计算默认的攻击位数量
        float newSlotNum = attackInfoNumDict[attackDist_] > (attackDist_ * 7) ? attackInfoNumDict[attackDist_] : attackDist_ * 7;

        float existSlotNum = 0;
        if (slotDict.ContainsKey(attackDist_))
            existSlotNum = slotDict[attackDist_].Count;

        // 生成攻击位
        List<slotInfo> nowSlotList;
        float slotNum = newSlotNum - existSlotNum;
        float degrees = 360 / slotNum;

        //for (int i = 0; i < slotNum; ++i)
        //{
        //    Vector3 rolePos = targetObj.transform.position;   // 这里的targetObj在我的设计中就是攻击者或者被攻击者本身
        //    Vector3 attackDist = new Vector3(0f, 0f, attackDist_);

        //    // 得到攻击位
        //    Vector3 slotPos = rolePos + (Quaternion.Euler(new Vector3(0f, degrees * i, 0f)) * attackDist);

        //    // 创建新的攻击槽
        //    slotInfo slotItem = new slotInfo(slotPos);
        //    nowSlotList.Add(slotItem);
        //}

        Vector3 rolePos = targetObj.transform.position;   // 这里的targetObj在我的设计中就是攻击者或者被攻击者本身
        Vector3 attackDist = new Vector3(0f, 0f, attackDist_);

        // 得到攻击位
        Vector3 slotPos = rolePos + (Quaternion.Euler(new Vector3(0f, degrees * (existSlotNum + 1), 0f)) * attackDist);
        slotInfo slotItem = new slotInfo(slotPos);

        if (slotDict.ContainsKey(attackDist_))
        {
            nowSlotList = slotDict[attackDist_];
            nowSlotList.Add(slotItem);
        }
        else
        {
            nowSlotList = new List<slotInfo>();
            nowSlotList.Add(slotItem);
            slotDict.Add(attackDist_, nowSlotList);
        }

        return nowSlotList;
    }


    // --------------------------------测试代码--------------------------------
    /// <summary>
    /// 生成攻击角色
    /// </summary>
    private void OnCreateAttackRole()
    {
        int index = 1;
        for (int i = 0; i < attackRoleY; ++i)
        {
            for (int j = 0; j < attackRoleY; ++j)
            {
                GameObject obj = Instantiate<GameObject>(attackRoleTemp);
                obj.transform.position = new Vector3(attackRolePos.transform.position.x + j, 0f, attackRolePos.transform.position.z + i);
                obj.transform.localScale = new Vector3(1f, 1f, 1f);
                obj.transform.SetParent(attackRolePos.transform);

                float attackDist = 10;
                if ((index % 2) == 0)
                    attackDist = 7;
                if ((index % 3) == 0)
                    attackDist = 3;

                index++;

                attackList.Add(new attackRoleInfo(obj.transform, attackDist));
            }
        }
    }

    /// <summary>
    /// 寻找目标攻击点
    /// </summary>
    private void OnSelectAttackPos()
    {
        // 攻击距离
        float attackDist = 5f;

        for (int i = 0; i < attackList.Count; ++i)
        {
            attackRoleInfo attackInfo = attackList[i];

            // 获取攻击位
            Vector3 slotPos = OnGetAttackSlot(attackInfo);

            if (slotPos != Vector3.zero)
            {
                attackInfo.targetPos = slotPos;
                StartCoroutine(OnSetModelMove(attackInfo));
            }
        }
    }

    /// <summary>
    /// 测试工具函数-设置移动
    /// </summary>
    private IEnumerator OnSetModelMove(attackRoleInfo obj_)
    {
        while (true)
        {
            if (Vector3.Distance(obj_.attackRole.position, obj_.targetPos) > 0.01f)
            {
                // 均速移动
                obj_.attackRole.position = Vector3.MoveTowards(obj_.attackRole.position, obj_.targetPos, Time.deltaTime * 2f);
            }
            else
            {
                yield break;
            }

            yield return new WaitForSeconds(0.01f);
        }
    }

    // --------------------------------测试代码--------------------------------
}
