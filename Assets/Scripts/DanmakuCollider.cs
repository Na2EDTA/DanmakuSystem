using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ColliderType = GameEnum.ColliderType;

public class DanmakuCollider : MonoBehaviour
{
    public ColliderType type;
    public float mainArg;
    public float mainArg2;
    Collider2D c2d;

    public UnityEvent<Collider2D> collisionHandler;
    public UnityEvent<Collider2D> decollisionHandler;

    private void Awake()
    {
        switch (type)
        {
            case ColliderType.Circle:
                CircleCollider2D cc2d;
                if (!TryGetComponent<CircleCollider2D>(out cc2d))
                    c2d = gameObject.AddComponent<CircleCollider2D>();
                else
                    c2d = cc2d;
                break;
            case ColliderType.Rectangle:
                BoxCollider2D bc2d;
                if (!TryGetComponent<BoxCollider2D>(out bc2d))
                    c2d = gameObject.AddComponent<CircleCollider2D>();
                else
                    c2d = bc2d;
                break;
            case ColliderType.Linear:
                EdgeCollider2D ec2d;
                if (TryGetComponent<EdgeCollider2D>(out ec2d))
                    c2d = gameObject.AddComponent<EdgeCollider2D>();
                else
                    c2d = ec2d;
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        switch (type)
        {
            case ColliderType.Circle:
                (c2d as CircleCollider2D).radius = mainArg;
                break;
            case ColliderType.Rectangle:
                (c2d as BoxCollider2D).size = new Vector2(mainArg, mainArg2);
                break;
            case ColliderType.Linear:
                (c2d as EdgeCollider2D).edgeRadius = mainArg;
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collisionHandler.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        decollisionHandler.Invoke(collision);
    }

    [ContextMenu("AddCollider")]
    void AddColider()
    {
        switch (type)
        {
            case ColliderType.Circle:
                gameObject.AddComponent<CircleCollider2D>();
                break;
            case ColliderType.Rectangle:
                gameObject.AddComponent<BoxCollider2D>();
                break;
            case ColliderType.Linear:
                gameObject.AddComponent<EdgeCollider2D>();
                break;
            default:
                break;
        }
    }
}
