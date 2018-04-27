using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A* 寻路
/// </summary>
public class CreatGridView : MonoBehaviour
{
    // 创建格子参数
    // 格子模板
    [SerializeField] private GameObject objTemp;
    // 格子根节点
    [SerializeField] private GameObject parent;
    // 格子横向数量
    public int xNum = 0;
    // 格子纵向数量
    public int yNum = 0;
    // 格子间隔
    public float space = 0;
    // 物件相机
    [SerializeField]
    private Camera CameraRay;

    // UI
    // 起始点名称
    [SerializeField] private Text StartPosName;
    // 目标点名称
    [SerializeField] private Text TargetPosName;
    // 寻路按钮
    [SerializeField] private Button FindPathBtn;
    // 清除点按钮
    [SerializeField] private Button ClearPosBtn;

    // 寻路参数
    // 格子信息
    private class gridItem
    {
        // 格子位置
        public Vector3 gridPos = Vector3.zero;
        // 格子
        public GameObject gridObj = null;
        // 上一个格子到自己的移动成本
        public int moveCost = 0;
        // 自己到目标点的步数 (自己当前的位置只通过横向+纵向移动到目标点的步数)
        public int targetLength = 0;
        // 寻路权重值(moveCost + targetLength)
        public int weight = 0;

        /// <summary>
        /// 构造
        /// </summary>
        public gridItem(Vector3 pos_, GameObject obj_)
        {
            gridPos = pos_;
            gridObj = obj_;
        }
    }

    // 起始点
    private GameObject startPosObj = null;
    // 目标点
    private GameObject TargetPosObj = null;
    // 相邻格子
    private List<gridItem> borderGridList = new List<gridItem>();
    // 路径集合
    private Dictionary<GameObject, gridItem> pathList = new Dictionary<GameObject, gridItem>();
    // 格子键值对
    private Dictionary<GameObject, gridItem> gridDict = new Dictionary<GameObject, gridItem>();

    // 障碍物
    [SerializeField] private GameObject gridBarrier;
    // 障碍物格子列表
    private Dictionary<GameObject, gridItem> barrierGridDict = new Dictionary<GameObject, gridItem>();
    

    /// <summary>
    /// 初始
    /// </summary>
	private void Start ()
    {
        // 注册按钮事件
        FindPathBtn.onClick.AddListener(OnClickFindPath);
        ClearPosBtn.onClick.AddListener(OnClickClearPos);

        OnCreatGrid();
    }

    /// <summary>
    /// 更新
    /// </summary>
    private void Update ()
    {
        // 鼠标抬起
        if(Input.GetMouseButtonUp(0))
            OnSelectPos();
	}

    /// <summary>
    /// 创建格子 
    /// </summary>
    private void OnCreatGrid()
    {
        for (int i = 0; i<xNum; ++i)
        {
            for (int j = 0; j < yNum; ++j)
            {
                float posX = i + (i * space);
                float posY = 0;
                float posZ = j + (j * space);

                GameObject gridObj = GameObject.Instantiate(objTemp);
                gridObj.name = "Grid   X=" + posX + " Z=" + posZ;
                gridObj.layer = LayerMask.NameToLayer("Grid");
                gridObj.gameObject.SetActive(true);
                gridObj.transform.SetParent(parent.transform);
                gridObj.transform.position = new Vector3(posX, posY, posZ);
                
                gridDict[gridObj] = new gridItem(new Vector3(posX, posY, posZ), gridObj);
            }
        }
    }

    /// <summary>
    /// 选择起始点目标点
    /// </summary>
    private void OnSelectPos()
    {
        Ray ray = CameraRay.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;

        if (Physics.Raycast(ray, out rayHit, 1 << LayerMask.NameToLayer("Grid")))
        {
            if (rayHit.collider != null)
            {
                // 是否已经选了起始点
                if (startPosObj == null)
                    startPosObj = rayHit.collider.gameObject;
                else
                    TargetPosObj = rayHit.collider.gameObject;

                OnRefreshUI();
            }
        }
    }

    /// <summary>
    /// 开始寻路
    /// </summary>
    private void OnClickFindPath()
    {
        if (startPosObj != null && TargetPosObj != null)
        {
            // 添加障碍物格子
            OnGetBarrierGrid();
            // 将起始点加入路径列表中
            OnAddPaht(gridDict[startPosObj]);
            OnPlayerFindPath();
        }
        else
        {
            Debug.LogError("请添加一个起始点或者目标点");
        }
    }

    /// <summary>
    /// 执行寻路
    /// </summary>
    private void OnPlayerFindPath()
    {
        // 清空相邻格子
        borderGridList.Clear();

        gridItem startGridItem = gridDict[startPosObj];
        gridItem targetGridItem = gridDict[TargetPosObj];
        // 得到八个相邻格子
        OnGetStartBorderPos(startGridItem);
        // 得到相邻格子的移动成本
        OnGetBorderGridMoveCost(startGridItem);
        // 得到相邻格子到目标到的步长
        OnGetBorderGridTargetLength(targetGridItem);

        // 得到下一个移动点(权重最小的格子)
        gridItem newStarGridItem = null;
        for (int i = 0; i < borderGridList.Count; ++i)
        {
            gridItem item = borderGridList[i];

            //Debug.Log("权重 " + item.weight);
            if (newStarGridItem != null)
            {
                if (item.weight < newStarGridItem.weight)
                    newStarGridItem = item;
            }
            else
            {
                newStarGridItem = item;
            }
        }

        //Debug.Log("权重最小 "+ newStarGridItem.gridObj.name +" " + newStarGridItem.weight);

        // 将新的路径点加入路径列表并更新起始点
        if (newStarGridItem != null)
        {
            // ------------判断是否需要继续寻路------------
            bool isFindPath = true;
            for (int i = 0; i < borderGridList.Count; ++i)
            {
                gridItem item = borderGridList[i];
                if (item.gridObj == targetGridItem.gridObj)
                {
                    isFindPath = false;
                    break;
                }
            }

            if (isFindPath)
            {
                startPosObj = newStarGridItem.gridObj;
                OnAddPaht(newStarGridItem);
                // 将路径格子改变颜色(调试使用功能)
                OnSetModelMaterColor(newStarGridItem.gridObj, Color.gray);

                OnPlayerFindPath();
            }
                
            // ------------判断是否需要继续寻路------------
        }
    }

    /// <summary>
    /// 得到指定点周围的八个点
    /// </summary>
    private void OnGetStartBorderPos(gridItem gridItem_)
    {
        // 得到周围格子中位置最小点(目标格子相邻左下角的格子)
        float findMinX = gridItem_.gridPos.x - (1 + space);
        float findMinZ = gridItem_.gridPos.z - (1 + space);

        // 得到周围的格子
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < 3; ++j)
            {
                float x = i * (1 + space);
                float z = j * (1 + space);

                Vector3 nPos = new Vector3(findMinX + x, 0, findMinZ + z);
                
                // 在格子列表中查找格子
                gridItem gridItem = null;
                foreach(KeyValuePair<GameObject, gridItem> val in gridDict)
                {
                    // 通过位置找到相邻格子
                    if (val.Value.gridPos == nPos)
                    {
                        // 过滤掉自己和障碍物和寻到的格子
                        if (val.Value.gridObj != startPosObj && !barrierGridDict.ContainsKey(val.Value.gridObj) && !pathList.ContainsKey(val.Value.gridObj))
                        {
                            gridItem = val.Value;
                            break;
                        }
                    }
                }

                // 如果格子存在，填充到零时列表中
                if (gridItem != null)
                {
                    //Debug.Log("找到 " + nPos);
                    borderGridList.Add(gridItem);
                }
                else
                {
                    //Debug.Log("没有找到 " + nPos);
                }
            }
        }
    }

    /// <summary>
    /// 得到相邻格子的移动成本
    /// </summary>
    private void OnGetBorderGridMoveCost(gridItem startPos_)
    {
        for (int i = 0; i < borderGridList.Count; ++i)
        {
            gridItem item = borderGridList[i];

            if (item.gridPos.x == startPos_.gridPos.x || item.gridPos.z == startPos_.gridPos.z)
            {
                //Debug.Log("成本小 " + item.gridObj.name);
                item.moveCost = 10;
            }
            else
            {
                //Debug.Log("成本大 " + item.gridObj.name);
                item.moveCost = 14;
            }
        }
    }

    /// <summary>
    /// 得到相邻格子到目标到的长度
    /// </summary>
    private void OnGetBorderGridTargetLength(gridItem targetGridItem_)
    {
        for (int i = 0; i < borderGridList.Count; ++i)
        {
            gridItem item = borderGridList[i];

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
                item.targetLength = (int)(14 * cntZ + 10 * (cntX - cntZ));
            else
                item.targetLength = (int)(14 * cntX + 10 * (cntZ - cntX));
            // ---------对角线估价法(网上的算法)---------

            // 计算权重(因为这里是得到计算权重值的最后一个参数)
            item.weight = item.moveCost + item.targetLength;
            //Debug.Log(item.gridObj.name + " 步长 " + item.targetLength);
        }
    }

    /// <summary>
    /// 添加格子到路径列表中
    /// </summary>
    private void OnAddPaht(gridItem pathGrid_)
    {
        // 加入新的路径
        pathList[pathGrid_.gridObj] = pathGrid_;
    }

    /// <summary>
    /// 得到障碍物格子
    /// </summary>
    private void OnGetBarrierGrid()
    {
        // 清空障碍物格子列表
        barrierGridDict.Clear();

        GridBarrierView[] barrierList = gridBarrier.transform.GetComponentsInChildren<GridBarrierView>();
        for(int i = 0; i < barrierList.Length; ++i)
        {
            GridBarrierView gridBarrierItem = barrierList[i];
            for (int j = 0; j < gridBarrierItem.barrierGridLIST.Count; ++j)
            {
                GameObject obj = gridBarrierItem.barrierGridLIST[j];
                barrierGridDict[obj] = gridDict[obj];
            }
        }

        //Debug.Log("障碍物 " + barrierGridDict.Count);
    }

    /// <summary>
    /// 清除点
    /// </summary>
    private void OnClickClearPos()
    {
        if (startPosObj != null)
            OnSetModelMaterColor(startPosObj, Color.white);
        if (TargetPosObj != null)
            OnSetModelMaterColor(TargetPosObj, Color.white);

        foreach(KeyValuePair<GameObject, gridItem> val in pathList)
            OnSetModelMaterColor(val.Value.gridObj, Color.white);
            
        pathList.Clear();

        startPosObj = null;
        TargetPosObj = null;
        OnRefreshUI();
    }

    /// <summary>
    /// 刷新UI显示
    /// </summary>
    private void OnRefreshUI()
    {
        string startName = "未选择";
        string targetName = "未选择";

        if (startPosObj != null)
        {
            startName = startPosObj.name;
            OnSetModelMaterColor(startPosObj, Color.green);
        }

        if (TargetPosObj != null)
        {
            targetName = TargetPosObj.name;
            OnSetModelMaterColor(TargetPosObj, Color.red);
        }

        StartPosName.text = startName;
        TargetPosName.text = targetName;
    }

    /// <summary>
    /// 工具方法-设置模型颜色
    /// </summary>
    private void OnSetModelMaterColor(GameObject obj_, Color color_)
    {
        obj_.GetComponent<MeshRenderer>().material.color = color_;
    }
}
