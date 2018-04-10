using System.IO;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using GameResLoad;

/// <summary>
/// 资源加载/拷贝沙盒加载
/// </summary>
public class ziyuantihuan : MonoBehaviour
{
    // 进入界面背景
    [SerializeField] private Image longBackObj;
    // 进入界面背景
    [SerializeField] private Text debugObj;
    // 进入界面背景
    [SerializeField] private Button loadStartBtn;
    // 进入界面背景
    [SerializeField] private Button loadStartBtn2;

    // 是否已经从沙盒中读取过
    private bool isSandboxLoad = false;

    /// <summary>
    /// 初始
    /// </summary>
    private void Awake()
    {
        loadStartBtn.onClick.AddListener(OnLoadResSandbox);
        loadStartBtn2.onClick.AddListener(OnLoadRes);
    }

    /// <summary>
    /// 起始
    /// </summary>
	private void Start ()
    {
        //OnResCopy();
    }

    /// <summary>
    /// 更新
    /// </summary>
    private void Update ()
    {

    }

    /// <summary>
    /// 得到应用程序平台沙盒资源加载
    /// </summary>
    /// <returns></returns>
    private void OnLoadResSandbox()
    {
        ResAssetManage.Instance.LoadRes("loginBg.png", LoadResFinishHandler, null, LoadResErrorHandler);
        //StartCoroutine(LoadRes(OnGetResPath(true), "loginBg.png", LoadResFinishHandler, LoadResErrorHandler));
    }

    /// <summary>
    /// 得到应用程序不同资源加载
    /// </summary>
    /// <returns></returns>
    private void OnLoadRes()
    {
        //StartCoroutine(LoadRes(OnGetResPath(), "loginBg.png", LoadResFinishHandler, LoadResErrorHandler));
    }

    ///// <summary>
    ///// 资源拷贝
    ///// </summary>
    //private void OnResCopy()
    //{
    //    if (Application.isMobilePlatform)
    //    {
    //        OnWartDebug("移动平台资源拷贝开始");
    //        StartCoroutine(LoadRes(OnGetResPath(), "loginBg.png", OnMobileCopy, LoadResErrorHandler));
    //    }
    //    else
    //    {
    //        OnWartDebug("编辑器资源拷贝开始");
    //        string sourceFile = @Application.streamingAssetsPath + "/loginBg.png";
    //        string destinationFile = @Application.persistentDataPath + "/loginBg.png";
    //        // 拷贝StreamingAssets中的文件到游戏程序沙盒目录中
    //        OnWartDebug("sourceFile  " + sourceFile + "    destinationFile  " + destinationFile);

    //        File.Copy(sourceFile, destinationFile, true);
    //        File.Delete(sourceFile);
    //        OnWartDebug("编辑器资源拷贝完成");
    //    }
    //}

    ///// <summary>
    ///// 移动平台下将streamingAssetsPath 拷贝到 persistentDataPath
    ///// </summary>
    //private void OnMobileCopy(object obj_)
    //{
    //    string destinationFile = @Application.persistentDataPath + "/loginBg.png";
    //    OnWartDebug("destinationFile  " + destinationFile);
    //    if (!File.Exists(destinationFile))
    //    {
    //        WWW load = obj_ as WWW;

    //        FileStream fsDes = File.Create(destinationFile);
    //        fsDes.Write(load.bytes, 0, load.bytes.Length);
    //        fsDes.Flush();
    //        fsDes.Close();
    //        OnWartDebug("移动平台资源拷贝完成");
    //    }
    //    else
    //    {
    //        OnWartDebug("移动平台资源已存在");
    //    }
    //}

    /// <summary>
    /// 资源完成加载成功回调
    /// </summary>
    private void LoadResFinishHandler(ResAssetLoad.ResInfo obj_)
    {
        OnWartDebug(" 资源加载成功");

        ResAssetLoad.ResInfo load = obj_ as ResAssetLoad.ResInfo;
        Texture2D textAsset = load.GameObj as Texture2D;
        if (textAsset != null)
        {
            Sprite longBack = Sprite.Create(textAsset, new Rect(0, 0, textAsset.width, textAsset.height), Vector2.one);
            longBackObj.sprite = longBack;
        }
        else
        {
            OnWartDebug(" 资源不存在");
        }
    }

    /// <summary>
    /// 资源完成加载失败回调
    /// </summary>
    private void LoadResErrorHandler(ResAssetLoad.ResInfo obj_)
    {
        OnWartDebug("资源加载失败");
    }

    ///// <summary>
    ///// 加载资源
    ///// </summary>
    ///// <param name="resPath_">资源根路径</param>
    ///// <param name="filePath_">文件路径</param>
    ///// <returns></returns>
    //private IEnumerator LoadRes(string resPath_, string filePath_, UnityAction<object> finishHandler_, UnityAction<string> errorHandler_)
    //{
    //    OnWartDebug(resPath_ + filePath_);
    //    WWW load = new WWW(resPath_ + filePath_);
    //    yield return load;

    //    if(load != null)
    //    {
    //        if(load.error == null)
    //            finishHandler_(load);
    //        else
    //            errorHandler_("资源加载失败");
    //    }
    //    else
    //    {
    //        errorHandler_("资源不存在");
    //    }
    //}

    ///// <summary>
    ///// 得到应用程序内资源路径
    ///// </summary>
    ///// <param name="sandboxLoad_">是否从沙盒目录加载</param>
    ///// <returns></returns>
    //private string OnGetResPath(bool sandboxLoad_ = false)
    //{
    //    string path = string.Empty;
    //    switch (Application.platform)
    //    {
    //        case RuntimePlatform.Android:
    //            if (sandboxLoad_)
    //                path = "file:///" + Application.persistentDataPath + "/";
    //            else
    //                path = Application.streamingAssetsPath + "/"; // "jar:file://" + Application.dataPath + "!/assets/"
    //            break;
    //        case RuntimePlatform.IPhonePlayer:
    //            if(sandboxLoad_)
    //                path = "file:///" + Application.persistentDataPath + "/"; // 未验证是否可用
    //            else
    //                path = "file:///" + Application.streamingAssetsPath + "/"; // 未验证是否可用 //Application.dataPath + "/Raw/";
    //            break;
    //        default:
    //            if (sandboxLoad_)
    //                path = "file:///" + Application.persistentDataPath + "/";
    //            else
    //                path = "file://" + Application.streamingAssetsPath + "/";
    //            break;
    //    }

    //    return path;
    //}

    /// <summary>
    /// 输出日志
    /// </summary>
    /// <param name="debug_"></param>
    private void OnWartDebug(string debug_)
    {
        debugObj.text += debug_ + "\n";
    }
}
