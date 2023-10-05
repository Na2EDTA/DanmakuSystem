using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] Text fpsUI;
    public float fps;
    int intTimer = 0;
    float timeRecorder = 0;
    UniTask fpsTask;

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
        fpsTask = UniTask.Create(FpsInUI);
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

    async UniTask FpsInUI()
    {
        while (true)
        {
            fpsUI.text = fps.ToString("F2");
            await UniTask.Delay(500);
        }
    }
}
