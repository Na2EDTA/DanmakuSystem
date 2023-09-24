using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

public class BezierPath : DanmakuPath, IEquatable<BezierPath>
{
    [SerializeField] [Range(1, 200)] int precision = 60;
    public List<BezierWaypoint> waypoints;
    public List<BezierCurve> segments = new();

    private void Awake()
    {
        UpdateAllSegments();
    }

    public int Precision
    {
        get
        {
            return precision;
        }
        set
        {
            precision = value;
            Resample(value);
        }
    }

    public BezierPath(params BezierWaypoint[] points)
    {
        waypoints.AddRange(points);
        UpdateAllSegments();
    }

    public void AddWaypoint(int index, Vector2 pos)
    {
        if (index > 0 && index < waypoints.Count)
        {
            Vector2 tangent = waypoints[index].position - waypoints[index - 1].position;
            BezierWaypoint waypoint = new(pos, tangent / 3, tangent / 3, TangentMode.Smooth);
            waypoints.Insert(index, waypoint);
            UpdateAllSegments();
        }
        else if (index == 0)
        {
            Vector2 tangent = waypoints[0].position - pos;
            BezierWaypoint waypoint = new(pos, tangent / 3, tangent / 3, TangentMode.Smooth);
            waypoints.Insert(index, waypoint);
            UpdateAllSegments();
        }
        else if (index == waypoints.Count)
        {
            Vector2 tangent = pos - waypoints[waypoints.Count - 1].position;
            BezierWaypoint waypoint = new(pos, tangent / 3, tangent / 3, TangentMode.Smooth);
            waypoints.Insert(index, waypoint);
            UpdateAllSegments();
        }
        else
            Debug.LogWarning("Your index is out of the range.");
    }

    public void MoveWaypoint(int index, Vector2 pos)
    {
        if (index >= 0 && index < waypoints.Count)
        {
            waypoints[index] = new BezierWaypoint(pos, waypoints[index].InTangent, waypoints[index].OutTangent, TangentMode.Smooth);
            UpdateSegmentsAt(index);
        }
        else
            Debug.LogWarning("Your index is out of the range.");
    }

    public void ResortWaypoint(int index, int newIndex)
    {
        if (index >= 0 && index < waypoints.Count && newIndex >= 0 && newIndex < waypoints.Count)
        {
            BezierWaypoint waypoint = waypoints[index];
            waypoints.RemoveAt(index);
            waypoints.Insert(newIndex, waypoint);
            UpdateAllSegments();
        }
        else
            Debug.LogWarning("Your index is out of the range.");
    }

    public void RemoveWaypoint(int index)
    {
        if (index >= 0 && index < waypoints.Count)
        {
            waypoints.RemoveAt(index);
            UpdateAllSegments();
        }
        else
            Debug.LogWarning("Your index is out of the range.");
    }

    /// <summary>
    /// 计算曲线上参数splineValue对应的点的坐标
    /// </summary>
    /// <param name="splineValue"></param>
    /// <returns></returns>
    public override Vector2 Evaluate(float splineValue)
    {
        splineValue = Mathf.Clamp(splineValue, 0, segments.Count);

        int segmentIndex = (int)splineValue;
        float t = splineValue - segmentIndex;

        if (segmentIndex >= 0 && segmentIndex < segments.Count)
            return segments[segmentIndex].Evaluate(t);
        else if (splineValue == segments.Count)
            return waypoints[segmentIndex].position;
        else
        {
            Debug.LogWarning("The point is NOT on the path.");
            return Vector2.zero;
        }
    }

    /// <summary>
    /// 计算曲线上参数splineValue对应的点上的切向量
    /// </summary>
    /// <param name="splineValue"></param>
    /// <returns></returns>
    public override Vector2 EvaluateTangent(float splineValue)
    {
        splineValue = Mathf.Clamp(splineValue, 0, segments.Count);

        int segmentIndex = (int)splineValue;
        float t = splineValue - segmentIndex;
        if (segmentIndex >= 0 && segmentIndex < segments.Count)
            return segments[segmentIndex].EvaluateTangent(t);
        else if (splineValue == segments.Count)
            return waypoints[^1].OutTangent;
        else
        {
            Debug.LogWarning("The point is NOT on the path.");
            return Vector2.zero;
        }
    }

    /// <summary>
    /// 计算曲线上参数splineValue对应的点到起点的总路程
    /// </summary>
    /// <param name="splineValue"></param>
    /// <returns></returns>
    public override float EvaluateLength(float splineValue)
    {
        splineValue = Mathf.Clamp(splineValue, 0, segments.Count);

        if (splineValue > segments.Count)
        {
            Debug.LogWarning("The point is NOT on the path.");
            return length;
        }
        int segmentIndex = (int)splineValue;
        float t = splineValue - segmentIndex;
        float len = 0;

        for (int i = 0; i < segmentIndex; i++)
        {
            len += segments[i].length;
        }
        if (t == 0) return len;
        len += segments[segmentIndex].EvaluateLength(t);
        return len;
    }

    /// <summary>
    /// 计算曲线上距离起点路程为length的点对应的样条曲线参数splineValue
    /// </summary>
    /// <param name="length"></param>
    /// <returns>splineValue</returns>
    public override float EvaluateArgument(float length)
    {
        length = Mathf.Clamp(length, 0, this.length);

        float res = 0;
        int i = 0; float len, l;
        if (length > this.length)
        {
            Debug.LogWarning("The point is NOT on the path.");
            return segments.Count;
        }
        for (len = 0; len < length; i++)
        {
            len += segments[i].length;
            res++;
        }
        --res;
        l = segments[i-1].length - (len - length);
        res += segments[i-1].EvaluateArgument(l);
        return res;
    }

    /// <summary>
    /// 更新所有曲线段
    /// </summary>
    [ContextMenu("UpdateSegments")]
    public void UpdateAllSegments()
    {
        segments.Clear();
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            segments.Add(new BezierCurve(waypoints[i], waypoints[i + 1], Precision));
        }
        length = EvaluateLength(segments.Count);
    }

    /// <summary>
    /// 更新某点附近的曲线段
    /// </summary>
    /// <param name="index"></param>
    void UpdateSegmentsAt(int index)
    {
        if (index > 0 || index < waypoints.Count - 1)
        {
            segments[index - 1] = new BezierCurve(waypoints[index - 1], waypoints[index], Precision);
            segments[index] = new BezierCurve(waypoints[index], waypoints[index + 1], Precision);
        }
        else if (index == 0)
            segments[0] = new BezierCurve(waypoints[0], waypoints[1], Precision);
        else if (index == waypoints.Count - 1)
            segments[index - 1] = new BezierCurve(waypoints[index - 1], waypoints[index], Precision);
        else
            Debug.LogWarning("The point is NOT on the path.");

        length = EvaluateLength(segments.Count);
    }

    /// <summary>
    /// 按照新的精度再次取样
    /// </summary>
    /// <param name="precision"></param>
    void Resample(int precision)
    {
        for (int i = 0; i < segments.Count; i++)
            segments[i].UpdateSamples(precision);

        length = EvaluateLength(segments.Count);
    }

    public void Render()
    {
        for (int i = 0; i < segments.Count; i++)
            segments[i].Render(color);

        for (int i = 0; i < waypoints.Count; i++)
        {
            Vector2 pos;
            pos = Handles.PositionHandle(waypoints[i].position, Quaternion.identity);
            waypoints[i] = new(pos, waypoints[i].InTangent, waypoints[i].OutTangent, waypoints[i].Mode);
            Undo.RecordObject(this, "Modifies curve");
        }
    }

    /// <summary>
    /// 引用判等
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        return obj is BezierPath path &&
               EqualityComparer<List<BezierWaypoint>>.Default.Equals(waypoints, path.waypoints) &&
               EqualityComparer<List<BezierCurve>>.Default.Equals(segments, path.segments);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(waypoints, segments, Precision);
    }

    /// <summary>
    /// 数值判等
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(BezierPath other)
    {
        if (other.waypoints.Count != waypoints.Count) return false;
        else if (other.segments.Count != segments.Count) return false;

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i].position != other.waypoints[i].position ||
                waypoints[i].InTangent != other.waypoints[i].InTangent ||
                waypoints[i].OutTangent != other.waypoints[i].OutTangent)
                return false;
        }

        for (int i = 0; i < segments.Count; i++)
        {
            if (segments[i].start != other.segments[i].start ||
                segments[i].control1 != other.segments[i].control1 ||
                segments[i].control2 != other.segments[i].control2 ||
                segments[i].end != other.segments[i].end)
                return false;
        }

        return true;
    }
}

[Serializable]
public class BezierCurve: IEquatable<BezierCurve>
{
    public Vector2 start, control1, control2, end;
    public int sampleCount = 60;
    public float length = 0;
    public List<float> samples = new(64);

    public BezierCurve(in Vector2 start, in Vector2 p1, in Vector2 p2, in Vector2 end, int sampleCount)
    {
        this.start = start;
        control1 = p1;
        control2 = p2;
        this.end = end;
        this.sampleCount = sampleCount;
        UpdateSamples(sampleCount);
    }

    public BezierCurve(in BezierWaypoint waypoint1, in BezierWaypoint waypoint2, int sampleCount)
    {
        start = waypoint1.position;
        control1 = waypoint1.OutTangent + start;
        end = waypoint2.position;
        control2 = end - waypoint2.InTangent;
        this.sampleCount = sampleCount;
        UpdateSamples(sampleCount);
    }

    /// <summary>
    /// 计算曲线上参数t对应的点的坐标
    /// </summary>
    /// <param name="t">spline value, 样条曲线参数</param>
    /// <returns></returns>
    public Vector2 Evaluate(float t)
    {
        t = Mathf.Clamp01(t);//处理不合法值

        // 根据三次贝塞尔曲线的方程计算给定t时刻的点位置
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector2 point = uuu * start; // P0 * (1-t)^3
        point += 3 * uu * t * control1; // + 3 * P1 * t * (1-t)^2
        point += 3 * u * tt * control2; // + 3 * P2 * t^2 * (1-t)
        point += ttt * end; // + P3 * t^3

        return point;
    }

    /// <summary>
    /// 计算参数t对应点处的切向量
    /// </summary>
    /// <param name="t">spline value, 样条曲线参数</param>
    /// <returns>切向量（未归一化）</returns>
    public Vector2 EvaluateTangent(float t)
    {
        t = Mathf.Clamp01(t);//处理不合法值

        float u = 1 - t;
        Vector2 tan = (control1 - start) * u * u * 3;
        tan += (control2 - control1) * t * u * 6;
        tan += (end - control2) * t * t * 3;
        return tan;
    }

    /// <summary>
    /// 计算参数t对应点处到起点的路程
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public float EvaluateLength(float t)
    {
        t = Mathf.Clamp01(t);
        float dt = 1f / sampleCount;
        int passedCount = (int)(t / dt);
        float remaining = t - passedCount * dt;
        float len = 0;
        int i = 0;
        for (; i < passedCount; i++)
        {
            len += samples[i];
        }
        len += samples[i] * (remaining / dt);
        return len;
    }

    /// <summary>
    /// 计算路程length对应的参数t
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public float EvaluateArgument(float length)
    {
        length = Mathf.Clamp(length, 0, this.length);
        float res = 0;
        float dt = 1f / sampleCount;

        int i = 0; float len;
        for (len = 0; len < length; i++)
        {
            len += samples[i];
            res += dt;
        }
        /*if (len - length > samples[i - 1] * 0.5f) res-=dt;*/
        res -= ((len - length) / samples[i - 1]) * dt;
        return res;
    }

    /// <summary>
    /// 计算在给定的点构成的贝塞尔曲线上，参数t对应的点的坐标
    /// </summary>
    /// <param name="t">spline value, 样条曲线参数</param>
    /// <param name="p0">起点</param>
    /// <param name="p1">控制点1</param>
    /// <param name="p2">控制点2</param>
    /// <param name="p3">终点</param>
    /// <returns></returns>
    public static Vector2 Evaluate(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        t = Mathf.Clamp01(t); //处理不合法值

        // 根据三次贝塞尔曲线的方程计算给定t时刻的点位置
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector2 point = uuu * p0; // P0 * (1-t)^3
        point += 3 * uu * t * p1; // + 3 * P1 * t * (1-t)^2
        point += 3 * u * tt * p2; // + 3 * P2 * t^2 * (1-t)
        point += ttt * p3; // + P3 * t^3

        return point;
    }

    /// <summary>
    /// 计算在给定的点构成的贝塞尔曲线上，参数t对应点处的切向量
    /// </summary>
    /// <param name="t">spline value, 样条曲线参数</param>
    /// <param name="p0">起点</param>
    /// <param name="p1">控制点1</param>
    /// <param name="p2">控制点2</param>
    /// <param name="p3">终点</param>
    /// <returns>切向量（未归一化）</returns>
    public static Vector2 EvaluateTangent(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        t = Mathf.Clamp01(t); //处理不合法值
        return (-3f * p0 + 9f * p1 - 9f * p2 + 3f * p3) * (t * t)
            + (6f * p0 - 12f * p1 + 6f * p2) * t
            - 3f * p0 + 3f * p1;
    }
    
    /// <summary>
    /// 更新取样点数组
    /// </summary>
    /// <param name="sampleCount"></param>
    public void UpdateSamples(float sampleCount)
    {
        samples?.Clear();
        length = 0;
        float dt = 1f / sampleCount;
        float len;
        Vector2 current = start;
        for (int i = 1; i <= sampleCount; i++)
        {
            Vector2 next = Evaluate(i * dt);
            len = (next - current).magnitude;
            samples.Add(len);
            length += len;
            current = next;
        }
    }

    public void Render(in Color color)
    {

        Vector2 start, end;
        start = this.start;
        float dt = 1f / sampleCount;

        for (int i = 0; i < sampleCount; i++)
        {
            end = Evaluate(dt * (i + 1));
            Handles.color = color;
            Handles.DrawLine(start, end);
            start = end;
        }
    }

    /// <summary>
    /// 判定逻辑相等
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(BezierCurve other)
    {
        return start.Equals(other.start) &&
               control1.Equals(other.control1) &&
               control2.Equals(other.control2) &&
               end.Equals(other.end);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(start, control1, control2, end, sampleCount, length, samples);
    }
}

[Serializable]
public struct BezierWaypoint: IEquatable<BezierWaypoint>
{
    public Vector2 position;
    [SerializeField] Vector2 inTangent;
    [SerializeField] Vector2 outTangent;
    [SerializeField] TangentMode mode;

    public Vector2 InTangent 
    { 
        get 
        {
            return inTangent;
        } 
        set 
        {
            inTangent = value;
            if (mode == TangentMode.Smooth)
                outTangent = outTangent.magnitude * inTangent.normalized;
        } 
    }
    public Vector2 OutTangent { 
        get 
        {
            return outTangent;
        } 
        set
        {
            outTangent = value;
            if (mode == TangentMode.Smooth)
                inTangent = inTangent.magnitude * outTangent.normalized;
        } 
    }
    public TangentMode Mode
    {
        get
        {
            return mode;
        }
        set
        {
            mode = value;
            if (value == TangentMode.Smooth)
                inTangent = inTangent.magnitude * outTangent.normalized;
        }

    }

    public BezierWaypoint(in Vector2 position, in Vector2 inTangent, in Vector2 outTangent, TangentMode mode)
    {
        this.position = position;
        this.inTangent = inTangent;
        this.outTangent = outTangent;
        this.mode = mode;
        if(mode == TangentMode.Smooth)
            this.inTangent = inTangent.magnitude * outTangent.normalized;
    }

    public bool Equals(BezierWaypoint other)
    {
        return other.position == position && other.OutTangent == OutTangent
        && other.InTangent == InTangent && other.Mode == Mode;
    }
}


[Serializable]
public enum TangentMode { Smooth, Broken }
