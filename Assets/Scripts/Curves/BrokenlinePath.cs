using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrokenlinePath : DanmakuPath
{
    public List<Vector2> waypoints = new();
    public List<Vector2> segments = new();

    public override Vector2 Evaluate(float t)
    {
        t = Mathf.Clamp(t, 0, waypoints.Count - 1);

        int seg = (int)t;
        float remain = t - seg;

        if (Mathf.Approximately(t, waypoints.Count - 1)) return waypoints[seg];
        return waypoints[seg] + segments[seg] * remain;
    }

    public override float EvaluateArgument(float length)
    {
        length = Mathf.Clamp(length, 0, this.length);

        float res = 0;
        int i = 0; float len;

        for (len = 0; len < length; i++)
        {
            len += segments[i].magnitude;
            res++;
        }
        res -= (len - length) / segments[i - 1].magnitude;
        return res;
    }

    public override float EvaluateLength(float t)
    {
        t = Mathf.Clamp(t, 0, waypoints.Count - 1);
        int seg = (int)t;
        float remain = t - seg;

        float len = 0;
        for (int i = 0; i < seg; i++)
        {
            len += segments[i].magnitude;
        }
        len += segments[seg-1].magnitude * remain;

        return len;
    }

    public override Vector2 EvaluateTangent(float t)
    {
        t = Mathf.Clamp(t, 0, waypoints.Count - 1);
        int seg = (int)t;

        if (Mathf.Approximately(t, waypoints.Count - 1)) return segments[^1];
        return segments[seg];
    }

    public void UpdateSegments()
    {
        segments.Clear();
        for (int i = 0; i < waypoints.Count-1; i++)
            segments.Add((waypoints[i + 1] - waypoints[i]));
        length = EvaluateLength(waypoints.Count - 1);
    }
}
