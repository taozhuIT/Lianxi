using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 转盘
/// </summary>
public class ZhuanPanView : MonoBehaviour
{
    // 指针
    [SerializeField] private Transform pointer;
    // 开始按钮
    [SerializeField] private Button startBtn;

    // 当前速度
    private float speedVla = 0;
    // 最小速度
    private float speedMin = 0;
    // 设置速度
    [SerializeField] private float speed;
    // 速度递减值
    [SerializeField] private float decreVal;

    // 开始悬停速度
    private float hoverSpeed = -1;

    // 协程
    private Coroutine coroutine = null;

    /// <summary>
    /// 起始
    /// </summary>
    private void Awake()
    {
        startBtn.onClick.AddListener(OnStartRotate);
    }

    /// <summary>
    /// 初始
    /// </summary>
	private void Start ()
    {
		
	}
	
    /// <summary>
    /// 更新 
    /// </summary>
	private void Update ()
    {
		
	}

    /// <summary>
    /// 开始旋转
    /// </summary>
    private void OnStartRotate()
    {
        speedVla = speed;

        if (coroutine != null)
            StopCoroutine(coroutine);

        coroutine = StartCoroutine(pointerRotate());
    }

    /// <summary>
    /// 执行旋转
    /// </summary>
    /// <returns></returns>
    private IEnumerator pointerRotate()
    {
        hoverSpeed = -1;
        RectTransform rectt = pointer.GetComponent<RectTransform>();

        while (true)
        {
            if (speedVla > 100)
            {
                pointer.Rotate(Vector3.forward * (speedVla * Time.deltaTime));

                // 记录最小速度
                speedMin = speedVla;
                // 速度递减
                speedVla -= decreVal;
            }
            else
            {
                // 没有旋转到指定格子时，以最小速度旋转。直到旋转到指定格子停止
                if (hoverSpeed == -1)
                    hoverSpeed = speedMin - 5f;

                // 是否旋转到指定格子
                Debug.Log(rectt.rotation);
                if (rectt.localRotation.z >= 0.12f && rectt.localRotation.z <= 0.14f)
                    hoverSpeed = 0;

                // 是否还需要旋转
                if (hoverSpeed > 0)
                {
                    pointer.Rotate(Vector3.forward * (hoverSpeed * Time.deltaTime));
                    hoverSpeed -= 0.1f;
                }
            }

            yield return new WaitForSeconds(0.001f);
        }
    }
}
