using System;
using System.Linq.Expressions;

namespace Danmaku.BehaviourTree
{
    public class ExpressionUtility
    {
        /// <summary>
        /// 创建指定字段的设置器（Setter）委托。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="fieldName">字段名称。</param>
        /// <returns>指定字段的设置器委托。</returns>
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
        /// 创建指定字段的获取器（Getter）委托。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        /// <param name="fieldName">字段名称。</param>
        /// <returns>指定字段的获取器委托。</returns>
        public static Func<T, object> CreateGetter<T>(string fieldName)
        {
            var objParam = Expression.Parameter(typeof(T), "obj");

            var fieldExpr = Expression.Field(objParam, fieldName);
            var castExpr = Expression.Convert(fieldExpr, typeof(object));

            var lambda = Expression.Lambda<Func<T, object>>(castExpr, objParam);

            return lambda.Compile();
        }

        /// <summary>
        /// 创建指定字段的设置器（Setter）委托。
        /// </summary>
        /// <param name="fieldName">字段名称。</param>
        /// <param name="objectType">对象类型。</param>
        /// <returns>指定字段的设置器委托。</returns>
        public static Action<object, object> CreateSetter(string fieldName, Type objectType)
        {
            var objParam = Expression.Parameter(typeof(object), "obj");
            var valueParam = Expression.Parameter(typeof(object), "value");

            //从object转换到objectType里的类型
            var castObjExpr = Expression.Convert(objParam, objectType);
            //在上面所成的类型中取成员field
            var fieldExpr = Expression.Field(castObjExpr, fieldName);
            //将value转换到field的类型（可能拆箱）
            var castValueExpr = Expression.Convert(valueParam, fieldExpr.Type);

            //将转换到field对应类型之后的value赋给field
            var assignExpr = Expression.Assign(fieldExpr, castValueExpr);

            var lambda = Expression.Lambda<Action<object, object>>(assignExpr, objParam, valueParam);

            return lambda.Compile();
        }

        /// <summary>
        /// 创建指定字段的获取器（Getter）委托。
        /// </summary>
        /// <param name="fieldName">字段名称。</param>
        /// <param name="objectType">对象类型。</param>
        /// <returns>指定字段的获取器委托。</returns>
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