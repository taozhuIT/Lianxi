using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameApp;

namespace GameResLoad
{
    /// <summary>
    /// 资源管理器工具类
    /// </summary>
    public class ResAssetUtil
    {
        /// <summary>
        /// 得到文件路径
        /// </summary>
        /// <returns></returns>
        public static string OnGetFilePath(string filePath_)
        {
            string fullPath = string.Empty;
            
            if (AppConst.isEditor && AppConst.setting.isDebug)
            {
                fullPath = "Assets/Resources/" + filePath_;
            }
            else
            {
                // 当前资源在沙盒目录中是否存在
                bool isSanboxMeet = File.Exists(GetAppSanboxPath + filePath_);
                // 协议头
                string protoTitle = OnGetFileProtoTitle(isSanboxMeet);

                // 如果文件在沙盒目录中存在从沙盒目录加载,否则从程序数据目录加载
                fullPath = isSanboxMeet ? protoTitle + GetAppSanboxPath + filePath_ : protoTitle + GetAppStreamPath + filePath_;
            }

            return fullPath;
        }

        /// <summary>
        /// 得到文件加载协议头
        /// 这个是用于采用WWW来加载资源的时候用的，因为WWW需要确定是加载文件还File://是发送网络消息http://
        ///  AssetBundle.LoadFromFile 也可以加载磁盘上的AB包，这个方法是不需要加协议头的。实际还没验证过
        /// </summary>
        /// <returns></returns>
        public static string OnGetFileProtoTitle(bool isSanbox_)
        {
            string protoTitle = string.Empty;
           
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.OSXEditor:
                    protoTitle = "file:///";
                    break;
                case RuntimePlatform.Android:
                    // 使用www加载，在Android环境的沙盒目录加载需要加协议头，如果是从程序数据目录streamingAssets中加载不需要加协议头了，因为默认已经有了
                    protoTitle = isSanbox_ ? "file://" : ""; 
                    break;
                case RuntimePlatform.IPhonePlayer:
                    protoTitle = "file://";
                    break;
            }

            return protoTitle;
        }

        /// <summary>
        /// 得到程序数据目录路径  streamingAssets文件夹
        /// </summary>
        public static string GetAppStreamPath
        {
            get
            {
                string dataPath = string.Empty;
                // 其实现在都可以简化成一行代码
                dataPath = Application.streamingAssetsPath + "/";

                //switch(Application.platform)
                //{
                //    case RuntimePlatform.WindowsEditor:
                //    case RuntimePlatform.WindowsPlayer:
                //    case RuntimePlatform.OSXEditor:
                //        dataPath = Application.streamingAssetsPath + "/";
                //        break;
                //    case RuntimePlatform.Android:
                //        //unity当前5.0以上的版本Application.streamingAssetsPath在Android环境下得到的路径就是带"jar:file://" + Application.dataPath + "!/assets/"
                //        dataPath = Application.streamingAssetsPath + "/"; 
                //        break;
                //    case RuntimePlatform.IPhonePlayer:
                //        dataPath = Application.streamingAssetsPath + "/";
                //        break;
                //}

                return dataPath;
            }
        }

        /// <summary>
        /// 得到程序沙盒目录路径  persistentData文件夹
        /// </summary>
        public static string GetAppSanboxPath
        {
            get
            {
                string sandboxPath = string.Empty;
                // 其实现在都可以简化成一行代码
                sandboxPath = Application.persistentDataPath + "/";

                //switch (Application.platform)
                //{
                //    case RuntimePlatform.WindowsEditor:
                //    case RuntimePlatform.WindowsPlayer:
                //    case RuntimePlatform.OSXEditor:
                //        sandboxPath = Application.persistentDataPath + "/";
                //        break;
                //    case RuntimePlatform.Android:
                //        sandboxPath = Application.persistentDataPath + "/";
                //        break;
                //    case RuntimePlatform.IPhonePlayer:
                //        sandboxPath = Application.persistentDataPath + "/";
                //        break;
                //}

                return sandboxPath;
            }
        }
    }
}


