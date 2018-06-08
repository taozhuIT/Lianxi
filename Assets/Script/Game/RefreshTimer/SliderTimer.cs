using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderTimer : MonoBehaviour {
    
    [SerializeField] private Slider slider;
    [SerializeField] private Text text;
    [SerializeField] private Button startBtn;
    [SerializeField] private float time;

    private bool isStart = false;
    private float startTime = -1;
    // 速度
    private float speed = -1;

    private float cccc = 0;

    private void Awake()
    {
        startBtn.onClick.AddListener(OnClickStart);
    }

    private void Start ()
    {
        speed = time / 1; 
    }
    
    private void Update ()
    {
        if (isStart)
        {
            if (slider.value < 1)
            {
                if (cccc == 0)
                    cccc = Time.realtimeSinceStartup + time;

                slider.value += Time.deltaTime / speed;

                float timeDiffer = cccc - Time.realtimeSinceStartup;
                text.text = ((int)timeDiffer + 1).ToString();
            }
            else
            {
                text.text = "0";
            }
        }
	}

    private IEnumerator bbbb()
    {
        long startMillis = OnGetCurMillis;
        while (slider.value < 1)
        {
            slider.value += 0.05f / time;
            Debug.Log(slider.value);
            startMillis = OnGetCurMillis;

            yield return new WaitForSeconds(0.01f);
        }
    }

    private long OnGetCurMillis
    {
        get
        {
            long currentTicks = DateTime.Now.Ticks;
            DateTime dtFrom = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long currentMillis = (currentTicks - dtFrom.Ticks) / 10000;
            return currentMillis;
        }
    }

    private void OnClickStart()
    {
        //StartCoroutine(bbbb());

        isStart = !isStart;
        if (!isStart)
        {
            slider.value = 0;
        }
    }

    private IEnumerator bbb()
    {
        DateTime time = DateTime.Now;
        int i = 0;
        while(true)
        {
            TimeSpan spand = DateTime.Now - time;
            if (spand.Milliseconds > 100)
            {
                Debug.Log(i);
                i++;
                time = DateTime.Now;
            }

            if(i == 1000)
            {
                yield break;
            }
        }
    }
}
