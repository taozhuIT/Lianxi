using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameApp
{
    /// <summary>
    /// 定制管理器
    /// </summary>
    public class CustomManager : MonoBehaviour
    {
        
        /// <summary>
        /// 定制_创建协程
        /// </summary>
        public Coroutine StartCoroutineShell(IEnumerator function_)
        {
            return StartCoroutine(function_);
        }

        /// <summary>
        /// 定制_停止协程
        /// </summary>
        public void StopCoroutineShell(Coroutine coroutine_)
        {
            StopCoroutine(coroutine_);
        }
    }
}


