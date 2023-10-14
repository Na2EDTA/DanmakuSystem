using UnityEngine;
using System;

public class Simple : DanmakuObject
{
    MainCharacter ch;
    public float angle, speed, maxSpeed, acceleration, rotation;
    bool hasvmax = false;
    bool hasa = false;
    public bool aim;

    override protected void Awake()
    {
        base.Awake();
    }

    public override void OnInit(params float[] ps)
    {
        angle = ps[0];
        speed = ps[1];
        aim = ps[2] > 0f;
        maxSpeed = ps[3];
        acceleration = ps[4];
        rotation = ps[5];

        velocity.method = GameEnum.MovingMethod.Polar;
        velocity.Angle = angle;
        velocity.magnitude = speed;

        ch = FindObjectOfType<MainCharacter>();
        if (aim)
        {
            velocity.target = ch.transform.position;
            velocity.IsAiming = true;
        }

        if (Mathf.Approximately(maxSpeed, 0)) hasvmax = false;
        else hasvmax = true;

        if (Mathf.Approximately(acceleration, 0)) hasa = false;
        else hasa = true;
    }

    public override void Dispose()
    {
        //Pool.instance.Dispose(gameObject);
        Pool.instance.Dispose<Simple>(gameObject);
    }

    private void Update()
    {
        if (hasa)
        {
            velocity.magnitude += acceleration * Time.timeScale;
            if (hasvmax)
            {
                velocity.magnitude = Mathf.Clamp(velocity.magnitude, -maxSpeed, maxSpeed);
            }
        }

        if(!velocity.isFollowingTangent)
            tr.Rotate(rotation * Time.timeScale * Vector3.forward);
    }
}
