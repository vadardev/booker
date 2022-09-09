namespace BookLibrary.DataLayer.DbMapper.Attributes
{
    /// <summary>
    /// Маппить коллекцию Enum в массив строк в базе даных и обратно
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public class EnumCollectionDbMapAttribute : Attribute
    {
    }
}