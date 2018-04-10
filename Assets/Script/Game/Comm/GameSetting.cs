using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameApp
{
    /// <summary>
    /// 游戏全局设置
    /// </summary>
    public class GameSetting : MonoBehaviour
    {
        // 是否调试模式 (是否是开发模式，直接加载资源文件，不加载AB文件)
        public bool isDebug = false;
        /// <summary>
        /// 是否为开发模式 
        /// (true 从自定义资源文件夹中加载资源)
        /// (false 从streamingAssets或者persistentData中加载资源)
        /// </summary>
        //public static bool IsDebug
        //{
        //    get
        //    {
        //        return IsDebug;
        //    }
        //}
    }
}


