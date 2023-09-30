using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System.Linq.Expressions;
using System;

public static class DanmakuEmission
{
    /// <summary>
    /// 生成单个简单子弹
    /// <para>args[]: angle, speed, aim(bool), maxSpeed, acceleration, rotation</para>
    /// </summary>
    /// <param name="style"></param>
    /// <param name="pos"></param>
    /// <param name="args">angle, speed, aim(bool), maxSpeed, acceleration, rotation</param>
    /// <returns></returns>
    public static GameObject CreateSimpleBullet(BulletStyle style, Vector2 pos, params float[] args)
    {
        GameObject obj = Pool.instance?.Create("Simple", pos, args);
        DanmakuCollider collider = obj.GetComponent<DanmakuCollider>();
        obj.GetComponent<SpriteRenderer>().sprite = BulletStyleManager.instance?.GetStyle(style.color,style.shape, out collider.mainArg);
        return obj;
    }

    /// <summary>
    /// 生成多个简单子弹
    /// <para>args[]: aim(bool), maxSpeed, acceleration, rotation</para>
    /// </summary>
    /// <param name="style"></param>
    /// <param name="pos"></param>
    /// <param name="args">angle, speed, aim(bool), maxSpeed, acceleration, rotation</param>
    /// <returns></returns>
    public static async UniTask CreateSimpleBulletsAsync(int num, int interval,float angle, float angleSpread, float speedStart, float speedEnd, BulletStyle style, Vector2 pos, params float[] args)
    {
        
        float speedSpread = speedEnd - speedStart;
        float a, v;
        for (int i = 0; i < num; i++)
        {
            a = angle - angleSpread * 0.5f + angleSpread / (num - 1) * i;//angle iter
            v = speedStart + speedSpread / (num - 1) * i;//speed iter
            await AsyncCreateSimpleBullet(interval, style, pos, a, v, args[0], args[1], args[2], args[3]);
        }
        return;
    }

    async static UniTask AsyncCreateSimpleBullet(int interval, BulletStyle style, Vector2 pos, params float[] args)
    {
        await UniTask.DelayFrame(interval);
        CreateSimpleBullet(style, pos, args);
    }

}
