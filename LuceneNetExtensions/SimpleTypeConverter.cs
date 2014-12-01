namespace LuceneNetExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;

    public class SimpleTypeConverter
    {
        public static object ConvertValue(Type toType, params object[] values)
        {
            // Handle Arrays
            if (toType.IsArray)
            {
                var elementType = toType.GetElementType();
                var elements = Array.CreateInstance(elementType ?? typeof(object), values.Length);

                var method = toType.GetMethod("SetValue", new[] { elementType, typeof(int) });

                if (method != null)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        var typedVal = ConvertValue(elementType, values[i]);
                        if (typedVal != null)
                        {
                            method.Invoke(elements, new[] { typedVal, i });
                        }
                    }
                }

                return elements;
            }

            // Handle Generic types such as List<T>, Collection<T>, IEnumerable<T>
            if (toType.IsGenericType)
            {
                var generictype = toType.GetGenericTypeDefinition();
                var elementType = toType.GetGenericArguments()[0];

                // Handle Nullable separatly
                if (generictype == typeof(Nullable<>))
                {
                    return ConvertSingleValue(elementType, values[0]);
                }

                // Try to create collection/list
                var elements = CreateGenericInstance(generictype, elementType);

                if (elements != null)
                {
                    var method = elements.GetType().GetMethod("Add");
                    if (method != null)
                    {
                        foreach (var val in values)
                        {
                            var typedVal = ConvertValue(elementType, val);
                            if (typedVal != null)
                            {
                                method.Invoke(elements, new[] { typedVal });
                            }
                        }
                    }
                }

                return elements;
            }

            // Handle single values, convert the first value to toType
            return ConvertSingleValue(toType, values[0]);
        }

        private static object ConvertSingleValue(Type toType, object value)
        {
            if (value is string)
            {
                var stringValue = value as string;
                if (toType == typeof(Guid))
                {
                    return new Guid(stringValue);
                }

                if (toType == typeof(float))
                {
                    return float.Parse(stringValue, CultureInfo.CurrentCulture);
                }

                if (toType == typeof(int))
                {
                    return int.Parse(stringValue);
                }

                if (toType == typeof(decimal))
                {
                    return decimal.Parse(stringValue, CultureInfo.CurrentCulture);
                }
            }

            try
            {
                return Convert.ChangeType(value, toType);
            }
            catch (Exception)
            {
                return GetDefaultValue(toType);
            }
        }

        private static object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        private static object CreateGenericInstance(Type generictype, Type elementType)
        {
            if (!generictype.IsInterface)
            {
                return Activator.CreateInstance(generictype.MakeGenericType(elementType));
            }

            if (generictype == typeof(IList<>))
            {
                return Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            }

            if (generictype == typeof(IEnumerable<>))
            {
                return Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            }

            if (generictype == typeof(ICollection<>))
            {
                return Activator.CreateInstance(typeof(Collection<>).MakeGenericType(elementType));
            }

            return null;
        }
    }
}
