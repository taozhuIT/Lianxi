 using UnityEngine;
 using System.Collections.Generic;
 
 
public class CharacterCombine : MonoBehaviour
{
    // 目标物体（角色根物体）
    public GameObject target;

    // 测试
    public GameObject body;

    /// <summary>
    /// 初始
    /// </summary>
    private void Start()
    {
        CombineTest();
    }

    public void CombineTest()
    {
        Debug.Log("模型合并开始");
  
        // 合并蒙皮网格和贴图，刷新骨骼
        Combine(target.transform);
    }
  
  
    /// <summary>
    /// 合并蒙皮网格，刷新骨骼
    /// 注意：合并后的网格会使用同一个Material
    /// </summary>
    /// <param name="root">角色根物体</param>
    public void Combine(Transform root)
    {
        float startTime = Time.realtimeSinceStartup;
  
        List<CombineInstance> combineInstances = new List<CombineInstance>();
        List<Transform> boneList = new List<Transform>();
        Transform[] transforms = root.GetComponentsInChildren<Transform>();

        Material material = null;
        List<Texture2D> textures = new List<Texture2D>();
        int width = 0;
        int height = 0;
        int uvCount = 0;
        List<Vector2[]> uvList = new List<Vector2[]>();

        // 遍历所有蒙皮网格渲染器，以计算出所有需要合并的网格、UV、骨骼的信息
        foreach (SkinnedMeshRenderer smr in root.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            smr.gameObject.SetActive(false);

            if (material == null)
                material = Instantiate(smr.sharedMaterial);
  
            for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
            {
                CombineInstance ci = new CombineInstance();
                ci.mesh = smr.sharedMesh;
                ci.subMeshIndex = sub;
                combineInstances.Add(ci);

                uvList.Add(smr.sharedMesh.uv);
                uvCount += smr.sharedMesh.uv.Length;

                Debug.Log(smr.name + " " + smr.bounds);
            }
  
  
            if (smr.material.mainTexture != null)
            {
                Renderer render = smr.GetComponent<Renderer>();
                for (int m = 0; m < render.materials.Length; ++m)
                {
                    textures.Add(render.materials[m].mainTexture as Texture2D);
                    width += render.materials[m].mainTexture.width;
                    height += render.materials[m].mainTexture.height;
                }
            }
  
            foreach (Transform bone in smr.bones)
            {
                foreach (Transform item in transforms)
                {
                    if (item.name != bone.name) continue;
                    boneList.Add(item);
                    break;
                }
            }
  
            smr.gameObject.SetActive(false);
        }
 
        // 获取并配置角色所有的SkinnedMeshRenderer
        SkinnedMeshRenderer tempRenderer = root.gameObject.GetComponent<SkinnedMeshRenderer>();
        if (!tempRenderer)
            tempRenderer = root.gameObject.AddComponent<SkinnedMeshRenderer>();

        
        // 合并网格，刷新骨骼，附加材质
        tempRenderer.sharedMesh = new Mesh();
        tempRenderer.sharedMesh.CombineMeshes(combineInstances.ToArray(), true, false);
        tempRenderer.bones = boneList.ToArray();
        tempRenderer.material = material;

        // 设置渲染包围盒
        SkinnedMeshRenderer bodySkinned = body.GetComponent<SkinnedMeshRenderer>();
        tempRenderer.localBounds = new Bounds(bodySkinned.localBounds.center, new Vector3(2f, 5f, 2f));

        Texture2D skinnedMeshAtlas = new Texture2D(get2Pow(width), get2Pow(height));
        Rect[] packingResult = skinnedMeshAtlas.PackTextures(textures.ToArray(), 0);
        Vector2[] atlasUVs = new Vector2[uvCount];
 
        // 因为将贴图都整合到了一张图片上，所以需要重新计算UV
        int j = 0;
        for (int i = 0; i<uvList.Count; i++)
        {
            foreach (Vector2 uv in uvList[i])
            {
                atlasUVs[j].x = Mathf.Lerp(packingResult[i].xMin, packingResult[i].xMax, uv.x);
                atlasUVs[j].y = Mathf.Lerp(packingResult[i].yMin, packingResult[i].yMax, uv.y);
                j++;
            }
        }
 
        // 设置贴图和UV
        tempRenderer.material.mainTexture = skinnedMeshAtlas;
        tempRenderer.sharedMesh.uv = atlasUVs;

        Debug.Log("合并耗时 : " + (Time.realtimeSinceStartup - startTime) * 1000 + " ms");
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
