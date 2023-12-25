using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Method = GameEnum.MovingMethod;

public class S00BA : DanmakuObject
{
    [SerializeField] float a;
    public override void OnInit(params float[] ps)
    {
        a = ps[0];
    }

    void OnEnable()
    {
        Method method = velocity.method;
        velocity.SwitchMethod(Method.Polar);
        velocity.SwitchMethod(method);
    }

}
