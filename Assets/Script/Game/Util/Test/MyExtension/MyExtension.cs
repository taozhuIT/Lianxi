using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyExtension
{
    public static class MyExtensionCustion
    {
        /// <summary>
        /// GameObject自定义扩展方法(测试)
        /// </summary>
        /// <param name="target"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static int OnAngleTao(this GameObject target, float angle)
        {
            return 123;
        }

    }
}

