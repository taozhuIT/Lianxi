using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 快速排序工具类
/// </summary>
public class SortTool : MonoBehaviour
{
    
    /// <summary>
    /// 起始
    /// </summary>
	private void Start () {
		
	}

    /// <summary>
    /// 更新
    /// </summary>
    private void Update () {
		
	}

    /// <summary>
    /// 快速排序
    /// </summary>
    public void fastSortTool(List<int> dataList_)
    {
        if (dataList_.Count > 0)
        {
            // 排序基准数
            int basicsVal = dataList_[0];

            // 因为基准数取的第一个，所以第一步先从后面查找
            for (int j = (dataList_.Count - 1); j > -1; --j)
            {
                // 从后往前查小于基准数的索引
                int lessIndex = 0;
                // 从前往后查大于基准数的索引
                int greaIndex = 0;

                int jVal = dataList_[j];
                if (jVal < basicsVal)
                {
                    // 设置索引
                    lessIndex = j;

                    // 再前往后查
                    for (int i = 0; i < dataList_.Count; ++i)
                    {
                        int iVal = dataList_[i];
                        if (jVal > basicsVal)
                        {
                            greaIndex = i;
                            break;
                        }
                    }
                }

                // 交换位置
                int tempVal = dataList_[lessIndex];
                dataList_[lessIndex] = dataList_[greaIndex];
                dataList_[greaIndex] = tempVal;
            }
        }
    }
}
