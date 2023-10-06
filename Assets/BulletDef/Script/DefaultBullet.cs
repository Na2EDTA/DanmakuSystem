using UnityEngine;
using System;

public class DefaultBullet : DanmakuObject 
{
    MainCharacter ch;
    float a, vmax, rot;
    bool hasvmax = false;
    bool hasa = false;

    override protected void Awake()
    {
        base.Awake();
    }

    private void OnEnable()
    {
        velocity.method = GameEnum.MovingMethod.Polar;
        velocity.magnitude = variables["speed"];
        velocity.Angle = variables["angle"];

        ch = FindObjectOfType<MainCharacter>();
        if ((int)variables["aim"] >= 1)
        {
            velocity.target = ch.transform.position;
            velocity.IsAiming = true;
        }
        vmax = variables["maxSpeed"];
        if (Mathf.Approximately(vmax, 0)) hasvmax = false;
        else hasvmax = true;
        a = variables["acceleration"];
        if (Mathf.Approximately(a, 0)) hasa = false;
        else hasa = true;
        rot = variables["rotation"];

    }

    private void Update()
    {
        if (hasa)
        {
            velocity.magnitude += a * Time.timeScale;
            if (hasvmax)
            {
                velocity.magnitude = Mathf.Clamp(velocity.magnitude, -vmax, vmax);
            }
        }

        if(!velocity.isFollowingTangent)
            tr.Rotate(rot * Time.timeScale * Vector3.forward);
    }
}
