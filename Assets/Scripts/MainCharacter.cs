using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCharacter: MonoBehaviour
{
    [HideInInspector]public float movingSpeed;
    [SerializeField] float defaultSpeed;
    [SerializeField] float slowSpeed;

    Vector2 current, next;
    PlayerInput playerInput;

    private void Awake()
    {
        playerInput = PlayerInput.Instance;

        playerInput.Direction_Event += Move;

        playerInput.OkKey_Event += Shoot;
        playerInput.CancelKeyDown_Event += Bomb;
        playerInput.ShiftKeyDown_Event += EnableSlow;
        playerInput.ShiftKeyUp_Event += UnableSlow;
    }

    private void Update()
    {
        Debug.DrawLine(Vector3.left, Vector3.up, Color.magenta, 1f);
    }

    private void Move(Vector2 vector2)
    {
        current = transform.position;
        transform.Translate(vector2 * (movingSpeed * Time.timeScale));
        next = transform.position;
        Debug.DrawLine(current, next, Color.magenta, 1f);
    }
    void Shoot()
    {

    }

    void Bomb()
    {
        
    }

    void EnableSlow() => movingSpeed = slowSpeed;

    void UnableSlow() => movingSpeed = defaultSpeed;

}
