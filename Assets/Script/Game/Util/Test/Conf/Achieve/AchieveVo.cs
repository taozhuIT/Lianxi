using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using LitJson;


/// <summary>
/// 活动配置数据
/// </summary>
[Serializable]
public class AchieveVo : ConfAssetVo
{
    // 活动数据列表
    public List<AchieveItemVo> achieveList = new List<AchieveItemVo>();
    // 活动数据索引
    private Dictionary<int, AchieveItemVo> achieveDict = new Dictionary<int, AchieveItemVo>();

    // 筛选标准
    private static BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// 数据填充
    /// </summary>
    public override void OnConfDataFill(JsonData jsonData_)
    {
        for (int i = 0; i < jsonData_.Count; ++i)
        {
            AchieveItemVo itemVo = new AchieveItemVo();
            JsonData dataItem = jsonData_[i];

            foreach (KeyValuePair<string, JsonData> KV in dataItem)
            {
                string key = KV.Key.ToString();
                var value = KV.Value.ToString();

                // 反射
                FieldInfo fi = itemVo.GetType().GetField(key, flags);
                if (fi != null)
                {
                    if (fi.FieldType.Equals(typeof(int)))
                    {
                        int intData;
                        int.TryParse(value, out intData);
                        fi.SetValue(itemVo, intData);
                    }
                    else if (fi.FieldType.Equals(typeof(long)))
                    {
                        long longData;
                        long.TryParse(value, out longData);
                        fi.SetValue(itemVo, longData);
                    }
                    else if (fi.FieldType.Equals(typeof(bool)))
                    {
                        bool boolData;
                        bool.TryParse(value, out boolData);
                        fi.SetValue(itemVo, boolData);
                    }
                    else if (fi.FieldType.Equals(typeof(double)))
                    {
                        double doubleData;
                        double.TryParse(value, out doubleData);
                        fi.SetValue(itemVo, doubleData);
                    }
                    else if (fi.FieldType.Equals(typeof(float)))
                    {
                        float floatData;
                        float.TryParse(value, out floatData);
                        fi.SetValue(itemVo, floatData);
                    }
                    else if (fi.FieldType.Equals(typeof(short)))
                    {
                        short shortData;
                        short.TryParse(value, out shortData);
                        fi.SetValue(itemVo, shortData);
                    }
                    else
                    {
                        fi.SetValue(itemVo, value);
                    }
                }
            }

            achieveList.Add(itemVo);
            achieveDict.Add(itemVo.q_id, itemVo);
        }

        for (int i = 0; i < achieveList.Count; ++i)
        {
            Debug.Log(achieveList[i].q_id);
        }
    }
}
