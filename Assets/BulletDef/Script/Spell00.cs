using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class Spell00 : DanmakuObject
{
    float a = 0, da = 0, dda = 0, v;
    int t = 0;
    [SerializeField] BulletStyle bulletStyle;
    UniTask task;
    CancellationToken cancellationToken;
    CancellationTokenSource cancellationTokenSource = new();

    private void Start()
    {
        dda = variables["dda"];
        t = (int)variables["interval"];
        v = variables["velocity"];
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
            await UniTask.DelayFrame(t, PlayerLoopTiming.Update, cancellationToken);
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
