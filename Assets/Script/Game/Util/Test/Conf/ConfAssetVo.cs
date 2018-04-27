using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;

/// <summary>
/// 配置生成Asset基类
/// </summary>
[SerializeField]
public abstract class ConfAssetVo : ScriptableObject
{
    /// <summary>
    /// 数据填充
    /// </summary>
    public virtual void OnConfDataFill(JsonData jsonData_) { }

    /// <summary>
    /// 图集填充
    /// </summary>
    public virtual void OnConfDataFill(List<Sprite> atlasData_) { }
}
