using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float fps;
    int intTimer = 0;
    float timeRecorder = 0;

    private void Awake()
    {
        Application.targetFrameRate = 60;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void OnEnable()
    {
        intTimer = 0;
        timeRecorder = Time.time;
    }

    private void Update()
    {
        intTimer++;
        float dt = Time.time - timeRecorder;
        fps = 1 / dt;
        timeRecorder = Time.time;
    }

}
