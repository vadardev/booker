using System.Data;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Npgsql;
using NpgsqlTypes;

namespace bookerbot.DataLayer.DbMapper.TypeHandlers
{
    internal class DapperNpgsqlJsonTypeHandler : SqlMapper.ITypeHandler
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter()
            }
        };

        public void SetValue(IDbDataParameter parameter, object? value)
        {
            if (value == null)
            {
                parameter.Value = DBNull.Value;
            }
            else
            {
                parameter.Value = JsonConvert.SerializeObject(value, SerializerSettings);
            }

            ((NpgsqlParameter) parameter).NpgsqlDbType = NpgsqlDbType.Jsonb;
        }

        public object? Parse(Type destinationType, object value)
        {
            return JsonConvert.DeserializeObject(value.ToString(), destinationType, SerializerSettings);
        }
    }
}