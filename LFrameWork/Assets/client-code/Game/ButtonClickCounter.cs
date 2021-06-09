using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickCounter : MonoBehaviour
{
    public Button btn;
    int count = 0;

    void Start()
    {
        btn.onClick.AddListener(() =>
        {
            Click();
        });
    }


    public void Click()
    {
        count++;
        Debug.LogError(count);
        //Debug.Log(string.Format("{0}点击了{1}次！", btnName, count));
    }
}
