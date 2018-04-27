using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 生成AssetBundle包
/// </summary>
public class CreateAssetBundle : Editor
{
    /// <summary>
    /// 资源路径信息
    /// </summary>
    public class ResPathInfo
    {
        // 资源路径
        public string resPath = "";
        // 资源打包路径 (如果当前文件夹下的所有文件需要打成一个整包就使用这个路径，这个路径是在初始就设置好了的)
        public string resBundlePath = "";
        // 是否打成整包 (将同一个文件夹下面的文件打到一个bundle里面)
        public bool isLoosePack = false;
        // 是否打入依赖
        public bool isRely = false;

        /// <summary>
        /// 构造
        /// </summary>
        public ResPathInfo(string resPath_, string resBundlePath_, bool isLoosePack_, bool isRely_)
        {
            resPath = resPath_;
            resBundlePath = resBundlePath_;
            isLoosePack = isLoosePack_;
            isRely = isRely_;
        }
    }

    // 资源路径信息列表
    private static List<ResPathInfo> resPathInfoList = new List<ResPathInfo>();

    [MenuItem("生成AssetBundle/一键生成全部AssetBundle")]
    public static void OnCreateAllAssetBundle()
    {
        OnClearAssetBundlePath();
        OnSetAssetBundlePath();

        // AssetBundel包输出目录
        string outputPath = "Assets/StreamingAssets";

        // 检查是否已经有AssetBundel包文件夹，如果存在先删除
        string ownPackPath = outputPath + "/data";
        if (Directory.Exists(ownPackPath))
            Directory.Delete(ownPackPath, true);

        // 根据BuildSetting里面所激活的平台进行打包  
        BuildPipeline.BuildAssetBundles(outputPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
        AssetDatabase.Refresh();
        Debug.Log("AssetBundle资源打包----完成");    
    }

    [MenuItem("生成AssetBundle/生成--资源路径")]
    public static void OnSetAssetBundlePath()
    {
        OnGetResPathList();

        // 创建路径
        foreach (ResPathInfo pathInfo in resPathInfoList)
            OnSetResAssetLabels(pathInfo);

        AssetDatabase.Refresh();
        Debug.Log("设置资源文件路径----完成");
    }

    /// <summary>
    /// 设置资源AssetLabels
    /// AssetLabels 路径如果一样的资源会打成一个AB包
    /// AssetLabels 路径如果设置成xx/xx/xx生成出来Unity会按照这个路径，自动生成文件夹
    /// </summary>
    public static void OnSetResAssetLabels(ResPathInfo resPathInfo_)
    {
        DirectoryInfo directInfo = new DirectoryInfo(resPathInfo_.resPath);
        FileSystemInfo[] fileInfos = directInfo.GetFileSystemInfos();

        for (int i = 0; i < fileInfos.Length; ++i)
        {
            FileSystemInfo fileInfo = fileInfos[i];
            
            // 递归搜索子目录下面的文件
            if (Directory.Exists(fileInfo.FullName))
            {
                // 子目录的打包路径、打包类型、是否打入依赖，根据父目录设置
                OnSetResAssetLabels(new ResPathInfo(fileInfo.FullName, resPathInfo_.resBundlePath, resPathInfo_.isLoosePack, resPathInfo_.isRely));
            }
            else
            {
                // 过滤文件类型
                if (fileInfo.Extension != ".meta")
                {
                    int index = fileInfo.FullName.IndexOf("Data");
                    string resPathA = fileInfo.FullName.Substring(index, fileInfo.FullName.Length - index);
                    string resPathB = resPathA.Substring(0, resPathA.IndexOf(".")).Replace("\\", "/");
                    string assetLable = resPathInfo_.isLoosePack ? resPathInfo_.resBundlePath : resPathB;

                    //设置Bundle文件的名称/扩展名
                    string getAtPath = "Assets" + fileInfo.FullName.Substring(Application.dataPath.Length);
                    AssetImporter importer = AssetImporter.GetAtPath(getAtPath);
                    importer.assetBundleName = assetLable;     //设置Bundle文件的名称 
                    importer.assetBundleVariant = "asset";     //设置Bundle文件的扩展名    

                    // 是否打入依赖
                    if (resPathInfo_.isRely)
                        fileDependencies(getAtPath);
                }
            }
        }
    }

    [MenuItem("生成AssetBundle/清除--资源路径")]
    public static void OnClearAssetBundlePath()
    {
        OnGetResPathList();

        // 创建路径
        foreach (ResPathInfo pathInfo in resPathInfoList)
            OnClearResAssetLabels(pathInfo);

        AssetDatabase.Refresh();
        Debug.Log("清除资源文件路径----完成");
    }

    /// <summary>
    /// 清除资源AssetLabels
    /// </summary>
    public static void OnClearResAssetLabels(ResPathInfo resPathInfo_)
    {
        DirectoryInfo directInfo = new DirectoryInfo(resPathInfo_.resPath);
        FileSystemInfo[] fileInfos = directInfo.GetFileSystemInfos();

        for (int i = 0; i < fileInfos.Length; ++i)
        {
            FileSystemInfo fileInfo = fileInfos[i];

            // 递归搜索子目录下面的文件
            if (Directory.Exists(fileInfo.FullName))
            {
                // 子目录的打包路径、打包类型、是否打入依赖都使用父目录参数设置
                OnSetResAssetLabels(new ResPathInfo(fileInfo.FullName, resPathInfo_.resBundlePath, resPathInfo_.isLoosePack, resPathInfo_.isRely));
            }
            else
            {
                // 过滤文件类型
                if (fileInfo.Extension != ".meta")
                {
                    int index = fileInfo.FullName.IndexOf("Data");
                    string resPathA = fileInfo.FullName.Substring(index, fileInfo.FullName.Length - index).Replace("\\", "/");
                    string resPathB = resPathA.Substring(0, resPathA.IndexOf("."));
                    string assetLable = resPathInfo_.isLoosePack ? resPathInfo_.resBundlePath : resPathB;

                    //清除Bundle文件的名称/扩展名（这里只需要清除bundleName就可以了，扩展名会自动去除）
                    string getAtPath = "Assets" + fileInfo.FullName.Substring(Application.dataPath.Length).Replace("\\", "/");
                    AssetImporter importer = AssetImporter.GetAtPath(getAtPath);
                    importer.assetBundleName = string.Empty;  // 清除Bundle文件的名称   
                }
            }
        }
    }

    /// <summary>
    /// 打包依赖资源
    /// </summary>
    /// <param name="assetPath"></param>
    static void fileDependencies(string assetPath)
    {
        string[] pathNames = new string[1];
        pathNames[0] = assetPath;
        string[] dps = AssetDatabase.GetDependencies(pathNames);

        for (int i = 0; i < dps.Length; i++)
        {
            string dpPath = dps[i];

            ///对于依赖资源，也只打prefab
            ///png 分两部分，一部分在Atlas中，不用管了，一部分在Texture中，也不用管了（单独打包）
            ///mat 在prefab中
            Debug.Log(dpPath);

            if (!dpPath.EndsWith(".prefab")) continue;

            AssetImporter importer = AssetImporter.GetAtPath(dpPath);
            if (importer.assetBundleName == null || importer.assetBundleName == "")
            {
                string abName = AssetDatabase.AssetPathToGUID(dpPath);
                importer.assetBundleName = abName;
            }
        }
    }

    /// <summary>
    /// 得到资源数据列表
    /// </summary>
    private static void OnGetResPathList()
    {
        resPathInfoList.Clear();

        // 填充资源路径信息列表
        resPathInfoList.Add(new ResPathInfo("Assets/Resassets/Data/Prefab/UI", "", false, true));
        resPathInfoList.Add(new ResPathInfo("Assets/Resassets/Data/AssetConf", "", false, false));
        resPathInfoList.Add(new ResPathInfo("Assets/Resassets/Data/AssetUIAtlas", "", false, false));
        
    }
}
