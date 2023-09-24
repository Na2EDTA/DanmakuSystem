using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class IBulletAdditiveInfo
{
    
}

[Serializable]
public class STestMA01 : IBulletAdditiveInfo
{
    public int a;
    public STestMA01(int a)
    {
        this.a = a;
    }
}
