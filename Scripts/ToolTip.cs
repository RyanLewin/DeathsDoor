using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolTip : MonoBehaviour
{
    public static ToolTip GetToolTip { get; private set; }
    TextMeshProUGUI txt;

    // Start is called before the first frame update
    void Awake ()
    {
        GetToolTip = this;
        gameObject.SetActive(false);
        txt = GetComponentInChildren<TextMeshProUGUI>();    
    }

    private void FixedUpdate ()
    {
        //Debug.Log(Input.mousePosition);
        float offset = GetComponent<RectTransform>().sizeDelta.y / 2;
        transform.position = Input.mousePosition + new Vector3(0, 10 + offset, 0);
    }

    public void SetText (string _text)
    {
        txt.text = _text;
        SetSize();
    }

    void SetSize ()
    {
        RectTransform t = GetComponent<RectTransform>();
        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, 21);

        txt.ForceMeshUpdate();
        int j = txt.textInfo.lineCount;
        for (int i = 0; i < j - 1; i++)
        {
            t.sizeDelta = new Vector2(t.sizeDelta.x, t.sizeDelta.y + 19);
        }

        float offset = GetComponent<RectTransform>().sizeDelta.y / 2;
        transform.position = Input.mousePosition + new Vector3(0, 5 + offset, 0);
    }
}
