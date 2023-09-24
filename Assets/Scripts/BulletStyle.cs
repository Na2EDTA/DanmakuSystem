using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct BulletStyle
{
    public BulletColor color;
    public BulletShape shape;
}

public enum BulletColor: short
{
    NULL, RED, DEEP_RED, PURPLE, DEEP_PURPLE,
    BLUE, DEEP_BLUE, DEEP_CYAN, CYAN,
    DEEP_GREEN, GREEN, CHARTREUSE,
    YELLOW, GOLDEN_YELLOW, ORANGE,
    BLACK, WHITE
}

public enum BulletShape: short
{
    NULL, ARROW_BIG, ARROW_MID, ARROW_SMALL, GUN_BULLET, BUTTERFLY, SQUARE,
    BALL_SMALL, BALL_MID, BALL_MID_C, BALL_BIG, BALL_HUGE, BALL_LIGHT,
    STAR_SMALL, STAR_BIG, GRAIN, GRAIN_HARD, GRAIN_DARK, DROP,
    KNIFE, KNIFE_B, WATER, FUNGUS, ECLIPSE, HEART, MONEY, NOTE, REST,
    WATER_DARK, BALL_HUGE_DARK, BALL_LIGHT_DARK
}