using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 进度条(减速缓冲/加速缓冲的进度条)
/// </summary>
public class ProgressBarView : MonoBehaviour
{
    // 进度条
    [SerializeField] private Slider slider;
    // 开始按钮
    [SerializeField] private Button startBtn;


    // 是否开始
    private bool isStart = false;

    private float velcoty = 0.0f;
    public float smoothtime = 0.2f;
    public float minProgres = 0f;
    public float maxProgres = 0.6f;

    /// <summary>
    /// 开始
    /// </summary>
	private void Start ()
    {
        startBtn.onClick.AddListener(OnStartHandelr);
	}
	
    /// <summary>
    /// 更新
    /// </summary>
	private void Update ()
    {
        if (isStart && minProgres < maxProgres)
        {
            minProgres = Mathf.SmoothDamp(minProgres, maxProgres, ref velcoty, smoothtime);
            slider.value = minProgres;
        }
        else
        {
            isStart = false;
        }
	}

    /// <summary>
    /// 点击开始
    /// </summary>
    private void OnStartHandelr()
    {
        minProgres = 0f;
        isStart = true;

        this.transform.FindChild("Cube").gameObject.AddComponent<qwe>();
    }
}
