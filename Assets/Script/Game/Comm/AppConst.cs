using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameApp
{
    /// <summary>
    /// 游戏常量
    /// </summary>
    public class AppConst
    {
        // 是否在编辑器环境下
        public static bool isEditor = Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor;

        // 游戏设置
        public static GameSetting setting = null;
        // 定制管理器
        public static CustomManager custom = null;
    }
}
