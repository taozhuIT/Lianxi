using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动态模型合并 （带有动画的角色，不能合并成一个模型）
/// 使用 Skinned Mesh Renderer 合并
/// </summary>
public class CombineSkinMeshView : MonoBehaviour
{
    // 裸模/骨骼
    [SerializeField] private GameObject roleObj;
    // 裸模/骨骼
    [SerializeField] private GameObject avatar;
    // 部位列表
    [SerializeField] private List<GameObject> rolePartList;

    // 部位模型对应骨骼名字
    private Dictionary<SkinnedMeshRenderer, List<string>> subBones = new Dictionary<SkinnedMeshRenderer, List<string>>();

    /// <summary>
    /// 初始
    /// </summary>
    private void Start ()
    {
        OnInitModelBones();
    }

    /// <summary>
    /// 更新
    /// </summary>
    private void Update ()
    {
		
	}

    /// <summary>
    /// 初始化模型骨骼信息
    /// </summary>
    private void OnInitModelBones()
    {
        foreach (GameObject prefabs_z in rolePartList)
        {
            //将装备的父类设置为裸模
            //prefabs_z.transform.parent = avatar.transform;  
            //初始化初始模型数据
            foreach (SkinnedMeshRenderer smr in prefabs_z.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                List<string> boneNames = new List<string>();
                foreach (Transform t in smr.bones)
                    boneNames.Add(t.name);

                subBones.Add(smr, boneNames);
            }
            prefabs_z.gameObject.SetActive(false);
        }

        createModel();
    }

    public GameObject createModel()
    {
        //获取已经实例化好的裸模骨骼
        List<CombineInstance> t_combineInstances = new List<CombineInstance>();
        //List<Material> t_materials = new List<Material>();
        List<Transform> t_bones = new List<Transform>();
        //获取所有骨架对象
        Transform[] transforms = avatar.GetComponentsInChildren<Transform>();

        List<Texture2D> textures = new List<Texture2D>();
        int width = 0;
        int height = 0;
        int uvCount = 0;
        List<Vector2[]> uvList = new List<Vector2[]>();
        Material material = null;
        

        //开始配置每一个部件
        foreach (SkinnedMeshRenderer element in subBones.Keys)
        {
            for (int sub = 0; sub < element.sharedMesh.subMeshCount; sub++)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = element.sharedMesh;
                ci.subMeshIndex = sub;
                t_combineInstances.Add(ci);
            }

            // ------------------材质处理------------------
            if (material == null)
            {
                material = Instantiate(element.sharedMaterial);
            }
            uvList.Add(element.sharedMesh.uv);
            uvCount += element.sharedMesh.uv.Length;
            
            if (element.material.mainTexture != null)
            {
                Material[] maters = element.GetComponent<Renderer>().materials;
                for (int b = 0; b < maters.Length; ++b)
                {
                    textures.Add(maters[b].mainTexture as Texture2D);
                    width += maters[b].mainTexture.width;
                    height += maters[b].mainTexture.height;
                }
            }
            
            //加入材质
            //t_materials.AddRange(element.materials);
            // ------------------材质处理------------------

            //将骨骼对象按源序列排列
            foreach (string bone in subBones[element])
            {
                foreach (Transform transform in transforms)
                {
                    if (transform.name != bone) continue;
                    t_bones.Add(transform);
                    break;
                }
            }
        }

        //combineInstances bones materials
        //网格，骨骼，材质，全部对应上了后，合并网格并将骨骼和材质对应上
        //如果第二个参数为true，所有的网格会被结合成一个单个子网格。
        //否则每一个网格都将变成单个不同的子网格。如果所有的网格共享同一种材质，
        //设定它为真。如果第三个参数为false，在CombineInstance结构中的变换矩阵将被忽略。

        SkinnedMeshRenderer r = roleObj.GetComponent<SkinnedMeshRenderer>();
        if (r == null)//如果没有，则给他创建一个
            r = (SkinnedMeshRenderer)roleObj.AddComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
        r.sharedMesh = new Mesh();
        r.sharedMesh.CombineMeshes(t_combineInstances.ToArray(), false, false);
        r.bones = t_bones.ToArray();
        r.material = material;




        Texture2D skinnedMeshAtlas = new Texture2D(get2Pow(width), get2Pow(height));
        Rect[] packingResult = skinnedMeshAtlas.PackTextures(textures.ToArray(), 0);
        Vector2[] atlasUVs = new Vector2[uvCount];

        // 因为将贴图都整合到了一张图片上，所以需要重新计算UV
        int j = 0;
        for (int i = 0; i < uvList.Count; i++)
        {
            foreach (Vector2 uv in uvList[i])
            {
                atlasUVs[j].x = Mathf.Lerp(packingResult[i].xMin, packingResult[i].xMax, uv.x);
                atlasUVs[j].y = Mathf.Lerp(packingResult[i].yMin, packingResult[i].yMax, uv.y);
                j++;
            }
        }

        // 设置贴图和UV
        r.material.mainTexture = skinnedMeshAtlas;
        r.sharedMesh.uv = atlasUVs;




        return roleObj;
    }

         /// <summary>
     /// 获取最接近输入值的2的N次方的数，最大不会超过1024，例如输入320会得到512
     /// </summary>
     public int get2Pow(int into)
     {
         int outo = 1;
         for (int i = 0; i< 10; i++)
         {
             outo *= 2;
             if (outo > into)
             {
                 break;
             }
         }
 
         return outo;
     }
}
