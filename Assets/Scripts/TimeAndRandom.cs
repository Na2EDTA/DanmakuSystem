using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using System;

public class TimeAndRandom : MonoBehaviour
{
    static DateTime dateTime;
    static int seed = 0;
    void Start()
    {
        RandomSeed();
    }

    public static void RandomSeed()
    {
        dateTime = DateTime.Now;
        seed = (int)dateTime.Ticks;
        Random.InitState(seed);
    }
}
