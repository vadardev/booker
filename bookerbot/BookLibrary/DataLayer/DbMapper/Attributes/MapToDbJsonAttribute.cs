namespace bookerbot.DataLayer.DbMapper.Attributes
{
    /// <summary>
    /// Маппить в json в базе
    /// </summary>
    /// <remarks>
    /// Для Postgres: в jsonb
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class MapToDbJsonAttribute : Attribute
    {
    }
}