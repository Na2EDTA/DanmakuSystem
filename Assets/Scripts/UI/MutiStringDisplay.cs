using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MutiStringDisplay : MonoBehaviour
{
    Text text;
    List<string> strings = new List<string>();
    bool flag = false;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    public void Update()
    {
        if (flag)
        {
            text.text = "";
            for (int i = 0; i < strings.Count; i++)
                text.text += " " + strings[i];

            flag = !flag;
        }
    }

    public void OnPush(string str)
    {
        strings.Add(str);
        flag = true;
    }

    public void OnPop(string str)
    {
        strings.Remove(str);
        flag = true;
    }
}
