using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleCtrlView : MonoBehaviour
{
    // 物件
    private Transform mode = null;
    // 起始位置
    private Vector3 _mouseStartPos;
    // 旋转量
    private float _rotate = 0f;

    /// <summary>
    /// 起始
    /// </summary>
    private void Start () 
    {
        mode = this.transform.Find("10001");
	}
 
    /// <summary>
    /// 更新
    /// </summary>
    private void Update () 
    {
        ScreenSlide();
	}

    /// <summary>
    /// 滑动
    /// </summary>
    private void ScreenSlide()
    {
        // 起始位置
        if (Input.GetMouseButtonDown(0))
            _mouseStartPos = Input.mousePosition;

        if (Input.GetMouseButton(0))
        {
            // 移动距离
            Vector3 distance = Input.mousePosition - _mouseStartPos;

            // 是否移动
            if (distance != Vector3.zero)
            {
                // 得到旋转量
                float angY = (distance.x / Screen.width) * 360f;
                _rotate += angY;

                // 设置旋转量(负号是为相反方向)
                mode.rotation = Quaternion.Euler(new Vector3(0, -_rotate, 0));

                // 更新位置
                _mouseStartPos = Input.mousePosition;
            }
        }
    }
}
