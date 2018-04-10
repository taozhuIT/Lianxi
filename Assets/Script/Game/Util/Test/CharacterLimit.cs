using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterLimit : MonoBehaviour {

    // 测试输入框
    [SerializeField] private InputField InputField;

    // Use this for initialization
    void Start () {
        InputField.onValueChanged.AddListener(OnTestStringLength);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTestStringLength(string cont_)
    {
        byte[] bbb = System.Text.Encoding.Default.GetBytes(cont_);
        Debug.Log(bbb.Length);
    }
}
