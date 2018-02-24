using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 生成图片法线图
/// </summary>
public class CreateNormalMap : EditorWindow
{
    [MenuItem("Assets/纹理/生成法线纹理")]
    public static void OnCreateNormalMap()
    {
        Object[] select = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);
          
        for (int i = 0; i < select.Length; ++i)
        {
            // 原始纹理
            Texture2D texture = select[i] as Texture2D;

            // 生成法线纹理数据
            Texture2D normalTexture = toNormalMap(texture);
            // 生成法线纹理
            byte[] bytes = normalTexture.EncodeToPNG();
            FileStream filestr = File.Open(Application.dataPath + "/Data/UIAtlas/" + texture.name + "_NormalMap.png", FileMode.Create);
            filestr.Write(bytes, 0, bytes.Length);
            filestr.Close();
            AssetDatabase.Refresh();

            // 修改纹理属性 (将纹理属性修改成法线纹理 NormalMap)
            string assetsPath = "Assets/Data/UIAtlas/" + texture.name + "_NormalMap.png";
            Object obj = AssetDatabase.LoadAssetAtPath(assetsPath, typeof(Texture2D));
            string path = AssetDatabase.GetAssetPath(obj);
            TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            textureImporter.textureType = TextureImporterType.NormalMap;
            textureImporter.convertToNormalmap = true;
            //textureImporter.isReadable = true;

            // 写入参数
            AssetDatabase.ImportAsset(path);
        }
    }

    /// <summary>
    /// 生成法线纹理
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public static Texture2D toNormalMap(Texture2D t)
    {
        Texture2D n = new Texture2D(t.width, t.height, TextureFormat.ARGB32, true);
        Color oldColor = new Color();
        Color newColor = new Color();

        for (int x = 0; x < t.width; x++)
        {
            for (int y = 0; y < t.height; y++)
            {
                oldColor = t.GetPixel(x, y);
                newColor.r = oldColor.g;
                newColor.b = oldColor.g;
                newColor.g = oldColor.g;
                newColor.a = oldColor.r;
                n.SetPixel(x, y, newColor);
            }
        }

        n.Apply();

        return n;
    }
}
