using System.Reflection;
using BookLibrary.DataLayer.DbMapper.Attributes;
using BookLibrary.DataLayer.DbMapper.TypeHandlers;
using Dapper;

namespace BookLibrary.DataLayer.DbMapper.TypeMappings
{
    public static class DapperNpgsqlEnumCollectionMappings
    {
        private static EnumCollectionTypeHandler _enumCollectionTypeHandler = new EnumCollectionTypeHandler();

        /// <summary>
        /// Добавить маппинг коллекций енам, над которыми указан аттрибут <see cref="EnumCollectionDbMapAttribute"/>
        /// </summary>
        public static void AddFromAssemblies()
        {
            AddFromAssembliesByAttr<EnumCollectionDbMapAttribute>();
        }

        /// <summary>
        /// Добавить маппинг коллекций енам, над которыми указан аттрибут TEnumAttribute 
        /// </summary>
        public static void AddFromAssembliesByAttr<TEnumAttribute>() where TEnumAttribute : Attribute
        {
            AddFromAssembliesByAttr(typeof(TEnumAttribute));
        }

        /// <summary>
        /// Добавить маппинг коллекций енам, над которыми указан аттрибут enumAttribute 
        /// </summary>
        public static void AddFromAssembliesByAttr(Type enumAttribute)
        {
            var enumTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes()
                    .Where(x => x.IsEnum && x.IsPublic &&
                                CustomAttributeExtensions.GetCustomAttributes(x, enumAttribute).Any()))
                .ToList();
            foreach (var enumType in enumTypes)
            {
                AddEnumCollectionHandlers(enumType);
            }
        }

        /// <summary>
        /// Добавить маппинг коллекций енам
        /// </summary>
        /// <param name="enumType">тип енам</param>
        public static void AddEnumCollectionHandlers(Type enumType)
        {
            //даппер падла сам проверяет тип на енам и это никак не исправить
            SqlMapper.AddTypeHandler(typeof(List<>).MakeGenericType(enumType), _enumCollectionTypeHandler);
            SqlMapper.AddTypeHandler(typeof(IEnumerable<>).MakeGenericType(enumType), _enumCollectionTypeHandler);
            SqlMapper.AddTypeHandler(enumType.MakeArrayType(), _enumCollectionTypeHandler);
        }

        /// <summary>
        /// Добавить маппинг коллекций енам
        /// </summary>
        /// <typeparam name="TEnum">тип енам</typeparam>
        public static void AddEnumCollectionHandlers<TEnum>() where TEnum : Enum
        {
            //даппер падла сам проверяет тип на енам и это никак не исправить
            SqlMapper.AddTypeHandler(typeof(List<TEnum>), _enumCollectionTypeHandler);
            SqlMapper.AddTypeHandler(typeof(IEnumerable<TEnum>), _enumCollectionTypeHandler);
            SqlMapper.AddTypeHandler(typeof(TEnum[]), _enumCollectionTypeHandler);
        }
    }
}