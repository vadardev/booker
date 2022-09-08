using System.Collections;
using System.Reflection;
using bookerbot.DataLayer.DbMapper.Attributes;
using bookerbot.DataLayer.DbMapper.TypeHandlers;
using Dapper;

namespace bookerbot.DataLayer.DbMapper.TypeMappings
{
    public static class DapperNpgsqlJsonMappings
    {
        private static readonly DapperNpgsqlJsonTypeHandler JsonTypeHandler = new DapperNpgsqlJsonTypeHandler();

        public static void AddFromAssemblies()
        {
            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .Where(x => !x.IsDynamic)
                .SelectMany(x => x.GetTypes());

            foreach (var type in types)
            {
                try
                {
                    var hasAttribute = type
                        .GetCustomAttributes<MapToDbJsonAttribute>()
                        .Any();

                    if (hasAttribute)
                    {
                        Add(type);
                    }
                }
                catch
                {
                    //
                }
            }
        }

        private static void Add(Type type)
        {
            if (!type.IsClass)
            {
                throw new ArgumentException($"Ожидали класс, а пришёл {type.FullName}");
            }

            if (type.IsGenericType)
            {
                throw new ArgumentException("Json-маппинг не работает с дженериками");
            }

            var typeInterfaces = type.GetInterfaces();

            if (typeInterfaces.Contains(typeof(IEnumerable)))
            {
                throw new ArgumentException("Json-маппинг не работает с коллекциями");
            }

            SqlMapper.AddTypeHandler(type, JsonTypeHandler);
        }
    }
}