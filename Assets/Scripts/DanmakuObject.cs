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
    [Header("弹幕样式")]
    public bool isBullet;
    public BulletStyle style;
    public Sprite fallBackSprite;
    [HideInInspector] public Velocity velocity;
    [HideInInspector] public SpriteRenderer r;

    [Header("自定义变量")]
    [Tooltip("变量名与变量初始值之间以等号分隔\n空格会被自动去除")]
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
    /// 赋初值的API函数，在弹幕被Create(...)函数生成后被调用，比编辑器当中的赋值靠后触发
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


