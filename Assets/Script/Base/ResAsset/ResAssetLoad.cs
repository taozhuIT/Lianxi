using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif
using GameApp;

namespace GameResLoad
{
    /// <summary>
    /// 资源加载器
    /// </summary>
    public class ResAssetLoad
    {
        /// <summary>
        /// 加载的资源数据
        /// </summary>
        public class ResInfo
        {
            // 文件名称
            private string fileName = null;
            // 文件路径
            private string filePath = null;
            // 加载的AB包
            private AssetBundle bundle = null;
            // 加载后生成的Obj文件
            private Object gameObj = null;

            // 资源加载完成回调
            public UnityAction<ResInfo> ResFinishHandle;
            // 资源加载失败回调
            public UnityAction<ResInfo> ResDefeateHandle;
            // 资源加载中回调
            public UnityAction<ResInfo> ResProceedHandle;

            /// <summary>
            /// 设置/获取文件路径
            /// </summary>
            public string FileName
            {
                set { fileName = value; }
                get { return fileName; }
            }

            /// <summary>
            /// 设置/获取文件路径
            /// </summary>
            public string FilePath
            {
                set { filePath = value; }
                get { return filePath; }
            }

            /// <summary>
            /// 设置/获取文件路径
            /// </summary>
            public AssetBundle Bundle
            {
                set { bundle = value; }
                get { return bundle; }
            }

            /// <summary>
            /// 设置/获取文件路径
            /// </summary>
            public Object GameObj
            {
                set { gameObj = value; }
                get { return gameObj; }
            }
        }

        /// <summary>
        /// 资源加载
        /// </summary>
        public static void ResLoad(ResInfo resInfo_)
        {
            OnBaseLoad(resInfo_);
        }

        /// <summary>
        /// 加载基础
        /// </summary>
        private static void OnBaseLoad(ResInfo resInfo_)
        {
            resInfo_.FilePath = ResAssetUtil.OnGetFilePath(resInfo_.FileName);

            // 是否是调试模式
            if (AppConst.isEditor && AppConst.setting.isDebug)
            {
                OnDebugAssetLoad(resInfo_);
            }
            else
            {
                // 这里还要处理依赖文件加载问题

                AppConst.custom.StartCoroutineShell(AssetLoad(resInfo_));
            }
        }

        /// <summary>
        /// 调试模式资源加载
        /// </summary>
        private static void OnDebugAssetLoad(ResInfo resinfo_)
        {
#if UNITY_EDITOR
            resinfo_.GameObj = AssetDatabase.LoadMainAssetAtPath(resinfo_.FilePath);
            if (resinfo_.GameObj != null)
            {
                // 成功
                resinfo_.ResFinishHandle(resinfo_);
            }
            else
            {
                resinfo_.ResDefeateHandle(resinfo_);
            }
#endif
        }

        /// <summary>
        /// 正式环境资源加载
        /// </summary>
        /// <returns></returns>
        private static IEnumerator AssetLoad(ResInfo resInfo_)
        {
            WWW wwwLoad = new WWW(resInfo_.FilePath);
            yield return wwwLoad;

            if(string.IsNullOrEmpty(wwwLoad.error))
            {
                while (!wwwLoad.isDone)
                {
                    resInfo_.ResProceedHandle(resInfo_);
                }

                if(wwwLoad.isDone)
                {
                    resInfo_.GameObj = wwwLoad.assetBundle.LoadAsset(resInfo_.FileName);
                    resInfo_.ResFinishHandle(resInfo_);

                    // 是否需要保存AssetBundle文件
                    if (false)
                        resInfo_.Bundle = wwwLoad.assetBundle;
                    else
                        wwwLoad.assetBundle.Unload(false);
                }
            }
            else
            {
                resInfo_.ResDefeateHandle(resInfo_);
            }
        }
    }
}
