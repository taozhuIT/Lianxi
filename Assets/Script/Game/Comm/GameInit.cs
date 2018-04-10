using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameApp
{
    /// <summary>
    /// 游戏入口
    /// </summary>
    public class GameInit : MonoBehaviour
    {
        /// <summary>
        /// 起始
        /// </summary>
        private void Start()
        {
            AppConst.custom = this.gameObject.AddComponent<CustomManager>();
            AppConst.setting = this.gameObject.GetComponent<GameSetting>();
        }
        
        /// <summary>
        /// 更新
        /// </summary>
        private void Update()
        {

        }
    }
}


