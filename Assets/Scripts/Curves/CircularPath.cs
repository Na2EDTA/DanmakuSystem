using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CircularPath : DanmakuPath
{
    [SerializeField] float radius;
    public bool clockwise;
    public float argOffset;
    public Vector2 positionOffset;
    
    public override Vector2 Evaluate(float t)
    {
        Vector2 center = new(-radius * MathF.Cos(argOffset), -radius * MathF.Sin(argOffset));
        if (clockwise) t *= -1;
        Vector2 pos = new(radius * MathF.Cos(argOffset + t), radius * MathF.Sin(argOffset + t));
        return center + pos + positionOffset;
    }

    public override float EvaluateArgument(float length)
    {
        return length / radius;
    }

    public override float EvaluateLength(float t)
    {
        return radius * t;
    }

    public override Vector2 EvaluateTangent(float t)
    {
        if (clockwise) t *= -1;
        Vector2 res = new(-radius * MathF.Sin(t + argOffset), radius * MathF.Cos(t + argOffset));
        if (clockwise) return -res;
        else return res;
    }
}


