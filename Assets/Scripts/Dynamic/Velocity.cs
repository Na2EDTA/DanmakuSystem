using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MovingMethod = GameEnum.MovingMethod;

[DisallowMultipleComponent]
public class Velocity : MonoBehaviour
{
    static float scale = 100;

    [Header("切线跟随选项")]
    public bool isFollowingTangent = true;//如果为FALSE，速度本身的方向和贴图朝向无关
    public float followAngle = -90;

    [Header("自机狙选项")] [SerializeField] bool isAiming = false;
    public Vector2 target;

    [Header("运动学选项")]
    public MovingMethod method = MovingMethod.Polar;
    public float x;
    public float y;
    public float magnitude; 
    [SerializeField] float angle;
    public float length = 0;
    public DanmakuPath path;
    Transform tr;
    Vector2 initialPos;
    float initialAngle;

    public float Angle
    {
        get
        {
            return angle;
        }
        set
        {
            angle = value;
            angle %= 360f;
        }
    }

    public bool IsAiming
    {
        get
        {
            return isAiming;
        }

        set
        {
            isAiming = value;
            if (!value) return;

            Vector2 pos = transform.position;
            Vector2 dir = target - pos;
            MovingMethod m = method; 
            SwitchMethod(MovingMethod.Polar);
            angle = MathF.Atan2(dir.y, dir.x) * radDivPi + angle;//更改角度，使之相对于目标的速度等于原值
            SwitchMethod(m);
        }
    }

    const float pi = MathF.PI;
    public const float radDivPi = 180f / pi;

    private void Awake()
    {
        tr = transform;
        IsAiming = isAiming;
    }

    private void OnEnable()
    {
        initialPos = tr.position;
        initialAngle = tr.eulerAngles.z/radDivPi;
    }

    private void Update()   //根据不同坐标系进行位置更新、切线追踪
    {
        switch (method)
        {
            case MovingMethod.Cartesian://直角坐标系
                tr.position += (new Vector3(x, y) * Time.timeScale / scale);

                magnitude = MathF.Sqrt(x * x + y * y);
                Angle = MathF.Atan2(y, x) * radDivPi;

                break;

            case MovingMethod.Polar://极坐标系
                float a = Angle / radDivPi;
                tr.position +=
                    new Vector3(MathF.Cos(a), MathF.Sin(a)) * (magnitude * Time.timeScale / scale);

                x = magnitude * MathF.Cos(a);
                y = magnitude * MathF.Sin(a);

                break;

            case MovingMethod.Natural://自然坐标系
                float arg;
                if(length >= path.length) 
                {
                    SwitchMethod(MovingMethod.Polar);
                    return;
                }
                length += magnitude * Time.timeScale / scale;
                arg = path.EvaluateArgument(length);
                Vector2 ori = path.Evaluate(arg);
                float _x = MathF.Cos(initialAngle) * ori.x - MathF.Sin(initialAngle) * ori.y;
                float _y = MathF.Sin(initialAngle) * ori.x + MathF.Cos(initialAngle) * ori.y;
                tr.position = new Vector2(_x,_y) + initialPos;
                Vector2 tan;
                tan = path.EvaluateTangent(arg);
                Angle = (MathF.Atan2(tan.y, tan.x) + initialAngle) * radDivPi;
                x = magnitude * MathF.Cos(Angle/radDivPi);
                y = magnitude * MathF.Sin(Angle/radDivPi);
                break;

            default:
                break;
        }

        if (isFollowingTangent)
            FollowTangent(followAngle);
    }

    // 直角坐标系 <----> 极坐标系 <---- 自然坐标系
    public void SwitchMethod(MovingMethod method)
    {
        if (method == this.method) return;

        switch (method) 
        {
            case MovingMethod.Cartesian:

                float _magnitude = magnitude;
                float _angle = Angle;

                this.method = method;

                float _a = _angle / radDivPi;
                x = _magnitude * MathF.Cos(_a);
                y = _magnitude * MathF.Sin(_a);

                break;

            case MovingMethod.Polar:
                if (this.method == MovingMethod.Natural)
                {
                    this.method = method;
                    break;
                }
                float _x = x;
                float _y = y;

                this.method = method;

                magnitude = MathF.Sqrt(_x * _x + _y * _y);
                Angle = MathF.Atan2(_y, _x) * radDivPi;
                break;
        }
    }

    void FollowTangent(float a)
    {
        float angle;
        if (method == MovingMethod.Cartesian) angle = MathF.Atan2(y, x) * radDivPi;
        else angle = Angle;
        transform.rotation = Quaternion.Euler(0, 0, angle + a);
    }

    /// <summary>
    /// 简单地初始化速度，不包含切线追踪情况
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="method"></param>
    /// <param name="follow"></param>
    /// <param name="followAngle"></param>
    public void SetVelocity(float a, float b, MovingMethod method)
    {
        switch (method)
        {
            case MovingMethod.Cartesian:
                SwitchMethod(MovingMethod.Cartesian);
                x = a; y = b;
                break;
            case MovingMethod.Polar:
                SwitchMethod(MovingMethod.Polar);
                magnitude = a; Angle = b;
                break;
            default:
                return;
        }
    }

    [ContextMenu("SwitchToAnotherMethod")]
    public void SwitchMethodTest()
    {
        if (method == MovingMethod.Cartesian) SwitchMethod(MovingMethod.Polar);
        else if (method == MovingMethod.Polar) SwitchMethod(MovingMethod.Cartesian);
    }

    /// <summary>
    /// 初始化速度
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="method"></param>
    /// <param name="follow"></param>
    /// <param name="followAngle"></param>
    public void SetVelocity(float a, float b, MovingMethod method, bool follow, float followAngle = -90f)
    {
        this.method = method;
        SetVelocity(a, b, method);
        isFollowingTangent = follow;
        this.followAngle = followAngle;
    }

    public void SetTarget(Vector2 tar)
    {
        target = tar;
        IsAiming = true;
        return;
    }

    [ContextMenu("Aiming")]
    public void TargetTest1() => IsAiming = true;
    [ContextMenu("Aimless")]
    public void TargetTest2() => IsAiming = false;
}
