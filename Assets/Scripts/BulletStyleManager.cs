using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletStyleManager : MonoBehaviour
{
    public static BulletStyleManager instance;

    [SerializeField] List<Sprite> red;
    [SerializeField] List<Sprite> deepRed;
    [SerializeField] List<Sprite> purple;
    [SerializeField] List<Sprite> deepPurple;
    [SerializeField] List<Sprite> blue;
    [SerializeField] List<Sprite> deepBlue;
    [SerializeField] List<Sprite> deepCyan;
    [SerializeField] List<Sprite> cyan;
    [SerializeField] List<Sprite> deepGreen;
    [SerializeField] List<Sprite> green;
    [SerializeField] List<Sprite> chartreuse;
    [SerializeField] List<Sprite> yellow;
    [SerializeField] List<Sprite> goldenYellow;
    [SerializeField] List<Sprite> orange;
    [SerializeField] List<Sprite> black;
    [SerializeField] List<Sprite> white;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// 查询贴图并输出对应判定半径
    /// </summary>
    /// <param name="color"></param>
    /// <param name="shape"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public Sprite GetStyle(BulletColor color, BulletShape shape, out float radius)
    {
        List<Sprite> list;
        int index;

        switch (shape)
        {
            case BulletShape.NULL: radius = 0; break;
            case BulletShape.ARROW_BIG: radius = 2.4f; break;
            case BulletShape.ARROW_MID: radius = 4; break;
            case BulletShape.ARROW_SMALL: radius = 2.4f; break;
            case BulletShape.GUN_BULLET:radius = 2.4f; break;
            case BulletShape.BUTTERFLY: radius = 7; break;
            case BulletShape.SQUARE: radius = 2.8f; break;
            case BulletShape.BALL_SMALL: radius = 2.4f; break;
            case BulletShape.BALL_MID: radius = 4; break;
            case BulletShape.BALL_MID_C: radius = 4; break;
            case BulletShape.BALL_BIG: radius = 8.5f; break;
            case BulletShape.BALL_HUGE: radius = 14; break;
            case BulletShape.BALL_LIGHT: radius = 12; break;
            case BulletShape.STAR_SMALL: radius = 4; break;
            case BulletShape.STAR_BIG: radius = 7; break;
            case BulletShape.GRAIN: radius = 2.4f; break;
            case BulletShape.GRAIN_HARD: radius = 2.4f; break;
            case BulletShape.GRAIN_DARK: radius = 2.4f; break;
            case BulletShape.DROP: radius = 2.4f; break;
            case BulletShape.KNIFE: radius = 6; break;
            case BulletShape.KNIFE_B: radius = 6; break;
            case BulletShape.WATER: radius = 4; break;
            case BulletShape.FUNGUS: radius = 2.4f; break;
            case BulletShape.ECLIPSE: radius = 7; break;
            case BulletShape.HEART: radius = 10; break;
            case BulletShape.MONEY: radius = 4; break;
            case BulletShape.NOTE: radius = 4; break;
            case BulletShape.REST: radius = 5; break;
            case BulletShape.WATER_DARK: radius = 4; break;
            case BulletShape.BALL_HUGE_DARK: radius = 14; break;
            case BulletShape.BALL_LIGHT_DARK: radius = 12; break;
            default: radius = 2.4f; break;
        }

        radius /= 100;

        switch (color)
        {
            case BulletColor.RED: list = red; break;
            case BulletColor.DEEP_RED:list = deepRed;break;
            case BulletColor.PURPLE: list = purple; break;
            case BulletColor.DEEP_PURPLE:list = deepPurple; break;
            case BulletColor.BLUE: list = blue; break;
            case BulletColor.DEEP_BLUE: list = deepBlue;break;
            case BulletColor.DEEP_CYAN: list = deepCyan;break;
            case BulletColor.CYAN: list = cyan; break;
            case BulletColor.DEEP_GREEN:list = deepGreen;break;
            case BulletColor.GREEN: list = green; break;
            case BulletColor.CHARTREUSE:list = chartreuse;break;
            case BulletColor.YELLOW: list = yellow; break;
            case BulletColor.GOLDEN_YELLOW:list = goldenYellow;break;
            case BulletColor.ORANGE: list = orange; break;
            case BulletColor.BLACK: list = black; break;
            case BulletColor.WHITE: list = white; break;
            default: return null;

        }

        index = (int)shape;

        if (index <= 30 && index > 0)
        {
            return list[index-1];
            
        }
        else
            return null;
    }

}
