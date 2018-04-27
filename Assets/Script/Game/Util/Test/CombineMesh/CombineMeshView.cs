using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 关于合并场景静态物体网格和材质测试
/// 静态物体（比如相同的石头，花草）使用同一材质
/// </summary>
public class CombineMeshView : MonoBehaviour
{
    // 模型模板
    private Transform roleModel = null;

    // 模型列表
    private List<GameObject> roleModelList = new List<GameObject>();

    /// <summary>
    /// 起始
    /// </summary>
    private void Awake()
    {
        roleModel = this.transform.Find("RoleModel");
    }

    /// <summary>
    /// 初始
    /// </summary>
	private void Start ()
    {
        OnCreateModel();
        CombineMeshMaterial();
    }
	
	private void Update ()
    {
		
	}

    /// <summary>
    /// 创建模型
    /// </summary>
    private void OnCreateModel()
    {
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 10; ++j)
            {
                GameObject obj = Instantiate<GameObject>(roleModel.gameObject);
                obj.name = j + "_" + i;
                obj.transform.localScale = new Vector3(1f, 1f, 1f);
                obj.transform.position = new Vector3(j, 0, i);
                obj.transform.SetParent(this.transform);
                obj.SetActive(true);

                roleModelList.Add(obj);
            }
        }
    }

    /// <summary>
    /// 合并模型和材质
    /// </summary>
    private void CombineMeshMaterial()
    {
        // 处理材质
        MeshRenderer[] meshList = this.transform.GetComponentsInChildren<MeshRenderer>();
        List<Material> materiaList = new List<Material>();
        for (int i = 0; i < meshList.Length; ++i)
        {
            Material material = meshList[i].sharedMaterial;
            // 如果材质相同则过滤掉
            if (!materiaList.Contains(material))
                materiaList.Add(material);
        }

        // 处理网格
        MeshFilter[] filterList = this.transform.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combineList = new CombineInstance[filterList.Length];

        for (int i = 0; i< filterList.Length; ++i)
        {
            combineList[i].mesh = filterList[i].sharedMesh;
            combineList[i].transform = filterList[i].transform.localToWorldMatrix;
            filterList[i].gameObject.SetActive(false);
        }


        GameObject combineObj = new GameObject("CombineObj");
        MeshFilter combineObjFilter = combineObj.AddComponent<MeshFilter>();
        MeshRenderer combineObjRenderer = combineObj.AddComponent<MeshRenderer>();
        combineObj.transform.SetParent(this.transform);

        //为新的整体新建一个mesh  
        combineObjFilter.mesh = new Mesh();
        //合并Mesh. 第二个false参数, 表示并不合并为一个网格, 而是一个子网格列表  
        combineObjFilter.mesh.CombineMeshes(combineList, false);

        //为合并后的新Mesh指定材质 ------------------------------  
        combineObjRenderer.sharedMaterials = materiaList.ToArray();

        
    }
}
