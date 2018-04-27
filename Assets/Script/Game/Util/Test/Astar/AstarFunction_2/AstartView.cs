using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A*寻路组件3D
/// </summary>
public class AstartView : MonoBehaviour
{
    // 委托
    public delegate void dele(List<GridInfo> list_);
    // 事件(寻路完成)
    public event dele findPathFinishHandel;

    // 格子X轴数量
    [SerializeField] private int gridNumX;
    // 格子Z轴数量
    [SerializeField] private int gridNumZ;
    // 格子宽度(x 轴)
    [SerializeField] private int gridItemWide;
    // 格子高度(z 轴)
    [SerializeField] private int gridItemHigh;
    // 格子原点
    [SerializeField] private Transform gridOnePos;
    // 障碍物根节点
    [SerializeField] private Transform barrierRoot;

    // 是否开启调试
    [SerializeField] private bool isDebug = false;
    // 调试节点-模型模板
    [SerializeField] private GameObject debugModelTemp;
    // 调试节点-模型父节点
    [SerializeField] private GameObject debugModelParent;

    /// <summary>
    /// 格子信息
    /// </summary>
    public class GridInfo
    {
        
        // 格子位置
        public Vector3 gridPos = Vector3.zero;
        // 格子范围Xmin
        public float XscopeMin = 0;
        // 格子范围Xmax
        public float XscopeMax = 0;
        // 格子范围Zmin
        public float ZscopeMin = 0;
        // 格子范围Zmax
        public float ZscopeMax = 0;
        // 格子移动代价
        public int gridPrice = 0;
        // 格子到目标点的估价
        public int gridReckon = 0;
        // 格子权重
        public int gridWeight = 0;

        // 调试变量
        public GameObject debugObj = null;

        /// <summary>
        /// 构造
        /// </summary>
        public GridInfo(Vector3 gridPos_, int gridWide_, int gridHigh_)
        {
            gridPos = gridPos_;

            // 计算格子范围
            XscopeMin = gridPos.x - (gridWide_ / 2f);
            XscopeMax = gridPos.x + (gridWide_ / 2f);
            ZscopeMin = gridPos.z - (gridHigh_ / 2f);
            ZscopeMax = gridPos.z + (gridHigh_ / 2f);
        }
    }
    
    // 起始格子
    private GridInfo starGrid = null;
    // 目标格子
    private GridInfo endGrid = null;
    // 格子列表
    private List<GridInfo> gridInfoList = new List<GridInfo>();
    // 障碍物列表
    private List<BoxCollider> BarrierInfoList = new List<BoxCollider>();
    // 障碍物格子列表 (寻路时不能通过的格子)
    private List<GridInfo> BarrierGridInfoList = new List<GridInfo>();
    // 寻路点相邻格子列表 (当前格子周围八个格子)
    private List<GridInfo> borderGridList = new List<GridInfo>();
    // 路径列表
    private List<GridInfo> pathList = new List<GridInfo>();

    /// <summary>
    /// 起始
    /// </summary>
    private void Start()
    {
        OnCreateGrid();
        OnSetBarrier();
        OnGetBarrierGrid();
    }

    /// <summary>
    /// 更新
    /// </summary>
    private void Update()
    {
    }

    /// <summary>
    /// 创建格子
    /// </summary>
    private void OnCreateGrid()
    {
        for(int i = 0; i < gridNumX; ++i)
        {
            for (int j = 0; j < gridNumZ; ++j)
            {
                float posX = gridOnePos.position.x + (i * gridItemWide);
                float posZ = gridOnePos.position.z + (j * gridItemHigh);

                Vector3 pos = new Vector3(posX, 0, posZ);
                GridInfo gridItem = new GridInfo(pos, gridItemWide, gridItemHigh);

                gridInfoList.Add(gridItem);

                // 是否调试
                if (isDebug)
                {
                    gridItem.debugObj = OnDebugCreatObj(gridItem);
                    OnDebugObjColor(gridItem.debugObj, Color.green);
                }
            }
        }
    }

    /// <summary>
    /// 得到障碍物信息
    /// </summary>
    private void OnSetBarrier()
    {
        BarrierInfoList.Clear();

        BoxCollider[] barrierList = barrierRoot.transform.GetComponentsInChildren<BoxCollider>();
        for (int i = 0; i < barrierList.Length; ++i)
        {
            BoxCollider barrierObj = barrierList[i];
            //Vector3 pos = barrierObj.transform.position;
            //Vector3 sca = barrierObj.transform.localScale;
            //BarrierInfo barrierItem = new BarrierInfo(pos, sca.x + 1, sca.z + 1);
            BarrierInfoList.Add(barrierObj);
        }
    }

    /// <summary>
    /// 得到障碍物格子（不能通过）
    /// </summary>
    private void OnGetBarrierGrid()
    {
        BarrierGridInfoList.Clear();

        for (int i = 0; i < gridInfoList.Count; ++i)
        {
            // 是否在障碍物范围内
            GridInfo gridInfo = gridInfoList[i];

            // 粗判断 
            // 先通过Box边界盒判断格子是否在边界盒范围内(缩小需要用射线判断的格子数范围)
            bool isBoundsCover = false;
            for (int j = 0; j < BarrierInfoList.Count; ++j)
            {
                BoxCollider boxColl = BarrierInfoList[j];
                if (boxColl.bounds.Contains(gridInfo.gridPos))
                {
                    isBoundsCover = true;
                    break;
                }
            }

            // 细判断
            // 如果在Box边界盒范围内，再用射线检测
            if (isBoundsCover)
            {
                Debug.Log("1");
                // 通过射线判断
                RaycastHit caseHit;
                // PS:这里将Y轴位置设置成-3是因为如果格子的Y轴，和障碍物的Box相交（就是已经在Box中）射线无法碰撞。
                // 所以，新New一个三维向量，修改Y轴位置让格子不和障碍物Box相交
                Vector3 pos = new Vector3(gridInfo.gridPos.x, -3, gridInfo.gridPos.z);
                if (Physics.Raycast(pos, Vector3.up, out caseHit, 3f, 1 << LayerMask.NameToLayer("Barrier")))
                {
                    BarrierGridInfoList.Add(gridInfo);

                    // 是否调试
                    if (isDebug)
                        OnDebugObjColor(gridInfo.debugObj, Color.red);
                }
            }

            // 这个方法计算最省，但是无法处理障碍物旋转的问题
            //for (int j = 0; j < BarrierInfoList.Count; ++j)
            //{
            //BoxCollider barrierInfo = BarrierInfoList[j];
            // 计算位置判断
            //float gridPosX = float.Parse(gridInfo.gridPos.x.ToString("#0.0"));
            //float gridPosZ = float.Parse(gridInfo.gridPos.z.ToString("#0.0"));
            //bool xScope = gridPosX >= barrierInfo.XscopeMin && gridPosX <= barrierInfo.XscopeMax;
            //bool zScope = gridPosZ >= barrierInfo.ZscopeMin && gridPosZ <= barrierInfo.ZscopeMax;
            //}
        }
    }

    /// <summary>
    /// 执行寻路
    /// 留给外部调用的接口(起始点和目标点是外部传进来的)
    /// </summary>
    public void OnPlayerFindPath(Vector3 startGrid_, Vector3 endGrid_, AstartView.dele findPathFinish_)
    {
        // 重置环境
        pathList.Clear();

        // 调试函数-颜色还原
        if (isDebug)
            OnDebugRestore();

        // 初始化参数
        starGrid = OnFindGrid(startGrid_);
        endGrid = OnFindGrid(endGrid_);

        // 是否调试
        if (isDebug)
        {
            OnDebugObjColor(starGrid.debugObj, Color.red);
            OnDebugObjColor(endGrid.debugObj, Color.red);
        }

        // 初始化回调
        findPathFinishHandel = findPathFinish_;

        OnFindPathCompute();
    }

    /// <summary>
    /// 执行寻路计算
    /// </summary>
    private void OnFindPathCompute()
    {
        // 得到相邻格子
        OnGetStartBorderPos();
        // 得到相邻格子的移动成本
        OnGetBorderGridMoveCost(starGrid);
        // 得到相邻格子到目标到的步长
        OnGetBorderGridTargetLength(endGrid);

        // 得到下一个移动点(权重最小的格子)
        GridInfo newStarGridItem = null;
        for (int i = 0; i < borderGridList.Count; ++i)
        {
            GridInfo item = borderGridList[i];
            
            if (newStarGridItem != null)
            {
                if (item.gridWeight < newStarGridItem.gridWeight)
                    newStarGridItem = item;
            }
            else
            {
                newStarGridItem = item;
            }
        }
        
        if (newStarGridItem != null)
        {
            // 是否寻到目标点
            bool isFindPath = true;
            for (int i = 0; i < borderGridList.Count; ++i)
            {
                GridInfo item = borderGridList[i];
                if (item == endGrid)
                {
                    isFindPath = false;
                    break;
                }
            }

            if (isFindPath)
            {
                // 更新寻路点
                starGrid = newStarGridItem;
                // 将格子加入到路径中
                pathList.Add(newStarGridItem);

                // 是否调试
                if (isDebug)
                    OnDebugObjColor(newStarGridItem.debugObj, Color.white);

                // 寻找下一个点
                OnFindPathCompute();
            }
            else
            {
                // 将目标格子加入到路径中
                pathList.Add(endGrid);
                // 回调
                findPathFinishHandel(pathList);
            }
        }
    }

    /// <summary>
    /// 得到指定点周围的八个点
    /// </summary>
    private void OnGetStartBorderPos()
    {
        // 清空列表
        borderGridList.Clear();

        // 得到周围格子中位置最小点(目标格子相邻左下角的格子)
        float findMinX = starGrid.gridPos.x - gridItemWide;
        float findMinZ = starGrid.gridPos.z - gridItemHigh;

        // 得到周围的格子
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                float x = i * gridItemWide;
                float z = j * gridItemHigh;

                Vector3 nPos = new Vector3(findMinX + x, 0, findMinZ + z);

                // 在格子列表中查找格子
                for (int f = 0; f < gridInfoList.Count; ++f)
                {
                    GridInfo gridItem = gridInfoList[f];
                    // 通过位置找到相邻格子
                    if (gridItem.gridPos == nPos)
                    {
                        // 过滤掉自己和障碍物和寻到的格子
                        if (gridItem != starGrid && !BarrierGridInfoList.Contains(gridItem) && !pathList.Contains(gridItem))
                        {
                            if (isDebug)
                                OnDebugObjColor(gridItem.debugObj, Color.blue);

                            borderGridList.Add(gridItem);
                            break;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 得到相邻格子的移动成本
    /// </summary>
    private void OnGetBorderGridMoveCost(GridInfo startPos_)
    {
        for (int i = 0; i < borderGridList.Count; ++i)
        {
            GridInfo item = borderGridList[i];

            if (item.gridPos.x == startPos_.gridPos.x || item.gridPos.z == startPos_.gridPos.z)
                item.gridPrice = 10;
            else
                item.gridPrice = 14;
        }
    }

    /// <summary>
    /// 得到相邻格子到目标到的长度
    /// </summary>
    private void OnGetBorderGridTargetLength(GridInfo targetGridItem_)
    {
        for (int i = 0; i < borderGridList.Count; ++i)
        {
            GridInfo item = borderGridList[i];

            // ---------曼哈顿估价法(自己的实现)--------- (乘10是为了简化估价计算)
            // PS:计算估价有三种算法：曼哈顿估价法、几何估价法、对角线估价法
            // 曼哈顿估价法和我这里实现是一样，不过有些问题，如果两个格子在同一行（x轴相同），并且中间隔了障碍物。这个时候在起始点会多寻出点
            // 具体原因：是因为查找相邻八个格子顺序问题，导致先找到的格子会被先使用。暂时没有想到更好的办法
            //float xLength = Mathf.Abs(targetGridItem_.gridPos.x - item.gridPos.x) * 10;
            //float zLength = Mathf.Abs(targetGridItem_.gridPos.z - item.gridPos.z) * 10;
            //item.targetLength = (int)(xLength + zLength);
            // ---------曼哈顿估价法(自己的实现)---------


            // ---------对角线估价法(网上的算法)---------
            // 为了解决上面曼哈顿算法的问题和使计算步长更优，这里就使用对角线算法。
            // 当然还有几何算法,但是个人感觉不是最优的，因为对角线是集合了曼哈顿和几何算法思路的。
            // 对角线估价法，暂时还没弄的很明白，乘14和乘10应该也是为了简化计算和计算移动代价没关系
            float cntX = Mathf.Abs(item.gridPos.x - targetGridItem_.gridPos.x);
            float cntZ = Mathf.Abs(item.gridPos.z - targetGridItem_.gridPos.z);

            if (cntX > cntZ)
                item.gridReckon = (int)(14 * cntZ + 10 * (cntX - cntZ));
            else
                item.gridReckon = (int)(14 * cntX + 10 * (cntZ - cntX));
            // ---------对角线估价法(网上的算法)---------

            // 计算权重(因为这里是得到计算权重值的最后一个参数)
            item.gridWeight = item.gridPrice + item.gridReckon;
        }
    }

    /// <summary>
    /// 通过位置得到格子
    /// </summary>
    private GridInfo OnFindGrid(Vector3 pos_)
    {
        GridInfo gridItem = null;

        for (int i = 0; i < gridInfoList.Count; ++i)
        {
            GridInfo info = gridInfoList[i];
            float clickPosX = float.Parse(pos_.x.ToString("#0.0"));
            float clickPosZ = float.Parse(pos_.z.ToString("#0.0"));
            bool xScope = clickPosX >= info.XscopeMin && clickPosX <= info.XscopeMax;
            bool zScope = clickPosZ >= info.ZscopeMin && clickPosZ <= info.ZscopeMax;

            if (xScope && zScope)
            {
                gridItem = info;
                break;
            }
        }

        return gridItem;
    }

    /// <summary>
    /// 调试函数-还原可寻路点颜色
    /// </summary>
    private void OnDebugRestore()
    {
        // 还原可寻路点颜色
        for (int i = 0; i < gridInfoList.Count; ++i)
        {
            bool isRestore = false;
            GridInfo gridItem = gridInfoList[i];

            if(!BarrierGridInfoList.Contains(gridItem))
                OnDebugObjColor(gridItem.debugObj, Color.green);
        }
    }

    /// <summary>
    /// 调试函数-创建模型
    /// </summary>
    private GameObject OnDebugCreatObj(GridInfo gridInfo_)
    {
        GameObject obj = Instantiate(debugModelTemp);
        obj.name = "debugModel";
        obj.SetActive(true);
        obj.transform.SetParent(debugModelParent.transform);
        obj.transform.position = gridInfo_.gridPos;

        return obj;
    }

    /// <summary>
    /// 调试函数-设置模型颜色
    /// </summary>
    private void OnDebugObjColor(GameObject obj_, Color color_)
    {
        obj_.GetComponent<MeshRenderer>().material.color = color_;
    }
}
