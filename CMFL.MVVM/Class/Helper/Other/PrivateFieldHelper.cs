using System;
using System.Reflection;

namespace CMFL.MVVM.Class.Helper.Other
{
    public static class PrivateFieldHelper
    {
        /// <summary>
        ///     获取对象的私有字段的值，感谢Aaron Lee Murgatroyd
        /// </summary>
        /// <typeparam name="TResult">字段的类型</typeparam>
        /// <param name="obj">要从其中获取字段值的对象</param>
        /// <param name="fieldName">字段的名称.</param>
        /// <returns>字段的值</returns>
        /// <exception cref="System.InvalidOperationException">无法找到该字段.</exception>
        public static TResult GetPrivateField<TResult>(this object obj, string fieldName)
        {
            if (obj == null) return default;
            var ltType = obj.GetType();
            var lfiFieldInfo = ltType.GetField(fieldName,
                BindingFlags.GetField | BindingFlags.Instance |
                BindingFlags.NonPublic);
            if (lfiFieldInfo != null)
                return (TResult) lfiFieldInfo.GetValue(obj);
            throw new InvalidOperationException(
                $"Instance field '{fieldName}' could not be located in object of type '{obj.GetType().FullName}'.");
        }
    }
}