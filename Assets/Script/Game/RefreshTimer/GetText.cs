using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/// <summary>
/// C# 提取字符串中的中文词语
/// </summary>
public class GetText : MonoBehaviour
{
    void Start ()
    {
        string s = @"C# Aggh从入 qq门到11精通";
        Regex reg = new Regex("[\u4e00-\u9fa5]+");
        foreach (Match v in reg.Matches(s))
            Debug.Log(v);
    }
	
	void Update ()
    {
	}
}
