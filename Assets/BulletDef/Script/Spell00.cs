using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Spell00 : DanmakuObject
{
    float a = 0, da = 0;
    [SerializeField] float dda = 0, v;
    [SerializeField]int interval = 0;
    [SerializeField] BulletStyle bulletStyle;
    UniTask task;
    CancellationToken cancellationToken;
    CancellationTokenSource cancellationTokenSource = new();

    public override void OnInit(params float[] ps)
    {
        dda = ps[0];
        interval = (int)ps[1];
        v = ps[2];
    }

    public override void Dispose()
    {
        Pool.instance.Dispose<Spell00>(gameObject);
    }

    private void OnEnable()
    {
        cancellationToken = cancellationTokenSource.Token;

        task = UniTask.Create(()=>Emit(cancellationToken));
    }

    private void Update()
    {
        da += dda;
        a += da;
        a %= 360;
    }

    async UniTask Emit(CancellationToken cancellationToken)
    {
        while (true)
        {
            for (int i = 0; i < 5; i++)
            {
                DanmakuEmission.CreateSimpleBullet
                    (bulletStyle, tr.position, a + 72 * i, v, 0, 0, 0, 0);
            }
            await UniTask.DelayFrame(interval, PlayerLoopTiming.Update, cancellationToken);
        }
    }

    private void OnDisable()
    {
        try
        {
            cancellationTokenSource.Cancel();
        }
        catch
        {

        }
    }

}
