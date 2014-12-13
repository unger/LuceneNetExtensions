namespace LuceneNetExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading;

    public class SimpleTypeConverter
    {
        private static Converter<object, object> guidToString = input => input.ToString();
        private static Converter<object, object> stringToGuid = input =>
            {
                Guid guid;
                return Guid.TryParse((string)input, out guid) ? guid : Guid.Empty;
            };

        private static Dictionary<string, Converter<object, object>> converters = new Dictionary<string, Converter<object, object>>();

        static SimpleTypeConverter()
        {
            var guidToStringKey = string.Format("{0},{1}", typeof(Guid).FullName, typeof(string).FullName);
            var stringToGuidKey = string.Format("{0},{1}", typeof(string).FullName, typeof(Guid).FullName);

            converters.Add(guidToStringKey, guidToString);
            converters.Add(stringToGuidKey, stringToGuid);
        }

        public static TDestination ConvertTo<TDestination>(object value)
        {
            return ConvertTo<TDestination>(value, Thread.CurrentThread.CurrentCulture);
        }

        public static TDestination ConvertTo<TDestination>(object value, IFormatProvider formatProvider)
        {
            return (TDestination)ConvertTo(typeof(TDestination), value, formatProvider);
        }

        public static TDestination ConvertTo<TSource, TDestination>(TSource value)
        {
            return ConvertTo<TSource, TDestination>(value, Thread.CurrentThread.CurrentCulture);
        }

        public static TDestination ConvertTo<TSource, TDestination>(TSource value, IFormatProvider formatProvider)
        {
            return (TDestination)ConvertTo(typeof(TDestination), value, formatProvider);
        }

        public static object ConvertTo(Type toType, object value)
        {
            return ConvertTo(toType, value, Thread.CurrentThread.CurrentCulture);
        }

        public static object ConvertTo(Type toType, object value, IFormatProvider formatProvider)
        {
            // Handle Arrays
            if (toType.IsArray)
            {
                var values = GetValuesArray(value);
                var elementType = toType.GetElementType();
                var elements = Array.CreateInstance(elementType ?? typeof(object), values.Length);

                var method = toType.GetMethod("SetValue", new[] { elementType, typeof(int) });

                if (method != null)
                {
                    for (int i = 0; i < values.Length; i++)
                    {
                        var typedVal = ConvertTo(elementType, values[i]);
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
                    return ConvertSingleValue(elementType, value, formatProvider);
                }

                // Try to create collection/list
                var elements = CreateGenericInstance(generictype, elementType);

                if (elements != null)
                {
                    var method = elements.GetType().GetMethod("Add");
                    if (method != null)
                    {
                        var values = GetValuesArray(value);
                        foreach (var val in values)
                        {
                            var typedVal = ConvertTo(elementType, val);
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
            return ConvertSingleValue(toType, value, formatProvider);
        }

        private static object[] GetValuesArray(object value)
        {
            if (value.GetType().IsArray)
            {
                return (object[])value;
            }

            return new[] { value };
        }

        private static object ConvertSingleValue(Type toType, object value, IFormatProvider formatProvider)
        {
            var converter = GetConverter(value.GetType(), toType);

            try
            {
                if (converter != null)
                {
                    return converter(value);
                }

                return Convert.ChangeType(value, toType, formatProvider);
            }
            catch (Exception)
            {
                return GetDefaultValue(toType);
            }
        }

        private static Converter<object, object> GetConverter(Type fromType, Type toType)
        {
            var key = string.Format("{0},{1}", fromType.FullName, toType.FullName);
            if (converters.ContainsKey(key))
            {
                return converters[key];
            }

            // Return null converter
            return null;
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
