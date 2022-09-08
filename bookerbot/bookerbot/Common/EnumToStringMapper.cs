using System.Collections.Concurrent;
using System.Reflection;

namespace bookerbot.Common;

public static class EnumToStringMapper
{
    private static readonly ConcurrentDictionary<Type, Dictionary<string, Func<object, object?>>>
        Cache = new ConcurrentDictionary<Type, Dictionary<string, Func<object, object>>>();

    public static Dictionary<string, object?> Map(object obj)
    {
        Type type = obj.GetType();
        Dictionary<string, Func<object, object>> source;
        if (!EnumToStringMapper.Cache.TryGetValue(type, out source))
        {
            source = new Dictionary<string, Func<object, object>>();
            foreach (PropertyInfo propertyInfo in ((IEnumerable<PropertyInfo>)type.GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>)(p => p.CanRead)))
            {
                PropertyInfo prop = propertyInfo;
                Type underlyingType = Nullable.GetUnderlyingType(prop.PropertyType);
                if (prop.PropertyType.IsEnum || underlyingType != (Type)null && underlyingType.IsEnum)
                    source[prop.Name] = (Func<object, object>)(o =>
                    {
                        object obj1 = prop.GetValue(o);
                        return obj1 == null ? (object)null : (object)obj1.ToString();
                    });
                else
                    source[prop.Name] = (Func<object, object>)(o => prop.GetValue(o));
            }

            EnumToStringMapper.Cache[type] = source;
        }

        return source.ToDictionary<KeyValuePair<string, Func<object, object>>, string, object>((Func<KeyValuePair<string, Func<object, object>>, string>)(kvp => kvp.Key),
            (Func<KeyValuePair<string, Func<object, object>>, object>)(kvp => kvp.Value(obj)));
    }
}