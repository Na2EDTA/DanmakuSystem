using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class TestEmitter : DanmakuObject
{
    [SerializeField] BulletStyle newstyle;
    [SerializeField] BezierPath path;
    [SerializeField] float arg, len;

    public override void OnInit(params float[] ps)
    {
        
    }

    private async void Start()
    {
        DanmakuEmission.CreateSimpleBullet(style, Vector2.zero, 90, 0.2f, 0, 0, 0, 2.5f);
        await DanmakuEmission.CreateSimpleBulletsAsync(6, 10, 0, 180, 0, 0.4f, newstyle, new Vector2(-0.50f, -0.50f), 0, 0.7f, 0.002f, 0);
    }
}
