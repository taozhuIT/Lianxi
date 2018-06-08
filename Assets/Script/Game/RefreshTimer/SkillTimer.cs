using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 刷新技能遮罩
/// 指定时间比如技能冷却10秒的遮罩刷新
/// </summary>
public class SkillTimer : MonoBehaviour
{
    [SerializeField] private Image skillMask;
    [SerializeField] private Button startBtn;
    [SerializeField] private float refreshTime;

    private bool maskIsOpen = false;
    private float maskMaxVal = 1f;
    private float speed = 0f;

	void Start ()
    {
        speed = refreshTime / maskMaxVal;
        skillMask.fillAmount = maskMaxVal;

        // 按钮事件监听
        startBtn.onClick.AddListener(OnClickStart);
    }

	void Update ()
    {
		if(maskIsOpen && skillMask.fillAmount > 0)
        {
            skillMask.fillAmount -= (Time.deltaTime / speed);
        }
	}

    private void OnClickStart()
    {
        skillMask.fillAmount = maskMaxVal;
        maskIsOpen = !maskIsOpen;
    }
}
