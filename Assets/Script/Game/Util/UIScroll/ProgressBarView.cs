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
    // 是否减速
    public bool isSpeedCut = true;

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
        // 减速进度条
        if (isStart && minProgres < maxProgres)
        {
            if (isSpeedCut)
                OnSpeedCut();
            else
                OnSpeedUp();
        }
        else
        {
            isStart = false;
        }
	}

    /// <summary>
    /// 进度条减速的渐进
    /// </summary>
    private void OnSpeedCut()
    {
        minProgres = Mathf.SmoothDamp(minProgres, maxProgres, ref velcoty, smoothtime);
        slider.value = minProgres;
    }

    float nowRate = 0f;
    float time = 0f;
    /// <summary>
    /// 进度条加速的渐进
    /// </summary>
    private void OnSpeedUp()
    {
        if (nowRate < maxProgres)
            nowRate += time * 0.1f;

        slider.value = nowRate;
        time += Time.deltaTime;
    }

    /// <summary>
    /// 进度条加速的渐进
    /// </summary>
    //private IEnumerator Rate()
    //{
    //    float nowRate = 0f;
    //    float time = 0f;
    //    while (true)
    //    {
    //        if (nowRate < _totleRate)
    //            nowRate += time * _addValue;
    //        else
    //            yield break;

    //        _content.fillAmount = nowRate;
    //        time += Time.deltaTime;

    //        yield return new WaitForSeconds(_referTime);
    //    }
    //}

    /// <summary>
    /// 点击开始
    /// </summary>
    private void OnStartHandelr()
    {
        minProgres = 0f;
        slider.value = minProgres;
        isStart = true;
    }
}
