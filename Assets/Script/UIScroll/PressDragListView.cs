using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 动态拖动列表框
/// </summary>
public class PressDragListView : MonoBehaviour
{
    // 鼠标开始位置
    private Vector3 startPos;
    // 列表面板当前状态
    private bool listPanelType = true;

    // UI
    // 列表
    [SerializeField] private RectTransform content;
    // 收起/弹出按钮
    [SerializeField] private Button button;

    /// <summary>
    /// 起始
    /// </summary>
	private void Start ()
    {
        button.onClick.AddListener(OnListZoomHandelr);
	}

    /// <summary>
    /// 更新
    /// </summary>
    private void Update ()
    {
		
	}

    /// <summary>
    /// 列表面板缩放
    /// </summary>
    private void OnListZoomHandelr()
    {
        if (listPanelType)
        {
            Tweener tween = content.transform.DOLocalMoveX(0f, 0.7f, true);
            tween.SetEase(Ease.OutCubic);
        }
        else
        {
            Tweener tween = content.transform.DOLocalMoveX(-(content.rect.width + 30f), 0.7f, true);
            tween.SetEase(Ease.OutCubic);
        }
        
        listPanelType = !listPanelType;
        button.transform.Find("Text").GetComponent<Text>().text = listPanelType ? "展开" : "收回";
    }

    /// <summary>
    /// 拖动开始
    /// </summary>
    public void OnDragStartHandelr()
    {
        startPos = Input.mousePosition;
    }

    /// <summary>
    /// 拖动中
    /// </summary>
    public void OnDragTheHandelr()
    {
        // 鼠标X轴拖动差
        float x = (Input.mousePosition - startPos).x;
        // 设置列表宽度
        content.sizeDelta = new Vector2(content.rect.width + x, content.rect.height);
        // 更新鼠标位置
        startPos = Input.mousePosition;
    }

    /// <summary>
    /// 拖动结束
    /// </summary>
    public void OnDragExitHandelr()
    {
        Debug.Log("OnDragExitHandelr");
    }
}
