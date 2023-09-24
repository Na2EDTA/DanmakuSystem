using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Danmaku.OdinSerializer;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Velocity))]
[RequireComponent(typeof(Transform))]
[DisallowMultipleComponent]
public abstract class DanmakuObject : MonoBehaviour
{
    [HideInInspector] public Transform tr;
    [Header("��Ļ��ʽ")]
    public bool isBullet;
    public BulletStyle style;
    public Sprite fallBackSprite;
    [HideInInspector] public Velocity velocity;
    [HideInInspector] public SpriteRenderer r;

    [Header("�Զ������")]
    [Tooltip("�������������ʼֵ֮���ԵȺŷָ�\n�ո�ᱻ�Զ�ȥ��")]
    public Danmaku.SerializeExtension.Dictionary<string, float> variables = new();
    public List<AnimationCurve> curves = new();

    protected virtual void Awake()
    {
        tr = transform;
        velocity = GetComponent<Velocity>();
        r = GetComponent<SpriteRenderer>();
        if (isBullet)
        {
            r.sprite = BulletStyleManager.instance?.GetStyle(style.color, style.shape, 
                out GetComponent<DanmakuCollider>().mainArg);
            if (r.sprite == null) r.sprite = fallBackSprite;
        }
        else
            r.sprite = fallBackSprite;
    }

    /// <summary>
    /// ����ֵ��API�������ڵ�Ļ��Create(...)�������ɺ󱻵��ã��ȱ༭�����еĸ�ֵ���󴥷�
    /// </summary>
    /// <param name="ps"></param>
    public virtual void InitParams(params float[] ps)
    {
        for (int i = 0; i < variables.Count; i++)
        {
            string[] names = variables.Keys as string[];
            variables[names[i]] = ps[i];
        }
    }

    void Analyze(string expression, out string paraName, out float paraValue)
    {
        expression = expression.Replace(" ", string.Empty);
        string[] str = expression.Split('=');
        paraName = str[0];
        paraValue = float.Parse(str[1]);
    }
}


