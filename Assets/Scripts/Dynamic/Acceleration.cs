using System;
using System.Collections.Generic;
using UnityEngine;
using MovingMethod = GameEnum.MovingMethod;

[RequireComponent(typeof(Velocity))]
public class Acceleration : MonoBehaviour
{
    public MovingMethod method;
    MovingMethod _method;

    public float x;
    public float y;
    public float tangent;
    public float normal;
    Velocity velocity;
    public const float radDivPi = Velocity.radDivPi;

    private void Awake() => velocity = GetComponent<Velocity>(); 

    void Update()
    {
        switch (method)
        {
            case MovingMethod.Cartesian:
                _method = velocity.method;                      //ʹ�л�����ϵ�󣬼��ٶȶ��ٶȵ�Ӱ��ЧӦ����
                velocity.SwitchMethod(MovingMethod.Cartesian);  //ʹ�л�����ϵ�󣬼��ٶȶ��ٶȵ�Ӱ��ЧӦ����
                velocity.x += x * Time.deltaTime;
                velocity.y += y * Time.deltaTime;

                Convert(out tangent, out normal, x, y, MovingMethod.Cartesian);
                velocity.SwitchMethod(_method);                 //ʹ�л�����ϵ�󣬼��ٶȶ��ٶȵ�Ӱ��ЧӦ����
                break;

            case MovingMethod.Polar:
                _method = velocity.method;
                velocity.SwitchMethod(MovingMethod.Polar);
                velocity.magnitude += tangent * Time.deltaTime;
                velocity.Angle += normal / velocity.magnitude * Time.deltaTime;

                Convert(out x, out y, tangent, normal, MovingMethod.Polar);
                velocity.SwitchMethod(_method);
                break;

            default:
                return;
        }
    }

    [ContextMenu("SwitchToAnotherMethod")]
    void SwitchMethodTest()
    {
        if (method == MovingMethod.Cartesian) SwitchMethod(MovingMethod.Polar);
        else if (method == MovingMethod.Polar) SwitchMethod(MovingMethod.Cartesian);
    }

    void SwitchMethod(MovingMethod method)
    {
        if (method == this.method)
            return;

        switch (method)
        {
            case MovingMethod.Cartesian:
                Convert(out tangent, out normal, x, y, MovingMethod.Cartesian);
                break;

            case MovingMethod.Polar:
                Convert(out x, out y, tangent, normal, MovingMethod.Polar);
                break;

            default:
                break;
        }
        this.method = method;
    }


    /// <summary>
    /// ���þ��������
    /// <para>1.�����ٶȵĵ�λ������</para>
    /// <para>2.�õ���xy����ϵ����Ȼ����ϵ�ı任������������</para>
    /// <para>3.�����������ڵ�ǰ�ļ��ٶ������������a, b��</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="targetMethod"></param>
    void Convert(out float a, out float b, float c, float d,MovingMethod targetMethod)
    {
        Vector2 tan;
        tan = new Vector2(velocity.x, velocity.y).normalized;

        switch (targetMethod)
        {
            case MovingMethod.Polar:
                a = c * tan.x - d * tan.y;
                b = c * tan.y + d * tan.x;
                break;

            case MovingMethod.Cartesian:
                a = c * tan.x + d * tan.y;
                b = - c * tan.y + d * tan.x;
                break;

            default:
                a = 0;
                b = 0;
                break;
        }
    }

    public void MutiplyBy(float multiplier)
    {
        x *= multiplier;
        y *= multiplier;
        tangent *= multiplier;
        normal *= multiplier;
    }
}
