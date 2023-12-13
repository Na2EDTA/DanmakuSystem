using System;
using System.Linq.Expressions;

namespace Danmaku.BehaviourTree
{
    public class ExpressionUtility
    {
        /// <summary>
        /// ����ָ���ֶε���������Setter��ί�С�
        /// </summary>
        /// <typeparam name="T">�������͡�</typeparam>
        /// <param name="fieldName">�ֶ����ơ�</param>
        /// <returns>ָ���ֶε�������ί�С�</returns>
        public static Action<T, object> CreateSetter<T>(string fieldName)
        {
            var objParam = Expression.Parameter(typeof(T), "obj");
            var valueParam = Expression.Parameter(typeof(object), "value");

            var fieldExpr = Expression.Field(objParam, fieldName);
            var valueExpr = Expression.Convert(valueParam, fieldExpr.Type);

            var assignExpr = Expression.Assign(fieldExpr, valueExpr);

            var lambda = Expression.Lambda<Action<T, object>>(assignExpr, objParam, valueParam);

            return lambda.Compile();
        }

        /// <summary>
        /// ����ָ���ֶεĻ�ȡ����Getter��ί�С�
        /// </summary>
        /// <typeparam name="T">�������͡�</typeparam>
        /// <param name="fieldName">�ֶ����ơ�</param>
        /// <returns>ָ���ֶεĻ�ȡ��ί�С�</returns>
        public static Func<T, object> CreateGetter<T>(string fieldName)
        {
            var objParam = Expression.Parameter(typeof(T), "obj");

            var fieldExpr = Expression.Field(objParam, fieldName);
            var castExpr = Expression.Convert(fieldExpr, typeof(object));

            var lambda = Expression.Lambda<Func<T, object>>(castExpr, objParam);

            return lambda.Compile();
        }

        /// <summary>
        /// ����ָ���ֶε���������Setter��ί�С�
        /// </summary>
        /// <param name="fieldName">�ֶ����ơ�</param>
        /// <param name="objectType">�������͡�</param>
        /// <returns>ָ���ֶε�������ί�С�</returns>
        public static Action<object, object> CreateSetter(string fieldName, Type objectType)
        {
            var objParam = Expression.Parameter(typeof(object), "obj");
            var valueParam = Expression.Parameter(typeof(object), "value");

            //��objectת����objectType�������
            var castObjExpr = Expression.Convert(objParam, objectType);
            //���������ɵ�������ȡ��Աfield
            var fieldExpr = Expression.Field(castObjExpr, fieldName);
            //��valueת����field�����ͣ����ܲ��䣩
            var castValueExpr = Expression.Convert(valueParam, fieldExpr.Type);

            //��ת����field��Ӧ����֮���value����field
            var assignExpr = Expression.Assign(fieldExpr, castValueExpr);

            var lambda = Expression.Lambda<Action<object, object>>(assignExpr, objParam, valueParam);

            return lambda.Compile();
        }

        /// <summary>
        /// ����ָ���ֶεĻ�ȡ����Getter��ί�С�
        /// </summary>
        /// <param name="fieldName">�ֶ����ơ�</param>
        /// <param name="objectType">�������͡�</param>
        /// <returns>ָ���ֶεĻ�ȡ��ί�С�</returns>
        public static Func<object, object> CreateGetter(string fieldName, Type objectType)
        {
            var objParam = Expression.Parameter(typeof(object), "obj");

            var castObjExpr = Expression.Convert(objParam, objectType);
            var fieldExpr = Expression.Field(castObjExpr, fieldName);
            var castExpr = Expression.Convert(fieldExpr, typeof(object));

            var lambda = Expression.Lambda<Func<object, object>>(castExpr, objParam);

            return lambda.Compile();
        }
    }

    public class ReflexiveAccessor
    {
        public Func<object, object> getter;
        public Action<object, object> setter;

        public ReflexiveAccessor(string fieldName, Type objectType)
        {
            getter = ExpressionUtility.CreateGetter(fieldName, objectType);
            setter = ExpressionUtility.CreateSetter(fieldName, objectType);
        }
    }
}