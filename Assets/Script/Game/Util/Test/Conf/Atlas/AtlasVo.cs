using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

/// <summary>
/// 图集配置数据
/// </summary>
[Serializable]
public class AtlasVo : ConfAssetVo
{
    // 图集列表
    public List<Sprite> atlasList = new List<Sprite>();
    // 图集索引
    private Dictionary<string, Sprite> atlasDict = new Dictionary<string, Sprite>();

    /// <summary>
    /// 填充数据
    /// </summary>
    /// <param name="atlasData_"></param>
    public override void OnConfDataFill(List<Sprite> atlasData_)
    {
        base.OnConfDataFill(atlasData_);

        for (int i = 0; i < atlasData_.Count; ++i)
        {
            Sprite sprite = atlasData_[i];

            atlasList.Add(sprite);
            atlasDict.Add(sprite.name, sprite);
        }
    }

    /// <summary>
    /// 得到纹理
    /// </summary>
    /// <returns></returns>
    public Sprite OnGetSprite(string name_)
    {
        if(atlasDict.Count <= 0)
        {
            for (int i = 0; i < atlasList.Count; ++i)
            {
                Sprite sprite = atlasList[i];
                atlasDict.Add(sprite.name, sprite);
            }
        }


        return atlasDict[name_];
    }
}
