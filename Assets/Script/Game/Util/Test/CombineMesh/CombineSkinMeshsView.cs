using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineSkinMeshsView : MonoBehaviour
{
    // 目标物体（角色根物体）
    public Transform target;

    // 网格资源
    private static List<Object> _meshList = new List<Object>();
    // 物件资源
    private static List<Object> _gobjList = new List<Object>();
    // 材质资源
    private static List<Object> _mtlsList = new List<Object>();

    private void Start ()
    {
        BatchMesh(target, 80, true, true, "Batch");
    }

    /// <summary>
    /// Batch网格
    /// </summary>
    public static void BatchMesh(Transform trans_, int maxMesh_, bool disMesh_, bool mgrBone_, string mgrName_)
    {
        List<BoneWeight> wetList = new List<BoneWeight>();
        List<Transform> boneList = new List<Transform>();
        List<Matrix4x4> poseList = new List<Matrix4x4>();
        List<CombineInstance> combList = new List<CombineInstance>();
        Material material = null;

        SkinnedMeshRenderer[] smrArr = trans_.GetComponentsInChildren<SkinnedMeshRenderer>();
        for (int smi = 0; smi < smrArr.Length; ++smi)
        {
            SkinnedMeshRenderer smr = smrArr[smi];

            // 网格
            for (int si = 0; si < smr.sharedMesh.subMeshCount; ++si)
            {
                CombineInstance comb = new CombineInstance();
                comb.mesh = smr.sharedMesh;
                comb.subMeshIndex = si;

                combList.Add(comb);
            }

            // 权重
            BoneWeight[] smrWetArr = smr.sharedMesh.boneWeights;
            foreach (BoneWeight smrWet in smrWetArr)
            {
                BoneWeight wet = smrWet;

                wet.boneIndex0 += boneList.Count;
                wet.boneIndex1 += boneList.Count;
                wet.boneIndex2 += boneList.Count;
                wet.boneIndex3 += boneList.Count;

                wetList.Add(wet);
            }

            // 骨骼
            boneList.AddRange(smr.bones);
            poseList.AddRange(smr.sharedMesh.bindposes);

            // 材质
            if (material == null && smr.sharedMaterial != null)
            {
                material = Object.Instantiate(smr.sharedMaterial);
                _mtlsList.Add(material);
            }

            if (disMesh_)
            {
#if UNITY_EDITOR
                Object.DestroyImmediate(smr.gameObject);
#else
                Object.Destroy(smr.gameObject);
#endif
            }
            else
            {
                smr.gameObject.SetActive(false);
            }

            // 合并
            if (smi % maxMesh_ == maxMesh_ - 1 || smi == smrArr.Length - 1)
            {
                // 去除重复的Bone
                if (mgrBone_)
                {
                    List<Transform> boneMgrList = new List<Transform>();
                    List<Matrix4x4> poseMgrList = new List<Matrix4x4>();
                    Dictionary<int, int> boneIdxDict = new Dictionary<int, int>();

                    for (int bpi = 0; bpi < boneList.Count; ++bpi)
                    {
                        Transform bone = boneList[bpi];
                        Matrix4x4 pose = poseList[bpi];

                        int bpIdx;
                        for (bpIdx = 0; bpIdx < boneMgrList.Count; ++bpIdx)
                        {
                            // 找到了，加入映射
                            if (boneList[bpIdx] == bone && poseList[bpIdx] == pose)
                            {
                                boneIdxDict.Add(bpi, bpIdx);

                                break;
                            }
                        }

                        // 没找到，加入之
                        if (bpIdx == boneMgrList.Count)
                        {
                            boneMgrList.Add(bone);
                            poseMgrList.Add(pose);
                            boneIdxDict.Add(bpi, bpIdx);
                        }
                    }

                    // 修正权重
                    for (int wi = 0; wi < wetList.Count; ++wi)
                    {
                        BoneWeight wet = wetList[wi];

                        wet.boneIndex0 = boneIdxDict[wet.boneIndex0];
                        wet.boneIndex1 = boneIdxDict[wet.boneIndex1];
                        wet.boneIndex2 = boneIdxDict[wet.boneIndex2];
                        wet.boneIndex3 = boneIdxDict[wet.boneIndex3];

                        wetList[wi] = wet;
                    }

                    boneList = boneMgrList;
                    poseList = poseMgrList;
                }

                // 构建网格
                GameObject obj;
                if (mgrName_ == null)
                {
                    obj = trans_.gameObject;
                }
                else
                {
                    obj = new GameObject(mgrName_);
                    obj.transform.SetParent(trans_, false);

                    _gobjList.Add(obj);
                }

                SkinnedMeshRenderer instSmr = obj.AddComponent<SkinnedMeshRenderer>();
                instSmr.sharedMesh = new Mesh();
                instSmr.bones = boneList.ToArray();
                instSmr.sharedMesh.CombineMeshes(combList.ToArray(), true, false);
                instSmr.sharedMesh.boneWeights = wetList.ToArray();
                instSmr.sharedMesh.bindposes = poseList.ToArray();
                instSmr.sharedMaterial = material;
                instSmr.localBounds = new Bounds(Vector3.zero, new Vector3(10000, 10000, 10000));
                instSmr.sharedMesh.UploadMeshData(true);

                wetList.Clear();
                boneList.Clear();
                poseList.Clear();
                combList.Clear();
                material = null;
                _meshList.Add(instSmr.sharedMesh);
            }
        }
    }
}
