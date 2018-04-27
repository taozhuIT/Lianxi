using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 测试加载UI界面
/// </summary>
public class OpenPanel : MonoBehaviour
{
    // 加载面板
    [SerializeField] private Button mainPanel;
    // 加载面板2
    [SerializeField] private Button heroPanel;
    // 加载序列化资源
    [SerializeField] private Button seriaRes;

    // 加载资源1001001
    [SerializeField] private Button equipRes_1;
    // 加载资源1001002
    [SerializeField] private Button equipRes_2;
    // 加载资源1001003
    [SerializeField] private Button equipRes_3;
    // 资源1001001定位
    [SerializeField] private Transform equipPos_3;

    /// <summary>
    /// 初始
    /// </summary>
    private void Start ()
    {
        // 将StreamingAssets文件夹下的AB包，拷贝到沙盒目录
        Debug.Log("拷贝开始");
        CopyDir(Application.streamingAssetsPath, Application.persistentDataPath);
        Debug.Log("拷贝结束");

        seriaRes.onClick.AddListener(OnClickLoadSeria);
        mainPanel.onClick.AddListener(OnClickLoadUIPanel);
        heroPanel.onClick.AddListener(OnClickLoadUI2Panel);
        
        equipRes_1.onClick.AddListener(OnClickLoadEquip1);
        equipRes_2.onClick.AddListener(OnClickLoadEquip2);
        equipRes_3.onClick.AddListener(OnClickLoadEquip3);
    }

    /// <summary>
    /// UI界面
    /// </summary>
    private void OnClickLoadUIPanel()
    {
        GameObject obj = Instantiate(OnLoadUIPanelObj("TrainMainPanel")) as GameObject;
        obj.name = "TrainMainPanel";
        obj.transform.SetParent(this.transform);
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        obj.transform.SetSiblingIndex(0);
    }

    /// <summary>
    /// UI界面
    /// </summary>
    private void OnClickLoadUI2Panel()
    {
        GameObject obj = Instantiate(OnLoadUIPanelObj("TrainHeroPanel")) as GameObject;
        obj.name = "TrainHeroPanel";
        obj.transform.SetParent(this.transform);
        obj.transform.localScale = new Vector3(1f, 1f, 1f);
        obj.transform.SetSiblingIndex(1);
    }

    /// <summary>
    /// 装备1001001
    /// </summary>
    private void OnClickLoadEquip1()
    {
        AtlasVo atlasItem = OnLoadUIAtlasAsset<AtlasVo>("equip"); // 参数填写asset资源名
        Sprite sprite = atlasItem.OnGetSprite("1001001");
        equipPos_3.GetComponent<Image>().sprite = sprite;
    }

    /// <summary>
    /// 装备1001002
    /// </summary>
    private void OnClickLoadEquip2()
    {
        AtlasVo atlasItem = OnLoadUIAtlasAsset<AtlasVo>("equip");   // 参数填写asset资源名
        Sprite sprite = atlasItem.OnGetSprite("1001002");
        equipPos_3.GetComponent<Image>().sprite = sprite;
    }

    /// <summary>
    /// 装备1001003
    /// </summary>
    private void OnClickLoadEquip3()
    {
        AtlasVo atlasItem = OnLoadUIAtlasAsset<AtlasVo>("equip"); // 参数填写asset资源名
        Sprite sprite = atlasItem.OnGetSprite("1001003");
        equipPos_3.GetComponent<Image>().sprite = sprite;
    }

    /// <summary>
    /// 加载序列化资源
    /// </summary>
    private void OnClickLoadSeria()
    {
        AchieveVo achieve = OnLoadConfAsset<AchieveVo>("AchieveVo"); // 参数填写asset资源名
        for (int i = 0; i < achieve.achieveList.Count; ++i)
            Debug.Log(achieve.achieveList[i].q_id);
    }



    // ------------------------------测试使用的资源管理------------------------------
    // 需要常驻内存的bundle文件列表 (公用资源，装备icon，道具icon)
    private Dictionary<string, AssetBundle> leaveBundleDict = new Dictionary<string, AssetBundle>();
    // 文件依赖资源Bundle
    private Dictionary<string, AssetBundle> DependenciesBundleDict = new Dictionary<string, AssetBundle>();
    // 1.加载Manifest文件  
    private AssetBundleManifest manifest = null;

    /// <summary>
    /// 加载UI面板
    /// </summary>
    private UnityEngine.Object OnLoadUIPanelObj(string name_)
    {
        string path = "Data/Prefab/UI/" + name_;

        return OnGetResObj(path, name_, true);
    }

    /// <summary>
    /// 加载装备/道具
    /// </summary>
    private UnityEngine.Object OnLoadEquipObj(string name_)
    {
        string path = "Data/Prefab/Equip";
        
        return OnGetResObj(path, name_, false);
    }

    /// <summary>
    /// 加载json序列化配置
    /// </summary>
    /// <returns></returns>
    private T OnLoadConfAsset<T>(string name_) where T : ScriptableObject 
    {
        string path = "Data/AssetConf/" + name_;
        return OnGetResObj(path, name_, false) as T;
    }

    /// <summary>
    /// 加载图集序列化配置
    /// </summary>
    /// <returns></returns>
    private T OnLoadUIAtlasAsset<T>(string name_) where T : ScriptableObject
    {
        string path = "Data/AssetUIAtlas/" + name_;
        return OnGetResObj(path, name_, false) as T;
    }

    /// <summary>
    /// 得到AB包
    /// </summary>
    /// <param name="path_"></param>
    /// <param name="resName_"></param>
    /// <param name="isUnLoad_">是否需要卸载assetBundle压缩文件</param>
    /// <returns></returns>
    private UnityEngine.Object OnGetResObj(string path_, string resName_, bool isUnLoad_)
    {
        string loadPath = Application.persistentDataPath + "/" + path_ + ".asset";

        Debug.Log(loadPath);

        // 加载依赖
        OnGetResDependencies(path_ + ".asset");

        // 查找AB包池中有没有
        AssetBundle bundle = null;
        if (leaveBundleDict.ContainsKey(path_))
            bundle = leaveBundleDict[path_];
        else
            bundle = AssetBundle.LoadFromFile(loadPath);

        // 得到资源类型
        UnityEngine.Object obj = bundle.LoadAsset(resName_);

        // 是否需要卸载内存中AssetBundle压缩文件，节约内存
        if (isUnLoad_)
            bundle.Unload(false);
        else if (!leaveBundleDict.ContainsKey(path_))
            leaveBundleDict.Add(path_, bundle);
        
        return obj;
    }

    /// <summary>
    /// 加载资源依赖
    /// </summary>
    private void OnGetResDependencies(string abResPath_)
    {
        // PS:关于依赖自己的一些理解。依赖的bundle文件加载后不能卸载，否则依赖关系丢失。并且同样的依赖的bundle文件只能加载一份，不然再次加载会报错

        // 这是个恶心的问题。考虑一种情况，你有3个AssetBundle：A保存Prefab，B保存Mesh，C保存Texture和Material。依赖关系是A->B->C。于是你依次载入C,B,A，再把A里的Prefab给Instantiate一个，就会得到完整的GameObject。这时，你想这仨AssetBundle不要了卸了吧。卸载完，你发现Mesh,Texture,Material都没了。因为B,C里的资源是都被依赖的，但是卸载时缺没有被考虑到。但是你要是将B,C的东西也Instantiate各一份出来，依赖关系就没了，只能自己添加。

        // 所以，解决思路是在打包资源时最好将相关资源打包成一个AssetBundle (比如：一个角色模型的prefab、mesh、材质、纹理都打成一个bundle包)。如果实在避免不了，那么就尽量避免多个文件依赖同一个文件，最好它自己依赖自己独有的依赖文件。这样自己可以维护一个表，在文件卸载的时候。同时卸载它依赖的bundle包

        // 1.加载Manifest文件  
        if (manifest == null)
        {
            AssetBundle manifestBundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/StreamingAssets");
            manifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");

            manifestBundle.Unload(false);
        }
            
        if (manifest != null)
        {
            // 2.获取依赖文件列表  
            string[] cubedepends = manifest.GetAllDependencies(abResPath_);
            for (int index = 0; index < cubedepends.Length; index++)
            {
                string dependPath = cubedepends[index];
                Debug.Log(dependPath);

                // PS:依赖资源压缩文件不能卸载
                // 依赖资源是否已经加载
                if(!DependenciesBundleDict.ContainsKey(dependPath))
                {
                    AssetBundle dependsAssetbundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/" + dependPath);
                    DependenciesBundleDict.Add(dependPath, dependsAssetbundle);
                }
            }
        }
    }

    // ------------------------------测试使用的资源管理------------------------------


    /// <summary>
    /// 复制AB包到应用程序沙盒文件夹
    /// </summary>
    /// <param name="sourceFolderName">源文件夹目录</param>
    /// <param name="destFolderName">目标文件夹目录</param>
    /// <param name="overwrite">允许覆盖文件</param>
    public static void CopyDir(string sourceFolderName, string destFolderName)
    {
        try
        {
            DirectoryInfo directInfo = new DirectoryInfo(sourceFolderName);
            //获取目录下（不包含子目录）的文件和子目录
            FileSystemInfo[] fileinfos = directInfo.GetFileSystemInfos();  

            foreach (FileSystemInfo fileInfo in fileinfos)
            {
                // 得到子文件/子文件夹路径
                string childPath = destFolderName + "\\" + fileInfo.Name;

                // 判断是否文件夹
                if (Directory.Exists(fileInfo.FullName))     
                {
                    // 目标目录下不存在此文件夹即创建子文件夹
                    if (!Directory.Exists(childPath))
                        Directory.CreateDirectory(childPath);

                    // 递归调用复制子文件夹
                    CopyDir(fileInfo.FullName, childPath);    
                }
                else
                {
                    // 不是文件夹即复制文件，true表示可以覆盖同名文件
                    File.Copy(fileInfo.FullName, childPath, true);      
                }
            }
        }
        catch (Exception e)
        {
            throw;
        }
    }
}
