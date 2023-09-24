using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnum
{
    

    public enum MovingMethod: byte { Cartesian, Polar, Natural }

    public enum ColliderType:byte { Circle, Rectangle, Linear}

    public enum PlayersDevice:byte { keyboard, joystick, XR}

    public enum SpellType : byte { common, survival}
}
