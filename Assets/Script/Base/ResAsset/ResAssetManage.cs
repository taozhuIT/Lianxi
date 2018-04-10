using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameResLoad
{
    /// <summary>
    /// 资源管理器
    /// </summary>
    public class ResAssetManage
    {
        private static ResAssetManage resInstace = null;
        private static object locks = new object();

        /// <summary>
        /// 单利模式
        /// </summary>
        /// <returns></returns>
        public static ResAssetManage Instance
        {
            get
            {
                if (resInstace == null)
                {
                    lock (locks)
                    {
                        if (resInstace == null)
                            resInstace = new ResAssetManage();
                    }
                }

                return resInstace;
            }
        }



        // 资源加载池
        public Dictionary<string, ResAssetLoad.ResInfo> loadFileDict = new Dictionary<string, ResAssetLoad.ResInfo>();


        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="fileName_"></param>
        public ResAssetLoad.ResInfo LoadRes(string fileName_, UnityAction<ResAssetLoad.ResInfo> ResFinishHandle_ = null, UnityAction<ResAssetLoad.ResInfo> ResProceedHandle_ = null, UnityAction<ResAssetLoad.ResInfo> ResDefeateHandle_ = null)
        {
            // 检查资源池里有没有
            ResAssetLoad.ResInfo resInfo = null;

            if (loadFileDict.ContainsKey(fileName_))
            {
                resInfo = loadFileDict[fileName_];
                resInfo.ResFinishHandle = ResFinishHandle_;
                resInfo.ResProceedHandle = ResProceedHandle_;
                resInfo.ResDefeateHandle = ResDefeateHandle_;
            }
            else
            {
                resInfo = new ResAssetLoad.ResInfo();
                resInfo.FileName = fileName_;
                resInfo.ResFinishHandle = ResFinishHandle_;
                resInfo.ResProceedHandle = ResProceedHandle_;
                resInfo.ResDefeateHandle = ResDefeateHandle_;

                loadFileDict.Add(fileName_, resInfo);
                // 执行加载
                ResAssetLoad.ResLoad(resInfo);
            }

            return resInfo;
        }
    }
}
