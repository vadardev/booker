namespace BookLibrary.DataLayer.DbMapper
{
    public enum EIsolationLevel
    {
        ReadUncommited,
        ReadCommited,
        RepeatableRead,
        Serializable
    }
}