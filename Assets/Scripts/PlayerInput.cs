using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance { get; private set; }

    #region public static KeyCodesOnKeyboard
    public KeyCode left, right, up, down;
    public KeyCode ok;
    public KeyCode cancel;
    public KeyCode special;
    public KeyCode shift;
    public KeyCode skip;
    public KeyCode screenShot;
    #endregion

    #region public static KeyCodesOnJoystick
    public KeyCode ok_Joystick;
    public KeyCode cancel_Joystick;
    public KeyCode special_Joystick;
    public KeyCode shift_Joystick;
    public KeyCode skip_Joystick;
    public KeyCode screenShot_Joystick;
    #endregion

    static KeyCode temp;

    #region public static Events
    public event Action<Vector2> Direction_Event;
    public event Action OkKey_Event, OkKeyUp_Event, OkKeyDown_Event;
    public event Action CancelKey_Event, CancelKeyUp_Event, CancelKeyDown_Event;
    public event Action SpecialKey_Event, SpecialKeyUp_Event, SpecialKeyDown_Event;
    public event Action ShiftKey_Event, ShiftKeyUp_Event, ShiftKeyDown_Event;
    public event Action SkipKey_Event;
    public event Action ScreenShotKeyDown_Event;
    #endregion

    public Event currentEvent = new();

    bool isConfiging = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (isConfiging) return;

        Direction_Event?.Invoke(InputDirection());

        if (Input.GetKey(ok) || Input.GetKey(ok_Joystick)) OkKey_Event?.Invoke();
        if (Input.GetKeyUp(ok) || Input.GetKeyUp(ok_Joystick)) OkKeyUp_Event?.Invoke();
        if (Input.GetKeyDown(ok) || Input.GetKeyDown(ok_Joystick)) OkKeyDown_Event?.Invoke();

        if (Input.GetKey(cancel) || Input.GetKey(cancel_Joystick)) CancelKey_Event?.Invoke();
        if (Input.GetKeyUp(cancel) || Input.GetKeyUp(cancel_Joystick)) CancelKeyUp_Event?.Invoke();
        if (Input.GetKeyDown(cancel) || Input.GetKeyDown(cancel_Joystick)) CancelKeyDown_Event?.Invoke();

        if (Input.GetKey(special) || Input.GetKey(special_Joystick)) SpecialKey_Event?.Invoke();
        if (Input.GetKeyUp(special) || Input.GetKeyUp(special_Joystick)) SpecialKeyUp_Event?.Invoke();
        if (Input.GetKeyDown(special) || Input.GetKeyDown(special_Joystick)) SpecialKeyDown_Event?.Invoke();
       
        if (Input.GetKey(shift) || Input.GetKey(shift_Joystick)) ShiftKey_Event?.Invoke();
        if (Input.GetKeyUp(shift) || Input.GetKeyUp(shift_Joystick)) ShiftKeyUp_Event?.Invoke();
        if (Input.GetKeyDown(shift) || Input.GetKeyDown(shift_Joystick)) ShiftKeyDown_Event?.Invoke();
        
        if (Input.GetKey(skip) || Input.GetKey(skip_Joystick)) SkipKey_Event?.Invoke();
        if (Input.GetKeyDown(screenShot) || Input.GetKeyDown(screenShot_Joystick)) ScreenShotKeyDown_Event.Invoke();
    }

    private void OnGUI() => currentEvent = Event.current;

    public void StartConfig(string key)
    {
        isConfiging = true;
        switch (key)
        {
            case "left": temp = left; break;
            case "right": temp = right; break;
            case "up": temp = up; break;
            case "down": temp = down; break;

            case "ok": temp = ok; break;
            case "ok_Joystick": temp = ok_Joystick; break;

            case "cancel": temp = cancel; break;
            case "cancel_Joystick": temp = cancel_Joystick; break;

            case "special": temp = special; break;
            case "special_Joystick": temp = special_Joystick; break;

            case "shift": temp = shift; break;
            case "shift_Joystick": temp = shift_Joystick; break;

            case "skip": temp = skip; break;
            case "skip_Joystick": temp = skip_Joystick; break;

            case "screenShot": temp = screenShot; break;
            case "screenShot_Joystick": temp = screenShot_Joystick; break;

            default:
                Debug.Log("Invalid Key-Value Pair");
                break;
        }
    }

    public void SetKey()
    {        
        if (currentEvent.isKey) 
            temp = currentEvent.keyCode;
        else if(!currentEvent.isMouse)
        {
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                    temp = keyCode;
            }
        }
    }

    public void EndConfig(string key)
    {
        switch (key)
        {
            case "left": left = temp; break;
            case "right": right = temp; break;
            case "up": up = temp; break;
            case "down": down = temp; break;

            case "ok": ok = temp; break;
            case "ok_Joystick": ok_Joystick = temp; break;

            case "cancel": cancel = temp; break;
            case "cancel_Joystick": cancel_Joystick = temp; break;

            case "special": special = temp; break;
            case "special_Joystick": special_Joystick = temp; break;

            case "shift": shift = temp; break;
            case "shift_Joystick": shift_Joystick = temp; break;

            case "skip": skip = temp; break;
            case "skip_Joystick": skip_Joystick = temp; break;

            case "screenShot": screenShot = temp; break;
            case "screenShot_Joystick": screenShot_Joystick = temp; break;

            default:
                Debug.Log("Invalid Key-Value Pair");
                break;
        }

        isConfiging = false;
    }

    Vector2 InputDirection()
    {
        float x = 0, y = 0;

        
        x = Input.GetAxisRaw("JoystickHorizontal");
        y = Input.GetAxisRaw("JoystickVertical");
        

        if (Input.GetKey(left)) x --;
        if (Input.GetKey(right)) x ++;
        if (Input.GetKey(down)) y --;
        if (Input.GetKey(up)) y ++;


        return new Vector2(x, y).normalized;

        
    }
}
