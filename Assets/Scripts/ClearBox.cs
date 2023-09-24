using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearBox : MonoBehaviour
{
    [SerializeField] Vector4 range;
    

    private void Awake()
    {
        
        range /= 100;
    }

    void Update()
    {
        if (transform.position.x < range.x || transform.position.x > range.y || transform.position.y < range.z
            || transform.position.y > range.w)
        {
            Pool.instance.Dispose(gameObject);
        }
    }
}
