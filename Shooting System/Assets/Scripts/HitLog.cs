using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class HitLog : MonoBehaviour
{
    Text logText;

    private void Start()
    {
        logText = GetComponentInChildren<Text>();
    }

    public void AppendMessage(string message)
    {
        string newText = logText.text;

        if (newText != "")
        {
            newText += "\n";
        }

        newText += message;

        logText.text = newText;

        Canvas.ForceUpdateCanvases();
        this.transform.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
    }
}
