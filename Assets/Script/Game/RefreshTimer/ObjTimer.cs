using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 定时移动指定的距离
/// 5秒z轴移动10
/// </summary>
public class ObjTimer : MonoBehaviour {

    private float maxDist = 10f;
    private float limitTime = 5f;
    private float moveSpeed = 0f;
	void Start ()
    {
        // 得到速度
        moveSpeed = limitTime / maxDist;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (this.transform.position.z < maxDist)
        {
            this.transform.position += new Vector3(0f, 0f, Time.deltaTime / moveSpeed);
        }
	}
}
