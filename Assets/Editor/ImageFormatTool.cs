using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Drawing.Imaging;
using System.Drawing;

/// <summary>
/// 图片格式转换
/// </summary>
public class ImageFormatTool : MonoBehaviour
{
    /// <summary>
    /// 图片透明通道类型
    /// </summary>
    private enum imageAlphaType
    {
        /// 默认
        NOT,
        /// 有全透明/全不透明/半透明 转jpg 
        ALL_HALF_ALPHA,
        /// 全半透明
        HALF_ALPHA,
        /// 全不透明
        ALL_NOTALPHA, 
    }


    [MenuItem("Assets/图片格式转换")]
    public static void OnSelectImages()
    {
        UnityEngine.Object[] selectObjs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

        string selectPath = AssetDatabase.GetAssetPath(selectObjs[0]);
        OnGetImageFile(selectPath);
    }

    /// <summary>
    /// 遍历文件夹
    /// </summary>
    /// <param name="fileName_"></param>
    private static void OnGetImageFile(string fileName_)
    {
        DirectoryInfo directInfo = new DirectoryInfo(fileName_);
        FileSystemInfo[] filesInfos = directInfo.GetFileSystemInfos();

        foreach (FileSystemInfo file in filesInfos)
        {
            // 是否是文件夹
            if (Directory.Exists(file.FullName))
            {
                OnGetImageFile(file.FullName);
            }
            else
            {
                // 是否是图片
                if (!file.Extension.Contains(".meta") && (file.Extension == ".png" || file.Extension == ".jpg" || file.Extension == ".jpeg"))
                {
                    // 加载图片
                    Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(OnPathDisPose(file.FullName));
                    TextAsset textasset = AssetDatabase.LoadAssetAtPath<TextAsset>(OnPathDisPose(file.FullName));
                    imageAlphaType type = OnGetPictureType(file.Name, texture);
                    OnDisposeImageSave(file.Name, type, texture, textasset);
                }
            }
        }
    }

    /// <summary>
    /// 路径处理
    /// </summary>
    private static string OnPathDisPose(string filePath_)
    {
        string newFilePath = filePath_.Replace("\\", "/");
        string newDataPath = "Assets" + newFilePath.Replace(Application.dataPath, string.Empty);

        return newDataPath;
    }

    /// <summary>
    /// 得到图片类型
    /// </summary>
    private static imageAlphaType OnGetPictureType(string name_, Texture2D texture)
    {
        // 判断范围
        float minA = 0.10f;
        float maxA = 0.90f;

        // 判断次数
        Boolean hasTM = false;
        Boolean hasBTM = false;
        int hasTMNum = 0;
        int hasBTMNum = 0;

        // 图片类型
        imageAlphaType textureType = imageAlphaType.NOT;


        // 像素数量判断范围
        int minCheckTMNum = 10;
        if (texture != null)
        {
            //透明像素点（半，全）小于10或者1/1000 暂时认为是美术规范问题！！！后期需要定美术规范
            minCheckTMNum = Math.Max((texture.width * texture.height) / 1000, minCheckTMNum);
        }


        for (int x = 0; x < texture.width; ++x)
        {
            for (int y = 0; y < texture.height; ++y)
            {
                UnityEngine.Color pixel = texture.GetPixel(x, y);

                // a = 0 全透明   a = 1 不透明
                if (pixel.a > minA && pixel.a < maxA)
                {
                    hasBTMNum++;// 有半透明的图片
                    hasBTM = true;
                }
                else if (pixel.a <= minA)
                {
                    hasTMNum++;// 有透明的图片
                    hasTM = true;
                }
            }
        }
  
        if (hasBTM && hasBTMNum >= minCheckTMNum)
        {
            textureType = imageAlphaType.HALF_ALPHA;
        }
        else if (hasTM && hasTMNum >= minCheckTMNum)
        {
            textureType = imageAlphaType.ALL_HALF_ALPHA;
        }
        else
        {
            textureType = imageAlphaType.ALL_NOTALPHA;
        }

       
        return textureType;
    }

    /// <summary>
    /// 转换图片并保存
    /// </summary>
    /// <param name="name_"></param>
    /// <param name="filePath_"></param>
    /// <returns></returns>
    private static void OnDisposeImageSave(string name_, imageAlphaType textureType_, Texture2D texture_,  TextAsset TextAsset_)
    {
        switch (textureType_)
        {
            case imageAlphaType.HALF_ALPHA:
                //Debug.Log(name_ + " -------- 半透明");
                break;
            case imageAlphaType.ALL_HALF_ALPHA:
                //Debug.Log(name_ + " -------- 透明/不透明/半透明");
                break;
            case imageAlphaType.ALL_NOTALPHA:
                //Debug.Log(name_ + " -------- 不透明");
                // 图片转换
                OnConvertImageJPG(name_, texture_, TextAsset_);
                break;
        }

        // 刷新工程资源
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 转换图片到JPG格式，但是保存后缀还是原后缀
    /// </summary>
    private static void OnConvertImageJPG(string name_, Texture2D texture_, TextAsset TextAsset_)
    {
        // 验证后发现不管是直接用UnityAPI还是调用外部库设置压缩质量，压缩后大小都一样。所以直接用UnityAPI

        // 用UnityAPI转换压缩
        byte[] bytes = texture_.EncodeToJPG();
        FileStream file = File.Open(Application.dataPath + "/Script/Game/RefreshTimer/" + name_, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        file.Close();

        // 调用外部图片库System.Drawing压缩,System.Drawing.dll文件在Plugins下面
        //System.Drawing.Image im = System.Drawing.Image.FromFile(Application.dataPath + "/Script/Game/RefreshTimer/" + name_);
        //ThumImageByQuality(im, 10, Application.dataPath + "/Script/Game/RefreshTimer/" + "yasuo_1.png");
    }

    /// <summary>
    /// 按照图片质量生成图片,
    /// </summary>
    /// <param name="sourceFile">原始图片文件</param>  
    /// <param name="quality">质量压缩比</param>  
    /// <param name="outputFile">输出文件名</param>  
    /// <returns>成功返回true,失败则返回false</returns>  
    private static bool ThumImageByQuality(Image sourceFile, long quality, String outputFile)
    {
        bool flag = false;
        try
        {
            long imageQuality = quality;
            Bitmap sourceImage = new Bitmap(sourceFile);
            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, imageQuality);
            myEncoderParameters.Param[0] = myEncoderParameter;
            float xWidth = sourceImage.Width;
            float yWidth = sourceImage.Height;
            Bitmap newImage = new Bitmap((int)(xWidth), (int)(yWidth));
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newImage);
            g.DrawImage(sourceImage, 0, 0, xWidth, yWidth);
            sourceImage.Dispose();
            g.Dispose();
            newImage.Save(outputFile, myImageCodecInfo, myEncoderParameters);
            flag = true;
        }
        catch
        {
            flag = false;
        }
        return flag;
    }

    /// <summary>  
    /// 获取图片编码信息  
    /// </summary>  
    private static ImageCodecInfo GetEncoderInfo(String mimeType)
    {
        int j;
        ImageCodecInfo[] encoders;
        encoders = ImageCodecInfo.GetImageEncoders();
        for (j = 0; j < encoders.Length; ++j)
        {
            if (encoders[j].MimeType == mimeType)
                return encoders[j];
        }
        return null;
    }
}

