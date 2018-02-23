using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

/// <summary>
/// 自动生成3D动画状态机
/// </summary>
public class AutoCreationAnim : EditorWindow
{
    // 角色模型和动画根文件夹
    private static string roleRootDictName = "WesternKingdoms";
    // 选择路径
    private static string selectPath = null;

    // FBX模型数据
    private static Dictionary<string, GameObject> roleModelDict = new Dictionary<string, GameObject>();
    // FBX动画数据
    private static Dictionary<string, Dictionary<string, AnimationClip>> roleAnimDict = new Dictionary<string, Dictionary<string, AnimationClip>>();

    // 创建的动画控制器
    private static Dictionary<string, AnimatorController> animControDict = new Dictionary<string, AnimatorController>();

    // 动画clip宽度
    private static float animClipWidth = 200f;
    // 动画clip高度
    private static float animClipHight = 30f;
    // 动画clip间距
    private static float animClipSpac = 30f;

    [MenuItem("动画/生成全部动画")]
    public static void OnCreationAnimControlle()
    {
        OnGetSelectionObj();
    }

    [MenuItem("Assets/动画/生成动画状态机")]
    private static void Assets_right_btn1()
    {
        OnGetSelectionObj();
    }

    /// <summary>
    /// 得到选中物件/文件夹路径
    /// </summary>
    private static void OnGetSelectionObj()
    {
        Object[] select = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets);

        if(select.Length > 0)
        {
            if (select[0].name == roleRootDictName)
            {
                selectPath = AssetDatabase.GetAssetPath(select[0]);
                roleModelDict.Clear();
                OnGetObjModelFBX(selectPath + "/models");
                Debug.Log("模型加载完成");

                roleAnimDict.Clear();
                OnGetObjAnimFBX(selectPath + "/animation");
                Debug.Log("模型动画加载完成");
                OnCreateAnimControlle();

                // 创建prefab
                OnCreateRolePrefab();
            }
            else
            {
                Debug.LogError("没有选择角色模型和角色动画根文件夹 "+ roleRootDictName);
            }
        }
    }

    /// <summary>
    /// 给角色动画生成动画状态机
    /// </summary>
    private static void OnCreateAnimControlle()
    {
        animControDict.Clear();

        foreach (KeyValuePair<string, Dictionary<string, AnimationClip>> anim in roleAnimDict)
        {
            string key = anim.Key;

            // 生成状态机的路径
            string controPath = key + key.Substring(key.LastIndexOf("/")) + ".controller";
            // 创建动画状态机
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controPath);

            // 设置基础层参数 (0层)
            AnimatorStateMachine baseLayerMachine = controller.layers[0].stateMachine;

            baseLayerMachine.entryPosition = Vector3.zero;
            baseLayerMachine.exitPosition = new Vector3(0f, baseLayerMachine.entryPosition.y + animClipWidth);
            baseLayerMachine.anyStatePosition = new Vector3(0f, baseLayerMachine.exitPosition.y + animClipWidth);

            animControDict.Add(controller.name, controller);

            // 设置自定义层数据 (1层)(暂时不加自定义层)
            /*
            // add crouch layer to controller
            controller.AddLayer("CrouchLayer");
            // get a copy from controller's layer
            AnimatorControllerLayer[] layers = controller.layers;
            // set layer parameters
            layers[1].defaultWeight = 1f;
            layers[1].blendingMode = AnimatorLayerBlendingMode.Override;
            // save layer setting to controller
            controller.layers = layers;
            // get state machine in crouch layer
            AnimatorStateMachine crouchLayerMachine = controller.layers[1].stateMachine;
            // set crouch machine parameters
            crouchLayerMachine.entryPosition = Vector3.zero;
            crouchLayerMachine.exitPosition = new Vector3(600f, 200f);
            crouchLayerMachine.anyStatePosition = new Vector3(0f, 200f);
            */

            // 添加动画clip
            OnCreateAnimClip(baseLayerMachine, anim.Value);
        }
    }

    /// <summary>
    /// 给状态机添加动画片段
    /// </summary>
    private static void OnCreateAnimClip(AnimatorStateMachine machine_, Dictionary<string, AnimationClip> clipDict_)
    {
        // 添加动画Clip
        Vector2 animClipPos = new Vector2(0f, 0f);
        int xPos = 1;
        int yPos = 1;
        foreach (KeyValuePair<string, AnimationClip> clip in clipDict_)
        {
            if (xPos <= 3)
            {
                animClipPos = new Vector2(animClipPos.x + (animClipWidth + animClipSpac), animClipPos.y);
                xPos++;
                yPos = 1;
            }
            else
            {
                animClipPos = new Vector2(0f + (animClipWidth + animClipSpac), animClipPos.y + (animClipHight + animClipSpac));
                yPos++;
                xPos = 1;
            }

            UnityEditor.Animations.AnimatorState state = machine_.AddState(clip.Key, animClipPos);
            state.motion = clip.Value;

            // Add behaviour to state  添加脚本文件
            //state.AddStateMachineBehaviour<RoleBehaviorView>();

            // 添加默认连线 
            //baseLayerMachine.defaultState = state;
        }
    }

    /// <summary>
    /// 创建prefab
    /// </summary>
    private static void OnCreateRolePrefab()
    {
        foreach(KeyValuePair<string, GameObject> roleModel in roleModelDict)
        {
            GameObject instanModel = Instantiate(roleModel.Value);
            
            instanModel.AddComponent<Animator>();
            Animator instanAnimator = instanModel.GetComponent<Animator>();
            instanAnimator.runtimeAnimatorController = animControDict[roleModel.Key];

            // 添加脚本
            instanModel.AddComponent<RoleAnimBehaviorView>();

            // 创建prefab
            PrefabUtility.CreatePrefab(selectPath + "/models/" + roleModel.Key + ".prefab", instanModel);
            DestroyImmediate(instanModel);
        }
    }

    /// <summary>
    /// 得到物件模型FBX数据
    /// </summary>
    private static void OnGetObjModelFBX(string rootPath_)
    {
        DirectoryInfo directInfo = new DirectoryInfo(rootPath_);
        FileSystemInfo[] filesInfo = directInfo.GetFileSystemInfos();

        for (int i = 0; i < filesInfo.Length; ++i)
        {
            FileSystemInfo fileInfo = filesInfo[i];

            // 是否是文件夹
            if (!Directory.Exists(fileInfo.FullName))
            {
                if (fileInfo.Extension == ".FBX")
                {
                    string[] names = fileInfo.Name.Replace(".FBX", "").Split('_');
                    string modelName = names[1];

                    string assetPath = OnGetUnityAssetPath(fileInfo.FullName);
                    GameObject obj = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;

                    if (roleModelDict.ContainsKey(modelName))
                        Debug.Log("模型名字不对不是 xx_xx_00_xx_x------" + modelName);

                    roleModelDict.Add(modelName, obj);
                }
            }
            else
            {
                // 模型加载不需要遍历
                //OnGetObjModelFBX(fileInfo.FullName);
            }
        }
    }

    /// <summary>
    /// 得到物件动画FBX数据
    /// </summary>
    private static void OnGetObjAnimFBX(string rootPath_)
    {
        DirectoryInfo directInfo = new DirectoryInfo(rootPath_);
        FileSystemInfo[] filesInfo = directInfo.GetFileSystemInfos();

        for (int i = 0; i < filesInfo.Length; ++i)
        {
            FileSystemInfo fileInfo = filesInfo[i];

            // 是否是文件夹
            if (!Directory.Exists(fileInfo.FullName))
            {
                if(fileInfo.Extension == ".FBX")
                {
                    string[] names = fileInfo.Name.Replace(".FBX", "").Split('_');
                    string animModelName = names[1] + "_" + names[3] + "_" + names[4];

                    string assetPath = OnGetUnityAssetPath(fileInfo.FullName);
                    AnimationClip objClip = AssetDatabase.LoadAssetAtPath(assetPath, typeof(AnimationClip)) as AnimationClip;

                    // 测试给动画添加事件回调
                    // ------------------------测试代码------------------------
                    if(OnGetAnimClipEvent(objClip, "OnRoleAttckAnimHandelr"))
                    {

                        AnimationEvent animEvent = new AnimationEvent();
                        animEvent.functionName = "OnRoleAttckAnimHandelr";
                        animEvent.time = 1f;

                        List<AnimationEvent> animEventList = new List<AnimationEvent>();
                        animEventList.Add(animEvent);
                        //AnimationEvent[] animEventArry = new

                        // 动态添加事件
                        // 方法1（当动画播放完成，或者玩家销毁，事件则销毁）
                        //objClip.AddEvent(animEvent);
                        // 方法2 添加固定动画，动画播放完成不销毁
                        AnimationUtility.SetAnimationEvents(objClip, animEventList.ToArray());
                    }
                    // ------------------------测试代码------------------------

                    if (roleAnimDict.ContainsKey(animModelName))
                        Debug.Log("动画名字不对不是 xx_xx_00_xx_x------" + animModelName);
                    
                    roleAnimDict[OnGetUnityAssetPath(rootPath_)].Add(animModelName, objClip);
                }
            }
            else
            {
                roleAnimDict.Add(OnGetUnityAssetPath(fileInfo.FullName), new Dictionary<string, AnimationClip>());
                OnGetObjAnimFBX(fileInfo.FullName);
            }
        }
    }

    /// <summary>
    /// 判断动画片段是否已经添加相同事件
    /// </summary>
    private static bool OnGetAnimClipEvent(AnimationClip objClip_, string funtionName_)
    {
        bool isMeet = false;

        for (int i = 0; i < objClip_.events.Length; ++i)
        {
            AnimationEvent eventItem = objClip_.events[i];
            if(eventItem.functionName == funtionName_)
            {
                isMeet = true;
                break;
            }
        }

        return !isMeet;
    }

    /// <summary>
    /// 得到Unity工程资源加载路径
    /// </summary>
    private static string OnGetUnityAssetPath(string path_)
    {
        return "Assets" + path_.Replace("\\", "/").Replace(Application.dataPath, "");
    }
}
