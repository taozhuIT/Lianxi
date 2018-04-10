using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 自定义动画曲线
/// </summary>
public class AnimationCurveTest : MonoBehaviour
{
    // 动画曲线
    public AnimationCurve anim;



    // 测试按钮
    [SerializeField] private Button startBtn;
    // 测试变量
    private bool isStart = false;

    /// <summary>
    /// 起始
    /// </summary>
    void Start ()
    {
        startBtn.onClick.AddListener(OnClickStart);
        
    }
	
    /// <summary>
    /// 更新
    /// </summary>
	void Update ()
    {
        transform.position = new Vector3(anim.Evaluate(Time.time), anim.Evaluate(Time.time), transform.position.z);
    }

    private void OnClickStart()
    {
        isStart = !isStart;
    }


}
