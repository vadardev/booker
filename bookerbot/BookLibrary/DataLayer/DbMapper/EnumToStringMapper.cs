using System.Collections;
using System.Collections.Concurrent;

namespace bookerbot.DataLayer.DbMapper
{
    public static class EnumToStringMapper
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<string, Func<object, object?>>> Cache =
            new ConcurrentDictionary<Type, Dictionary<string, Func<object, object?>>>();

        public static Dictionary<string, object?> Map(object obj)
        {
            var objType = obj.GetType();
            if (!Cache.TryGetValue(objType, out var getters))
            {
                getters = new Dictionary<string, Func<object, object?>>();

                var props = objType.GetProperties();
                foreach (var prop in props.Where(p => p.CanRead))
                {
                    var nullableType = Nullable.GetUnderlyingType(prop.PropertyType);
                    if (prop.PropertyType.IsEnum || nullableType != null && nullableType.IsEnum)
                    {
                        getters[prop.Name] = o => prop.GetValue(o)?.ToString();
                    }
                    else if (IsEnumCollection(prop.PropertyType))
                    {
                        getters[prop.Name] = o => MapEnumerableEnumToStringArray(prop.GetValue(o));
                    }
                    else
                    {
                        getters[prop.Name] = o => prop.GetValue(o);
                    }
                }

                Cache[objType] = getters;
            }

            return getters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value(obj));
        }

        public static string[] MapEnumerableEnumToStringArray(object? value)
        {
            if (value == null || value == DBNull.Value)
            {
                return Array.Empty<string>();
            }

            if (!IsEnumCollection(value.GetType()))
            {
                throw new Exception($"{nameof(value)} {value.GetType()} не фвляется коллекцией енам");
            }

            return ((IEnumerable) value)
                .Cast<Enum>()
                .Select(x => x.ToString())
                .ToArray();
        }

        private static bool IsEnumCollection(Type type)
        {
            if (!type.IsArray &&
                (!type.IsGenericType || type.GenericTypeArguments.Length != 1 ||
                 !typeof(IEnumerable).IsAssignableFrom(type)))
            {
                return false;
            }

            var elementType = type.IsArray
                ? type.GetElementType()
                : type.GenericTypeArguments[0];

            return elementType.IsEnum;
        }
    }
}