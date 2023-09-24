using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InputTest : MonoBehaviour
{
    float horizontal;
    float vertical;
    [SerializeField]float sensitivity;

    private void Awake()
    {
        
/*        PlayerInput.Ins.LeftKey_Event += Left;
        PlayerInput.Ins.RightKey_Event += Right;
        PlayerInput.Ins.UpKey_Event += Up;
        PlayerInput.Ins.DownKey_Event += Down;
*/      PlayerInput.Instance.ShiftKeyDown_Event += ShiftDown;
        PlayerInput.Instance.ShiftKeyUp_Event += ShiftUp;
        PlayerInput.Instance.Direction_Event += Move;
        /*PlayerInput.Ins.CancelKey_Event += Cancel;
        PlayerInput.Ins.OkKey_Event += OK;*/
    }

    private void Move(Vector2 vector) => transform.Translate(vector * sensitivity);

    /*    public void Left() => transform.Translate(Vector2.left * sensitivity);
        public void Right() => transform.Translate(Vector2.right * sensitivity);
        public void Up() => transform.Translate(Vector2.up * sensitivity);
        public void Down() => transform.Translate(Vector2.down * sensitivity);
    */
    public void ShiftDown() => sensitivity = 0.5f * sensitivity;
    public void ShiftUp() => sensitivity = 2 * sensitivity;

/*    public void Cancel()
    {
        PlayerInput.Ins.LeftKey_Event -= Left;
        PlayerInput.Ins.RightKey_Event -= Right;
        PlayerInput.Ins.UpKey_Event -= Up;
        PlayerInput.Ins.DownKey_Event -= Down;
    }

    public void OK()
    {
        PlayerInput.Ins.LeftKey_Event += Left;
        PlayerInput.Ins.RightKey_Event += Right;
        PlayerInput.Ins.UpKey_Event += Up;
        PlayerInput.Ins.DownKey_Event += Down;
    }
*/
}
