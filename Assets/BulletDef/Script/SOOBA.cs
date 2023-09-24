using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Method = GameEnum.MovingMethod;

public class SOOBA : DanmakuObject
{
    public override void InitParams(params float[] ps)
    {
        base.InitParams(ps);
    }

    void OnEnable()
    {
        Method method = velocity.method;
        velocity.SwitchMethod(Method.Polar);
        velocity.Angle = variables["da"];
        velocity.SwitchMethod(method);
    }

}
