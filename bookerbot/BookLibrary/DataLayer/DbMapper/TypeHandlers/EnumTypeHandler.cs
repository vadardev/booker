using System.Data;
using Dapper;
using Npgsql;
using NpgsqlTypes;

namespace BookLibrary.DataLayer.DbMapper.TypeHandlers
{
    internal class EnumTypeHandler : SqlMapper.ITypeHandler
    {
        public void SetValue(IDbDataParameter parameter, object value)
        {
            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = value.ToString();
            }
            
            ((NpgsqlParameter) parameter).NpgsqlDbType = NpgsqlDbType.Text;
        }

        public object Parse(Type destinationType, object value)
        {
            if (value == null)
            {
                return Activator.CreateInstance(destinationType);
            }

            return Enum.Parse(destinationType, (string) value!);
        }
    }
}