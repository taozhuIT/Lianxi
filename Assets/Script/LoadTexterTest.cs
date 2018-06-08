using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GameResLoad;

public class LoadTexterTest : MonoBehaviour
{
    void Start()
    {
        // Unity中加载非图片后缀名(.jpg/.png.....)的图片文件
        TextAsset texbbbbtAsset = AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Resources/1001001_y2_x7.bytes");
        Texture2D texture = new Texture2D(128, 128, TextureFormat.RGB24, false, false);
        texture.LoadImage(texbbbbtAsset.bytes);

        Sprite longBack = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one);
        this.GetComponent<SpriteRenderer>().sprite = longBack;

        // www可以加载自定义后缀名文件
        //string dataPath = Application.dataPath + "/Resources/1001001_y2_x7.djr";
        // 协议头
        //string protoTitle = ResAssetUtil.OnGetFileProtoTitle(true);
        //StartCoroutine(loadText(protoTitle + dataPath));

    }

    //	void Update () {

    //	}

    //    private IEnumerator loadText(string path_)
    //    {
    //        Debug.Log(path_);
    //        WWW www = new WWW(path_);
    //        yield return www;

    //        if(www.error == null)
    //        {
    //            byte[] texter = www.bytes;
    //            Debug.Log(texter.Length);

    //            Texture2D textAsset = www.texture;
    //            Sprite longBack = Sprite.Create(textAsset, new Rect(0, 0, textAsset.width, textAsset.height), Vector2.one);
    //            tileMap.sprite = longBack;
    //        }
    //        else
    //        {
    //            Debug.Log("错误");
    //        }
    //    }
}
