using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace HyrphusQ.Helpers
{
    public static class ReflectionHelper
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

        #region Extension
        public static T GetFieldValue<T>(this object obj, string fieldName)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            Type type = obj.GetType();
            FieldInfo fieldInfo = null;
            while (fieldInfo == null && type != null)
            {
                fieldInfo = type.GetField(fieldName, BINDING_FLAGS);
                type = type.BaseType;
            }
            if (fieldInfo == null)
                throw new ArgumentOutOfRangeException("fieldName", string.Format("Field {0} was not found in Type {1}", fieldInfo, obj.GetType().FullName));
            return (T)fieldInfo.GetValue(obj);
        }
        public static void SetFieldValue<T>(this object obj, string fieldName, T value)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            Type type = obj.GetType();
            FieldInfo fieldInfo = null;
            while (fieldInfo == null && type != null)
            {
                fieldInfo = type.GetField(fieldName, BINDING_FLAGS);
                type = type.BaseType;
            }
            if (fieldInfo == null)
                throw new ArgumentOutOfRangeException("fieldName", string.Format("Field {0} was not found in Type {1}", fieldInfo, obj.GetType().FullName));
            fieldInfo.SetValue(obj, value);
        }
        public static T InvokeMethod<T>(this object obj, string methodName, params object[] param)
        {
            if(obj == null)
                throw new ArgumentNullException("obj");
            Type type = obj.GetType();
            MethodInfo methodInfo = null;
            while(methodInfo == null && type != null)
            {
                methodInfo = type.GetMethod(methodName, BINDING_FLAGS);
                type = type.BaseType;
            }
            if(methodInfo == null)
                throw new ArgumentOutOfRangeException("methodName", string.Format("Method {0} was not found in Type {1}", methodInfo, obj.GetType().FullName));
            return (T)methodInfo.Invoke(obj, param);
        }
        #endregion

        public static Type[] GetAllTypesInNamespace(string @namespace, Func<Type, bool> predicate = null)
        {
            var types = new List<Type>();
            var assemblyTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
            foreach (var type in assemblyTypes)
            {
                if (type.Namespace == @namespace && (predicate?.Invoke(type) ?? true))
                {
                    types.Add(type);
                }
            }
            return types.ToArray();
        }
    }
}
