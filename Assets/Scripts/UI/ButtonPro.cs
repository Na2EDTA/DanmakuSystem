using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(CanvasRenderer))]
public class ButtonPro : Selectable
{
    protected ButtonPro() { }

    public UnityEvent onStartSelectedClick;
    public UnityEvent onKeepSelectedClick;
    public UnityEvent onEndSelectedClick;

    bool isKeeping = false;

    private void Update()
    {
        if (isKeeping)
        {
            onKeepSelectedClick.Invoke();
        }
        else
            return;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        isKeeping = !isKeeping;
        if (isKeeping)
        {
            onStartSelectedClick.Invoke();
            GetComponent<Image>().color -= new Color(0.02f, 0.02f, 0.02f, 0.4f);
        }
        else
        {
            onEndSelectedClick.Invoke();
            GetComponent<Image>().color += new Color(0.02f, 0.02f, 0.02f, 0.4f);
        }
    }

}
