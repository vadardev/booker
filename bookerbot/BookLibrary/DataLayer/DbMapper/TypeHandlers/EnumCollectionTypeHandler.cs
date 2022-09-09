using System.Collections;
using System.Data;
using Dapper;

namespace bookerbot.DataLayer.DbMapper.TypeHandlers
{
    internal class EnumCollectionTypeHandler : SqlMapper.ITypeHandler
    {
        public void SetValue(IDbDataParameter parameter, object value)
        {
            parameter.DbType = DbType.Object;
            parameter.Value = EnumToStringMapper.MapEnumerableEnumToStringArray(value);
        }

        public object Parse(Type destinationType, object? value)
        {
            Type GetEnumType() =>
                destinationType.IsArray
                    ? destinationType.GetElementType()!
                    : destinationType.GenericTypeArguments[0];

            if (value == null || value == DBNull.Value)
            {
                return destinationType.IsInterface
                    ? Array.CreateInstance(GetEnumType(), 0)
                    : Activator.CreateInstance(destinationType);
            }

            var enumType = GetEnumType();
            var targetType = typeof(List<>).MakeGenericType(enumType);
            var newList = Activator.CreateInstance(targetType);
            var addMethod = newList.GetType().GetMethod("Add");

            foreach (var item in (value as IEnumerable)!)
            {
                addMethod!.Invoke(newList, new[] {Enum.Parse(enumType, (string) item!)});
            }

            if (destinationType.IsArray)
            {
                return newList.GetType().GetMethod("ToArray")!.Invoke(newList, null)!;
            }

            return newList;
        }
    }
}