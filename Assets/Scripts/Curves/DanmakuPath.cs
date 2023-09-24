using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DanmakuPath : MonoBehaviour, IDanmakuPath
{
    public float length;
    public Color color;

    public abstract Vector2 Evaluate(float t);

    public abstract float EvaluateArgument(float length);

    public abstract float EvaluateLength(float t);

    public abstract Vector2 EvaluateTangent(float t);
    
}

interface IDanmakuPath
{
    public abstract Vector2 Evaluate(float t);
    public abstract Vector2 EvaluateTangent(float t);
    public abstract float EvaluateLength(float t);
    public abstract float EvaluateArgument(float length);
}

